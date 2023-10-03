using Oculus.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

public class ContinuousBreathingControl : MonoBehaviour
{
    public delegate void OnNewFullBreath();
    public static OnNewFullBreath NewFullBreath;

    public delegate void OnExhalingLongerThanMinimum(float howLongMore);
    public static OnExhalingLongerThanMinimum NewExhalingLongerThanMinimum;

    public GameObject periodFluid;
    public GameObject gapFluid;

    private float loudnessMultiplier = 500f;
    private float threshold = 0.003f; //was 0.005 before

    private float timeElapsed = 0f;
    private float timeSinceLastSizeChange = 0f;

    private float gap = 4f;
    private float minAccPeriod = 0.8f;
    private float maxAccPeriod = 3f;

    public Material periodMaterial;
    public Material gapMaterial;

    void Start()
    {
        if (UnityEngine.Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            StartMicrophone();
        }
        else
        {
            UnityEngine.Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }
        SetYScale(gapFluid, 0f);
        SetYScale(periodFluid, 0f);
    }
    void Update()
    {
        float loudness = GetLoudness();
        timeSinceLastSizeChange += Time.deltaTime;

        if (timeSinceLastSizeChange < gap)
        {
            float normalizedTime = timeSinceLastSizeChange / gap;
            SetYScale(gapFluid, normalizedTime);
            gapMaterial.color = Color.yellow;
        }
        else { gapMaterial.color = Color.green; }

        if (loudness * loudnessMultiplier >= threshold)
        {
            timeElapsed += Time.deltaTime;
            SetYScale(periodFluid, timeElapsed / maxAccPeriod);

            if (timeElapsed >= minAccPeriod)
            {
                periodMaterial.color = Color.green;

                if (timeElapsed <= maxAccPeriod) 
                {
                    if (timeSinceLastSizeChange > gap)
                    {
                        float fraction = ((timeElapsed - minAccPeriod) / (maxAccPeriod - minAccPeriod)) * Time.deltaTime;
                        NewExhalingLongerThanMinimum.Invoke(fraction);
                    }
                    else{}
                }
                else 
                {
                    periodMaterial.color = Color.red;

                    timeSinceLastSizeChange = 0f;
                    SetYScale(gapFluid, 0f);

                    timeElapsed = 0f;
                    SetYScale(periodFluid, 0f);
                }
            }
            else { periodMaterial.color = Color.yellow; }
        }
        else{}

        if (loudness * loudnessMultiplier < threshold && 
            timeElapsed <= maxAccPeriod &&
            timeElapsed > minAccPeriod)
        {
            timeSinceLastSizeChange = 0f;
            SetYScale(gapFluid, 0f);

            timeElapsed = 0f;
            SetYScale(periodFluid, 0f);
        }

        if (loudness * loudnessMultiplier < threshold &&
            timeElapsed < minAccPeriod)
        {
            timeElapsed = 0f;
            SetYScale(periodFluid, 0f);
        }
    }

    float GetLoudness()
    {
        float[] spectrumData = new float[128];
        AudioSource audioSource = GetComponent<AudioSource>();

        // Get the spectrum data from the microphone input
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Hamming);

        // Calculate the average loudness from the spectrum data
        float sum = 0f;
        for (int i = 0; i < spectrumData.Length; i++)
        {
            sum += spectrumData[i];
        }
        float averageLoudness = sum / spectrumData.Length;

        return averageLoudness;
    }
    void StartMicrophone()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        audioSource.clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);
        audioSource.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { } // Wait until microphone is recording

        audioSource.Play();
    }
    void SetYScale(GameObject fluid, float yScale)
    {
        Vector3 newScale = fluid.transform.localScale;
        newScale.y = yScale;
        fluid.transform.localScale = newScale;
    }

}

