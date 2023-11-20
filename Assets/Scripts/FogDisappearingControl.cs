using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;

public class FogDisappearingControl : Sequenceable
{
    [SerializeField] private VolumetricFog fog;

    [SerializeField] private float _initialValue; //TODO put it in the abstract class?
    
    protected override void Progress(float progress) 
    {
        if (_active)
        {
            if (progress <= _finalValue)
            {
                Completed(this);
                _active = false;
            }
            else
            {
                fog.settings.density = Utils.Map(progress, 0, 6, _initialValue, 0);
            }
        }
    }

    public override void Initialize()
    {
        _active = true;

        /* 
         fog = VolumetricFog.instance;
         fog.fogAreaPosition = Vector3.zero;
         fog.fogAreaTopology = FOG_AREA_TOPOLOGY.Box;
         fog.fogAreaDepth = 2f;
         fog.fogAreaHeight = 2.0f;
         */
    }

}
