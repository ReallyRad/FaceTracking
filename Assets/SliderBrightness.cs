using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SliderBrightness : MonoBehaviour
{
    [Range (-1,1)]
    public float currentValue;
    
    private ShadowsMidtonesHighlights shadowMidHigh;
    public Volume volum;

    [Range (0,1)]
    public float _hue;
    
    [Range (0,1)]
    public float _saturation;
    
    [Range (0,1)]
    public float _brightness;

    [Range (0,1)]
    public float _frequency;
    
    [Range (0,1)]
    public float _phase;

    
    // Update is called once per frame
    private void Update() {
        if (volum.profile.TryGet<ShadowsMidtonesHighlights>(out shadowMidHigh))
        {
            _hue =  Time.realtimeSinceStartup * _frequency % 1;
            
            Color color = Color.HSVToRGB(_hue, _saturation,_brightness);
            
            shadowMidHigh.midtones.SetValue(new Vector4Parameter(new Vector4(
                color.r,
                color.g,
                color.b,
                currentValue
                )));
        }
    }
}