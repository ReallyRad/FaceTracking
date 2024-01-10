using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;

public class FogDisappearingControl : ProgressiveSequenceable
{
    [SerializeField] private VolumetricFog fog;
    [SerializeField] private AnimationCurve _progressCurve;
    private float intensityValue;
    
    private void Start()
    {
        //TODO set in inspector
        fog.settings.distance = 1000f;
        fog.settings.distantFogColor.a = 0.8f;
        fog.settings.turbulence = 0.9f;
        fog.settings.windDirection = new Vector3(-0.005f, 0f, 0f);
    }

    protected override void Progress(float progress) 
    {
        if (_active)
        {
            _localProgress += progress;
            
            var wasTransitioning = _transitioning;
            _transitioning = _localProgress > _startNextPhaseAt;
            
            if (_localProgress >= _completedAt) //end of this sequence step
            {
                _active = false;
                _transitioning = false;
                fog.gameObject.SetActive(false);
            }
            else
            {
                if (_transitioning && !wasTransitioning) StartNextPhase(this); //notify progressmanager to starting next phase 
                    
                var val = _progressCurve.Evaluate(_localProgress/_completedAt);
                intensityValue = Utils.Map(val, 0, 1, _initialValue, _finalValue);
                fog.settings.density = intensityValue;
            }
        }
    }

    public override void Initialize()
    {
        _active = true;
        fog.settings.density = _initialValue;
        fog.gameObject.SetActive(true);
        _localProgress = 0;
    }
}
