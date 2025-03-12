using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    private bool _pucker;
    [SerializeField] private IntVariable _experienceVariable;
    
    private void OnEnable()
    {
        FaceTrackingManager.PuckerTrigger += PuckerTrigger;
    }

    private void OnDisable()
    {
        FaceTrackingManager.PuckerTrigger -= PuckerTrigger;
    }

    public void ExperienceSelected()
    {
        if (_experienceVariable.Value != (int) Experience.Control) DontDestroyOnLoad(gameObject);
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
