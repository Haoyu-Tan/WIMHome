using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class RegularGrabber : Grabber
{
    public InputActionProperty grabAction;

    Grabbable currentObject;
    
    Grabbable grabbedObject;
    Quaternion grabAngle;
    //Vector3 DeltaPos;

    //bubble cursor
    private float bubEnableDist;
    private float bubCheckDist = 0.8f;

    [SerializeField] private Transform pointer;


    //bubble cursor
    [SerializeField] private Transform currtRoom;
    [SerializeField] private Transform bubbleCursor;

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

        findBubble();

        if (grabbedObject)
        {
            
            grabbedObject.GetComponent<DReference>().UpdatePosAng(this.transform.position,this.transform.rotation*Quaternion.Inverse(grabAngle));
            grabAngle = this.transform.rotation;
        }
    }

    //bubble cusor find target, only checks grabbable
    GameObject findBubble() {
        
        if (currtRoom)
        {
            float minDist = Mathf.Infinity;
            float secMinDist = Mathf.Infinity;
            Transform firstNeartest = null;
            Transform secondNeartest = null;
            foreach (Transform child in currtRoom) {
                
                if (!child.gameObject.GetComponent<Grabbable>())
                    continue;

                BoxCollider bColidr = child.gameObject.GetComponent<BoxCollider>();
                Vector3 cloestP = bColidr.ClosestPointOnBounds(bubbleCursor.position);
                float dist = Vector3.Distance(bubbleCursor.position, cloestP);

                if (dist < secMinDist) {
                    if (dist < minDist)
                    {
                        secMinDist = minDist;

                        if (firstNeartest)
                            secondNeartest = firstNeartest;


                        minDist = dist;
                        firstNeartest = child;
                    }
                    else {
                        secMinDist = dist;
                        secondNeartest = child;
                    }
                }
            }
            
            
            //minDist longer than threshold
            if (minDist > bubCheckDist)
                return null;

            //find farthest dist from bubble cursor to neartest object
            float minFar = 2*findFarthest(firstNeartest.gameObject);

            

            //second min dist must shorter than threshold
            if (secMinDist > bubCheckDist)
            {
                setBubbleDia(minFar);
                return firstNeartest.gameObject;
            }

            //Debug.Log("neartest: " + firstNeartest.gameObject.name + ", second neartest: " + secondNeartest.gameObject.name);
            //Debug.Log("minFar: " + minFar + ", secMin: " + secMinDist);
            setBubbleDia(Mathf.Min(minFar, 2*secMinDist - 0.01f));
            return firstNeartest.gameObject;
        }

        return null;
    }

    float findFarthest(GameObject target) {

        BoxCollider colidr = target.GetComponent<BoxCollider>();
        if (!colidr) return Mathf.Infinity;

        List<Vector3> colVertex = new List<Vector3>();
        colVertex.Add(colidr.bounds.max);
        colVertex.Add(colidr.bounds.min);
        colVertex.Add(new Vector3(colidr.bounds.max.x, colidr.bounds.max.y, colidr.bounds.min.z));
        colVertex.Add(new Vector3(colidr.bounds.max.x, colidr.bounds.min.y, colidr.bounds.min.z));
        colVertex.Add(new Vector3(colidr.bounds.max.x, colidr.bounds.min.y, colidr.bounds.max.z));
        colVertex.Add(new Vector3(colidr.bounds.min.x, colidr.bounds.min.y, colidr.bounds.max.z));
        colVertex.Add(new Vector3(colidr.bounds.min.x, colidr.bounds.max.y, colidr.bounds.max.z));
        colVertex.Add(new Vector3(colidr.bounds.min.x, colidr.bounds.max.y, colidr.bounds.min.z));

        float maxDist = 0f;
        foreach (Vector3 endP in colVertex) {
            float tempD = Vector3.Distance(bubbleCursor.position, endP);
            if (tempD > maxDist)
                maxDist = tempD;
        }

        return maxDist;
    }

    void setBubbleDia(float dia) {
        bubbleCursor.localScale = new Vector3(dia, dia, dia);
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
