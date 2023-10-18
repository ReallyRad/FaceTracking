using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist;

public class FogDisappearingControl : MonoBehaviour
{
    private VolumetricFog fog;
    void Start()
    {
        fog = VolumetricFog.instance;
        fog.fogAreaPosition = Vector3.zero;
        fog.fogAreaTopology = FOG_AREA_TOPOLOGY.Box;
        fog.fogAreaDepth = 2f;
        fog.fogAreaHeight = 2.0f;
    }
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
        float intensityChange = AllVfxsControl.fogDisappearingFinalDensity - AllVfxsControl.fogDisappearingInitialDensity;
        float intensityChangeSteps = AllVfxsControl.fogDisappearingEndBN - AllVfxsControl.fogDisappearingStartBN;
        float intensityChangeSpeed = intensityChange / intensityChangeSteps;

        if (gameObject.activeSelf)
        {
            StartCoroutine(DiscreteProgressiveIncrement(AllVfxsControl.progressIncrementTimeForDiscrete, intensityChangeSpeed));
        }
    }
    private IEnumerator DiscreteProgressiveIncrement(float increment, float speed)
    {
        float startTime = Time.time;
        float endTime = startTime + increment;

        while (Time.time < endTime)
        {
            float deltaTime = Time.deltaTime;
            fog.density += (speed / increment) * deltaTime;
            yield return null;
        }
    }

    private void DiscreteInteractiveWhole()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(DiscreteInteractiveIncrement(AllVfxsControl.interactiveLoopTimeForDiscrete,
                                                        AllVfxsControl.fogDisappearingInitialDensity,
                                                        AllVfxsControl.fogDisappearingFinalDensity));
        }
    }
    private IEnumerator DiscreteInteractiveIncrement(float increment, float initialDensity, float finalDensity)
    {
        float startTime = Time.time;
        float middleTime = startTime + (increment / 2);
        float endTime = startTime + increment;

        while (Time.time < middleTime)
        {
            float progress = (middleTime - Time.time) / (increment / 2);
            float currentDensity = Mathf.Lerp(initialDensity, finalDensity, progress);
            fog.density = currentDensity;
            yield return null;
        }
        while (Time.time > middleTime && Time.time < endTime)
        {
            float progress = (endTime - Time.time) / (increment / 2);
            float currentDensity = Mathf.Lerp(finalDensity, initialDensity, progress);
            fog.density = currentDensity;
            yield return null;
        }
    }

    private void ContinuousProgressiveWhole(float fraction)
    {
        float densityChange = AllVfxsControl.fogDisappearingFinalDensity - AllVfxsControl.fogDisappearingInitialDensity;
        float maxIncrementalDensityChange = densityChange / AllVfxsControl.auraApproachingNumberOfExhalesIfCompleteToFinish;
        float currentIncrementalDensity = fraction * maxIncrementalDensityChange;

        if (fog.density > AllVfxsControl.fogDisappearingFinalDensity)
        {
            fog.density += currentIncrementalDensity;
        }
        else
        {
            gameObject.SetActive(false);
            fog.enabled = false;
        }
    }

    private void ContinuousInteractiveWhole(float fraction)
    {
        float currentDensity = Mathf.Lerp(AllVfxsControl.fogDisappearingInitialDensity, 
                                          AllVfxsControl.fogDisappearingFinalDensity, 
                                          fraction);
        fog.density = currentDensity;
    }
}
