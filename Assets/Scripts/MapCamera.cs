using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCamera : MonoBehaviour
{

    public float verticalScrollArea = 10f;
    public float horizontalScrollArea = 10f;
    public float verticalScrollSpeed = 10f;
    public float horizontalScrollSpeed = 10f;


#pragma warning disable 0649
    [SerializeField]
    private GameObject GO_MainCamera;
#pragma warning restore 0649

    public bool ZoomEnabled = true;
    public bool MoveEnabled = true;
    public bool CombinedMovement = true;

    private Vector2 _mousePos;
    private Vector3 _moveVector;
    private float _xMove;
    private float _yMove;
    private float _zMove;

    private float mouseScrollDelta = 0f;


    public void EnableControls(bool _enable)
    {

        if (_enable)
        {
            ZoomEnabled = true;
            MoveEnabled = true;
            CombinedMovement = true;
        }
        else
        {
            ZoomEnabled = false;
            MoveEnabled = false;
            CombinedMovement = false;
        }
    }


    private void Start()
    {
        this.transform.position = new Vector3(GameObject.Find("Command").transform.position.x, transform.position.y, GameObject.Find("Command").transform.position.z);
    }

    void Update()
    {
        _mousePos = Input.mousePosition;

        //Move camera if mouse is at the edge of the screen
        if (MoveEnabled)
        {

            //Move camera if mouse is at the edge of the screen
            if (_mousePos.x < horizontalScrollArea)
            {
                _xMove = -1;
            }
            else if (_mousePos.x >= Screen.width - horizontalScrollArea)
            {
                _xMove = 1;
            }
            else
            {
                _xMove = 0;
            }

            if (_mousePos.y < verticalScrollArea)
            {
                _zMove = -1;
            }
            else if (_mousePos.y >= Screen.height - verticalScrollArea)
            {
                _zMove = 1;
            }
            else
            {
                _zMove = 0;
            }

            //Move camera if wasd or arrow keys are pressed
            float xAxisValue = Input.GetAxis("Horizontal");
            float zAxisValue = Input.GetAxis("Vertical");

            if (xAxisValue != 0)
            {
                if (CombinedMovement)
                {
                    _xMove += xAxisValue;
                }
                else
                {
                    _xMove = xAxisValue;
                }
            }

            if (zAxisValue != 0)
            {
                if (CombinedMovement)
                {
                    _zMove += zAxisValue;
                }
                else
                {
                    _zMove = zAxisValue;
                }
            }

        }
        else
        {
            _xMove = 0;
            _yMove = 0;
        }


        // Scroll with physical mouse wheel
        if (Input.mouseScrollDelta.y != 0)
        {
            mouseScrollDelta = Input.mouseScrollDelta.y;
            Zoom("Mouse");
        }

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //move the object
            MoveMe(_xMove, _yMove, _zMove);

        }



}

    private void MoveMe(float x, float y, float z)
    {
        _moveVector = (new Vector3(x * horizontalScrollSpeed,
        y * verticalScrollSpeed, z * horizontalScrollSpeed) * Time.deltaTime);
        transform.Translate(_moveVector, Space.World);
    }

    // Only used for processing laptop touchpad scroll
    public void OnGUI()
    {
        // Input.mouseScrollDelta.y stays at 0 if laptop touchpad was used to scroll
        if (Event.current.isScrollWheel && Input.mouseScrollDelta.y == 0)
        {
            if (Event.current.delta.y > 0)
            {
                mouseScrollDelta = 1f;
                Zoom("Mouse");
            }
            else
            {
                mouseScrollDelta = -1f;
                Zoom("Mouse");
            }
        }
        else { }
    }


    private void Zoom(string inputType)
    {
        Vector3 currentPositionCamera = GO_MainCamera.transform.position;
        float zoomOffset = 0f;
        Vector3 newCameraPosition;

        if (inputType == "Mouse")
        {
            zoomOffset = mouseScrollDelta; // Global settings variable replaced by 1f                                                        
        }
        else if (inputType == "Touch")
        {
            // Method when touch display is used
        }
        else { }

        Vector3 zoomMovement = new Vector3(0f, -zoomOffset * 2, 0f);
        newCameraPosition = currentPositionCamera + zoomMovement;
        GO_MainCamera.transform.position = newCameraPosition;
    }


}


/// <summary>
/// Camera zoom depending on mouse (wheel) or touch (pinch) input
/// </summary>
/// <param name="inputType"></param>
/// 

//    // Zoom Camera in or out
//    if (ZoomEnabled)
//    {
//        if (Input.GetAxis("Mouse ScrollWheel") < 0)
//        {
//            _yMove = 1;
//        }
//        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
//        {
//            _yMove = -1;
//        }
//        else
//        {
//            _yMove = 0;
//        }
//    }
//    else
//    {
//        _zMove = 0;
//    }