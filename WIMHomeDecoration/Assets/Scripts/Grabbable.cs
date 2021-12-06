using UnityEngine;
using System.Collections.Generic;

public class Grabbable : MonoBehaviour
{

    private Grabber currentGrabber;
    private Vector3 oLocalPos; // local
    private Quaternion oLocalRot; // local
    private int overlapNum;
    [SerializeField]
    private Transform mySelf;
    [SerializeField]
    private Material warningMaterial;
    [SerializeField]
    private Material regularMaterial;
    [SerializeField]
    private Transform indicator;

    // Start is called before the first frame update
    void Start()
    {
        currentGrabber = null;
        if (this.GetComponent<Rigidbody>())
        {
            this.GetComponent<Rigidbody>().Sleep();
        }
        overlapNum = 0;
    }

    public void SetCurrentGrabber(Grabber grabber)
    {
        currentGrabber = grabber;
        if (!grabber)
        {
            if (overlapNum > 0)
            {
                this.transform.localPosition = oLocalPos;
                this.transform.localRotation = oLocalRot;
            }
            // reset to original version
            overlapNum = 0;
            grabModeOff();
        }
        else
        {
            
            oLocalPos = this.transform.localPosition;
            oLocalRot = this.transform.localRotation;

            // set to grab version
            grabModeOn();

        }
    }

    private void grabModeOn()
    {
        mySelf.localPosition = new Vector3(0.0f, 0.4f, 0.0f);
        mySelf.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        indicator.gameObject.SetActive(true);
        indicator.gameObject.GetComponent<Renderer>().material = regularMaterial;
    }

    private void grabModeOff()
    {
        mySelf.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        mySelf.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        indicator.gameObject.SetActive(false);
    }

    public Grabber GetCurrentGrabber()
    {
        return currentGrabber;
    }

    // Check collision
    private void OnTriggerEnter(Collider other)
    {
        // Only check collision when it is grabbed
        if (currentGrabber && other.transform.parent.name == this.transform.parent.name)
        {
            if(overlapNum == 0)
            {
                indicator.GetComponent<Renderer>().material = warningMaterial;
            }
            overlapNum += 1;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentGrabber && other.transform.parent.name == this.transform.parent.name)
        {
            overlapNum -= 1;
            if(overlapNum == 0)
            {
                indicator.GetComponent<Renderer>().material = regularMaterial;
            }
        }
    }
}
