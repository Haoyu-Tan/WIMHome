using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRef : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Transform myReference;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void myReferenceUpdate()
    {
        
        myReference.transform.position = this.transform.localPosition;
        Vector3 cameraRot = Camera.main.transform.localRotation.eulerAngles;
        cameraRot.x = 0;
        cameraRot.z = 0;
        myReference.transform.rotation = this.transform.localRotation
            * Quaternion.Inverse(Quaternion.Euler(cameraRot));

    }
}
