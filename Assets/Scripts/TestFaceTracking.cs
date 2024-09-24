using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFaceTracking : MonoBehaviour
{
    void Start()
    {
        if (OVRPlugin.faceTrackingEnabled) Debug.Log("face tracking is enabled");
        else OVRPlugin.StartFaceTracking();
    }

    void Update()
    {
        
    }
}
