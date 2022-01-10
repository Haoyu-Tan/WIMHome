using UnityEngine;
using System.Collections.Generic;

public class Grabbable : MonoBehaviour
{

    private Grabber currentGrabber;
    private Vector3 oLocalPos; // local
    private Quaternion oLocalRot; // local
    private Transform oParent;
    private Vector3 oScale;
    private Quaternion grabAngle;
    private Vector3 modelOScale;

    private int overlapNum;
    private bool outOfRoom;

    //private Quaternion lastGrabberAngle;

    [SerializeField]
    private Transform mySelf;
    [SerializeField]
    private Material warningMaterial;
    [SerializeField]
    private Material regularMaterial;
    [SerializeField]
    private Transform indicator;
    public bool isCamera = false;
    public bool isOut = false;
    public bool isUnstable = false;
    // Start is called before the first frame update
    private void Awake()
    {
        currentGrabber = null;
        overlapNum = 0;
        outOfRoom = false;
        if (!isCamera)
        {
            indicator.gameObject.SetActive(false);
        }
    }


    void Update()
    {
    }



    public void UpdatePosAng()
    {
        //outOfRoom = status;
        // status true -> out of bound
        this.transform.position = currentGrabber.transform.position;
        Vector3 lp = this.transform.localPosition;
        lp.y = 0.0f;
        this.transform.localPosition = lp;
        /*
        this.transform.rotation *= deltaAng;
        Vector3 eulers = this.transform.localRotation.eulerAngles;
        eulers.x = 0;
        eulers.z = 0;
        this.transform.localRotation = Quaternion.Euler(eulers);
        */
        DisplayWarningMaterial();

        // For Testing....
        /*
        totalTime += Time.deltaTime;
        Vector3 deltaP = new Vector3(2.5f, 1.0f, 0.0f);
        deltaP *= Time.deltaTime;
        if(totalTime > 2.0f)
        {
            totalTime = 0;
            indi *= -1;
        }
        deltaP *= indi;
        */
        // To here


        //Debug.Log("Position Changed to" + newPos);
    }

    public void UpdatePosAngNor(Vector3 newPos)
    {
        this.transform.position = newPos;
        Vector3 lp = this.transform.localPosition;
        lp.y = 0.0f;
        this.transform.localPosition = lp;

        DisplayWarningMaterial();
    }

    void DisplayWarningMaterial()
    {
        if (this.GetComponent<DReference>())
        {
            this.GetComponent<DReference>().UpdateRefObject();



        }
        if (outOfRoom || overlapNum > 0)
        {
            indicator.gameObject.GetComponent<Renderer>().material = warningMaterial;
        }
        else
        {
            indicator.gameObject.GetComponent<Renderer>().material = regularMaterial;
        }
    }

    public void EnterNewRoom(Transform room)
    {
        if (isOut)
            return;
        this.transform.parent = room;
        this.overlapNum = 0;
        this.outOfRoom = true;
        if(this.GetComponent<DReference>() && !isCamera)
            this.GetComponent<DReference>().ChangeRoom(room.GetComponent<RReference>().referredRoom);
    }

    public void RemoveThis()
    {
        if (this.GetComponent<DReference>())
        {
            this.GetComponent<DReference>().DestroyRefObj();
            Destroy(this.gameObject);
        }

    }

    public void SetCurrentGrabber(Grabber grabber)
    {
        currentGrabber = grabber;
        if (!grabber)
        {
            if (overlapNum > 0 || outOfRoom)
            {
                if (isUnstable)
                {
                    RemoveThis();
                    return;
                }
                this.transform.parent = oParent;
                this.transform.localPosition = oLocalPos;
                this.transform.localRotation = oLocalRot;
                this.transform.localScale = oScale;
                if (!isCamera)
                {
                    Transform room = oParent.GetComponent<RReference>().referredRoom;
                    this.GetComponent<DReference>().ChangeRoom(room);
                }

            }
            else if(isCamera)
            {
                if (this.GetComponent<CameraRef>())
                {
                    this.oParent.GetComponent<RReference>().referredRoom.gameObject.SetActive(false);
                    this.transform.parent.GetComponent<RReference>().referredRoom.gameObject.SetActive(true);
                    this.GetComponent<CameraRef>().myReferenceUpdate();
                }
            }
            else if(isUnstable)
            {
                isUnstable = false;
            }
            // reset to original version
            overlapNum = 0;
            grabModeOff();
        }
        else
        {
            
            oParent = this.transform.parent;
            oScale = this.transform.localScale;
            if (!isCamera)
            {
                oLocalPos = this.transform.localPosition;
                oLocalRot = this.transform.localRotation;
            }
            else
            {
                oLocalPos = this.transform.localPosition;
                oLocalRot = this.transform.localRotation;
            }
            //grabAngle = currentGrabber.transform.rotation;
            // set to grab version
            grabModeOn();

        }
    }

    public void warehouseGeneration()
    {
        // instantiate the global reference
        this.isUnstable = true;
        this.isOut = false;
        this.outOfRoom = true;
        this.GetComponent<DReference>().referObj.GetComponent<Grabbable>().isOut = true;
    }


    private void grabModeOn()
    {
        if (!isCamera)
        {
            mySelf.localPosition += new Vector3(0.0f, 0.4f, 0.0f);
            modelOScale = mySelf.localScale;
            mySelf.localScale = 0.95f * modelOScale;
            indicator.gameObject.SetActive(true);
        }
        if(overlapNum > 0)
            indicator.gameObject.GetComponent<Renderer>().material = warningMaterial;
        else
            indicator.gameObject.GetComponent<Renderer>().material = regularMaterial;
    }

    private void grabModeOff()
    {
        if (!isCamera)
        {
            mySelf.localPosition -= new Vector3(0.0f, 0.4f, 0.0f);
            mySelf.localScale = modelOScale;
            indicator.gameObject.SetActive(false);
        }
        else
        {
            indicator.gameObject.GetComponent<Renderer>().material = regularMaterial;
        }

    }

    public Grabber GetCurrentGrabber()
    {
        return currentGrabber;
    }

    // Check collision
    private void OnTriggerEnter(Collider other)
    {
        // Only check collision when it is grabbed
        if (currentGrabber)
        {
            if (!other.transform.parent)
                return;
            if (other.transform.parent.name == this.transform.parent.name)
            {
                if (overlapNum == 0)
                {
                    indicator.GetComponent<Renderer>().material = warningMaterial;
                }
                overlapNum += 1;
            }
            if(other.transform.name == this.transform.parent.name)
            {
                outOfRoom = false;
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (currentGrabber) {
            if (!other.transform.parent)
                return;
            if (other.transform.parent.name == this.transform.parent.name)
            {
                overlapNum -= 1;
                //Debug.Log("Exit " + other.transform.name);
                //Debug.Log("OverlapNum is " + overlapNum);
            }
            if (other.transform.name == this.transform.parent.name)
            {
                outOfRoom = true;
            }
        } 
    }
}
