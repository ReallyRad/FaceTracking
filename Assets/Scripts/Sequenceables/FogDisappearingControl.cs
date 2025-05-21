using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using VolumetricFogAndMist2;
using Random = UnityEngine.Random;

public class FogDisappearingControl : InteractiveSequenceable
{
    [SerializeField] private VolumetricFog fog;
    [SerializeField] private AnimationCurve _progressCurve;
    
    [Range(0f, 1f)] [SerializeField] private float _progressiveFactor; //stores the progressive part of the fog control
    [Range(0f, 1f)] [SerializeField] private float _interactiveVal; //stores the current interactive progress value; from 0 (no overshoot) to 1 (max overshoot)

    [Range(0f, 2f)] [SerializeField] private float _interactiveOvershootRange;

    [Range(0f, 15f)] [SerializeField] private float _perlinSpeed;
    [Range(0f, 2f)] [SerializeField] private float _noiseStrengthBaseline;
    [Range(0f, 1f)] [SerializeField] private float _noiseRange;
    
    [Range(0f, 1f)] [SerializeField] private float _density; //for display
    
    private int _interactTween;
    private int _decayTween;

    //TODO set color/alpha too?
    
    private void Update()
    {
        fog.settings.noiseStrength = _noiseStrengthBaseline + _interactiveVal * Mathf.PerlinNoise(Time.time * _perlinSpeed , 0.0f) * _noiseRange;
        fog.settings.density = 1 - (_interactiveVal * _interactiveOvershootRange + _progressiveFactor);
        _density = 1 - (_interactiveVal * _interactiveOvershootRange + _progressiveFactor);
    }

    public override void Initialize()
    {
        _active = true;
        fog.settings.density = _initialValue;
        _density = _initialValue;
        _interactiveVal = 0;
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
                _progressiveFactor = Utils.Map(val, 1, 0, _initialValue, _finalValue); //map it to the progress range
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
        
        //TODO integrate cycle ratio into interactive tween. 
        //float riseTime = (1 - _interactiveVal) * _riseTime; //time must be proportional to current progress to keep speed constant
        
        _interactTween = LeanTween
            .value(gameObject, _interactiveVal, 1f, _riseTime)
            .setOnUpdate(val =>
            {
                _interactiveVal = val;
                TweenHandling(val);                
            })
            .setEaseOutExpo()
            .id;
    }

    protected override void Decay()
    {
        if (_interactTween != 0)
        {
            LeanTween.pause(_interactTween);
            Debug.Log("paused interact tween  " + _interactTween);
        }

        //TODO integrate cycle ratio into interactive tween. 
        //float decayTime = _interactiveVal * _decayTime; //time must be proportional to current progress to keep speed constant
        
        _decayTween = LeanTween.value(gameObject, _interactiveVal, 0, _decayTime)
            .setOnUpdate(val =>
            {
                _interactiveVal = val;
                TweenHandling(val);                
            })
            .setEaseOutExpo()
            .id;
    }
    
    private void TweenHandling(float interactiveVal) //interactive tween handler
    {
        _interactiveVal = interactiveVal;
        //fog.settings.density = 1 - (interactiveVal * _interactiveOvershootRange + _progressiveFactor);
        //_density = 1 - (interactiveVal * _interactiveOvershootRange + _progressiveFactor);
    }

}
