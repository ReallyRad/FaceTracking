using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;

public class SnowFog : InteractiveSequenceable
{
    [SerializeField] private VolumetricFog fog;
    [SerializeField] private AnimationCurve _progressCurve;
    [SerializeField] private float _interactiveVal;
    [SerializeField] private Transform _head;
    [SerializeField] private float _windFinalSpeed;
    [SerializeField] private float _windInitialSpeed;

    [SerializeField] private ParticleSystem _snowPS;
    [SerializeField] private AudioSource _backgroundSound;
    private float initialBackgroundSoundVolume = 0f;
    private float finalBackgroundSoundVolume = 0.3f;
    private float backgroundSoundRaisingDuration = 7f;

    private float intensityValue;
    private int _interactTween;
    private int _decayTween;

    private void Start()
    {
        var emission = _snowPS.emission;
        emission.rateOverTime = 0;
        StartCoroutine(BackgroundSoundFadingIn());
    }
    IEnumerator BackgroundSoundFadingIn()
    {
        _backgroundSound.volume = initialBackgroundSoundVolume;
        float startTime = Time.time;

        while (Time.time < startTime + backgroundSoundRaisingDuration)
        {
            _backgroundSound.volume = Mathf.Lerp(initialBackgroundSoundVolume, finalBackgroundSoundVolume, (Time.time - startTime) / backgroundSoundRaisingDuration);
            yield return null;
        }
        _backgroundSound.volume = finalBackgroundSoundVolume;
    }

    public override void Initialize()
    {
        _active = true;
        fog.settings.density = _initialValue;
        fog.gameObject.SetActive(true);
        _localProgress = 0;
    }

    protected override void Interact()
    {
        if (_decayTween != 0)
        {
            LeanTween.pause(_decayTween);
            Debug.Log("paused decay tween  " + _decayTween);
        }

        float cycleRatio = Utils.Map(_interactiveVal, _windInitialSpeed, _windFinalSpeed, 0, 1); //calculate the ratio of the current progress
        float riseTime = (1 - cycleRatio) * _riseTime; //time must be proportional to current progress to keep speed constant

        _interactTween = LeanTween
            .value(gameObject, _interactiveVal, _windFinalSpeed, riseTime)
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

        float cycleRatio = Utils.Map(_interactiveVal, _windInitialSpeed, _windFinalSpeed, 0, 1);//calculate the ratio of the current progress
        float decayTime = cycleRatio * _decayTime; //time must be proportional to current progress to keep speed constant

        _decayTween = LeanTween
            .value(gameObject, _interactiveVal, _windInitialSpeed, decayTime)
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

                var val = _progressCurve.Evaluate(_localProgress / _completedAt);
                intensityValue = Utils.Map(val, 0, 1, _initialValue, _finalValue);
                fog.settings.density = intensityValue;
            }
        }
    }

    private void TweenHandling(float val)
    {
        //fog.settings.windDirection = val * _head.forward;
    }
}
