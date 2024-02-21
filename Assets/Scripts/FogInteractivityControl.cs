using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VolumetricFogAndMist2;

public class FogInteractivityControl : InteractiveSequenceable
{
    private int _interactTween;
    private int _decayTween;

    //stores the current interactive progress value;
    [SerializeField] private float _interactiveVal;
    [SerializeField] private VolumetricFog fog;
    [SerializeField] private Transform head;
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

        float cycleRatio = Utils.Map(_interactiveVal, _initialValue, _finalValue, 0, 1); //calculate the ratio of the current progress
        float riseTime = (1 - cycleRatio) * _riseTime; //time must be proportional to current progress to keep speed constant

        _interactTween = LeanTween
            .value(gameObject, _interactiveVal, _finalValue, riseTime)
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

        
        float cycleRatio = Utils.Map(_interactiveVal, _initialValue, _finalValue, 0, 1);//calculate the ratio of the current progress
        float decayTime = cycleRatio * _decayTime; //time must be proportional to current progress to keep speed constant

        _decayTween = LeanTween.value(gameObject, _interactiveVal, _initialValue, decayTime)
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
            else
            {
                //var val = _progressCurve.Evaluate(_localProgress / _completedAt);
                //intensityValue = Utils.Map(_localProgress, 0, _completedAt, _initialValue, _finalValue);
                //fog.settings.density = intensityValue;
            }
        }
    }

    private void TweenHandling(float val)
    {
        fog.settings.windDirection = val * head.forward;
    }
}
