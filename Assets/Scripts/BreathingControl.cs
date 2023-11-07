using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;

public class BreathingControl : MonoBehaviour
{
    // UI elements
    public Toggle smilingBool;
    public Slider smilingTime;

    public Toggle puckeringBool;
    public Slider puckeringTime;

    public Toggle slightSmilingBool;

    public Toggle slightPuckeringBool;

    public Toggle exhaleSoundingBool;
    public Slider exhaleSoundingTime;

    public Toggle quietingBool;
    public Slider quietingTime;

    public Toggle slightExhaleSoundingBool;
    // UI elements


    public GameObject ExFluid;
    public GameObject InFluid;
    public GameObject PeInPiFluid;

    public Material ExFluidMaterial;
    public Material InFluidMaterial;
    public Material PeInPiFluidMaterial;

    public Color lowerThanMinColor = Color.yellow;
    public Color betweenMinAndMaxColor = Color.green;
    public Color higherThanMaxColor = Color.red;

    private float ExFluidScale = 0f;
    private float InFluidScale = 0f;    
    private float PeInPiFluidScale = 0f;

    public delegate void On_In_Ex_FullBreath();
    public static On_In_Ex_FullBreath New_In_Ex_FullBreath;

    public delegate void On_PeInPi_Ex_FullBreath();
    public static On_PeInPi_Ex_FullBreath New_PeInPi_Ex_FullBreath;

    public delegate void On_Ex_LongerThanMinimum(float howLongMore);
    public static On_Ex_LongerThanMinimum New_Ex_LongerThanMinimum;

    public delegate void On_In_LongerThanMinimum(float howLongMore);
    public static On_In_LongerThanMinimum New_In_LongerThanMinimum;

    public delegate void On_PeInPi_LongerThanMinimum(float howLongMore);
    public static On_PeInPi_LongerThanMinimum New_PeInPi_LongerThanMinimum;

    private float minExDuration = 1.5f;
    private float maxExDuration = 4.5f;
    private float finishedExDuration;
    private float currentExDuration;

    private float minInDuration = 0f;
    private float maxInDuration = 4f;
    private float finishedInDuration;
    private float currentInDuration;

    private float minPeInPiDuration = 0f;
    private float maxPeInPiDuration = 6f;
    private float finishedPeInPiDuration;
    private float currentPeInPiDuration;

    //private bool PeInPiING;
    //private bool ExING;
    //private bool InING;

    //private Stopwatch In_Sw = new Stopwatch();
    //private Stopwatch Ex_Sw = new Stopwatch();
    //private Stopwatch PeInPi_Sw = new Stopwatch();

    private Stopwatch smile_Sw;
    private Stopwatch pucker_Sw;
    private bool smiling;
    private bool slightSmiling;
    private bool puckering;
    private bool slightPuckering;

    private Stopwatch exhaleSound_Sw;
    private Stopwatch quiet_Sw;
    private bool slightExhaleSounding;
    private bool exhaleSounding;
    private bool quieting;

    public FaceData faceExpression;

    void Start()
    {
        SetYScale(ExFluid, 0f);
        SetYScale(InFluid, 0f);
        SetYScale(PeInPiFluid, 0f);

        smile_Sw = new Stopwatch();
        pucker_Sw = new Stopwatch();
        exhaleSound_Sw = new Stopwatch();
        quiet_Sw = new Stopwatch();

        if (ModeControl.Discrete)
        {
            if (ModeControl.DiscreteFB_In_Ex)
            {
                ExFluid.transform.parent.gameObject.SetActive(true);
                InFluid.transform.parent.gameObject.SetActive(true);
                PeInPiFluid.transform.parent.gameObject.SetActive(false);
            }
            if (ModeControl.DiscreteFB_PeInPi_Ex)
            {
                ExFluid.transform.parent.gameObject.SetActive(true);
                InFluid.transform.parent.gameObject.SetActive(false);
                PeInPiFluid.transform.parent.gameObject.SetActive(true);
            }
        }
        if (ModeControl.Continuous)
        {
            if (ModeControl.ContinuousACT_Ex)
            {
                ExFluid.transform.parent.gameObject.SetActive(true);
                InFluid.transform.parent.gameObject.SetActive(false);
                PeInPiFluid.transform.parent.gameObject.SetActive(false);
            }
            if (ModeControl.ContinuousACT_In)
            {
                ExFluid.transform.parent.gameObject.SetActive(false);
                InFluid.transform.parent.gameObject.SetActive(true);
                PeInPiFluid.transform.parent.gameObject.SetActive(false);
            }
            if (ModeControl.ContinuousACT_PeInPi)
            {
                ExFluid.transform.parent.gameObject.SetActive(false);
                InFluid.transform.parent.gameObject.SetActive(false);
                PeInPiFluid.transform.parent.gameObject.SetActive(true);
            }
        }

        SettingUpUI();
    }
    private void OnEnable()
    {
        if (ModeControl.Auditory)
        {
            SoundDetectionManager.newSlightExhaleSoundDetected += SlightExhaleSound;
            SoundDetectionManager.newExhaleSoundDetected += ExhaleSound;
            SoundDetectionManager.newQuiet += Quit;
        }
        if (ModeControl.Visual)
        {
            UnityEngine.Debug.Log("Hellllllllllllllllllllllo");
            FaceTrackingManager.FaceExpression += WhichFaceExpression;
        }
    }
    private void OnDisable()
    {
        if (ModeControl.Auditory)
        {
            SoundDetectionManager.newSlightExhaleSoundDetected -= SlightExhaleSound;
            SoundDetectionManager.newExhaleSoundDetected -= ExhaleSound;
            SoundDetectionManager.newQuiet -= Quit;
        }
        if (ModeControl.Visual)
        {
            FaceTrackingManager.FaceExpression -= WhichFaceExpression;
        }
    }
    void Update()
    {
        if (ModeControl.Auditory)
        {
            if (ModeControl.Continuous)
            {
                if (ModeControl.ContinuousACT_Ex)
                {
                    currentExDuration = (float)exhaleSound_Sw.ElapsedMilliseconds / 1000.0f;
                    ExFluidScale = currentExDuration / maxExDuration;
                    SetYScale(ExFluid, ExFluidScale);
                    if (currentExDuration < minExDuration)
                    {
                        ExFluidMaterial.color = lowerThanMinColor;
                    }
                    if (currentExDuration > minExDuration &&
                        currentExDuration < maxExDuration)
                    {
                        ExFluidMaterial.color = betweenMinAndMaxColor;
                        New_Ex_LongerThanMinimum(currentExDuration);
                    }
                    if (currentExDuration > maxExDuration)
                    {
                        ExFluidMaterial.color = higherThanMaxColor;
                    }
                }
                if (ModeControl.ContinuousACT_PeInPi)
                {
                    currentPeInPiDuration = (float)quiet_Sw.ElapsedMilliseconds / 1000.0f;
                    PeInPiFluidScale = currentPeInPiDuration / maxPeInPiDuration;
                    SetYScale(PeInPiFluid, PeInPiFluidScale);
                    if (currentPeInPiDuration < minPeInPiDuration)
                    {
                        PeInPiFluidMaterial.color = lowerThanMinColor;
                    }
                    if (currentPeInPiDuration > minPeInPiDuration &&
                        currentPeInPiDuration < maxPeInPiDuration)
                    {
                        PeInPiFluidMaterial.color = betweenMinAndMaxColor;
                        New_Ex_LongerThanMinimum(currentPeInPiDuration);
                    }
                    if (currentPeInPiDuration > maxPeInPiDuration)
                    {
                        PeInPiFluidMaterial.color = higherThanMaxColor;
                    }
                }
            }
            if (ModeControl.Discrete)
            {
                if (ModeControl.DiscreteFB_PeInPi_Ex)
                {
                    currentPeInPiDuration = (float)quiet_Sw.ElapsedMilliseconds / 1000.0f;
                    PeInPiFluidScale = currentPeInPiDuration / maxPeInPiDuration;
                    SetYScale(PeInPiFluid, PeInPiFluidScale);
                    if (currentPeInPiDuration < minPeInPiDuration)
                    {
                        PeInPiFluidMaterial.color = lowerThanMinColor;
                    }
                    if (currentPeInPiDuration > minPeInPiDuration &&
                        currentPeInPiDuration < maxPeInPiDuration)
                    {
                        PeInPiFluidMaterial.color = betweenMinAndMaxColor;
                    }
                    if (currentPeInPiDuration > maxPeInPiDuration)
                    {
                        PeInPiFluidMaterial.color = higherThanMaxColor;
                    }

                    currentExDuration = (float)exhaleSound_Sw.ElapsedMilliseconds / 1000.0f;
                    ExFluidScale = currentExDuration / maxExDuration;
                    SetYScale(ExFluid, ExFluidScale);
                    if (currentExDuration < minExDuration)
                    {
                        ExFluidMaterial.color = lowerThanMinColor;
                    }
                    if (currentExDuration > minExDuration &&
                        currentExDuration < maxExDuration)
                    {
                        ExFluidMaterial.color = betweenMinAndMaxColor;
                    }
                    if (currentExDuration > maxExDuration)
                    {
                        ExFluidMaterial.color = higherThanMaxColor;
                    }

                    if (finishedExDuration > minExDuration &&
                        finishedExDuration < maxExDuration &&
                        finishedPeInPiDuration > minPeInPiDuration &&
                        finishedPeInPiDuration < maxPeInPiDuration)
                    {
                        New_PeInPi_Ex_FullBreath();
                        finishedExDuration = 0;
                        finishedPeInPiDuration = 0;
                    }
                }
            }
        }
        if (ModeControl.Visual)
        {
            if (ModeControl.Continuous)
            {
                if (ModeControl.ContinuousACT_Ex)
                {
                    currentExDuration = (float)pucker_Sw.ElapsedMilliseconds / 1000.0f;
                    //currentExDuration = currentExDuration / maxExDuration;
                    ExFluidScale = currentExDuration / maxExDuration;
                    
                    SetYScale(ExFluid, ExFluidScale);


                    if (currentExDuration < minExDuration)
                    {
                        ExFluidMaterial.color = lowerThanMinColor;
                        if(New_Ex_LongerThanMinimum != null) New_Ex_LongerThanMinimum(0f);
                    }
                    if (currentExDuration > minExDuration &&
                        currentExDuration < maxExDuration)
                    {
                        ExFluidMaterial.color = betweenMinAndMaxColor;
                        if (New_Ex_LongerThanMinimum != null) New_Ex_LongerThanMinimum(ExFluidScale);
                    }
                    if (currentExDuration > maxExDuration)
                    {
                        ExFluidMaterial.color = higherThanMaxColor;
                        if (New_Ex_LongerThanMinimum != null) New_Ex_LongerThanMinimum(0f);
                    }

                    //UnityEngine.Debug.Log("VISUAL-EXHALE: " + currentExDuration);

                }
                if (ModeControl.ContinuousACT_In)
                {
                    currentInDuration = (float)smile_Sw.ElapsedMilliseconds / 1000.0f;
                    InFluidScale = currentInDuration / maxInDuration;
                    SetYScale(InFluid, InFluidScale);
                    if (currentInDuration < minInDuration)
                    {
                        InFluidMaterial.color = lowerThanMinColor;
                    }
                    if (currentInDuration > minInDuration &&
                        currentInDuration < maxInDuration)
                    {
                        InFluidMaterial.color = betweenMinAndMaxColor;
                        New_In_LongerThanMinimum(currentInDuration);
                    }
                    if (currentInDuration > maxInDuration)
                    {
                        InFluidMaterial.color = higherThanMaxColor;
                    }
                }
            }
            if (ModeControl.Discrete)
            {
                if (ModeControl.DiscreteFB_In_Ex)
                {
                    currentInDuration = (float)smile_Sw.ElapsedMilliseconds / 1000.0f;
                    InFluidScale = currentInDuration / maxInDuration;
                    SetYScale(InFluid, InFluidScale);
                    if (currentInDuration < minInDuration)
                    {
                        InFluidMaterial.color = lowerThanMinColor;
                    }
                    if (currentInDuration > minInDuration &&
                        currentInDuration < maxInDuration)
                    {
                        InFluidMaterial.color = betweenMinAndMaxColor;
                    }
                    if (currentInDuration > maxInDuration)
                    {
                        InFluidMaterial.color = higherThanMaxColor;
                    }

                    currentExDuration = (float)pucker_Sw.ElapsedMilliseconds / 1000.0f;
                    ExFluidScale = currentExDuration / maxExDuration;
                    SetYScale(ExFluid, ExFluidScale);
                    if (currentExDuration < minExDuration)
                    {
                        ExFluidMaterial.color = lowerThanMinColor;
                    }
                    if (currentExDuration > minExDuration &&
                        currentExDuration < maxExDuration)
                    {
                        ExFluidMaterial.color = betweenMinAndMaxColor;
                    }
                    if (currentExDuration > maxExDuration)
                    {
                        ExFluidMaterial.color = higherThanMaxColor;
                    }

                    if (finishedExDuration > minExDuration &&
                        finishedExDuration < maxExDuration &&
                        finishedInDuration > minInDuration &&
                        finishedInDuration < maxInDuration)
                    {
                        New_In_Ex_FullBreath();
                        finishedExDuration = 0;
                        finishedInDuration = 0;
                    }
                }
            }
        }
        UpdatingUI();
    }

    void SetYScale(GameObject fluid, float yScale)
    {
        Vector3 newScale = fluid.transform.localScale;
        newScale.y = yScale;
        fluid.transform.localScale = newScale;
    }

    private void WhichFaceExpression()
    {
        UnityEngine.Debug.Log("Byeeeeeeeeeeeeeeeeeeeeee");
        if (faceExpression.smiling) Smile();
        if (faceExpression.pucker) Pucker();
        if (faceExpression.slightPucker) SlightPucker();
        if (faceExpression.slightSmile) SlightSmile();
    }
    private void Pucker()
    {
        StartCoroutine(BreathVibration());

        puckering = true;
        slightPuckering = false;
        slightSmiling = false;
        smiling = false;

        pucker_Sw.Start();

        finishedInDuration = currentInDuration;
        smile_Sw.Reset();
        
        currentInDuration = 0;
    }
    private void Smile()
    {
        OVRInput.SetControllerVibration(0, 0f, OVRInput.Controller.RTouch);
        StopCoroutine("BreathVibration");

        smiling = true;
        slightSmiling = false;
        slightPuckering = false;
        puckering = false;

        smile_Sw.Start();

        finishedExDuration = currentExDuration;
        pucker_Sw.Reset();
        currentExDuration = 0;
    }
    private void SlightPucker()
    {
        OVRInput.SetControllerVibration(0, 0f, OVRInput.Controller.RTouch);
        StopCoroutine("BreathVibration");

        slightPuckering = true;
        puckering = false;
        smiling = false;
        slightSmiling = false;

        pucker_Sw.Stop();
        smile_Sw.Stop();
    }
    private void SlightSmile()
    {
        OVRInput.SetControllerVibration(0, 0f, OVRInput.Controller.RTouch);
        StopCoroutine("BreathVibration");

        smiling = false;
        slightSmiling = true;
        puckering = false;
        slightPuckering = false;

        smile_Sw.Stop();
        pucker_Sw.Stop();
    }

    private void SlightExhaleSound()
    {
        slightExhaleSounding = true;
        exhaleSounding = false;
        quieting = false;

        exhaleSound_Sw.Stop();
        quiet_Sw.Stop();
    }
    private void ExhaleSound()
    {
        slightExhaleSounding = false;
        exhaleSounding = true;
        quieting = false;

        exhaleSound_Sw.Start();

        finishedPeInPiDuration = currentPeInPiDuration;
        quiet_Sw.Reset();
        currentPeInPiDuration = 0;

    }
    private void Quit()
    {
        slightExhaleSounding = false;
        exhaleSounding = false;
        quieting = true;

        quiet_Sw.Start();

        finishedExDuration = currentExDuration;
        exhaleSound_Sw.Reset();
        currentExDuration = 0;
    }

    private void SettingUpUI()
    {
        smilingBool.isOn = false;
        slightSmilingBool.isOn = false;
        slightPuckeringBool.isOn = false;
        puckeringBool.isOn = false;

        exhaleSoundingBool.isOn = false;
        slightExhaleSoundingBool.isOn = false;
        quietingBool.isOn = false;

        smilingTime.minValue = 0f;
        smilingTime.maxValue = 5f;
        puckeringTime.minValue = 0f;
        puckeringTime.maxValue = 5f;
        exhaleSoundingTime.minValue = 0f;
        exhaleSoundingTime.maxValue= 5f;
        quietingTime.minValue = 0f;
        quietingTime.maxValue = 5f;
    }
    private void UpdatingUI()
    {
        smilingBool.isOn = smiling;
        slightSmilingBool.isOn = slightSmiling;
        slightPuckeringBool.isOn = slightPuckering;
        puckeringBool.isOn = puckering;

        exhaleSoundingBool.isOn = exhaleSounding;
        slightExhaleSoundingBool.isOn = slightExhaleSounding;
        quietingBool.isOn = quieting;

        smilingTime.value = (float)smile_Sw.ElapsedMilliseconds / 1000.0f;
        puckeringTime.value = (float)pucker_Sw.ElapsedMilliseconds / 1000.0f;
        exhaleSoundingTime.value = (float)exhaleSound_Sw.ElapsedMilliseconds / 1000.0f;
        quietingTime.value = (float)quiet_Sw.ElapsedMilliseconds / 1000.0f;
    }

    private IEnumerator BreathVibration()
    {
        OVRInput.SetControllerVibration(1, 0.2f, OVRInput.Controller.RTouch);
        yield return new WaitForSeconds(2f);
        if (faceExpression.pucker) StartCoroutine(BreathVibration());
    }
}
