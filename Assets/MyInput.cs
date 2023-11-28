using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One))
            Debug.Log("button one");
        if (OVRInput.Get(OVRInput.Button.Two))
            Debug.Log("button two");
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
            Debug.Log("thumbstick pressed ");
            
        Debug.Log(OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger)); 
    }
}
