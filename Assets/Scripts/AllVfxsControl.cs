using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist;

public class AllVfxsControl : MonoBehaviour
{
    public static int currentBN;
    public static float progressIncrementTimeForDiscrete;
    public static float interactiveLoopTimeForDiscrete;


    private VolumetricFog fog;
    public GameObject fogDisappearingPrefab;
    private GameObject fogDisappearing;
    public static float fogDisappearingInitialDensity;
    public static float fogDisappearingFinalDensity;
    // Discrete
    public static int fogDisappearingStartBN;
    public static int fogDisappearingEndBN;
    // Continuous
    public static float fogDisappearingNumberOfExhalesIfCompleteToFinish;
    private float fogDisappearingStartTime;

    public GameObject auraApproachingPrefab;
    private GameObject auraApproachingInstance;
    private ParticleSystem auraApproaching;
    public static Vector3 auraApproachingInitialPosition;
    public static Vector3 auraApproachingFinalPosition;
    // Discrete
    public static int auraApproachingStartBN;
    public static int auraApproachingEndBN;
    // Continuous
    public static float auraApproachingNumberOfExhalesIfCompleteToFinish;
    private float auraApproachingStartTime;

    public GameObject waveEnlargingPrefab;
    private GameObject waveEnlargingInstance;
    private ParticleSystem waveEnlarging;
    public static float waveEnlargingIntialScale;
    public static float waveEnlargingFinalScale;
    // Discrete
    public static int waveEnlargingStartBN;
    public static int waveEnlargingEndBN;
    // Continuous
    public static float waveEnlargingNumberOfExhalesIfCompleteToFinish;
    private float waveEnlargingStartTime;

    public GameObject waveChangingColorPrefab;
    private GameObject waveChangingColorInstance;
    private ParticleSystem waveChangingColor;
    public static Color waveChangingColorInitialColor;
    public static Color waveChangingColorFinalColor;
    // Discrete
    public static int waveChangingColorStartBN;
    public static int waveChangingColorEndBN;
    // Continuous
    public static float waveChangingColorNumberOfExhalesIfCompleteToFinish;
    private float waveChangingColorStartTime;
    private void OnEnable()
    {
        if (ModeControl.Discrete)
        {
            if (ModeControl.DiscreteFB_In_Ex)
            {
                BreathingControl.New_In_Ex_FullBreath += CountingBreaths;
            }
            if (ModeControl.DiscreteFB_PeInPi_Ex)
            {
                BreathingControl.New_PeInPi_Ex_FullBreath += CountingBreaths;
            }
        }
    }
    private void OnDisable()
    {
        if (ModeControl.Discrete)
        {
            if (ModeControl.DiscreteFB_In_Ex)
            {
                BreathingControl.New_In_Ex_FullBreath -= CountingBreaths;
            }
            if (ModeControl.DiscreteFB_PeInPi_Ex)
            {
                BreathingControl.New_PeInPi_Ex_FullBreath -= CountingBreaths;
            }
        }
    }
    void Start()
    {
        currentBN = 0;
        progressIncrementTimeForDiscrete = 1.5f;
        interactiveLoopTimeForDiscrete = 2f;

        if (ModeControl.fogDisappearing)
        {
            fogDisappearing = Instantiate(fogDisappearingPrefab);

            fogDisappearingInitialDensity = 0.7f;
            fogDisappearingFinalDensity = 0;
            fog.density = fogDisappearingInitialDensity;

            fog = VolumetricFog.instance;
            fog.enabled = false;
            fogDisappearing.gameObject.SetActive(false);

            if (ModeControl.Discrete)
            {
                fogDisappearingStartBN = 0;
                fogDisappearingEndBN = 3;
            }
            if (ModeControl.Continuous)
            {
                fogDisappearingNumberOfExhalesIfCompleteToFinish = 3;
                fogDisappearingStartTime = 0.1f;
                StartCoroutine(ActivateFog(fogDisappearing, fogDisappearingStartTime));
            }
        }
        if (ModeControl.auraApproaching)
        {
            auraApproachingInstance = Instantiate(auraApproachingPrefab);
            auraApproaching = auraApproachingInstance.GetComponent<ParticleSystem>();

            auraApproachingInitialPosition = new Vector3(0f, 0f, 50f);
            auraApproachingFinalPosition = new Vector3(0f, 0f, 0f);
            auraApproaching.transform.position = auraApproachingInitialPosition;

            auraApproaching.gameObject.SetActive(false);

            if (ModeControl.Discrete)
            {
                auraApproachingStartBN = 8;
                auraApproachingEndBN = 11;
            }
            if (ModeControl.Continuous)
            {
                auraApproachingNumberOfExhalesIfCompleteToFinish = 3;
                auraApproachingStartTime = 48f;
                StartCoroutine(ActivateObject(auraApproaching.gameObject, auraApproachingStartTime));
            }
        }
        if (ModeControl.waveChangingColor)
        {
            waveChangingColorInstance = Instantiate(waveChangingColorPrefab);
            waveChangingColor = waveChangingColorInstance.GetComponent<ParticleSystem>();

            waveChangingColorInitialColor = new Color(1f, 0f, 0f, 0.05f);
            waveChangingColorFinalColor = new Color(1f, 0f, 0f, 0.95f);
            ParticleSystemRenderer psRenderer = waveChangingColor.GetComponent<ParticleSystemRenderer>();
            psRenderer.material.color = waveChangingColorInitialColor;
            psRenderer.material.SetFloat("_Alpha", waveChangingColorInitialColor.a);

            waveChangingColor.gameObject.SetActive(false);

            if (ModeControl.Discrete)
            {
                waveChangingColorStartBN = 12;
                waveChangingColorEndBN = 15;
            }
            if (ModeControl.Continuous)
            {
                waveChangingColorNumberOfExhalesIfCompleteToFinish = 3;
                waveChangingColorStartTime = 24f;
                StartCoroutine(ActivateObject(waveChangingColor.gameObject, waveChangingColorStartTime));
            }
        }
        if (ModeControl.waveEnlarging)
        {
            waveEnlargingInstance = Instantiate(waveEnlargingPrefab);
            waveEnlarging = waveEnlargingInstance.GetComponent<ParticleSystem>();

            waveEnlargingIntialScale = 0.2f;
            waveEnlargingFinalScale = 5f;
            waveEnlarging.transform.localScale = new Vector3(waveEnlargingIntialScale,
                                                             waveEnlargingIntialScale,
                                                             waveEnlargingIntialScale);

            waveEnlarging.gameObject.SetActive(false);

            if (ModeControl.Discrete)
            {
                waveEnlargingStartBN = 4;
                waveEnlargingEndBN = 7;
            }
            if (ModeControl.Continuous)
            {
                waveEnlargingNumberOfExhalesIfCompleteToFinish = 3;
                waveEnlargingStartTime = 72;
                StartCoroutine(ActivateObject(waveEnlarging.gameObject, waveEnlargingStartTime));
            }
        }
    }

    void Update()
    {
        
    }

    IEnumerator ActivateObject(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);
    }
    IEnumerator ActivateFog(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        fogDisappearing.gameObject.SetActive(true);
        fog.enabled = true;
    }
    private void CountingBreaths()
    {
        currentBN += 1;

        if (currentBN >= auraApproachingStartBN && currentBN <= auraApproachingEndBN)
        {
            auraApproaching.gameObject.SetActive(true);
        }
        else
        {
            auraApproaching.gameObject.SetActive(false);
        }

        if (currentBN >= waveEnlargingStartBN && currentBN <= waveEnlargingEndBN)
        {
            waveEnlarging.gameObject.SetActive(true);
        }
        else
        {
            waveEnlarging.gameObject.SetActive(false);
        }

        if (currentBN >= fogDisappearingStartBN && currentBN <= fogDisappearingEndBN)
        {
            fog.enabled = true;
            fogDisappearing.gameObject.SetActive(true);
        }
        else
        {
            fog.enabled = false;
            fogDisappearing.gameObject.SetActive(false);
        }

        if (currentBN >= waveChangingColorStartBN && currentBN <= waveChangingColorEndBN)
        {
            waveChangingColor.gameObject.SetActive(true);
        }
        else
        {
            waveChangingColor.gameObject.SetActive(false);
        }
    }
}

