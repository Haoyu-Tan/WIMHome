using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class RegularGrabber : Grabber
{
    public InputActionProperty grabAction;
    public InputActionProperty rotateAction;
    public InputActionProperty RemoveAction;
    Grabbable currentObject;

    Grabbable grabbedObject;
    Quaternion grabAngle;
    //Vector3 DeltaPos;


    //bubble cursor
    private float bubEnableDist = 0.9f;
    private float bubCheckDist = 0.7f;
    private bool grabbed = false;
    private GameObject secondBubble;

    //object manipulation
    private float mDeadZone = 0.75f;
    private float rotSpeed = 90f;
    private float scaleSpeed = 0.3f;
    private float scaleUpperLimit = 1.2f;
    private float scaleLowerLimit = 0.8f;

    //point grabber
    Material disableGrabMaterial;
    Vector3 pointPrevPos;

    bool pointGrabber;

    //wareHouse grabber
    bool wareHouseGrabber;

    //SerializeField
    [SerializeField] private Transform pointer;

    //bubble cursor
    //[SerializeField] 
    private Transform currtRoom;
    [SerializeField]
    private Transform bubbleCursor;
    [SerializeField]
    private GameObject bubblePrefab;
    [SerializeField]
    private Transform cursor;
    [SerializeField]


    private Transform mainCam;
    [SerializeField]
    private WIMController wim;

    [SerializeField]
    private Renderer rcmodel;

    [SerializeField]
    private LineRenderer laserPointer;
    [SerializeField]
    private Material enableGrabMaterial;
    [SerializeField]
    private Transform view;
    [SerializeField]
    private Transform warehouseRoom;


    private Transform pointRoom;

    private string floor = "floor";


    private float removeCountDown;
    private bool removing;

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

        pointGrabber = false;
        laserPointer.enabled = false;
        disableGrabMaterial = laserPointer.material;
        pointPrevPos = Vector3.zero;

        wareHouseGrabber = false;

        //secondBubble = Instantiate(bubblePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        secondBubble = Instantiate(bubblePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        secondBubble.SetActive(false);

        grabAction.action.performed += Grab;
        grabAction.action.canceled += Release;
        RemoveAction.action.performed += RemoveStart;
        RemoveAction.action.canceled += RemoveEnd;
        rotateAction.action.performed += ManipulateModel;
    }

    private void OnDestroy()
    {
        grabAction.action.performed -= Grab;
        grabAction.action.canceled -= Release;

        rotateAction.action.performed -= ManipulateModel;

        RemoveAction.action.performed -= RemoveStart;
        RemoveAction.action.canceled -= RemoveEnd;
    }

    // Update is called once per frame
    void Update()
    {

        disableBubbleCursor();
        disableSecondBubble();
        //currtRoom = wim.currRoom.transform;
        if (!grabbedObject)
        {
            rcmodel.enabled = false;
            pointGrabber = false;
            laserPointer.enabled = false;
            wareHouseGrabber = false;
            findCurrentObject();
        }

        if (grabbedObject)
        {
            //if (wareHouseGrabber) {
            //    grabbedObject.UpdatePosAng();
            //   grabAngle = this.transform.rotation;
            //}
            if (pointGrabber)
            {
                Vector3 currPos = IntersectWithFloor();
                //Vector3 diff = currPos - pointPrevPos;
                //Vector3 dest = (grabbedObject.transform.position + diff) * Time.deltaTime;

                grabbedObject.UpdatePosAngNor(currPos);
                grabAngle = this.transform.rotation;
                rcmodel.enabled = true;
                pointPrevPos = currPos;
            }
            else
            {
                grabbedObject.UpdatePosAng();
                grabAngle = this.transform.rotation;
            }




            if (removing)
            {
                if (grabbedObject.isCamera)
                {
                    removing = false;
                }
                else
                {
                    removeCountDown -= Time.deltaTime;
                    if (removeCountDown <= 0)
                    {
                        grabbedObject.RemoveThis();
                        grabbedObject = null;
                        removing = false;
                    }
                }
            }
            else
            {
                if (removing) removing = false;
            }

        }

    }
        public void reverseRotation(float angle)
        {
            if (this.grabbedObject)
            {
                float localY = this.grabbedObject.transform.localRotation.eulerAngles.y;
                localY -= angle;
                this.grabbedObject.transform.localRotation = Quaternion.Euler(new Vector3(0, localY, 0));
            }
        }


        //bubble cusor find currentObject, only checks grabbable
        //point grabber find currentObject, only when in current room and outside WIM box
        GameObject findCurrentObject()
        {
            if (cursorInsideWarehouse(cursor))
            {
                wareHouseGrabber = true;
                Transform firstNeartest = bubbleCursorFindTarget(warehouseRoom);
                currentObject = firstNeartest.gameObject.GetComponent<Grabbable>();

                return firstNeartest.gameObject;

            }
            else if (currtRoom && cursorInsideBox(cursor))
            {

                Transform firstNeartest = bubbleCursorFindTarget(currtRoom);
                currentObject = firstNeartest.gameObject.GetComponent<Grabbable>();

                return firstNeartest.gameObject;
            }
            else if (currtRoom) //&& !cursorInsideBox(cursor))
            {
                rcmodel.enabled = true;
                pointRoom = view.parent;
                pointGrabber = true;
                laserPointer.enabled = true;

                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
                {
                    laserPointer.SetPosition(1, new Vector3(0, 0, hit.distance));

                    if (hit.collider.GetComponent<Grabbable>())
                    {
                        //prepare for move, calculate the position
                        //pointPrevPos = IntersectWithFloor();

                        laserPointer.material = enableGrabMaterial;
                        currentObject = hit.collider.GetComponent<Grabbable>();
                        return currentObject.gameObject;
                    }
                    else
                    {
                        laserPointer.material = disableGrabMaterial;
                    }
                }
                else
                {
                    laserPointer.SetPosition(1, new Vector3(0, 0, 40));
                    laserPointer.material = disableGrabMaterial;
                }

            }
            currentObject = null;
            return null;
        }

        //check if the line renderer laserpoint is intersect with floor
        Vector3 IntersectWithFloor()
        {
            Vector3 res = grabbedObject.transform.position;
            laserPointer.SetPosition(1, new Vector3(0, 0, 30));
            Transform flr = pointRoom.GetComponent<RReference>().referredRoom.Find(floor);
            if (flr)
            {
                float dist = 0f;
                Ray myRay = new Ray(transform.position, transform.TransformDirection(Vector3.forward));

                if (flr.gameObject.GetComponent<MeshRenderer>().bounds.IntersectRay(myRay, out dist))
                {

                    laserPointer.SetPosition(1, new Vector3(0, 0, dist));
                    res = transform.position + transform.TransformDirection(Vector3.forward) * dist;
                    //Debug.Log("my ray: " + myRay + ", dist: " + dist + ", res: " + res);

                }

            }

            return res;
        }

        Transform bubbleCursorFindTarget(Transform currRoom)
        {
            float minDist = Mathf.Infinity;
            float secMinDist = Mathf.Infinity;
            Transform firstNeartest = null;
            Transform secondNeartest = null;
            foreach (Transform child in currRoom)
            {

                if (!child.gameObject.GetComponent<Grabbable>())
                    continue;

                BoxCollider bColidr = child.gameObject.GetComponent<BoxCollider>();
                Vector3 cloestP = bColidr.ClosestPointOnBounds(bubbleCursor.position);
                float dist = Vector3.Distance(bubbleCursor.position, cloestP);

                if (dist < secMinDist)
                {
                    if (dist < minDist)
                    {
                        secMinDist = minDist;

                        if (firstNeartest)
                            secondNeartest = firstNeartest;


                        minDist = dist;
                        firstNeartest = child;
                    }
                    else
                    {
                        secMinDist = dist;
                        secondNeartest = child;
                    }
                }
            }


            //minDist longer than threshold
            if (minDist > bubCheckDist)
            {
                currentObject = null;
                return null;
            }

            enableBubbleCursor();

            //find farthest dist from bubble cursor to neartest object
            float minFar = 2 * findFarthest(firstNeartest.gameObject) + 0.01f;

            //second min dist must shorter than threshold
            if (secMinDist > bubCheckDist)
                setBubbleDia(minFar);
            else
            {
                setBubbleDia(Mathf.Min(minFar, 2 * secMinDist - 0.01f));

                //if our radius is limited by the second neartest object, extend a second bubble
                //!!!add a constraint: if current bubble not able to cover all the furniture
                if (minFar > 2 * secMinDist)
                {
                    enableSecondBubble(firstNeartest);
                }
            }


            return firstNeartest;
        }

        float findFarthest(GameObject target)
        {

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
            foreach (Vector3 endP in colVertex)
            {
                float tempD = Vector3.Distance(bubbleCursor.position, endP);
                if (tempD > maxDist)
                    maxDist = tempD;
            }

            return maxDist;
        }

        void setBubbleDia(float dia)
        {
            bubbleCursor.localScale = new Vector3(dia, dia, dia);
        }

    public override void Grab(InputAction.CallbackContext context)
    {
        grabbed = true;
        if (currentObject && grabbedObject == null)
        {
            if (currentObject.GetCurrentGrabber() != null)
            {
                currentObject.GetCurrentGrabber().Release(new InputAction.CallbackContext());
            }

            if (wareHouseGrabber)
            {
                secondBubble.transform.parent = this.transform;
                //Debug.Log("Suprise! I am here~~~~");
                grabbedObject = Instantiate(currentObject.gameObject, currentObject.transform.position,
                    currentObject.transform.rotation,currtRoom.transform).GetComponent<Grabbable>();
                Transform parent = currtRoom.GetComponent<RReference>().referredRoom;
                Transform referredObj = Instantiate(currentObject.gameObject, parent).transform;
                grabbedObject.GetComponent<DReference>().referObj = referredObj;
                referredObj.GetComponent<DReference>().referObj = grabbedObject.transform;
                grabbedObject.warehouseGeneration();
            }
            else
            {
                grabbedObject = currentObject;
            }
            grabbedObject.SetCurrentGrabber(this);
            Debug.Log("I am in core");
            grabAngle = this.transform.rotation;
            //rcmodel.enabled = false;
        }
    }

    public void RemoveStart(InputAction.CallbackContext context)
    {
        removing = true;
        removeCountDown = 0.5f;
    }

    public void RemoveEnd(InputAction.CallbackContext context)
    {
        removing = false;
    }



    public override void Release(InputAction.CallbackContext context)
    {
        grabbed = false;
        if (grabbedObject)
        {
            /*
            if (wareHouseGrabber) {
                
                if (cursorInsideBox(cursor))
                {
                    grabbedObject.transform.SetParent(currtRoom);
                    grabbedObject.transform.localPosition = new Vector3(grabbedObject.transform.localPosition.x, 0, grabbedObject.transform.localPosition.z);

                }
                else
                {
                    Debug.Log(grabbedObject.transform.childCount);
                    Destroy(grabbedObject.gameObject);
                }
            }
            */

            if (grabbedObject != null)
            {
                grabbedObject.SetCurrentGrabber(null);
                grabbedObject = null;
            }

            //rcmodel.enabled = true;

        }
    }

    public void ManipulateModel(InputAction.CallbackContext context)
    {
        if (grabbed && grabbedObject)
        {
            Vector2 inputAxes = context.action.ReadValue<Vector2>();

            if (inputAxes.x >= mDeadZone)
            {
                grabbedObject.transform.Rotate(new Vector3(0, Time.deltaTime * rotSpeed, 0), Space.Self);
            }
            else if (inputAxes.x <= -mDeadZone)
            {
                grabbedObject.transform.Rotate(new Vector3(0, (-1.0f) * Time.deltaTime * rotSpeed, 0), Space.Self);
            }
            else
            {
                UnUniformScale(inputAxes);
            }

        }
    }

    void UniformScale(Vector2 inputAxes)
    {

        if (inputAxes.y >= mDeadZone)
        {
            grabbedObject.transform.localScale += new Vector3(scaleSpeed * Time.deltaTime, 0f, scaleSpeed * Time.deltaTime);
            if (grabbedObject.transform.localScale.x > scaleUpperLimit || grabbedObject.transform.localScale.z > scaleUpperLimit)
                grabbedObject.transform.localScale = new Vector3(scaleUpperLimit, 1f, scaleUpperLimit);
            grabbedObject.GetComponent<DReference>().UpdateRefScale();
        }
        else if (inputAxes.y <= -mDeadZone)
        {
            grabbedObject.transform.localScale -= new Vector3(scaleSpeed * Time.deltaTime, 0f, scaleSpeed * Time.deltaTime);
            if (grabbedObject.transform.localScale.x < scaleLowerLimit || grabbedObject.transform.localScale.z < scaleLowerLimit)
                grabbedObject.transform.localScale = new Vector3(scaleLowerLimit, 1f, scaleLowerLimit);
            grabbedObject.GetComponent<DReference>().UpdateRefScale();
        }

    }

    void UnUniformScale(Vector2 inputAxes)
    {
        //scaling: self.forward and self.right compare with rig.forward => use the angle
        if (inputAxes.y >= mDeadZone || inputAxes.y <= -mDeadZone)
        {
            if (grabbedObject.isCamera) return;
            Vector3 newForward = new Vector3(grabbedObject.transform.forward.x, 0f, grabbedObject.transform.forward.z);
            Vector3 newRight = new Vector3(grabbedObject.transform.right.x, 0f, grabbedObject.transform.right.z);
            Vector3 newCam = new Vector3(mainCam.forward.x, 0f, mainCam.forward.z);

            float angle1 = Mathf.Abs(Mathf.Asin(Vector3.Cross(newForward, newCam).magnitude / (newForward.magnitude * newCam.magnitude)));
            float angle2 = Mathf.Abs(Mathf.Asin(Vector3.Cross(newRight, newCam).magnitude / (newRight.magnitude * newCam.magnitude)));

            //Debug.Log("angle1: " + angle1 + ", angle2: " + angle2);

            if (angle1 > angle2)
            {
                //scale z
                //Debug.Log("scale z");

                grabbedObject.transform.localScale += new Vector3(0f, 0f, Mathf.Sign(inputAxes.y) * scaleSpeed) * Time.deltaTime;
                if (grabbedObject.transform.localScale.z > scaleUpperLimit)
                    grabbedObject.transform.localScale = new Vector3(grabbedObject.transform.localScale.x, grabbedObject.transform.localScale.y, scaleUpperLimit);
                else if (grabbedObject.transform.localScale.z < scaleLowerLimit)
                    grabbedObject.transform.localScale = new Vector3(grabbedObject.transform.localScale.x, grabbedObject.transform.localScale.y, scaleLowerLimit);

            }
            else
            {
                //scale x
                //Debug.Log("scale x");
                grabbedObject.transform.localScale += new Vector3(Mathf.Sign(inputAxes.y) * scaleSpeed, 0f, 0f) * Time.deltaTime;
                if (grabbedObject.transform.localScale.x > scaleUpperLimit)
                    grabbedObject.transform.localScale = new Vector3(scaleUpperLimit, grabbedObject.transform.localScale.y, grabbedObject.transform.localScale.z);
                else if (grabbedObject.transform.localScale.x < scaleLowerLimit)
                    grabbedObject.transform.localScale = new Vector3(scaleLowerLimit, grabbedObject.transform.localScale.y, grabbedObject.transform.localScale.z);
            }
            if (grabbedObject.GetComponent<DReference>())
                grabbedObject.GetComponent<DReference>().UpdateRefScale();
        }
    }

    bool cursorInsideWarehouse(Transform cursor)
    {
        if (wim.WareHouseIsOn() && warehouseRoom)
        {
            BoxCollider bxclidr = warehouseRoom.GetComponent<BoxCollider>();
            if (bxclidr.bounds.Contains(cursor.position)) return true;
        }

        return false;
    }

    bool cursorInsideBox(Transform cursor)
    {
        if (currtRoom)
        {
            //Debug.Log("current room is " + currtRoom.name);
            BoxCollider bxclidr = currtRoom.GetComponent<BoxCollider>();
            if (bxclidr.bounds.Contains(cursor.position))
            {
                return true;
            }
        }

        return false;
    }

    public void resetCurrentRoom(Transform newRoom)
    {
        this.currtRoom = newRoom;
        secondBubble.transform.parent = this.transform;
        if (this.grabbedObject)
            this.grabbedObject.EnterNewRoom(newRoom);
    }

    void enableSecondBubble(Transform firstNeartest)
    {
        secondBubble.SetActive(true);
        BoxCollider bclidr = firstNeartest.gameObject.GetComponent<BoxCollider>();
        secondBubble.transform.SetParent(firstNeartest);
        secondBubble.transform.localPosition = bclidr.center;
        secondBubble.transform.localScale = bclidr.size * 1.2f;
        secondBubble.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
    }

    void disableSecondBubble()
    {
        if (!secondBubble)
            secondBubble = Instantiate(bubblePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        secondBubble.SetActive(false);
    }

    void enableBubbleCursor()
    {
        bubbleCursor.gameObject.SetActive(true);
    }

    void disableBubbleCursor()
    {
        bubbleCursor.gameObject.SetActive(false);
    }

}
