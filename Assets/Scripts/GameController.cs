using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public int storedResources;
    UIController controller;
    // Start is called before the first frame update
    void Start()
    {
        storedResources = 500;
        controller = this.GetComponent<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        controller.refreshResources(storedResources);
    }
}
