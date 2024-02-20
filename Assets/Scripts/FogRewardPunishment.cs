using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VolumetricFogAndMist2;

public class FogRewardPunishment : InteractiveSequenceable
{
    private int _interactTween;
    private int _decayTween;
    private float intensityValue;

    //stores the current interactive progress value;
    [SerializeField] private float _interactiveVal;
    [SerializeField] private VolumetricFog fog;

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

        //time must be proportional to current progress to keep speed constant
        float riseTime = (1 - _interactiveVal) * _riseTime;

        //float targetVal = 1.4f * fog.settings.density;
        //if(targetVal >= _fogIntensityFinalValue) targetVal = _fogIntensityFinalValue;

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

        //time must be proportional to current progress to keep speed constant
        float decayTime = _interactiveVal * _decayTime;

        //float targetVal = 0.8f * fog.settings.density;
        //if (targetVal <= _fogIntensityInitialValue) targetVal = _fogIntensityInitialValue;

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
        fog.settings.density = val;
    }
}
