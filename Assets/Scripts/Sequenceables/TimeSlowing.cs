using System;
using Oculus.Interaction;
using RenderHeads.Media.AVProVideo;
using SCPE;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class TimeSlowing : InteractiveSequenceable
{
    public AudioSource wavesSound;
    public MediaPlayer mediaPlayer;

    public float playSpeed;

    private int _interactTween;
    private int _decayTween;

    [SerializeField] private float _interactiveVal;

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

        _decayTween = LeanTween
            .value(gameObject, _interactiveVal, _initialValue, decayTime)
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
            }
            else
            {
                if (_transitioning && !wasTransitioning) StartNextPhase(this);
            }
        }
    }
    public override void Initialize()
    {
        _active = true;
        _localProgress = 0;
    }

    private void TweenHandling(float val)
    {
        mediaPlayer.Control.SetPlaybackRate(GetClosestSnapValue(val));

        wavesSound.pitch = val; 

        playSpeed = mediaPlayer.PlaybackRate;
    }
    
    private float GetClosestSnapValue(float value) // Method to find the closest snap value
    {
        float[] snapValues = { 0.5f, 1.0f, 1.25f, 1.5f, 1.75f, 2f };
        
        return snapValues.OrderBy(x => Mathf.Abs(x - value)).First(); // Find the closest value from the snapValues array
    }

}
