using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousWaveChangingColorControl : MonoBehaviour
{
    private ParticleSystem ps;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        ContinuousBreathingControl.NewExhalingLongerThanMinimum += WholeColorChangeVFX;
    }

    private void OnDisable()
    {
        ContinuousBreathingControl.NewExhalingLongerThanMinimum -= WholeColorChangeVFX;
    }

    private void WholeColorChangeVFX(float fraction)
    {
        float redChange = ContinuousVfxControl.waveChangingColorFinalColor.r - ContinuousVfxControl.waveChangingColorInitialColor.r;
        float greenChange = ContinuousVfxControl.waveChangingColorFinalColor.g - ContinuousVfxControl.waveChangingColorInitialColor.g;
        float blueChange = ContinuousVfxControl.waveChangingColorFinalColor.b - ContinuousVfxControl.waveChangingColorInitialColor.b;
        float transChange = ContinuousVfxControl.waveChangingColorFinalColor.a - ContinuousVfxControl.waveChangingColorInitialColor.a;

        float maxIncrementalRedChange = redChange / ContinuousVfxControl.waveChangingColorNumberOfExhalesIfCompleteToFinish;
        float maxIncrementalGreenChange = greenChange / ContinuousVfxControl.waveChangingColorNumberOfExhalesIfCompleteToFinish;
        float maxIncrementalBlueChange = blueChange / ContinuousVfxControl.waveChangingColorNumberOfExhalesIfCompleteToFinish;
        float maxIncrementalTransChange = transChange / ContinuousVfxControl.waveChangingColorNumberOfExhalesIfCompleteToFinish;

        float currentIncrementalRedChange = fraction * maxIncrementalRedChange;
        float currentIncrementalGreenChange = fraction * maxIncrementalGreenChange;
        float currentIncrementalBlueChange = fraction * maxIncrementalBlueChange;
        float currentIncrementalTransChange = fraction * maxIncrementalTransChange;

        ParticleSystemRenderer psRenderer = ps.GetComponent<ParticleSystemRenderer>();
        Color currentCol = psRenderer.material.color;
        if (Vector4.Distance(new Vector4(currentCol.r, currentCol.g, currentCol.b, currentCol.a), 
                             new Vector4(ContinuousVfxControl.waveChangingColorFinalColor.r,
                                         ContinuousVfxControl.waveChangingColorFinalColor.g,
                                         ContinuousVfxControl.waveChangingColorFinalColor.b,
                                         ContinuousVfxControl.waveChangingColorFinalColor.a)) > 0.05f) 
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

    
}
