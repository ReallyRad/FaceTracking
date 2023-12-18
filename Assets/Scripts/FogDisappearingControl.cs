using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;

public class FogDisappearingControl : ProgressiveSequenceable
{
    [SerializeField] private VolumetricFog fog;
    [SerializeField] private AnimationCurve _progressCurve;
    private float intensityValue = 1;

    private void Start()
    {
        _progressCurve = new AnimationCurve();
        _progressCurve.AddKey(0f, 0f);
        _progressCurve.AddKey(_completedProgressAt, _completedProgressAt);
        Keyframe[] keys = _progressCurve.keys;
        keys[0].outTangent = 2f;
        keys[1].inTangent = 0.1f;
        _progressCurve.keys = keys;

        fog.settings.distance = 1000f;
        fog.settings.distantFogColor.a = 0.8f;
        fog.settings.turbulence = 0.9f;
        fog.settings.windDirection = new Vector3(-0.005f, 0f, 0f);
    }

    protected override void Progress(float progress) 
    {
        if (_active)
        {
            Debug.Log("fog progress " + progress);
            if (intensityValue <= _finalValue)
            {
                Completed(this);
                _active = false;
                fog.gameObject.SetActive(false);
            }
            else
            {
                float val = _progressCurve.Evaluate(progress);
                intensityValue = Utils.Map(val, 0, _completedProgressAt, _initialValue, _finalValue);
                //intensityValue = Utils.Map(progress, 0, _completedProgressAt, _initialValue, _finalValue);

                fog.settings.density = intensityValue;
            }
        }
    }

    public override void Initialize()
    {
        _active = true;
        fog.settings.density = _initialValue;
        fog.gameObject.SetActive(true);
        /* 
         fog = VolumetricFog.instance;
         fog.fogAreaPosition = Vector3.zero;
         fog.fogAreaTopology = FOG_AREA_TOPOLOGY.Box;
         fog.fogAreaDepth = 2f;
         fog.fogAreaHeight = 2.0f;
         */
    }

}
