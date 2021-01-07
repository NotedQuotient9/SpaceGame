using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnController : MonoBehaviour
{

    public GameObject playerCommandShip;
    public GameObject enemyCommandShip;
    public GameObject playerFighter;
    public GameObject playerSalvage;
    public GameObject playerBuilder;
    public GameObject enemyFighter;
    public GameObject playerScout;
    Vector3 newPlayerCommandPosition;
    Vector3 newEnemyCommandPosition;

    // Start is called before the first frame update

    //void PullTrigger()
    //{
    //    Transform baseAmmoTransform = loadedAmmo.GetComponentsInChildren<Transform>(true)[0];
    //    Quaternion baseAmmoRotation = baseAmmoTransform.rotation;
    //    Instantiate(loadedAmmo, transform.position, ammoRotation);
    //    GameObject shot = (GameObject)Instantiate(loadedAmmo, transform.position, ammoRotation);
    //    shot.sendMessage("GoTowards", transform.forward);
    //}



    void Start()
    {
        Transform basePlayerCommandShipTransform = playerCommandShip.GetComponent<Transform>();
        Quaternion basePlayerCommandShipRotation = basePlayerCommandShipTransform.rotation;
        GameObject clone;

        newPlayerCommandPosition = new Vector3(Random.Range(-200, 200), transform.position.y, Random.Range(-200, 200));
        newEnemyCommandPosition = new Vector3(Random.Range(-200, 200), transform.position.y, Random.Range(-200, 200));

        if (!checkDistance())
        {
            checkDistance();
        }
        

        clone = Instantiate(playerCommandShip, newPlayerCommandPosition, basePlayerCommandShipRotation) as GameObject;
        clone.name = "Command";
        spawnPlayerShips(basePlayerCommandShipRotation);

        Transform baseEnemyCommandShipTransform = enemyCommandShip.GetComponent<Transform>();
        Quaternion baseEnemyCommandShipRotation = baseEnemyCommandShipTransform.rotation;
        GameObject clone2;

        
        clone2 = Instantiate(enemyCommandShip, newEnemyCommandPosition, baseEnemyCommandShipRotation) as GameObject;
        clone2.name = "EnemyCommand";
        spawnEnemyShips(baseEnemyCommandShipRotation);
    }

    bool checkDistance()
    {

        if (Vector3.Distance(newPlayerCommandPosition, newEnemyCommandPosition) >= 200)
        {
            return true;
        } else
        {
            return false;
        }

    }

    void spawnPlayerShips(Quaternion rotation)
    {
        for (int i = 0; i < Random.Range(10,15); i++)
        {
            GameObject clone;
            clone = Instantiate(playerFighter, new Vector3((newPlayerCommandPosition.x + Random.Range(-30, 30)), transform.position.y, (newPlayerCommandPosition.z + Random.Range(-30, 30))), rotation) as GameObject;
            clone.name = "Player Fighter " + i;
        }

        for (int i = 0; i < Random.Range(2, 5); i++)
        {
            GameObject clone;
            clone = Instantiate(playerSalvage, new Vector3((newPlayerCommandPosition.x + Random.Range(-30, 30)), transform.position.y, (newPlayerCommandPosition.z + Random.Range(-30, 30))), rotation) as GameObject;
            clone.name = "Player Salvage " + i;
        }

        for (int i = 0; i < Random.Range(1, 2); i++)
        {
            GameObject clone;
            clone = Instantiate(playerBuilder, new Vector3((newPlayerCommandPosition.x + Random.Range(-30, 30)), transform.position.y, (newPlayerCommandPosition.z + Random.Range(-30, 30))), rotation) as GameObject;
            clone.name = "Player Builder " + i;
        }


        for (int i = 0; i < Random.Range(3, 6); i++)
        {
            GameObject clone;
            clone = Instantiate(playerScout, new Vector3((newPlayerCommandPosition.x + Random.Range(-30, 30)), transform.position.y, (newPlayerCommandPosition.z + Random.Range(-30, 30))), rotation) as GameObject;
            clone.name = "Player Scout " + i;
        }


    }

    void spawnEnemyShips(Quaternion rotation)
    {
        for (int i = 0; i < Random.Range(15, 20); i++)
        {
            GameObject clone;
            clone = Instantiate(enemyFighter, new Vector3((newEnemyCommandPosition.x + Random.Range(-30, 30)), transform.position.y, (newEnemyCommandPosition.z + Random.Range(-30, 30))), rotation) as GameObject;
            clone.name = "Enemy Fighter " + i;
            clone.GetComponent<enemyShipController>().health = Random.Range(60, 120);
            clone.GetComponent<enemyShipController>().damage = Random.Range(5, 20);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
