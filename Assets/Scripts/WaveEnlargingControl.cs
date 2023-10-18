using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEnlargingControl : MonoBehaviour
{
    private ParticleSystem ps;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
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
        float scaleChange;
        float scaleChangeSteps;
        float scaleChangeSpeed;

        scaleChange = AllVfxsControl.waveEnlargingFinalScale - AllVfxsControl.waveEnlargingIntialScale;
        scaleChangeSteps = AllVfxsControl.waveEnlargingEndBN - AllVfxsControl.waveEnlargingStartBN;
        scaleChangeSpeed = scaleChange / scaleChangeSteps;

        if (gameObject.activeSelf)
        {
            StartCoroutine(DiscreteProgressiveIncrement(AllVfxsControl.progressIncrementTimeForDiscrete, scaleChangeSpeed));
        }
    }
    private IEnumerator DiscreteProgressiveIncrement(float increment, float speed)
    {
        float startTime = Time.time;
        float endTime = startTime + increment;

        while (Time.time < endTime)
        {
            float deltaTime = Time.deltaTime;
            ps.transform.localScale += new Vector3((speed / increment) * deltaTime,
                                                   (speed / increment) * deltaTime,
                                                   (speed / increment) * deltaTime);
            yield return null;
        }
    }

    private void DiscreteInteractiveWhole()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(DiscreteInteractiveIncrement(AllVfxsControl.interactiveLoopTimeForDiscrete,
                                                        AllVfxsControl.waveEnlargingIntialScale,
                                                        AllVfxsControl.waveEnlargingFinalScale));
        }
    }
    private IEnumerator DiscreteInteractiveIncrement(float increment, float initialScale, float finalScale)
    {
        float startTime = Time.time;
        float middleTime = startTime + (increment / 2);
        float endTime = startTime + increment;

        while (Time.time < middleTime)
        {
            float progress = (middleTime - Time.time) / (increment / 2);
            float currentScale = Mathf.Lerp(initialScale, finalScale, progress);
            ps.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            yield return null;
        }
        while (Time.time > middleTime && Time.time < endTime)
        {
            float progress = (endTime - Time.time) / (increment / 2);
            float currentScale = Mathf.Lerp(finalScale, initialScale, progress);
            ps.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            yield return null;
        }
    }

    private void ContinuousProgressiveWhole(float fraction)
    {
        float scaleChange = AllVfxsControl.waveEnlargingFinalScale - AllVfxsControl.waveEnlargingIntialScale;
        float maxIncrementalScaleChange = scaleChange / AllVfxsControl.waveEnlargingNumberOfExhalesIfCompleteToFinish;
        float currentIncrementalScaleChange = fraction * maxIncrementalScaleChange;

        if (ps.transform.localScale.x < AllVfxsControl.waveEnlargingFinalScale)
        {
            ps.transform.localScale += new Vector3(currentIncrementalScaleChange,
                                                   currentIncrementalScaleChange,
                                                   currentIncrementalScaleChange);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    private void ContinuousInteractiveWhole(float fraction)
    {
        float currentScale = Mathf.Lerp(AllVfxsControl.waveEnlargingIntialScale,
                                        AllVfxsControl.waveEnlargingFinalScale,
                                        fraction);

        ps.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
    }
}
