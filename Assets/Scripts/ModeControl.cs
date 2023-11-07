using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModeControl : MonoBehaviour
{
    public static bool Visual;
    public static bool Auditory;

    public static bool Discrete;
    public static bool Continuous;

    public static bool ContinuousACT_Ex;
    public static bool ContinuousACT_In;
    public static bool ContinuousACT_PeInPi;

    public static bool DiscreteFB_PeInPi_Ex;
    public static bool DiscreteFB_In_Ex;

    public static bool fogDisappearing;
    public static string fogDisappearingRP;
    
    public static bool auraApproaching;
    public static string auraApproachingRP;
    
    public static bool waveEnlarging;
    public static string waveEnlargingRP;
    
    public static bool waveChangingColor;
    public static string waveChangingColorRP;

    public static bool backgroundChanging;
    public static string backgroundChangingRP;

    public static bool northernLights;
    public static string northernLightsRP;

    public static bool music;
    public static string musicRP;

    public static bool postProcessing;
    public static string postProcessingRP;

    //public Canvas CanvasSetting;
    //public Canvas CanvasTracking;
    //public GameObject BreathingControl;
    //public GameObject AllVfxsControl;
    //public GameObject InteractingCamera;
    //public GameObject FaceTrackingCamera;

    //public Toggle visualToggle;
    //public Toggle auditoryToggle;
    //public Toggle continuousToggle;
    //public Toggle discreteToggle;
    //public Toggle fogDisappearingToggle;
    //public Toggle auraApproachingToggle;
    //public Toggle waveEnlargingToggle;
    //public Toggle waveChangingColorToggle;
    //public TMP_Dropdown fogDisappearingDropdown;
    //public TMP_Dropdown auraApproachingDropdown;
    //public TMP_Dropdown waveEnlargingDropdown;
    //public TMP_Dropdown waveChangingColorDropdown;

    void Awake()
    {
        // the method of detection
        Visual = true;
        Auditory = false;
        
        // the visual effects
        fogDisappearing = true;
        fogDisappearingRP = "Progressive";

        auraApproaching = false;
        auraApproachingRP = "Progressive";

        waveEnlarging = false;
        waveEnlargingRP = "Interactive";

        waveChangingColor = false;
        waveChangingColorRP = "Interactive";

        backgroundChanging = false;
        backgroundChangingRP = "Interactive";

        northernLights = false;
        northernLightsRP = "Interactive";

        music = true;
        musicRP = "Progressive";

        postProcessing = false;
        postProcessingRP = "Interactive";

        // the temporalization mode
        Discrete = false;
        Continuous = true;

        // the actuation method
        ContinuousACT_Ex = true;
        ContinuousACT_In = false;
        ContinuousACT_PeInPi = false;

        // the full breath pattern
        DiscreteFB_PeInPi_Ex = false;
        DiscreteFB_In_Ex = true;

        // the Game
        //CanvasSetting.gameObject.SetActive(true);
        //InteractingCamera.gameObject.SetActive(true);
        //CanvasTracking.gameObject.SetActive(false);
        //BreathingControl.gameObject.SetActive(false);
        //AllVfxsControl.gameObject.SetActive(false);
        //FaceTrackingCamera.gameObject.SetActive(false);
    }
    //public void GoButtonClicked()
    //{
    //    if (visualToggle.isOn) Visual = true; else Visual = false;
    //    if (auditoryToggle.isOn) Auditory = true; else Auditory = false;
        
    //    //if (continuousToggle.isOn) Continuous = true; else Continuous = false;
    //    //if (discreteToggle.isOn) Discrete = true; else Discrete = false;
        
    //    if (fogDisappearingToggle.isOn) fogDisappearing = true; else fogDisappearing = false;
    //    fogDisappearingRP = fogDisappearingDropdown.options[fogDisappearingDropdown.value].text;
        
    //    if (auraApproachingToggle.isOn) auraApproaching = true; else auraApproaching = false;
    //    auraApproachingRP = auraApproachingDropdown.options[auraApproachingDropdown.value].text;
        
    //    if (waveEnlargingToggle.isOn) waveEnlarging = true; else waveEnlarging = false;
    //    waveEnlargingRP = waveEnlargingDropdown.options[waveEnlargingDropdown.value].text;
        
    //    if (waveChangingColorToggle.isOn) waveChangingColor = true; else waveChangingColor = false;
    //    waveChangingColorRP = waveChangingColorDropdown.options[waveChangingColorDropdown.value].text;

    //    CanvasSetting.gameObject.SetActive(false);
    //    InteractingCamera.gameObject.SetActive(false);
    //    CanvasTracking.gameObject.SetActive(true);
    //    BreathingControl.gameObject.SetActive(true);
    //    AllVfxsControl.gameObject.SetActive(true);
    //    FaceTrackingCamera.gameObject.SetActive(true);
    //}

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    CanvasSetting.gameObject.SetActive(true);
        //    InteractingCamera.gameObject.SetActive(true);
        //    CanvasTracking.gameObject.SetActive(false);
        //    BreathingControl.gameObject.SetActive(false);
        //    AllVfxsControl.gameObject.SetActive(false);
        //    FaceTrackingCamera.gameObject.SetActive(false);
        //}
    }
}
