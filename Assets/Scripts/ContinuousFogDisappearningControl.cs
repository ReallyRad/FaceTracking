using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist;

public class ContinuousFogDisappearningControl : MonoBehaviour
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
        ContinuousBreathingControl.NewExhalingLongerThanMinimum += WholeDisappearingFog;
    }

    private void OnDisable()
    {
        ContinuousBreathingControl.NewExhalingLongerThanMinimum -= WholeDisappearingFog;
    }

    private void WholeDisappearingFog(float fraction)
    {
        float intensityChange = ContinuousVfxControl.fogDisappearingFinalDensity - ContinuousVfxControl.fogDisappearingInitialDensity;
        float maxIncrementalIntensityChange = intensityChange / ContinuousVfxControl.fogDisappearingNumberOfExhalesIfCompleteToFinish;
        float currentIncrementalIntensityChange = fraction * maxIncrementalIntensityChange;

        if (fog.density > ContinuousVfxControl.fogDisappearingFinalDensity) 
        {
            fog.density += currentIncrementalIntensityChange;
        }
        else
        {
            gameObject.SetActive(false);
            fog.enabled = false;
        }
    }
}
