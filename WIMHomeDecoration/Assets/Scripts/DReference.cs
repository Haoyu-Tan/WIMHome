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

    public void DestroyRefObj()
    {
        if(this.referObj)
            Destroy(this.referObj.gameObject);
    }



    public void UpdateRefScale() {
        if (referObj) {
            referObj.transform.localScale = this.transform.localScale;
        }
    }
    public void ChangeRoom(Transform room)
    {
        if (referObj)
        {
            referObj.transform.parent = room;
            UpdateRefObject();
        }
    }


    public void UpdateRefObject()
    {
        if (referObj)
        {
            referObj.localScale = this.transform.localScale;
            referObj.localPosition = this.transform.localPosition;
            referObj.localRotation = this.transform.localRotation;
        }
    }

}
