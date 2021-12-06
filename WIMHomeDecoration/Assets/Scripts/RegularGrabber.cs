using UnityEngine;
using UnityEngine.InputSystem;

public class RegularGrabber : Grabber
{
    public InputActionProperty grabAction;

    Grabbable currentObject;
    
    Grabbable grabbedObject;
    Quaternion grabAngle;
    //Vector3 DeltaPos;

    [SerializeField] private Transform pointer;


    //private Material originalGBMaterial;
    //[SerializeField]
    //private Material grabbingMaterial;
    //[SerializeField]
    //private Transform spindle;
    //bool usingSpindle = true;

    // Start is called before the first frame update
    void Start()
    {
        grabbedObject = null;
        currentObject = null;

        grabAction.action.performed += Grab;
        grabAction.action.canceled += Release;
    }

    private void OnDestroy()
    {
        grabAction.action.performed -= Grab;
        grabAction.action.canceled -= Release;
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbedObject)
        {
            
            grabbedObject.GetComponent<DReference>().UpdatePosAng(this.transform.position,this.transform.rotation*Quaternion.Inverse(grabAngle));
            grabAngle = this.transform.rotation;
        }
    }

    public override void Grab(InputAction.CallbackContext context)
    {
        if (currentObject && grabbedObject == null)
        {
            if (currentObject.GetCurrentGrabber() != null)
            {
                currentObject.GetCurrentGrabber().Release(new InputAction.CallbackContext());
            }

            grabbedObject = currentObject;
            grabbedObject.SetCurrentGrabber(this);
            grabAngle = this.transform.rotation;
            //originalGBMaterial = grabbedObject.gameObject.GetComponent<Renderer>().material;
            //grabbedObject.gameObject.GetComponent<Renderer>().material = grabbingMaterial;
            //grabbedObject.transform.parent = spindle;
            //grabbedObject.transform.localPosition = Vector3.zero;
            //spindle.GetComponent<MeshRenderer>().enabled = false;
            //grabbedObject.transform.parent = this.transform;
        }
    }

    public override void Release(InputAction.CallbackContext context)
    {
        if (grabbedObject)
        {
            ////if (grabbedObject.GetComponent<Rigidbody>())
            //{
                //grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                //grabbedObject.GetComponent<Rigidbody>().useGravity = true;
            //}

            grabbedObject.SetCurrentGrabber(null);
            //grabbedObject.transform.parent = null;
            grabbedObject = null;
            //grabbedObject.gameObject.GetComponent<Renderer>().material = originalGBMaterial;
            //spindle.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (currentObject == null && other.GetComponent<Grabbable>())
        {
            
            currentObject = other.gameObject.GetComponent<Grabbable>();
            pointer.gameObject.SetActive(true);
            pointer.parent = currentObject.transform;
            pointer.localPosition = new Vector3 (0f,1f,0f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (currentObject)
        {
            if (other.GetComponent<Grabbable>() && currentObject.GetInstanceID() == other.GetComponent<Grabbable>().GetInstanceID())
            {
                currentObject = null;
                pointer.gameObject.SetActive(false);
            }
        }
    }
}
