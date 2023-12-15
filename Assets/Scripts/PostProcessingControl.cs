using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingControl : InteractiveSequenceable
{
    [SerializeField] private Volume _volume;
    private int _interactTween;
    private int _decayTween;
    
    [SerializeField] private AudioLowPassFilter[] _lowPassFilters;
    [SerializeField] private AnimationCurve _lowPassFilterMapping;

    //TODO add OnComplete()
    
    public override void Initialize()
    {
        _active = true;
        _volume.weight = 0;
    }

    public override void Interact()
    {
        if (_decayTween != 0)
        {
            LeanTween.pause(_decayTween);
            Debug.Log("paused decay tween  " + _decayTween);
        }
        
        _interactTween = LeanTween.value(gameObject, _volume.weight, 1, _riseTime)
            .setOnUpdate(val =>
            {
                TweenHandling(val);                
            })
            .id;
    }

    public override void Decay()
    {
        if (_interactTween != 0)
        {
            LeanTween.pause(_interactTween);
            Debug.Log("paused interact tween  " + _interactTween);
        }
        
        _decayTween = LeanTween.value(gameObject, _volume.weight, 0, _decayTime)
            .setOnUpdate(val =>
            {
                TweenHandling(val);                
            })
            .id;
    }

    private void TweenHandling(float val)
    {
        _volume.weight = val;
        foreach (AudioLowPassFilter lowPassFilter in _lowPassFilters)
            lowPassFilter.cutoffFrequency = _lowPassFilterMapping.Evaluate(val) * 18000; //multiply to map to the audible frequency range        
    }
    
}
