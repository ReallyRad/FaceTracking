using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    [SerializeField] private IntVariable _experienceVariable;
  
    private bool _pucker;
    
    private bool _shouldVibrate;

    private void OnEnable()
    {
        FaceTrackingManager.PuckerTrigger += PuckerTrigger;
        VideoControllerInstruction.VideoInstructionsShown += ShouldVibrate;
    }

    private void OnDisable()
    {
        FaceTrackingManager.PuckerTrigger -= PuckerTrigger;
        VideoControllerInstruction.VideoInstructionsShown += ShouldVibrate;
    }

    public void ExperienceSelected()
    {
        if (_experienceVariable.Value != (int)Experience.Control)
        {
            transform.SetParent(transform.parent.transform.parent);
            DontDestroyOnLoad(gameObject);
        }
    }
    
    private void PuckerTrigger(bool pucker)
    {
        _pucker = pucker;
        if (_shouldVibrate)
        {
            if (pucker) StartCoroutine(BreathVibration());
            else 
            {
                OVRInput.SetControllerVibration(0, 0f, OVRInput.Controller.RTouch);
                StopCoroutine("BreathVibration");
            }    
        }
    }

    private void ShouldVibrate()
    {
        _shouldVibrate = true;
    }
    
    private IEnumerator BreathVibration()
    {
        OVRInput.SetControllerVibration(1, 0.2f, OVRInput.Controller.RTouch);
        yield return new WaitForSeconds(2f);
        if (_pucker) StartCoroutine(BreathVibration());
    }
}
