using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public GameObject selectedShip;
    public GameObject selectedDistanceText;
    public GameObject selectedInfoText;
    public GameObject selectedOrderText;
    public GameObject cancelButtonObject;
    public GameObject targetInfoText;
    public GameObject orderTimerText;
    public GameObject resourcesText;
    public GameObject buildButton;
    public GameObject buildMenu;
    public GameObject summonButton;
    string newDistanceText;
    string newInfoText;
    string newOrderText;
    bool buildMenuShown = true;
    //string newTargetText;
    // Start is called before the first frame update
    void Start()
    {
        hideCancelButton();
        hideTargetInfo();
        hideOrderTimer();
        hideBuildButton();
        showHideBuildMenu();
        hideSummonButton();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedShip != null)
        {
            

            if (selectedShip.name.Equals("Command"))
            {
                showSummonButton();
            } else
            {
                hideSummonButton();
            }

            if (selectedShip.tag == "playerShip")
            {
                //newDistanceText = "Distance From Command: \n" + selectedShip.GetComponent<playerShipController>().getDistance().ToString("F0") + " Space Metres";
                newDistanceText = "Distance From Command \n" + selectedShip.GetComponent<playerShipController>().distanceFromCommand.ToString("F0") + "Space Metres";
                selectedDistanceText.GetComponent<Text>().text = newDistanceText;

                //   newInfoText = "Health: " + selectedShip.GetComponent<playerShipController>().getHealth() + "\nSpeed: " + selectedShip.GetComponent<playerShipController>().getSpeed() +
                //"\nDamage: " + selectedShip.GetComponent<playerShipController>().getDamage();
                newInfoText = "Health: " + selectedShip.GetComponent<playerShipController>().health + "\nSpeed: " + selectedShip.GetComponent<playerShipController>().speed +
"\nDamage: " + selectedShip.GetComponent<playerShipController>().damage;

                selectedInfoText.GetComponent<Text>().text = newInfoText;

                newOrderText = selectedShip.GetComponent<playerShipController>().currentOrder;
                selectedOrderText.GetComponent<Text>().text = newOrderText;

                if (string.Equals("Ready to Recieve Orders", newOrderText) || string.Equals("Current Order Status", newOrderText) || string.Equals("Collecting Scrap", newOrderText))
                {
                    hideCancelButton();
                    hideTargetInfo();
                } else
                {
                    showCancelButton();
                }

            } else
            {
                newInfoText = "Health: " + selectedShip.GetComponent<enemyShipController>().health + "\nSpeed: " + selectedShip.GetComponent<enemyShipController>().speed +
             "\nDamage: " + selectedShip.GetComponent<enemyShipController>().damage;

                targetInfoText.GetComponent<Text>().text = newInfoText;
            }

            

        } else
        {
            selectedDistanceText.GetComponent<Text>().text = "Distance From Command";
            selectedInfoText.GetComponent<Text>().text = "Ship Info";
            selectedOrderText.GetComponent<Text>().text = "Current Order Status";
            targetInfoText.GetComponent<Text>().text = "Enemy Ship Info";
            orderTimerText.GetComponent<Text>().text = "Time till Order arrives";
            hideCancelButton();
        }

    }

    public void showSummonButton()
    {
        summonButton.SetActive(true);
    }

    public void hideSummonButton()
    {
        summonButton.SetActive(false);
    }

    public void summonFleet()
    {

        if (GameObject.Find("GameController").GetComponent<GameController>().storedResources >= 60)
        {
            GameObject[] playerShips = GameObject.FindGameObjectsWithTag("playerShip");

            foreach (GameObject ship in playerShips)
            {
                if (!ship.name.Equals("Command"))
                {
                    ship.GetComponent<playerShipController>().returnToCommand();
                }
            }
            GameObject.Find("GameController").GetComponent<GameController>().storedResources -= 60;
        }

    }

    public void showTargetInfo(string targetInfo)
    {
        targetInfoText.GetComponent<Text>().text = targetInfo;
    }

    public void hideTargetInfo()
    {
        targetInfoText.GetComponent<Text>().text = "Enemy Ship Info";
    }

    public void hideCancelButton()
    {
        cancelButtonObject.SetActive(false);
    }

    public void showCancelButton()
    {
        cancelButtonObject.SetActive(true);
    }

    public void cancelButton()
    {
        selectedShip.GetComponent<playerShipController>().cancelOrders();
    }

    public void showOrderTimer(string newText)
    {
        //orderTimerText.SetActive(true);
        orderTimerText.GetComponent<Text>().text = newText;
    }

    public void hideOrderTimer()
    {
        //orderTimerText.SetActive(false);
        orderTimerText.GetComponent<Text>().text = "Time till Order arrives";
    }

    public void refreshResources(int scrap)
    {
        resourcesText.GetComponent<Text>().text = "Scrap: \n" + scrap;
    }

    public void showBuildButton()
    {
        buildButton.SetActive(true);
    }

    public void hideBuildButton()
    {
        buildButton.SetActive(false);
    }

    public void showHideBuildMenu()
    {

        if (buildMenuShown)
        {
            buildMenu.SetActive(false);
            buildMenuShown = false;
        } else
        {
            buildMenu.SetActive(true);
            buildMenuShown = true;
        }
    }

    public void hideBuildMenu()
    {
        buildMenu.SetActive(false);
        buildMenuShown = false;
    }

    public void buildStation(string type)
    {
        if (string.Equals(type, "repairbay"))
        {
            selectedShip.GetComponent<builderController>().buildRepairBay(60);
        } else if (string.Equals(type, "upgradebay"))
        {
            selectedShip.GetComponent<builderController>().buildUpgradeBay(150);
        } else if (string.Equals(type, "radararray"))
        {
            selectedShip.GetComponent<builderController>().buildRadarArray(100);
        }
    }
}
