using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationControl : MonoBehaviour
{

    private void OnEnable()
    {
        BreathingControl.New_Ex_LongerThanMinimum += Vibrator;
    }
    private void OnDisable()
    {
        BreathingControl.New_Ex_LongerThanMinimum -= Vibrator;
    }

    private void Vibrator(float fraction)
    {
        OVRInput.SetControllerVibration(1, fraction, OVRInput.Controller.RTouch);
    }
}
