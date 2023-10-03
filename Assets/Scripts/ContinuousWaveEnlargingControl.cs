using Meta.WitAi.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousWaveEnlargingControl : MonoBehaviour
{
    private ParticleSystem ps;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        ContinuousBreathingControl.NewExhalingLongerThanMinimum += WholeEnlargementVFX;
    }

    private void OnDisable()
    {
        ContinuousBreathingControl.NewExhalingLongerThanMinimum -= WholeEnlargementVFX;
    }

    private void WholeEnlargementVFX(float fraction)
    {
        float scaleChange = ContinuousVfxControl.waveEnlargingFinalScale - ContinuousVfxControl.waveEnlargingIntialScale;
        float maxIncrementalScaleChange = scaleChange / ContinuousVfxControl.waveEnlargingNumberOfExhalesIfCompleteToFinish;
        float currentIncrementalScaleChange = fraction * maxIncrementalScaleChange;
        
        if (ps.transform.localScale.x < ContinuousVfxControl.waveEnlargingFinalScale) 
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


}
