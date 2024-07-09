using System;
using System.Collections;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingControl : InteractiveSequenceable
{
    public delegate void OnPostProcessingCompleted(); //triggered to notify the next sequence phase should start fading inx
    public static OnPostProcessingCompleted PostProcessingCompleted;
    
    public delegate void OnPostProcessingInteractiveProgress(float progress); //triggered to notify the next sequence phase should start fading inx
    public static OnPostProcessingInteractiveProgress PostProcessingInteractiveProgress;

    public delegate void OnPostProcessingProgressiveProgress(float progress); //triggered to notify the next sequence phase should start fading inx
    public static OnPostProcessingProgressiveProgress PostProcessingProgressiveProgress;
    
    [SerializeField] private Volume _effectVolume;

    [SerializeField] private AudioLowPassFilter[] _lowPassFilters;
    [SerializeField] private AnimationCurve _lowPassFilterMapping;

    [SerializeField] private AudioReverbZone reverbZone;
    [SerializeField] private int reverbZoneRoomInitialValue;
    [SerializeField] private int reverbZoneRoomFinalValue;

    [SerializeField] private float _interactiveVal; //stores the current interactive progress value;
    
    private int _interactTween;
    private int _decayTween;
    private Bloom _bloom;

    public override void Initialize()
    {
        _active = true;
        reverbZone.room = reverbZoneRoomInitialValue;
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
                StartNextPhase(this);
                
                if (_interactTween != 0) LeanTween.pause(_interactTween);
                if (_decayTween != 0) LeanTween.pause(_decayTween);
                
                if (_effectVolume.profile.TryGet(out _bloom))
                {
                    LeanTween.value(gameObject, _interactiveVal, 0, 5).setOnUpdate(val =>
                    {
                        TweenHandling(val); //simulate interactivedecay. 
                        //TODO use reverb to fade out the music  more elegantly  instead of reusing the tweenhandling method
                    });
                }
            }
            else
            {
                var mappedProgress = Utils.Map(_localProgress,0, _completedAt, 0,1);

                PostProcessingProgressiveProgress(mappedProgress);
                
                _effectVolume
                    .gameObject
                    .GetComponent<VolumeProfileProgressiveInterpolator>()
                    .Progress(mappedProgress);
                
                _effectVolume.GetComponent<ModifyColorTuning>().SetEffectStrength(Utils.Map(mappedProgress, 0, 1, 0, 0.29f));
            }
        } 
    }

    private void TweenHandling(float val) //interactive tween handler
    {
        _effectVolume.GetComponent<HueShiftRotator>().SetSaturation(Utils.Map(val, 0, 1, 0, 0.117f));

        PostProcessingInteractiveProgress(val);
        
        foreach (AudioLowPassFilter lowPassFilter in _lowPassFilters)
            lowPassFilter.cutoffFrequency = _lowPassFilterMapping.Evaluate(val) * 18000; //multiply to map to the audible frequency range        

        int reverbValue = (int) ((reverbZoneRoomFinalValue - reverbZoneRoomInitialValue) * val + reverbZoneRoomInitialValue);
        reverbZone.room = reverbValue;
    }

}
