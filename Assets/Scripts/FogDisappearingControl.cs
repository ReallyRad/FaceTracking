using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;

public class FogDisappearingControl : ProgressiveSequenceable
{
    [SerializeField] private VolumetricFog fog;
    [SerializeField] private AnimationCurve _progressCurve;
    [SerializeField] private float _interactiveVal;
    
    private float intensityValue;

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
