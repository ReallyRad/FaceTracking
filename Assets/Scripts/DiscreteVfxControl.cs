using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VolumetricFogAndMist;

public class DiscreteVfxControl : MonoBehaviour
{
    public static int currentBN;
    public static float increment;
    private VolumetricFog fog;

    public GameObject fogDisappearing;
    public static int fogDisappearingStartBN;
    public static int fogDisappearingEndBN;
    public static float fogDisappearingInitialDensity;
    public static float fogDisappearingFinalDensity;
    
    public ParticleSystem auraApproaching;
    public static int auraApproachingStartBN;
    public static int auraApproachingEndBN;
    public static Vector3 auraApproachingInitialPosition;
    public static Vector3 auraApproachingFinalPosition;

    public ParticleSystem waveEnlarging;
    public static int waveEnlargingStartBN;
    public static int waveEnlargingEndBN;
    public static float waveEnlargingIntialScale;
    public static float waveEnlargingFinalScale;

    public ParticleSystem waveChangingColor;
    public static int waveChangingColorStartBN;
    public static int waveChangingColorEndBN;
    public static Color waveChangingColorInitialColor;
    public static Color waveChangingColorFinalColor;


    private void Start()
    {
        waveEnlarging.gameObject.SetActive(false);
        auraApproaching.gameObject.SetActive(false);
        waveChangingColor.gameObject.SetActive(false);
        fog = VolumetricFog.instance;
        fog.enabled = false;
        fogDisappearing.gameObject.SetActive(false);

        currentBN = 0;
        increment = 1.5f;

        fogDisappearingStartBN = 0;
        fogDisappearingEndBN = 3;
        fogDisappearingInitialDensity = 0.7f;
        fogDisappearingFinalDensity = 0;
        fog.density = fogDisappearingInitialDensity;

        auraApproachingStartBN = 8;
        auraApproachingEndBN = 11;
        auraApproachingInitialPosition = new Vector3(0f, 0f, 50f);
        auraApproachingFinalPosition = new Vector3(0f, 0f, 0f);
        auraApproaching.transform.position = auraApproachingInitialPosition;

        waveEnlargingStartBN = 4;
        waveEnlargingEndBN = 7;
        waveEnlargingIntialScale = 0.2f;
        waveEnlargingFinalScale = 5f;
        waveEnlarging.transform.localScale = new Vector3(waveEnlargingIntialScale, 
                                                         waveEnlargingIntialScale, 
                                                         waveEnlargingIntialScale);

        waveChangingColorStartBN = 12;
        waveChangingColorEndBN = 15;
        waveChangingColorInitialColor = new Color(1f, 0f, 0f, 0.05f);
        waveChangingColorFinalColor = new Color(1f, 0f, 0f, 0.95f);
        ParticleSystemRenderer psRenderer = waveChangingColor.GetComponent<ParticleSystemRenderer>();
        psRenderer.material.color = waveChangingColorInitialColor;
        psRenderer.material.SetFloat("_Alpha", waveChangingColorInitialColor.a);
    }
    private void OnEnable()
    {
        //FaceTrackingManager.NewFullBreath += CountingBreaths;
        DiscreteBreathingControl.NewFullBreath += CountingBreaths;
    }
    private void OnDisable()
    {
        //FaceTrackingManager.NewFullBreath -= CountingBreaths;
        DiscreteBreathingControl.NewFullBreath -= CountingBreaths;
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










