//using System.Collections;
//using System.Collections.Generic;
//using System.Net.Sockets;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class playerShipController : MonoBehaviour
//{
//    public int health = 100;
//    public int speed = 5;
//    public int damage = 10;
//    int recordedHealth;
//    int commandRadius; // will make part of commandercontroller

//    bool isSelected;
//    bool upgraded;
//    bool idle;
//    bool attacking;
//    bool idleTurn;
//    bool returningToCommand = false;
//    public bool inRadarRange = false;

//    float distanceFromCommand;

//    string currentOrder;

//    public GameObject ring;
//    private GameObject targetShip;
//    private UIController uiController;
//    private PlayerCommandController commandController;
//    public Sprite gunshipSprite;

//    Vector3 newLocation;
//    Vector3 direction;
//    Vector3 leftMove = new Vector3(0f, -.1f, 0f);
//    Vector3 upMove = new Vector3(-.1f, 0f, 0f);
//    Vector3 downMove = new Vector3(.1f, 0f, 0f);
//    Vector3 rightMove = new Vector3(0f, .1f, 0f);



//    void Start()
//    {
//        isSelected = false;
//        recordedHealth = health;
//        uiController = GameObject.Find("GameController").GetComponent<UIController>();
//        commandController = GameObject.Find("Command").GetComponent<PlayerCommandController>();
//        ring.SetActive(false);
//        changeOrder("Ready to Recieve Orders");
//        commandRadius = 20; // will make part of commandcontroller
//        idle = true;
//        idleTurn = true;
//    }

//    void Update()
//    {

//        distanceFromCommand = Vector3.Distance(transform.position, commandController.gameObject.transform.position);

//        // if ship is clicked but not yet selected then select it, if it is selected then deselect it
//        if (Input.GetMouseButtonDown(0))
//        {
//            RaycastHit hit;
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

//            if (Physics.Raycast(ray, out hit))
//            {
//                if (hit.transform.gameObject == this.gameObject)
//                {
//                    if (uiController.selectedShip == null || uiController.selectedShip == gameObject)
//                    {
//                        if (isSelected)
//                        {
//                            deselectShip();
//                        }
//                        else
//                        {
//                            selectShip();
//                        }

//                    }

//                }
//            }
//        }

//        if (Input.GetMouseButtonDown(0) && isSelected)
//        {
//            RaycastHit hit;
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            if (Physics.Raycast(ray, out hit))
//            {
//                // checks if click was on a ui element (like the cancel button)
//                if (!EventSystem.current.IsPointerOverGameObject())
//                {
//                    if (hit.transform.tag == "playerShip")
//                    {
//                        switchShip(hit.transform.gameObject);
//                    }
//                    else if (checkHitStation(hit))
//                    {
//                        visitStation(hit.transform.gameObject);
//                    }
//                    else if (hit.transform.tag == "enemyShip")
//                    {
//                        setTargetShip(hit.transform.gameObject);
//                    }
//                    else
//                    {
//                        //Debug.Log("made it here 1");
//                        setNewLocation(hit);
//                    }
//                }

//            }

//        }

//        if (targetShip != null)
//        {
//            checkDistanceFromTarget();
//        }

//        if (checkGettingAttacked())
//        {
//            setTargetShip(findNearestEnemy());
//        }

//        recordedHealth = health;

//        if (isSelected)
//        {
//            //Debug.Log(newLocation.ToString());

//        }

//        if (!idle)
//        {

//            moveShip();
//            checkDestinationReached();
//        }
//        else
//        {
//            idleMovement();
//        }


//    }

//    float calculateWaitTime()
//    {
//        if (inRadarRange)
//        {
//            return 0f;
//        }
//        else
//        {
//            float waitTime = distanceFromCommand;

//            if (waitTime > commandRadius)
//            {
//                waitTime /= 10;

//                if (this.name.Contains("Scout"))
//                {
//                    waitTime /= 3;
//                }
//            }
//            else
//            {
//                waitTime = 0;
//            }

//            return waitTime;
//        }
//    }

//    IEnumerator wait(string context, string station = "none")
//    {


//        if (context.Equals("setNewLocation"))
//        {

//            if (commandController.gameObject != this.gameObject)
//            {


//                float waitTime = calculateWaitTime();

//                float totalTime = 0;

//                while (totalTime <= waitTime)
//                {
//                    totalTime += Time.deltaTime;
//                    yield return null;
//                    uiController.showOrderTimer("Estimated Arrival: \n" + (waitTime - totalTime).ToString("F0") + " Rels");
//                }


//            }

//            idle = false;
//            if (getCurrentOrder().Equals("Getting Upgraded") || getCurrentOrder().Equals("Getting Repaired"))
//            {
//                Debug.Log("hi");
//            } else
//            {
//                changeOrder("Moving Towards " + newLocation.ToString());
//            }


//        }
//        else if (context.Equals("attack"))
//        {
//            attacking = true;
//            targetShip.GetComponent<enemyShipController>().health -= damage; // consider switching these 2 lines around
//            yield return new WaitForSeconds(1);

//            if (targetShip.GetComponent<enemyShipController>().health <= 0)
//            {

//                targetShip.GetComponent<enemyShipController>().spawnScrap();
//                Destroy(targetShip);
//                cancelOrders();
//                uiController.hideTargetInfo();
//            }
//            attacking = false;
//        }
//        else if (context.Equals("idleMovement"))
//        {
//            idleTurn = false;
//            changeDirection();
//            yield return new WaitForSeconds(Random.Range(1f, 4f));
//            idleTurn = true;

//        }
//        else if (context.Equals("visitStation"))
//        {
//            yield return new WaitForSeconds(1f);
//            if (station.Equals("upgrade") && !upgraded)
//            {
//                if (gameObject.name.Contains("Fighter"))
//                {
//                    health += 50;
//                    damage += 10;
//                    speed += 2;
//                    this.GetComponent<SpriteRenderer>().sprite = gunshipSprite;
//                    upgraded = true;
//                }
//            }
//            else
//            {
//                health = 100;
//            }
//        }

//    }

//    void checkDestinationReached()
//    {
//        if (Vector3.Distance(transform.position, newLocation) < 2)
//        {
//            if (returningToCommand)
//            {
//                transform.Translate(direction * 30f);
//                returningToCommand = false;
//            }
//            else if (getCurrentOrder().Equals("Getting Upgraded"))
//            {
//                StartCoroutine(wait("visitStation", "upgrade"));
//            } else if (getCurrentOrder().Equals("Getting Repaired"))
//            {
//                StartCoroutine(wait("visitStation", "repair"));
//            }
//            cancelOrders();
//        }
//    }

//    void moveShip()
//    {
//        //Debug.Log("1");
//        //Debug.Log("made it here6");

//        float step = speed * Time.deltaTime * 3; // calculate distance to move, the x number here controls the speed
//        transform.position = Vector3.MoveTowards(transform.position, newLocation, step);
//        //Debug.Log("step: " + step);
//        //Debug.Log(newLocation.ToString());
//    }

//    void setNewLocation(RaycastHit hit)
//    {
//        //Debug.Log("made it here2");
//        newLocation = new Vector3(hit.point.x, 0f, hit.point.z);
//        Debug.Log(newLocation.ToString());
//        StartCoroutine(wait("setNewLocation"));
//    }

//    void setTargetShip(GameObject enemyShip)
//    {
//        targetShip = enemyShip;
//        newLocation = enemyShip.transform.position;
//        StartCoroutine(wait("setNewLocation"));
//    }

//    void checkDistanceFromTarget()
//    {
//        uiController.showTargetInfo("Health: " + targetShip.GetComponent<enemyShipController>().health + "\nSpeed: " +
//                            targetShip.GetComponent<enemyShipController>().speed + "\nDamage: " + targetShip.GetComponent<enemyShipController>().damage);

//        // checks if enemy ship is near enough to attack, if yes then attackship, if no then newPosition is updated to follow it
//        if (Vector3.Distance(targetShip.transform.position, transform.position) <= 10f)
//        {

//            newLocation = transform.position;

//            if (!attacking)
//            {
//                StartCoroutine(wait("attack"));
//            }

//        }
//        else
//        {

//            newLocation = targetShip.transform.position;
//        }
//    }

//    void visitStation(GameObject station)
//    {
//        newLocation = station.transform.position;
//        StartCoroutine(wait("setNewLocation"));
//        if (station.name.Contains("Upgrade"))
//        {
//            changeOrder("Getting Upgraded");
//        }
//        else
//        {
//            changeOrder("Getting Repaired");
//        }
//    }

//    void switchShip(GameObject newShip)
//    {
//        deselectShip();
//        newShip.GetComponent<playerShipController>().selectShip();
//    }

//    bool checkHitStation(RaycastHit hit)
//    {
//        if (hit.transform.name.Contains("Repair") || hit.transform.name.Contains("Upgrade"))
//        {
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }

//    bool checkGettingAttacked()
//    {
//        if (health < recordedHealth && idle)
//        {
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }

//    GameObject findNearestEnemy()
//    {
//        GameObject[] ships = GameObject.FindGameObjectsWithTag("enemyShip");
//        GameObject nearestShip = ships[0];
//        foreach (GameObject s in ships)
//        {
//            if (Vector3.Distance(transform.position, s.transform.position) <= Vector3.Distance(transform.position, nearestShip.transform.position))
//            {
//                nearestShip = s;
//            }
//        }

//        return nearestShip;
//    }


//    void idleMovement()
//    {
//        if (idleTurn)
//        {
//            StartCoroutine(wait("idleMovement"));
//        }
//        transform.Translate(direction * 0.1f);
//    }


//    public void changeOrder(string order)
//    {
//        currentOrder = order;
//    }


//    public void selectShip()
//    {
//        ring.SetActive(true);
//        isSelected = true;
//        uiController.selectedShip = gameObject;
//    }

//    // deselects this ship
//    public void deselectShip()
//    {
//        ring.SetActive(false);
//        isSelected = false;
//        uiController.selectedShip = null;
//    }

//    public void returnToCommand()
//    {
//        cancelOrders();
//        returningToCommand = true;
//        newLocation = commandController.gameObject.transform.position;
//        idle = false;
//    }

//    public int getHealth()
//    {
//        return health;
//    }

//    public float getDistance()
//    {
//        return distanceFromCommand;
//    }

//    public int getSpeed()
//    {
//        return speed;
//    }

//    public int getDamage()
//    {
//        return damage;
//    }
//    public void lowerHealth(int enemyDamage)
//    {
//        health -= enemyDamage;
//    }

//    public string getCurrentOrder()
//    {
//        return currentOrder;
//    }

//    public void cancelOrders()
//    {
//        idle = true;
//        targetShip = null;
//        changeOrder("Ready to Recieve Orders");
//    }

//    void changeDirection()
//    {
//        int turnInt = Random.Range(0, 4);
//        if (turnInt == 0)
//        {
//            if (direction != leftMove)
//            {
//                direction = leftMove;
//            }
//            else
//            {
//                changeDirection();
//            }
//        }
//        else if (turnInt == 1)
//        {
//            if (direction != rightMove)
//            {
//                direction = rightMove;
//            }
//            else
//            {
//                changeDirection();
//            }
//        }
//        else if (turnInt == 2)
//        {
//            if (direction != upMove)
//            {
//                direction = upMove;
//            }
//            else
//            {
//                changeDirection();
//            }
//        }
//        else if (turnInt == 3)
//        {
//            if (direction != downMove)
//            {
//                direction = downMove;
//            }
//            else
//            {
//                changeDirection();
//            }
//        }
//    }

//}




using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;

public class playerShipController : MonoBehaviour
{

    public int health = 100;
    public int speed = 5;
    public int damage = 10;
    int recordedHealth;
    public bool isSelected = false;
    public GameObject ring;
    Vector3 newPosition;
    private UIController controller;
    public float distanceFromCommand;
    public int commandRadius = 20;
    bool attacking = false;
    bool moveToAttack = false;
    GameObject targetShip = null;
    public string currentOrder;
    bool moving;
    public bool gettingRepaired = false;
    public bool gettingUpgraded = false;
    bool upgraded = false;
    GameObject targetStation = null;
    public bool inRadarRange = false;
    bool returningToCommand = false;

    bool isIdle;
    bool idleTurn;
    Vector3 direction;
    Vector3 leftMove = new Vector3(0f, -.1f, 0f);
    Vector3 upMove = new Vector3(-.1f, 0f, 0f);
    Vector3 downMove = new Vector3(.1f, 0f, 0f);
    Vector3 rightMove = new Vector3(0f, .1f, 0f);


    // Start is called before the first frame update
    void Start()
    {

        idleTurn = false;
        recordedHealth = health;
        commandRadius = 20;
        controller = GameObject.Find("GameController").GetComponent<UIController>();
        ring.SetActive(false);
        newPosition = transform.position;
        moving = false;
        currentOrder = "Ready to Recieve Orders";
    }

    // Update is called once per frame
    void Update()
    {
        // checks if the ship has been destroyed (necessary for command ship)
        checkDestroyed();
        // current distance from command is calculated
        distanceFromCommand = Vector3.Distance(GameObject.Find("Command").transform.position, transform.position);

        // the ship is flying to a repair station, then call repair function
        if (gettingRepaired)
        {
            targetStation.GetComponent<stationsController>().healUnit(this.gameObject);

        }

        // if unit is getting upgraded then call upgrade station
        if (gettingUpgraded)
        {
            targetStation.GetComponent<stationsController>().upgradeUnit(this.gameObject);
        }


        // if ship is ready to recieve orders then start idle movement
        if (currentOrder.Equals("Ready to Recieve Orders") && name != "Command")
        {
            isIdle = true;
        }
        else
        {
            isIdle = false;
        }

        // if idle movement is enabled then move slowly in a random direction, which changes at a random time
        if (isIdle)
        {
            if (!idleTurn)
            {
                //Debug.Log("1");
                StartCoroutine(waitForIdleTurn());
            }
            transform.Translate(direction * 0.1f);
        }

        // if ship is returning then checks if it has arrived
        // all ships stop at a random distance so they are not on top of each other
        // wait for idle turn is called with a special condition that makes it spread out when it arrives
        if (returningToCommand)
        {

            currentOrder = "Returning to Command";
            if (distanceFromCommand < getRandomDistance())
            {
                cancelOrders();
                waitForIdleTurn();

            }
        }

        // if player is attacking a ship
        if (moveToAttack)
        {


            // checks if enemy ship has been destroyed, if yes then stops attacking
            if (targetShip == null)
            {
                moveToAttack = false;
            }
            // updates ui display on enemy ship
            controller.GetComponent<UIController>().showTargetInfo("Health: " + targetShip.GetComponent<enemyShipController>().health + "\nSpeed: " +
                            targetShip.GetComponent<enemyShipController>().speed + "\nDamage: " + targetShip.GetComponent<enemyShipController>().damage);



            // checks if enemy ship is near enough to attack, if yes then attackship, if no then newPosition is updated to follow it
            if (Vector3.Distance(targetShip.transform.position, transform.position) <= 10f)
            {
                newPosition = transform.position;

                if (!attacking)
                {
                    StartCoroutine(attackShip());
                }

            }
            else
            {

                newPosition = targetShip.transform.position;
            }

        }

        // checks if the enemy is currently getting attacked and is idle, if yes then fight back
        if (checkGettingAttacked() && string.Equals(currentOrder, "Ready to Recieve Orders"))
        {
            findNearestEnemyAndAttack();
        }


        // if ship is clicked but not yet selected then select it, if it is selected then deselect it
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

        // if mouse clicked and ship is selected
        if (Input.GetMouseButtonDown(0) && isSelected)
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {

                // checks if click was on a ui element (like the cancel button)
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    // checks if click was on another player ship, if yes then deselect this ship and select that one
                    if (hit.transform.tag == "playerShip")
                    {
                        if (hit.transform.gameObject != gameObject)
                        {
                            deselectShip();
                            hit.transform.gameObject.GetComponent<playerShipController>().selectShip();
                        }

                    }
                    else if (hit.transform.tag == "repairbay") //checks if click was on a repair bay, if yes start repair process
                    {
                        gettingRepaired = true;
                        targetStation = hit.transform.gameObject;
                        //hit.transform.gameObject.GetComponent<stationsController>().healUnit(this.gameObject);
                        StartCoroutine(waitForMove(hit));
                    }
                    else if (hit.transform.tag == "upgradebay" && !upgraded) // checks if click was on upgrade bay, if yes start upgrade process
                    {
                        gettingUpgraded = true;
                        upgraded = true;
                        targetStation = hit.transform.gameObject;
                        StartCoroutine(waitForMove(hit));
                    }
                    // checks if click was on an enemy ship, if so set that ship as target ship and start attacking it
                    else if (hit.transform.tag == "enemyShip")
                    {

                        moveToAttack = true;

                        targetShip = hit.transform.gameObject;
                        //moving = true;
                        StartCoroutine(waitForMove(hit));
                    } // if click was not on friendly or enemy ship then start coroutine to update newPosition to new point
                    else
                    {
                        cancelOrders();
                        if (moveToAttack)
                        {
                            moveToAttack = false;
                        }
                        //moving = true;
                        //Debug.Log("should move");
                        StartCoroutine(waitForMove(hit));

                    }

                }



            }

        }

        // the ship's health at the end of the update is recorded
        recordedHealth = health;
        // if moving is allowed then move ship
        if (moving)
        {
            moveShip();
        }




    }

    // returns true if the ship's current health is less than it's recorded health and it's not in battle
    bool checkGettingAttacked()
    {
        if (health < recordedHealth && !moveToAttack)
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    // loops through all player ships to find the nearest one to this ship
    void findNearestEnemyAndAttack()
    {
        GameObject[] ships = GameObject.FindGameObjectsWithTag("enemyShip");
        targetShip = ships[0];
        foreach (GameObject s in ships)
        {
            if (Vector3.Distance(transform.position, s.transform.position) <= Vector3.Distance(transform.position, targetShip.transform.position))
            {
                targetShip = s;
            }
        }
        moveToAttack = true;
    }

    // checks if the command ship has 0hp (has been destroyed)
    public void checkDestroyed()
    {
        if (health <= 0 && this.name == "Command")
        {
            Time.timeScale = 0;
            Debug.Log("Player Loses");
        }
    }


    // controls the delay between sending and receiving orders
    IEnumerator waitForMove(RaycastHit hit)
    {


        if (!inRadarRange)
        {
            // calculates wait time by using distance from command divided by 10
            float waitTime = distanceFromCommand;

            if (waitTime > commandRadius)
            {
                waitTime /= 10;

                if (this.name.Contains("Scout"))
                {
                    waitTime /= 3;
                }
            }
            else
            {
                waitTime = 0;
            }

            // starts at 0 and counts up until wait time is reached, the current time value is used to update ui
            float totalTime = 0;
            while (totalTime <= waitTime)
            {
                totalTime += Time.deltaTime;
                yield return null;
                controller.GetComponent<UIController>().showOrderTimer("Estimated Arrival: \n" + (waitTime - totalTime).ToString("F0") + " Rels");
            }
        }


        // when time is reached newPosition and orders are updated, and player is allowed to move
        newPosition = new Vector3(hit.point.x, 0f, hit.point.z);
        if (!moveToAttack)
        {
            currentOrder = "Moving Towards " + hit.point.ToString();
        }
        else
        {
            currentOrder = "Attacking " + hit.transform.gameObject.name;
        }

        if (gettingRepaired)
        {
            currentOrder = "Repairing Ship";
        }
        else if (gettingUpgraded)
        {
            currentOrder = "Upgrading Ship";
        }

        controller.GetComponent<UIController>().hideOrderTimer();
        moving = true;


    }

    // chooses a random time for the ship to turn if it is idle
    // if it is returning to command then turns and travels quickly so it can spread out when it arrives
    IEnumerator waitForIdleTurn()
    {
        if (!returningToCommand)
        {
            idleTurn = true;
            changeDirection();
            yield return new WaitForSeconds(Random.Range(1f, 4f));
            idleTurn = false;
        }
        else
        {
            transform.Translate(direction * 30f);
            yield return new WaitForSeconds(1f);
            returningToCommand = false;
            cancelOrders();
        }

    }

    // chooses a random direction for the ship to turn when idle
    void changeDirection()
    {

        int turnInt = Random.Range(0, 4);
        if (turnInt == 0)
        {
            if (direction != leftMove)
            {
                direction = leftMove;
            }
            else
            {
                changeDirection();
            }
        }
        else if (turnInt == 1)
        {
            if (direction != rightMove)
            {
                direction = rightMove;
            }
            else
            {
                changeDirection();
            }
        }
        else if (turnInt == 2)
        {
            if (direction != upMove)
            {
                direction = upMove;
            }
            else
            {
                changeDirection();
            }
        }
        else if (turnInt == 3)
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

    // moves ship using transform, sets moving to false if it reaches destination but is not attacking
    void moveShip()
    {
        float step = speed * Time.deltaTime * 3; // calculate distance to move, the x number here controls the speed
        if (transform.position != newPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, step);

        }
        if (Vector3.Distance(newPosition, transform.position) < 1f && !moveToAttack)
        {
            currentOrder = "Ready to Recieve Orders";
            moving = false;
            //controller.GetComponent<GameController>().hideTargetInfo();
        }

    }

    // attacks ship by lowering their health each second
    IEnumerator attackShip()
    {
        attacking = true;
        targetShip.GetComponent<enemyShipController>().health -= damage; // consider switching these 2 lines around
        yield return new WaitForSeconds(1);


        // if ship runs out of health, destroys ship and resets orders
        if (targetShip.GetComponent<enemyShipController>().health <= 0)
        {

            moveToAttack = false;
            targetShip.GetComponent<enemyShipController>().spawnScrap();
            Destroy(targetShip);
            currentOrder = "Ready to Recieve Orders";
            controller.GetComponent<UIController>().hideTargetInfo();
        }
        attacking = false;
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

    // gets a random distance for the ship to travel when returning to command ship
    float getRandomDistance()
    {
        float distance = Random.Range(2f, 15f);
        Debug.Log(distance);
        return distance;
    }


    // starts process for ship returning to command
    public void returnToCommand()
    {
        cancelOrders();
        newPosition = GameObject.Find("Command").transform.position;
        returningToCommand = true;
        moving = true;
    }


    // pauses everything the ship is doing, cancelling their orders
    public void cancelOrders()
    {
        attacking = false;
        moveToAttack = false;
        newPosition = transform.position;
        moving = false;
        gettingRepaired = false;
        currentOrder = "Ready to Recieve Orders";
    }
}

//StartCoroutine(waitForNextSpawn());

//IEnumerator waitForNextSpawn()
//{
//    yield return new WaitForSeconds(1f);
//    canSpawn = true;
//}


//IEnumerator waitForAttack(RaycastHit hit)
//{
//    float waitTime = distanceFromCommand;
//    if (waitTime > commandRadius)
//    {
//        waitTime /= 10;
//    }
//    else
//    {
//        waitTime = 0;
//    }
//    yield return new WaitForSeconds(waitTime);
//    newPosition = new Vector3(hit.point.x, 0f, hit.point.z);

//}




///*
// * if player clicks enemy they start following it -- DONE
// * if the player ship gets withing a certain distance of the enemy ship it stops and shoots
// * if the enemy ship moves then the player follows until it is in range again -- DONE
// * this continues until the enemy ship is dead or the player clicks somewhere else with this ship selected
// * 
// * 
// */


////float duration = 3f; // 3 seconds you can change this to
////to whatever you want

////  while (totalTime <= duration)
////{
////countdownImage.fillAmount = totalTime / duration;
////totalTime += Time.deltaTime;
////var integer = (int)totalTime; /* choose how to quantize this */
///* convert integer to string and assign to text */
////yield return null;
////}



////moveShip();