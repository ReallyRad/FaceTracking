using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NorthernLightsInteractiveControl : InteractiveSequenceable
{
    [SerializeField] private ParticleSystem _northernLightParticleSystem;
    [SerializeField] private float _interactiveVal; //stores the current interactive progress value;
    
    private Material _northernLightMaterial;
    private int _interactTween;
    private int _decayTween;
    
    public override void Initialize()
    {
        _active = true;
        _northernLightMaterial = _northernLightParticleSystem.GetComponent<Renderer>().material;
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
            .value(gameObject, _interactiveVal, 0.8f, riseTime)
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
        int x = 2;
    }
        
    private void TweenHandling(float val) //interactive tween handler
    {
        _northernLightMaterial.SetColor("Color_A02DDA31", new Color(val,val,val));
    }

}
