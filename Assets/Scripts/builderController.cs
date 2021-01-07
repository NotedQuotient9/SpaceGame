using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class builderController : MonoBehaviour
{

    UIController controller;
    bool isSelected;
    public GameObject repairBayToSpawn;
    public GameObject upgradeBayToSpawn;
    public GameObject radarArrayToSpawn;
    GameController gcontroller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("GameController").GetComponent<UIController>();
        gcontroller = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        // check whether this ship is being controlled by the player ship controller
        if (controller.selectedShip == this.gameObject)
        {
            isSelected = true;
        }
        else
        {
            isSelected = false;
        }

        if (isSelected)
        {
            controller.showBuildButton();
        } else
        {
            controller.hideBuildButton();
            controller.hideBuildMenu();
        }

    }

    public void buildRadarArray(int cost)
    {
        if (cost <= gcontroller.storedResources)
        {
            GameObject clone;
            clone = Instantiate(radarArrayToSpawn, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation) as GameObject;
            gcontroller.storedResources -= cost;
        }
    }

    public void buildRepairBay(int cost)
    {
        if (cost <= gcontroller.storedResources)
        {
            GameObject clone;
            clone = Instantiate(repairBayToSpawn, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation) as GameObject;
            gcontroller.storedResources -= cost;
        }
        
    }

    public void buildUpgradeBay(int cost)
    {
        if (cost <= gcontroller.storedResources)
        {
            GameObject clone;
            clone = Instantiate(upgradeBayToSpawn, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation) as GameObject;
            gcontroller.storedResources -= cost;
        }
    }
}
