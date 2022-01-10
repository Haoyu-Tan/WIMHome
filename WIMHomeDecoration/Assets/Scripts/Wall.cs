using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isOut = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOut) return;
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 myForward = this.transform.forward;
        float dp = Vector3.Dot(camForward, myForward);
        if(dp > 0)
        {
            this.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            this.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
