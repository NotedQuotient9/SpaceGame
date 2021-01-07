using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class enemyCommanderController : MonoBehaviour
{
    bool turn;
    int recordedHealth;
    Vector3 direction;
    Vector3 leftMove = new Vector3(0.1f, 0f, 0f);
    Vector3 rightMove = new Vector3(-0.1f, 0f, 0f);
    // Start is called before the first frame update
    void Start()
    {
        turn = false;
        direction = rightMove;
        recordedHealth = this.GetComponent<enemyShipController>().health;

    }

    // Update is called once per frame
    void Update()
    {

        checkDestroyed();
        //if (!turn)
        //{
        //    //Debug.Log("1");
        //    StartCoroutine(moveAndTurn());
        //}
        //transform.Translate(direction * 0.5f);

        if (checkHealthLowered())
        {
            getDefence();
        }

        recordedHealth = this.GetComponent<enemyShipController>().health;
    }

    public void checkDestroyed()
    {
        if (this.GetComponent<enemyShipController>().health <= 0)
        {
            Time.timeScale = 0;
            Debug.Log("Player Wins");
        }
    }

    bool checkHealthLowered()
    {
        if (this.GetComponent<enemyShipController>().health < recordedHealth)
        {
            return true;
        } else
        {
            return false;
        }
        
    }

    void getDefence()
    {
        GameObject[] ships = GameObject.FindGameObjectsWithTag("enemyShip");

        foreach (GameObject s in ships)
        {
            if (s != this)
            {
                s.GetComponent<enemyShipController>().mode = "defend";
            }
        }
    }

    IEnumerator moveAndTurn()
    {
        //Debug.Log("2");
        turn = true;
        if (direction == leftMove)
        {
            direction = rightMove;
        } else
        {
            direction = leftMove;
        }
        yield return new WaitForSeconds(10);
        turn = false;
        //Debug.Log("3");
        //direction = rightMove;
        

    }

}
