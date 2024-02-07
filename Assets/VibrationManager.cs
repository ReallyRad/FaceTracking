using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    private bool _pucker;
    
    private void OnEnable()
    {
        FaceTrackingManager.PuckerTrigger += PuckerTrigger;
    }

    private void OnDisable()
    {
        FaceTrackingManager.PuckerTrigger += PuckerTrigger;
    }

    private void PuckerTrigger(bool pucker)
    {
        _pucker = pucker;
        
        if (pucker) StartCoroutine(BreathVibration());
        else
        {
            OVRInput.SetControllerVibration(0, 0f, OVRInput.Controller.RTouch);
            StopCoroutine("BreathVibration");
        }
    }
    
    private IEnumerator BreathVibration()
    {
        OVRInput.SetControllerVibration(1, 0.2f, OVRInput.Controller.RTouch);
        yield return new WaitForSeconds(2f);
        if (_pucker) StartCoroutine(BreathVibration());
    }
}
