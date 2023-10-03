using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist;

public class DiscreteFogDisappearningControl : MonoBehaviour
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
        FaceTrackingManager.NewFullBreath += WholeDisappearingFog;
        DiscreteBreathingControl.NewFullBreath += WholeDisappearingFog;
    }

    private void OnDisable()
    {
        FaceTrackingManager.NewFullBreath -= WholeDisappearingFog;
        DiscreteBreathingControl.NewFullBreath -= WholeDisappearingFog;
    }

    private void WholeDisappearingFog()
    {
        float intensityChange = DiscreteVfxControl.fogDisappearingFinalDensity - DiscreteVfxControl.fogDisappearingInitialDensity;
        float intensityChangeSteps = DiscreteVfxControl.fogDisappearingEndBN - DiscreteVfxControl.fogDisappearingStartBN;
        float intensityChangeSpeed = intensityChange / intensityChangeSteps;

        if (gameObject.activeSelf) 
        {
            //fog.density += intensityChangeSpeed;
            StartCoroutine(IncrementalDisappearanceFog(DiscreteVfxControl.increment, intensityChangeSpeed));
        }
    }

    private IEnumerator IncrementalDisappearanceFog(float increment, float speed)
    {
        float startTime = Time.time;
        float endTime = startTime + increment;

        while (Time.time < endTime)
        {
            float deltaTime = Time.deltaTime;
            fog.density += (speed/increment) * deltaTime;
            yield return null;
        }
    }

}
