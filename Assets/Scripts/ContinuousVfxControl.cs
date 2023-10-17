using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VolumetricFogAndMist;

public class ContinuousVfxControl : MonoBehaviour
{
    private VolumetricFog fog;
    public GameObject fogDisappearing;
    public static float fogDisappearingNumberOfExhalesIfCompleteToFinish;
    public static float fogDisappearingInitialDensity;
    public static float fogDisappearingFinalDensity;
    private float fogDisappearingStartTime;
    
    private ParticleSystem auraApproaching;
    public static float auraApproachingNumberOfExhalesIfCompleteToFinish;
    public static Vector3 auraApproachingInitialPosition;
    public static Vector3 auraApproachingFinalPosition;
    private float auraApproachingStartTime;

    public ParticleSystem waveEnlarging;
    public static float waveEnlargingNumberOfExhalesIfCompleteToFinish;
    public static float waveEnlargingIntialScale;
    public static float waveEnlargingFinalScale;
    private float waveEnlargingStartTime;

    public ParticleSystem waveChangingColor;
    public static float waveChangingColorNumberOfExhalesIfCompleteToFinish;
    public static Color waveChangingColorInitialColor;
    public static Color waveChangingColorFinalColor;
    private float waveChangingColorStartTime;

    private float elapsedTime;

    private void Start()
    {
        elapsedTime = 0;

        waveEnlarging.gameObject.SetActive(false);
        auraApproaching.gameObject.SetActive(false);
        waveChangingColor.gameObject.SetActive(false);
        fog = VolumetricFog.instance;
        fog.enabled = false;
        fogDisappearing.gameObject.SetActive(false);

        
        fogDisappearingNumberOfExhalesIfCompleteToFinish = 3;
        fogDisappearingStartTime = 0.1f;
        StartCoroutine(ActivateFog(fogDisappearing, fogDisappearingStartTime));
        fogDisappearingInitialDensity = 0.7f;
        fogDisappearingFinalDensity = 0;
        fog.density = fogDisappearingInitialDensity;

        
        auraApproachingNumberOfExhalesIfCompleteToFinish = 3;
        auraApproachingStartTime = 48f;
        StartCoroutine(ActivateObject(auraApproaching.gameObject, auraApproachingStartTime));
        auraApproachingInitialPosition = new Vector3(50f, 0f, 0f);
        auraApproachingFinalPosition = new Vector3(0f, 0f, 0f);
        auraApproaching.transform.position = auraApproachingInitialPosition;

        
        waveEnlargingNumberOfExhalesIfCompleteToFinish = 3;
        waveEnlargingStartTime = 72;
        StartCoroutine(ActivateObject(waveEnlarging.gameObject, waveEnlargingStartTime));
        waveEnlargingIntialScale = 0.2f;
        waveEnlargingFinalScale = 5f;
        waveEnlarging.transform.localScale = new Vector3(waveEnlargingIntialScale, 
                                                         waveEnlargingIntialScale, 
                                                         waveEnlargingIntialScale);

        waveChangingColorNumberOfExhalesIfCompleteToFinish = 3;
        waveChangingColorStartTime = 24f;
        StartCoroutine(ActivateObject(waveChangingColor.gameObject, waveChangingColorStartTime));
        waveChangingColorInitialColor = new Color(1f, 0f, 0f, 1f);
        waveChangingColorFinalColor = new Color(0f, 1f, 0f, 0.2f);
        ParticleSystemRenderer psRenderer = waveChangingColor.GetComponent<ParticleSystemRenderer>();
        psRenderer.material.color = waveChangingColorInitialColor;
        psRenderer.material.SetFloat("_Alpha", waveChangingColorInitialColor.a);
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
}










