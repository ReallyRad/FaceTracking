using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
//using UnityEngine.Rendering.PostProcessing;
using Unity.VisualScripting;

public class PostProcessingControl : InteractiveSequenceable
{
    [SerializeField] private Volume _volume;
    private int _interactTween;
    private int _decayTween;
    
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
                _volume.weight = val;
            })
            .setEaseInCirc()
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
                _volume.weight = val;
            })
            .setEaseInCirc()
            .id;
    }
}
