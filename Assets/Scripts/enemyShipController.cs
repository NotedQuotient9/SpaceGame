using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class enemyShipController : MonoBehaviour
{

    public int health = 100;
    public int speed = 5;
    public int damage = 10;
    public GameObject ring;
    public bool isSelected = false;
    private UIController controller;
    public GameObject scrapToSpawn;
    public GameObject commandShip;
    bool attacking;
    bool waitingToMove;
    bool moving;
    bool turn;
    bool waitedToDefend = false;
    public string mode;
    float searchRadius;
    GameObject targetShip;
    Vector3 direction;
    Vector3 leftMove = new Vector3(0f, -.1f, 0f);
    Vector3 upMove = new Vector3(-.1f, 0f, 0f);
    Vector3 downMove = new Vector3(.1f, 0f, 0f);
    Vector3 rightMove = new Vector3(0f, .1f, 0f);



    // Start is called before the first frame update
    void Start()
    {
        // search radius is the distance where it will switch from patrol to attack if it detects a player ship
        searchRadius = 35f;
        // targetShip is the currect ship it is after, it is found using the nearest ship function
        targetShip = null;
        // the ui controller, used for working out the currently selected ship
        controller = GameObject.Find("GameController").GetComponent<UIController>();
        // find command ship which this ship is tethered too
        commandShip = GameObject.Find("EnemyCommand");
        // ring appears when this ship is selected
        ring.SetActive(false);
        // pick a random direction to start moving in
        changeDirection();
        // ship is not currently turning
        turn = false;
        // ship is in patrol mode, meaning it will move around randomly
        mode = "patrol";
        // ship is permitted to move
        moving = true;
        // ship is not attacking
        attacking = false;
        // ship is not waiting to move
        waitingToMove = false;
 
    }

    // Update is called once per frame
    void Update()
    {


        // ship checks whether or not it is above the map plane
        RaycastHit beneathHit;
        
        //Debug.Log(beneathHit.transform.gameObject.name);
        // if it is not above the map plane, move until it is
        if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) * 100, out beneathHit))
        {
            //changeDirection();
            float step = speed * Time.deltaTime * 3; // calculate distance to move, the x number here controls the speed
            transform.position = Vector3.MoveTowards(transform.position, GameObject.Find("Plane").transform.position, step);
        }

        // if target ship is destroyed, switch to patrol mode
        if (targetShip == null)
        {
            mode = "patrol";
        }

        //Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100, Color.white);

        // if this ship is clicked then select it
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == this.gameObject)
                {
                    if (controller.selectedShip == null || controller.selectedShip == gameObject)
                    {
                        if (isSelected)
                        {
                            deselectShip();
                        }
                        else
                        {
                            selectShip();
                        }

                    }

                }
            }
        }

        // if ship is not turning then start turning process
        if (!turn)
        {
            StartCoroutine(waitToTurn());
        }

        // if ship is allowed to move then move it in selected direction
        if (moving)
        {
            // if it is command ship then move slower (the * 0.2f value is the speed it moves at)
            if (this.name == "EnemyCommand")
            {
                transform.Translate(direction * 0.3f);
            } else
            {
                transform.Translate(direction * 1f);
            }
            
        }
        // each update find the nearest player ship to this one
        findNearestShip();

        // if the target ship is nearer than the searchRadius then switch to attack mode
        if (Vector3.Distance(transform.position, targetShip.transform.position) < searchRadius)
        {
            mode = "attack";
        }


        //Debug.Log(mode);

        // if in patrol mode then patrol (move randomly), if in attack mode then attack (follow target)
        if (string.Equals(mode, "patrol"))
        {
            patrol();
        } else if (string.Equals(mode, "attack"))
        {
            attack();
        } else if (string.Equals(mode, "defend"))
        {
            defend();
        }
        

        //move down
        //transform.Translate(.1f, 0f, 0f);

        //move right
        //transform.Translate(0f, .1f, 0f);

        //move left
        //transform.Translate(0f, -.1f, 0f);

        //move up
        //transform.Translate(-.1f, 0f, 0f);


    }

    // selects this ship
    public void selectShip()
    {
        ring.SetActive(true);
        isSelected = true;
        controller.selectedShip = gameObject;
    }

    // deselects this ship
    public void deselectShip()
    {
        ring.SetActive(false);
        isSelected = false;
        controller.selectedShip = null;
    }

    // if this ship is destroyed, spawns a piece of scrap where it was
    public void spawnScrap()
    {
        GameObject clone;
        clone = Instantiate(scrapToSpawn, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation) as GameObject;
    }



    // attacks ship by lowering their health each second
    IEnumerator attackShip()
    {
        attacking = true;
        //targetShip.GetComponent<playerShipController>().lowerHealth(damage); // consider switching these 2 lines around
        targetShip.GetComponent<playerShipController>().health = targetShip.GetComponent<playerShipController>().health -= damage;
        yield return new WaitForSeconds(1);


        // if ship runs out of health, destroys ship aand switched back to patrol mode
        if (targetShip.GetComponent<playerShipController>().health <= 0)
        {


            //targetShip.GetComponent<enemyShipController>().spawnScrap();
            Destroy(targetShip);
            mode = "patrol";
  
        }
        attacking = false;
    }

    // patrols by allowing random movement, but if ship gets too far from commandship, makes it move back within range
    void patrol()
    {
        if (Vector3.Distance(transform.position, commandShip.transform.position) >= 50f)
        {
            //Debug.Log("too far");
            float step = speed * Time.deltaTime * 3; // calculate distance to move, the x number here controls the speed
            transform.position = Vector3.MoveTowards(transform.position, commandShip.transform.position, step);
            moving = false;
        } else
        {
            if (!waitingToMove)
            {
                moving = true;
            }
            
        }
    }

    void attack()
    {

        // checks if enemy ship is near enough to attack, if yes then attackship, if no then move until it is near enough and try again
        if (Vector3.Distance(targetShip.transform.position, transform.position) <= 10f)
        {
             
            if (!attacking)
            {
                StartCoroutine(attackShip());
            }

        }
        else
        {
            float step = speed * Time.deltaTime * 3; // calculate distance to move, the x number here controls the speed
            transform.position = Vector3.MoveTowards(transform.position, targetShip.transform.position, step);
        }
    }

    // makes ship return to command ship
    void defend()
    {
        Debug.Log(waitedToDefend);
        if (waitedToDefend)
        {
            if (Vector3.Distance(transform.position, commandShip.transform.position) >= 5f)
            {
                //Debug.Log("too far");
                float step = speed * Time.deltaTime * 3; // calculate distance to move, the x number here controls the speed
                transform.position = Vector3.MoveTowards(transform.position, commandShip.transform.position, step);
                moving = false;
            }
            else
            {
                if (!waitingToMove)
                {
                    moving = true;
                    mode = "patrol"; //might change this to attack at some point
                    waitedToDefend = false;
                }

            }
        } else
        {
            StartCoroutine(waitToDefend());
        }

    }

    IEnumerator waitToDefend()
    {
        float waitTime = Vector3.Distance(transform.position, commandShip.transform.position);

        if (waitTime > 10)
        {
            waitTime /= 10;
            waitTime /= 2;
        }
        else
        {
            waitTime = 0;
        }
        yield return new WaitForSeconds(waitTime);
        waitedToDefend = true;
    }
    
    // waits a random amount of time before turning
    IEnumerator waitToTurn()
    {
        turn = true;
        StartCoroutine(waitToMove());
        //changeDirection();
        float waitTime = Random.Range(1.0f, 15.0f);
        yield return new WaitForSeconds(waitTime);
        turn = false;

    }

    // wait some time before moving, time calculated using distance from command ship
    IEnumerator waitToMove()
    {
        waitingToMove = true;
        moving = false;
        float waitTime = Vector3.Distance(transform.position, commandShip.transform.position);

        if (waitTime > 10)
        {
            waitTime /= 10;
        }
        else
        {
            waitTime = 0;
        }
        yield return new WaitForSeconds(waitTime);
        moving = true;
        waitingToMove = false;
        changeDirection();

    }




    // loops through all player ships to find the nearest one to this ship
    void findNearestShip()
    {
        GameObject[] ships = GameObject.FindGameObjectsWithTag("playerShip");
        targetShip = ships[0];
        foreach (GameObject s in ships)
        {
            if (Vector3.Distance(transform.position, s.transform.position) <= Vector3.Distance(transform.position, targetShip.transform.position))
            {
                targetShip = s;
            }
        }
    }

    // choose a random direction that is not the one currently taken
    void changeDirection()
    {

        int turnInt = Random.Range(0, 4);
        if (turnInt == 0)
        {
            if (direction != leftMove)
            {
                direction = leftMove;
            } else
            {
                changeDirection();
            }
        } else if (turnInt == 1)
        {
            if (direction != rightMove)
            {
                direction = rightMove;
            }
            else
            {
                changeDirection();
            }
        } else if (turnInt == 2)
        {
            if (direction != upMove)
            {
                direction = upMove;
            }
            else
            {
                changeDirection();
            }
        } else if (turnInt == 3)
        {
            if (direction != downMove)
            {
                direction = downMove;
            }
            else
            {
                changeDirection();
            }
        }

    }
}
