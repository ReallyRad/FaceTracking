using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist;

public class FogDisappearingControl : Sequenceable
{
    private VolumetricFog fog;
  
    protected override void Progress(float progress)
    {
        if (active)
            if (progress == 50) Completed(this);
    }

    public override void Initialize()
    {
        fog = VolumetricFog.instance;
        fog.fogAreaPosition = Vector3.zero;
        fog.fogAreaTopology = FOG_AREA_TOPOLOGY.Box;
        fog.fogAreaDepth = 2f;
        fog.fogAreaHeight = 2.0f;
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

}
