using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stationsController : MonoBehaviour
{
    public int health = 20;
    bool repairBegun;
    bool upgradeBegun;
    public bool radarArray = false;
    public Sprite gunshipSprite;


    // Start is called before the first frame update

    void Start()
    {
        repairBegun = false;
        upgradeBegun = false;
    }

    // Update is called once per frame
    void Update()
    {


        
    }

    // if the station is a radar array, cancels wait time for anything in it's radius
    private void OnTriggerEnter(Collider other)
    {
        if (radarArray)
        {
            if (other.gameObject.tag == "playerShip")
            {
                other.gameObject.GetComponent<playerShipController>().inRadarRange = true;
            }
        }
    }

    // if the station is a radar array, reactivates wait time for anything leaving it's radius
    private void OnTriggerExit(Collider other)
    {
        if (radarArray)
        {
            if (other.gameObject.tag == "playerShip"){
                other.gameObject.GetComponent<playerShipController>().inRadarRange = false;
            }
        }
    }

    // if the station is an upgrade bay, improves stats of units that visit it
    public void upgradeUnit(GameObject unit)
    {
        if (unit.name.Contains("Fighter"))
        {
            if (Vector3.Distance(transform.position, unit.transform.position) <= 10)
            {

                if (!upgradeBegun)
                {
                    StartCoroutine(waitForUpgrade(unit));
                }

            }

        }

    }

    IEnumerator waitForUpgrade(GameObject unit)
    {
        upgradeBegun = true;
        //unit.GetComponent<playerShipController>().currentOrder = "Repairing Ship";
        yield return new WaitForSeconds(1f);
        unit.GetComponent<playerShipController>().health += 50;
        unit.GetComponent<playerShipController>().damage += 10;
        unit.GetComponent<playerShipController>().speed += 2;
        unit.GetComponent<SpriteRenderer>().sprite = gunshipSprite;
        unit.GetComponent<playerShipController>().gettingUpgraded = false;
        unit.GetComponent<playerShipController>().currentOrder = "Ready to Recieve Orders";
        upgradeBegun = false;
    }

    // if the station is a repair bay, restores health of units that visit it
    public void healUnit(GameObject unit)
    {
        if (Vector3.Distance(transform.position, unit.transform.position) <= 10)
        {
            if (unit.GetComponent<playerShipController>().health < 100)
            {
                //Debug.Log("made it here");       
                //unit.GetComponent<playerShipController>().currentOrder = "Repairing Ship";
                if (!repairBegun)
                {
                    StartCoroutine(waitForHeal(unit));
                }


            }
        }

    }

    IEnumerator waitForHeal(GameObject unit)
    {
        repairBegun = true;
        //unit.GetComponent<playerShipController>().currentOrder = "Repairing Ship";
        yield return new WaitForSeconds(1f);
        unit.GetComponent<playerShipController>().health = 100;
        unit.GetComponent<playerShipController>().gettingRepaired = false;
        unit.GetComponent<playerShipController>().currentOrder = "Ready to Recieve Orders";
        repairBegun = false;
    }
}
