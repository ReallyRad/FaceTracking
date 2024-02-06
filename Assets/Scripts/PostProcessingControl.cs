using System;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingControl : InteractiveSequenceable
{
    [SerializeField] private Volume _effectVolume;
    [SerializeField] private Material _skyboxMaterial;
    [SerializeField] private Texture _spaceTexture;

    [SerializeField] private AudioLowPassFilter[] _lowPassFilters;
    [SerializeField] private SeamlessLoop _shimmerSeamlessLoop;
    [SerializeField] private AnimationCurve _lowPassFilterMapping;

    [SerializeField] private AudioReverbZone reverbZone;
    [SerializeField] private int reverbZoneRoomInitialValue;
    [SerializeField] private int reverbZoneRoomFinalValue;

    private int _interactTween;
    private int _decayTween;
    private Bloom _bloom;

    public override void Initialize()
    {
        _active = true;
        //_shimmerSeamlessLoop.SetVolume(1); //TODO use mixer?
        reverbZone.room = reverbZoneRoomInitialValue;
    }

    protected override void Interact()
    {
        if (_decayTween != 0)
        {
            LeanTween.pause(_decayTween);
            Debug.Log("paused decay tween  " + _decayTween);
        }
        
        _interactTween = LeanTween
            .value(gameObject, 0, 1, _riseTime)
            .setOnUpdate(val =>
            {
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
        
        _decayTween = LeanTween.value(gameObject, 1, 0, _decayTime)
            .setOnUpdate(val =>
            {
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
                if (_effectVolume.profile.TryGet(out _bloom))
                {
                    LeanTween.value(gameObject, 0.61f, 0, 5f)
                        .setOnUpdate(val =>
                        {
                            _bloom.threshold.value = val;
                            _skyboxMaterial.SetFloat("_Exposure", Utils.Map(val, 0.61f, 0, 0.6f, 2));
                            //TODO interpolate intensity as well
                            //TODO make sure weight is at 1 so that effect is applied 
                        })
                        .setRepeat(2)
                        .setLoopPingPong()
                        .setOnCompleteOnRepeat(true)
                        .setEaseInOutSine()
                        .setOnComplete(() =>
                        {
                            Debug.Log("Complete");
                            _skyboxMaterial.SetTexture("_MainTex", _spaceTexture);
                            _active = false;
                        });
                }
            }
            else
            {
                _effectVolume.gameObject.GetComponent<VolumeProfileProgressiveInterpolator>().Progress(Utils.Map(_localProgress,0,_completedAt,0,1));
            }
        } 
    }

    private void TweenHandling(float val) //interactive tween handler
    {
        _effectVolume.GetComponent<HueShiftRotator>().SetSaturation(Utils.Map(val, 0, 1, 0, 0.1f));

        foreach (AudioLowPassFilter lowPassFilter in _lowPassFilters)
            lowPassFilter.cutoffFrequency = _lowPassFilterMapping.Evaluate(val) * 18000; //multiply to map to the audible frequency range        

        float reverbValue = (reverbZoneRoomFinalValue - reverbZoneRoomInitialValue) * val + reverbZoneRoomInitialValue;
        int intReverbValue = Mathf.RoundToInt(reverbValue);
        reverbZone.room = intReverbValue;

    }

}
