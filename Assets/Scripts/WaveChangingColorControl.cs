using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveChangingColorControl : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystemRenderer psRenderer;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        psRenderer = ps.GetComponent<ParticleSystemRenderer>();
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
        float redChange = AllVfxsControl.waveChangingColorFinalColor.r - AllVfxsControl.waveChangingColorInitialColor.r;
        float greenChange = AllVfxsControl.waveChangingColorFinalColor.g - AllVfxsControl.waveChangingColorInitialColor.g;
        float blueChange = AllVfxsControl.waveChangingColorFinalColor.b - AllVfxsControl.waveChangingColorInitialColor.b;
        float transChange = AllVfxsControl.waveChangingColorFinalColor.a - AllVfxsControl.waveChangingColorInitialColor.a;

        float colorChangeSteps = AllVfxsControl.waveChangingColorEndBN - AllVfxsControl.waveChangingColorStartBN;

        float redChangeSpeed = redChange / colorChangeSteps;
        float greenChangeSpeed = greenChange / colorChangeSteps;
        float blueChangeSpeed = blueChange / colorChangeSteps;
        float transChangeSpeed = transChange / colorChangeSteps;

        Color speed = new Color(redChangeSpeed, greenChangeSpeed, blueChangeSpeed, transChangeSpeed);

        if (gameObject.activeSelf)
        {
            StartCoroutine(DiscreteProgressiveIncrement(AllVfxsControl.progressIncrementTimeForDiscrete, speed));
        }
    }
    private IEnumerator DiscreteProgressiveIncrement(float increment, Color speed)
    {
        float startTime = Time.time;
        float endTime = startTime + increment;

        while (Time.time < endTime)
        {
            float deltaTime = Time.deltaTime;

            
            Color currentCol = psRenderer.material.color;
            float newR = currentCol.r + ((speed.r) / increment) * deltaTime;
            float newG = currentCol.g + ((speed.g) / increment) * deltaTime;
            float newB = currentCol.b + ((speed.b) / increment) * deltaTime;
            float newT = currentCol.a + ((speed.a) / increment) * deltaTime;

            Color newColor = new Color(newR, newG, newB, newT);
            psRenderer.material.color = newColor;
            psRenderer.material.SetFloat("_Alpha", newT);

            yield return null;
        }
    }

    private void DiscreteInteractiveWhole()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(DiscreteInteractiveIncrement(AllVfxsControl.interactiveLoopTimeForDiscrete,
                                                        AllVfxsControl.waveChangingColorInitialColor,
                                                        AllVfxsControl.waveChangingColorFinalColor));
        }
    }
    private IEnumerator DiscreteInteractiveIncrement(float increment, Color initialColor, Color finalColor)
    {
        float startTime = Time.time;
        float middleTime = startTime + (increment / 2);
        float endTime = startTime + increment;

        while (Time.time < middleTime)
        {
            float progress = (middleTime - Time.time) / (increment / 2);
            Color currentColor = Color.Lerp(initialColor, finalColor, progress);
            float currentTransparency = Mathf.Lerp(initialColor.a, finalColor.a, progress);
            
            psRenderer.material.color = currentColor;
            psRenderer.material.SetFloat("_Alpha", currentTransparency);
            
            yield return null;
        }
        while (Time.time > middleTime && Time.time < endTime)
        {
            float progress = (endTime - Time.time) / (increment / 2);
            Color currentColor = Color.Lerp(finalColor, initialColor, progress);
            float currentTransparency = Mathf.Lerp(finalColor.a, initialColor.a, progress);

            psRenderer.material.color = currentColor;
            psRenderer.material.SetFloat("_Alpha", currentTransparency);

            yield return null;
        }
    }

    private void ContinuousProgressiveWhole(float fraction)
    {
        float redChange = AllVfxsControl.waveChangingColorFinalColor.r - AllVfxsControl.waveChangingColorInitialColor.r;
        float greenChange = AllVfxsControl.waveChangingColorFinalColor.g - AllVfxsControl.waveChangingColorInitialColor.g;
        float blueChange = AllVfxsControl.waveChangingColorFinalColor.b - AllVfxsControl.waveChangingColorInitialColor.b;
        float transChange = AllVfxsControl.waveChangingColorFinalColor.a - AllVfxsControl.waveChangingColorInitialColor.a;

        float maxIncrementalRedChange = redChange / AllVfxsControl.waveChangingColorNumberOfExhalesIfCompleteToFinish;
        float maxIncrementalGreenChange = greenChange / AllVfxsControl.waveChangingColorNumberOfExhalesIfCompleteToFinish;
        float maxIncrementalBlueChange = blueChange / AllVfxsControl.waveChangingColorNumberOfExhalesIfCompleteToFinish;
        float maxIncrementalTransChange = transChange / AllVfxsControl.waveChangingColorNumberOfExhalesIfCompleteToFinish;

        float currentIncrementalRedChange = fraction * maxIncrementalRedChange;
        float currentIncrementalGreenChange = fraction * maxIncrementalGreenChange;
        float currentIncrementalBlueChange = fraction * maxIncrementalBlueChange;
        float currentIncrementalTransChange = fraction * maxIncrementalTransChange;

        ParticleSystemRenderer psRenderer = ps.GetComponent<ParticleSystemRenderer>();
        Color currentCol = psRenderer.material.color;
        if (Vector4.Distance(new Vector4(currentCol.r, currentCol.g, currentCol.b, currentCol.a),
                             new Vector4(AllVfxsControl.waveChangingColorFinalColor.r,
                                         AllVfxsControl.waveChangingColorFinalColor.g,
                                         AllVfxsControl.waveChangingColorFinalColor.b,
                                         AllVfxsControl.waveChangingColorFinalColor.a)) > 0.05f)
        {
            float newR = currentCol.r + currentIncrementalRedChange;
            float newG = currentCol.g + currentIncrementalGreenChange;
            float newB = currentCol.b + currentIncrementalBlueChange;
            float newT = currentCol.a + currentIncrementalTransChange;

            Color newColor = new Color(newR, newG, newB, newT);
            psRenderer.material.color = newColor;
            psRenderer.material.SetFloat("_Alpha", newT);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void ContinuousInteractiveWhole(float fraction)
    {
        Color currentColor = Color.Lerp(AllVfxsControl.waveChangingColorInitialColor,
                                        AllVfxsControl.waveChangingColorFinalColor,
                                        fraction);
        float currentTransparency = Mathf.Lerp(AllVfxsControl.waveChangingColorInitialColor.a,
                                               AllVfxsControl.waveChangingColorFinalColor.a,
                                               fraction);
        psRenderer.material.color = currentColor;
        psRenderer.material.SetFloat("_Alpha", currentTransparency);
    }
}
