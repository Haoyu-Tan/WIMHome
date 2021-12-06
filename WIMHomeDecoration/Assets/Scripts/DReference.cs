using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DReference : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform referObj;
    float totalTime = 0;
    int indi = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //UpdatePos();
    }

    public void UpdatePosAng(Vector3 newPos,Quaternion deltaAng)
    {
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

        this.transform.position = newPos;
        Vector3 lp = this.transform.localPosition;
        lp.y = 0.0f;
        this.transform.localPosition = lp;
        this.transform.rotation *= deltaAng;
        Vector3 eulers = this.transform.localRotation.eulerAngles;
        eulers.x = 0;
        eulers.z = 0;
        this.transform.localRotation = Quaternion.Euler(eulers);
        if (referObj)
        {
            referObj.transform.localPosition = this.transform.localPosition;
            referObj.transform.localRotation = this.transform.localRotation;
        }
        //Debug.Log("Position Changed to" + newPos);
    }
}
