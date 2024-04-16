using System;
using System.Collections;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ImageSaturation: InteractiveSequenceable
{
    [SerializeField] private Volume _effectVolume;
    
    private int _interactTween;
    private int _decayTween;

    [SerializeField] private float _interactiveVal; //stores the current interactive progress value;
    
    public override void Initialize()
    {
        _active = true;
    }
    protected override void Interact() 
    {
        if (_decayTween != 0)
        {
            LeanTween.pause(_decayTween);
            Debug.Log("paused decay tween  " + _decayTween);
        }
        
        float riseTime = (1 - _interactiveVal) * _riseTime; //time must be proportional to current progress to keep speed constant
        
        _interactTween = LeanTween
            .value(gameObject, _interactiveVal, 1, riseTime)
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

        float decayTime = _interactiveVal * _decayTime; //time must be proportional to current progress to keep speed constant
        
        _decayTween = LeanTween.value(gameObject, _interactiveVal, 0, decayTime)
            .setOnUpdate(val =>
            {
                _interactiveVal = val;
                TweenHandling(val);                
            })
            .id;
    }

    protected override void Progress(float progress)
    {
        if (_active)
        {
            _localProgress += progress;
            
            if (_localProgress >= _completedAt) //end of this sequence step
            {
                _active = false;
                
                if (_interactTween != 0) LeanTween.pause(_interactTween);
                if (_decayTween != 0) LeanTween.pause(_decayTween);
            }
        } 
    }
    
    private void TweenHandling(float val) //interactive tween handler
    {
        var mappedVal = Utils.Map(val, 0, 1, 0, 1f);
        _effectVolume.weight = mappedVal;
    }
}
