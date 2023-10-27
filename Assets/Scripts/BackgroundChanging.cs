using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackgroundChanging : MonoBehaviour
{
    public Material backgroundMat;
    private void OnEnable()
    {
        if (ModeControl.Discrete)
        {
            if (ModeControl.auraApproachingRP == "Progressive")
            {
                if (ModeControl.DiscreteFB_In_Ex)
                {
                    BreathingControl.New_In_Ex_FullBreath += DiscreteProgressiveWhole;
                }
                if (ModeControl.DiscreteFB_PeInPi_Ex)
                {
                    BreathingControl.New_PeInPi_Ex_FullBreath += DiscreteProgressiveWhole;
                }
            }
            if (ModeControl.auraApproachingRP == "Interactive")
            {
                if (ModeControl.DiscreteFB_In_Ex)
                {
                    BreathingControl.New_In_Ex_FullBreath += DiscreteInteractiveWhole;
                }
                if (ModeControl.DiscreteFB_PeInPi_Ex)
                {
                    BreathingControl.New_PeInPi_Ex_FullBreath += DiscreteInteractiveWhole;
                }
            }
        }
        if (ModeControl.Continuous)
        {
            if (ModeControl.auraApproachingRP == "Progressive")
            {
                if (ModeControl.ContinuousACT_Ex)
                {
                    BreathingControl.New_Ex_LongerThanMinimum += ContinuousProgressiveWhole;
                }
                if (ModeControl.ContinuousACT_In)
                {
                    BreathingControl.New_In_LongerThanMinimum += ContinuousProgressiveWhole;
                }
                if (ModeControl.ContinuousACT_PeInPi)
                {
                    BreathingControl.New_PeInPi_LongerThanMinimum = ContinuousProgressiveWhole;
                }
            }
            if (ModeControl.auraApproachingRP == "Interactive")
            {
                if (ModeControl.ContinuousACT_Ex)
                {
                    //UnityEngine.Debug.Log("HELOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
                    BreathingControl.New_Ex_LongerThanMinimum += ContinuousInteractiveWhole;
                }
                if (ModeControl.ContinuousACT_In)
                {
                    BreathingControl.New_In_LongerThanMinimum += ContinuousInteractiveWhole;
                }
                if (ModeControl.ContinuousACT_PeInPi)
                {
                    BreathingControl.New_PeInPi_LongerThanMinimum += ContinuousInteractiveWhole;
                }
            }
        }
    }
    private void OnDisable()
    {
        if (ModeControl.Discrete)
        {
            if (ModeControl.auraApproachingRP == "Progressive")
            {
                if (ModeControl.DiscreteFB_In_Ex)
                {
                    BreathingControl.New_In_Ex_FullBreath -= DiscreteProgressiveWhole;
                }
                if (ModeControl.DiscreteFB_PeInPi_Ex)
                {
                    BreathingControl.New_PeInPi_Ex_FullBreath -= DiscreteProgressiveWhole;
                }
            }
            if (ModeControl.auraApproachingRP == "Interactive")
            {
                if (ModeControl.DiscreteFB_In_Ex)
                {
                    BreathingControl.New_In_Ex_FullBreath -= DiscreteInteractiveWhole;
                }
                if (ModeControl.DiscreteFB_PeInPi_Ex)
                {
                    BreathingControl.New_PeInPi_Ex_FullBreath -= DiscreteInteractiveWhole;
                }
            }
        }
        if (ModeControl.Continuous)
        {
            if (ModeControl.auraApproachingRP == "Progressive")
            {
                if (ModeControl.ContinuousACT_Ex)
                {
                    BreathingControl.New_Ex_LongerThanMinimum -= ContinuousProgressiveWhole;
                }
                if (ModeControl.ContinuousACT_In)
                {
                    BreathingControl.New_In_LongerThanMinimum -= ContinuousProgressiveWhole;
                }
                if (ModeControl.ContinuousACT_PeInPi)
                {
                    BreathingControl.New_PeInPi_LongerThanMinimum -= ContinuousProgressiveWhole;
                }
            }
            if (ModeControl.auraApproachingRP == "Interactive")
            {
                if (ModeControl.ContinuousACT_Ex)
                {
                    BreathingControl.New_Ex_LongerThanMinimum -= ContinuousInteractiveWhole;
                }
                if (ModeControl.ContinuousACT_In)
                {
                    BreathingControl.New_In_LongerThanMinimum -= ContinuousInteractiveWhole;
                }
                if (ModeControl.ContinuousACT_PeInPi)
                {
                    BreathingControl.New_PeInPi_LongerThanMinimum -= ContinuousInteractiveWhole;
                }
            }
        }
    }

    private void DiscreteProgressiveWhole()
    {
        float exposureChange = AllVfxsControl.backgroundChangingFinalExposure - AllVfxsControl.backgroundChangingInitialExposure;
        float transparencyChange = AllVfxsControl.backgroundChangingFinalTransparency - AllVfxsControl.backgroundChangingInitialTransparency;

        float colorChangeSteps = AllVfxsControl.backgroundChangingEndBN - AllVfxsControl.backgroundChangingStartBN;

        float exposureChangeSpeed = exposureChange / colorChangeSteps;
        float transarencyChangeSpeed = transparencyChange / colorChangeSteps;

        if (gameObject.activeSelf)
        {
            StartCoroutine(DiscreteProgressiveIncrement(AllVfxsControl.progressIncrementTimeForDiscrete,
                                                        exposureChangeSpeed,
                                                        transarencyChangeSpeed));
        }
    }
    private IEnumerator DiscreteProgressiveIncrement(float increment, float exposureSpeed, float transparencySpeed)
    {
        float startTime = Time.time;
        float endTime = startTime + increment;

        while (Time.time < endTime)
        {
            float deltaTime = Time.deltaTime;
            float currentExposure = backgroundMat.GetFloat("_Exposure");
            backgroundMat.SetFloat("_Exposure", currentExposure + ((exposureSpeed / increment) * deltaTime));
            Color color = backgroundMat.color;
            color.a += (transparencySpeed / increment) * deltaTime;
            yield return null;
        }
    }

    private void DiscreteInteractiveWhole()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(DiscreteInteractiveIncrement(AllVfxsControl.interactiveLoopTimeForDiscrete,
                                                        AllVfxsControl.backgroundChangingInitialExposure,
                                                        AllVfxsControl.backgroundChangingFinalExposure,
                                                        AllVfxsControl.backgroundChangingInitialTransparency,
                                                        AllVfxsControl.backgroundChangingFinalTransparency));
        }
    }
    private IEnumerator DiscreteInteractiveIncrement(float increment, 
                                                     float initialExposure, 
                                                     float finalExposure,
                                                     float initialTransparency,
                                                     float finalTransparency)
    {
        float startTime = Time.time;
        float middleTime = startTime + (increment / 2);
        float endTime = startTime + increment;

        while (Time.time < middleTime)
        {
            float progress = (middleTime - Time.time) / (increment / 2);
            float currentExposure = Mathf.Lerp(initialExposure, finalExposure, progress);
            float currentTransparency = Mathf.Lerp(initialTransparency, finalTransparency, progress);

            backgroundMat.SetFloat("_Exposure", currentExposure);
            Color color = backgroundMat.color;
            color.a = currentTransparency;

            yield return null;
        }
        while (Time.time > middleTime && Time.time < endTime)
        {
            float progress = (endTime - Time.time) / (increment / 2);
            float currentExposure = Mathf.Lerp(finalExposure, initialExposure, progress);
            float currentTransparency = Mathf.Lerp(finalTransparency, initialTransparency, progress);

            backgroundMat.SetFloat("_Exposure", currentExposure);
            Color color = backgroundMat.color;
            color.a = currentTransparency;

            yield return null;
        }
    }

    private void ContinuousProgressiveWhole(float fraction)
    {
        float exposureChange = AllVfxsControl.backgroundChangingFinalExposure - AllVfxsControl.backgroundChangingInitialExposure;
        float transparencyChange = AllVfxsControl.backgroundChangingFinalTransparency - AllVfxsControl.backgroundChangingInitialTransparency;

        float maxIncrementalExposureChange = exposureChange / AllVfxsControl.auraApproachingNumberOfExhalesIfCompleteToFinish;
        float maxIncrementalTransparencyChange = transparencyChange / AllVfxsControl.auraApproachingNumberOfExhalesIfCompleteToFinish;

        float currentIncrementalExposure = fraction * maxIncrementalExposureChange;
        float currentIncrementalTransparency = fraction * maxIncrementalTransparencyChange;

        Color color = backgroundMat.color;
        if (Mathf.Abs(color.a - AllVfxsControl.backgroundChangingFinalTransparency) > 0.01)
        {
            backgroundMat.SetFloat("_Exposure", currentIncrementalExposure);
            color.a = currentIncrementalTransparency;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void ContinuousInteractiveWhole(float fraction)
    {
        float currentExposure = Mathf.Lerp(AllVfxsControl.backgroundChangingInitialExposure,
                                           AllVfxsControl.backgroundChangingFinalExposure,
                                           fraction);
        //float currentTransparency = Mathf.Lerp(AllVfxsControl.backgroundChangingFinalTransparency,
        //                                       AllVfxsControl.backgroundChangingInitialTransparency,
        //                                       fraction);
        backgroundMat.SetFloat("_Exposure", currentExposure);
        //Color color = backgroundMat.color;
        //color.a = currentTransparency;
    }
}
