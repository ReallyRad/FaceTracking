using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;

public class FogDisappearingControl : ProgressiveSequenceable
{
    [SerializeField] private VolumetricFog fog;

    protected override void Progress(float progress) 
    {
        if (_active)
        {
            Debug.Log("fog progress " + progress);
            if (progress >= _completedProgressAt)
            {
                Completed(this);
                _active = false;
            }
            else
            {
                fog.settings.density = Utils.Map(progress, 0, _completedProgressAt, _initialValue, _finalValue);
            }
        }
    }

    public override void Initialize()
    {
        _active = true;
        fog.settings.density = _initialValue;
        /* 
         fog = VolumetricFog.instance;
         fog.fogAreaPosition = Vector3.zero;
         fog.fogAreaTopology = FOG_AREA_TOPOLOGY.Box;
         fog.fogAreaDepth = 2f;
         fog.fogAreaHeight = 2.0f;
         */
    }

}
