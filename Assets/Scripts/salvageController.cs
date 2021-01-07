using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class salvageController : MonoBehaviour
{

    UIController controller;
    bool isSelected;
    GameObject targetScrap;
    bool scrapping;
    bool scrappingBegun;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("GameController").GetComponent<UIController>();
        scrapping = false;
        scrappingBegun = false;
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


            // if ship is selected and mouse clicked, check if it clicked on scrap
            // if yes then scrap becomes target and ship begins scrapping
        if (Input.GetMouseButtonDown(0) && isSelected)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "scrap")
                {
                    
                    scrapping = true;
                    targetScrap = hit.transform.gameObject;

                }
            }

        }

        // if ship is scrapping then move until scrap is reached, then begin collecting
        if (scrapping)
        {

            if (Vector3.Distance(this.transform.position, targetScrap.transform.position) <= 5)
            {

                //this.gameObject.GetComponent<playerShipController>().changeOrder("Collecting Scrap");
                this.gameObject.GetComponent<playerShipController>().currentOrder = "Collecting Scrap";
                if (!scrappingBegun)
                {
                    StartCoroutine(collectScrap());
                }
                
            }
        }

    }

    // collect scrap by waiting 2 seconds and adding value of scrap to player resources, then scrap is destroyed
    // scrappingBegun is used so coroutine only starts once
    IEnumerator collectScrap()
    {
        scrappingBegun = true;
        yield return new WaitForSeconds(2f);
        //this.gameObject.GetComponent<playerShipController>().changeOrder("Ready to Recieve Orders");
        this.gameObject.GetComponent<playerShipController>().currentOrder = "Ready to Recieve Orders";

        GameObject.Find("GameController").GetComponent<GameController>().storedResources += targetScrap.GetComponent<scrapController>().value;
        scrapping = false;
        scrappingBegun = false;
        Destroy(targetScrap);
 
    }
}
