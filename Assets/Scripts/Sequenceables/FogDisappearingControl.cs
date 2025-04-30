using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;

public class FogDisappearingControl : InteractiveSequenceable
{
    [SerializeField] private VolumetricFog fog;
    [SerializeField] private AnimationCurve _progressCurve;
    
    private float _prgressiveFactor;

    private int _interactTween;
    private int _decayTween;
    
    [SerializeField] private float _interactiveVal; //stores the current interactive progress value;

    
    public override void Initialize()
    {
        _active = true;
        fog.settings.density = _initialValue;
        fog.gameObject.SetActive(true);
        _localProgress = 0;
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
                    
                var val = _progressCurve.Evaluate(_localProgress/_completedAt); // get the point of local progres
                _prgressiveFactor = Utils.Map(val, 0, 1, _initialValue, _finalValue); //map it to the progress range
                //fog.settings.density = intensityValue; //apply it to the fog
            }
        }
    }

    protected override void Interact() 
    {
        if (_decayTween != 0)
        {
            LeanTween.pause(_decayTween);
            Debug.Log("paused decay tween  " + _decayTween);
        }
        
        //float riseTime = (1 - _interactiveVal) * _riseTime; //time must be proportional to current progress to keep speed constant
        
        _interactTween = LeanTween
            .value(gameObject, _interactiveVal, 1, _riseTime)
            .setOnUpdate(val =>
            {
                _interactiveVal = val;
                TweenHandling(val);                
            })
            .id;
    }

    protected override void Decay()
    {
        if (_interactTween != 0)
        {
            LeanTween.pause(_interactTween);
            Debug.Log("paused interact tween  " + _interactTween);
        }

        //float decayTime = _interactiveVal * _decayTime; //time must be proportional to current progress to keep speed constant
        
        _decayTween = LeanTween.value(gameObject, _interactiveVal, 0, _decayTime)
            .setOnUpdate(val =>
            {
                _interactiveVal = val;
                TweenHandling(val);                
            })
            .id;
    }
    
    private void TweenHandling(float val) //interactive tween handler
    {
        fog.settings.density = val;
    }

}
