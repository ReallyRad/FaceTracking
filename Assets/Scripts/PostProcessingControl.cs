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
    
    public delegate void OnPostProcessingProgress(float progress); //triggered to notify the next sequence phase should start fading inx
    public static OnPostProcessingProgress PostProcessingProgress;

    [SerializeField] private Volume _effectVolume;
    [SerializeField] private Material _skyboxMaterial;
    [SerializeField] private Material _nightSkyMaterial;
    [SerializeField] private Material _pseudoMovementMaterial;

    [SerializeField] private AudioLowPassFilter[] _lowPassFilters;
    [SerializeField] private SeamlessLoop _shimmerSeamlessLoop;
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
                
                if (_effectVolume.profile.TryGet(out _bloom))
                {
                    StartCoroutine(WaitAndSwitchSkybox());  //switch texture halfway through

                    LeanTween.value(gameObject, _interactiveVal, 0, 5).setOnUpdate(val =>
                    {
                        TweenHandling(val); //simulate interactivedecay. 
                        //TODO use reverb to fade out the music  more elegantly  instead of reusing the tweenhandling method
                    });
                    
                    LeanTween.value(gameObject, 0.61f, 0, 5f)
                        .setOnUpdate(val =>
                        {
                            _bloom.threshold.value = val;
                            _skyboxMaterial.SetFloat("_Exposure", Utils.Map(val, 0.61f, 0, 0.6f, 2));
                            //TODO interpolate intensity as well
                            //TODO make sure weight is at 1 so that effect is applied 
                        })
                        .setRepeat(2)
                        .setEaseInOutCubic()
                        .setLoopPingPong();
                }

            }
            else
            {
                var mappedProgress = Utils.Map(_localProgress, 0, _completedAt, 0, 1);

                _effectVolume
                    .gameObject
                    .GetComponent<VolumeProfileProgressiveInterpolator>()
                    .Progress(mappedProgress);
                
                PostProcessingProgress(mappedProgress);
            }
        } 
    }

    private IEnumerator WaitAndSwitchSkybox()
    {
        Debug.Log("waiting, will switching skybox now");
        
        yield return new WaitForSeconds(5f);
        
        Debug.Log("waited, switching skybox now");
        
        StartNextPhase(this); //notify progressmanager to starting next phase 
        PostProcessingCompleted();
        
        LeanTween
            .value(1, 0, 5)
            .setOnUpdate(val =>
            {
                _effectVolume.gameObject.GetComponent<Volume>().weight = val; 
            })
            .setOnComplete(() => { _effectVolume.gameObject.GetComponent<VolumeProfileProgressiveInterpolator>().Progress(0); });

        RenderSettings.skybox = _nightSkyMaterial;
    }
    
    private void TweenHandling(float val) //interactive tween handler
    {
        _effectVolume.GetComponent<HueShiftRotator>().SetSaturation(Utils.Map(val, 0, 1, 0, 0.25f));

        foreach (AudioLowPassFilter lowPassFilter in _lowPassFilters)
            lowPassFilter.cutoffFrequency = _lowPassFilterMapping.Evaluate(val) * 18000; //multiply to map to the audible frequency range        

        int reverbValue = (int) ((reverbZoneRoomFinalValue - reverbZoneRoomInitialValue) * val + reverbZoneRoomInitialValue);
        reverbZone.room = reverbValue;
    }

}
