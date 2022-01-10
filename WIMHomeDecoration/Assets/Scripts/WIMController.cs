using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine;

public class WIMController : MonoBehaviour
{
    public InputActionProperty manipulateAction;
    public InputActionProperty triggerAction;
    public InputActionProperty wareHouseAction;

    //private
    private float angleY = 0f;
    private int roomIndex = 0;
    private float deadZone = 0.75f;
    private float rotSpeed = 90f;
    private float scaleSpeed = 0.3f;
    private float scaleUpperLimit = 3.0f;
    private float scaleLowerLimit = 0.2f;
    private bool changeRoom = false;
    private bool turnOnWareHouse = false;

    //default
    float coolDown = 0;

    //public
    public GameObject currRoom;

    //Serialize
    [SerializeField]
    private GameObject[] rooms;
    [SerializeField]
    private Transform currWIM;
    [SerializeField]
    private Transform mainCam;
    [SerializeField]
    private RegularGrabber rightHand;
    [SerializeField]
    private Transform view;
    [SerializeField]
    private Transform XRRig;
    [SerializeField]
    private GameObject wareHouse;

    private float minScale = 0.3f;
    private float maxScale = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        manipulateAction.action.performed += ManipulateWIM;
        triggerAction.action.performed += TriggerDown;
        triggerAction.action.canceled += TriggerUp;
        wareHouseAction.action.performed += WareHouseOnOff;
        currRoom = rooms[roomIndex];
        rightHand.resetCurrentRoom(currRoom.transform);
        currRoom.SetActive(true);
        view.SetParent(currRoom.transform, false);
        view.localPosition = XRRig.position;
        wareHouse.SetActive(false);
    }




    private void OnDestroy()
    {
        manipulateAction.action.performed -= ManipulateWIM;
        triggerAction.action.performed -= TriggerDown;
        triggerAction.action.canceled -= TriggerUp;
        wareHouseAction.action.performed -= WareHouseOnOff;
    }

    // Update is called once per frame
    void Update()
    {
        if (coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }
        // currRoom.transform.localRotation = Quaternion.Euler(new Vector3(0, angleY, 0));
    }



    public void ManipulateWIM(InputAction.CallbackContext context) {
        Vector2 inputAxes = context.action.ReadValue<Vector2>();
        if (currWIM && !changeRoom) {
            

            if (inputAxes.x >= deadZone) {
                float deltaAngle = Time.deltaTime * rotSpeed;
                angleY = currRoom.transform.localRotation.eulerAngles.y + deltaAngle;
                //if (angleY >= 360.0f)
                //    angleY -= 360.0f;
                currRoom.transform.localRotation = Quaternion.Euler(new Vector3(0, angleY, 0));
                rightHand.reverseRotation(deltaAngle);
                //currWIM.Rotate(new Vector3(0, Time.deltaTime * rotSpeed, 0), Space.Self);
            }
            else if (inputAxes.x <= -deadZone) {
                float deltaAngle = -Time.deltaTime * rotSpeed;
                angleY = currRoom.transform.localRotation.eulerAngles.y + deltaAngle;
                //if (angleY >= 360.0f)
                //    angleY -= 360.0f;
                currRoom.transform.localRotation = Quaternion.Euler(new Vector3(0, angleY, 0));
                rightHand.reverseRotation(deltaAngle);
                //currWIM.Rotate(new Vector3(0, (-1.0f) * Time.deltaTime * rotSpeed, 0), Space.Self);
            }

            
            //scaling: self.forward and self.right compare with rig.forward => use the angle
            if (inputAxes.y >= deadZone || inputAxes.y <= -deadZone) {
                /*
                float angle1 = Mathf.Abs(Mathf.Asin(Vector3.Cross(currWIM.forward, mainCam.forward).magnitude / (currWIM.forward.magnitude * mainCam.forward.magnitude)));
                float angle2 = Mathf.Abs(Mathf.Asin(Vector3.Cross(currWIM.right, mainCam.forward).magnitude / (currWIM.right.magnitude * mainCam.forward.magnitude)));

                //Debug.Log("angle1: " + angle1 + ", angle2: " + angle2);

                if (angle1 > angle2)
                {
                    //scale x
                    Debug.Log("scale z");
                    currWIM.localScale += new Vector3(0f, 0f, Mathf.Sign(inputAxes.y) * scaleSpeed) * Time.deltaTime;
                }
                else {
                    //scale z
                    Debug.Log("scale x");
                    currWIM.localScale += new Vector3(Mathf.Sign(inputAxes.y) * scaleSpeed, 0f, 0f) * Time.deltaTime;
                }
                */
                currWIM.localScale += Vector3.one * Mathf.Sign(inputAxes.y) * scaleSpeed * Time.deltaTime;
                if(currWIM.localScale.x > maxScale)
                {
                    currWIM.localScale = Vector3.one * maxScale;
                }
                if(currWIM.localScale.x < minScale)
                {
                    currWIM.localScale = Vector3.one * minScale;
                }
            }
            
        }
        if (changeRoom && coolDown <= 0)
        {
            if ( inputAxes.x >= deadZone)
            {
                roomIndex += 1;
                if (roomIndex >= rooms.Length)
                    roomIndex = 0;
                
                GameObject nextRoom = rooms[roomIndex];
                nextRoom.SetActive(true);
                coolDown = 0.5f;
                rightHand.resetCurrentRoom(nextRoom.transform);
                currRoom.SetActive(false);
                currRoom = nextRoom;
            }
            else if (inputAxes.x <= -deadZone)
            {
                roomIndex -= 1;
                if (roomIndex < 0)
                    roomIndex = rooms.Length - 1;
                GameObject nextRoom = rooms[roomIndex];
                nextRoom.SetActive(true);
                coolDown = 0.5f;
                rightHand.resetCurrentRoom(nextRoom.transform);
                currRoom.SetActive(false);
                currRoom = nextRoom;
            }
 

        }
    }


    public void TriggerUp(InputAction.CallbackContext context)
    {
        changeRoom = false;
    }

    public void TriggerDown(InputAction.CallbackContext context)
    {
        changeRoom = true;
    }

    public void WareHouseOnOff(InputAction.CallbackContext context) {
        turnOnWareHouse = !turnOnWareHouse;

        if (turnOnWareHouse)
        {
            wareHouse.SetActive(true);
        }
        else {
            wareHouse.SetActive(false);
        }
    }

    public bool WareHouseIsOn() {
        return turnOnWareHouse;
    }
}
