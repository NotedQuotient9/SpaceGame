using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boundsController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        GameObject.Find("Main Camera").GetComponent<MapCamera>().MoveEnabled = true;
        GameObject.Find("Main Camera").GetComponent<MapCamera>().verticalScrollSpeed = 50;
        GameObject.Find("Main Camera").GetComponent<MapCamera>().horizontalScrollSpeed = 50;
    }

    private void OnMouseExit()
    {
        GameObject.Find("Main Camera").GetComponent<MapCamera>().MoveEnabled = false;
        GameObject.Find("Main Camera").GetComponent<MapCamera>().verticalScrollSpeed = 0;
        GameObject.Find("Main Camera").GetComponent<MapCamera>().horizontalScrollSpeed = 0;

    }
}
