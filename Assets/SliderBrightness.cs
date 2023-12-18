using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SliderBrightness : MonoBehaviour
{
  //  private float lastValue;
    
    [Range (0,1)]
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

    private void Start()
    {
      // TweenRecursive();
    }

    private void TweenRecursive()
    {
        LeanTween.value(0, 1, 1/_frequency).setOnUpdate(val =>
        { 
            if (volum.profile.TryGet<ShadowsMidtonesHighlights>(out shadowMidHigh))
            {
                _hue = val;
                    
                Debug.Log("_amplitude, frequency, _phase, currentValue");
                Debug.Log(shadowMidHigh.shadows.value);
            
                //Color color = Color.HSVToRGB(_hue, _saturation,_brightness);
            
                shadowMidHigh.shadows.SetValue(new Vector4Parameter(new Vector4(
                       _hue,
                       _hue,
                       _hue,
                        currentValue
                    )))
                
                /*
                shadowMidHigh.shadows.SetValue(new Vector4Parameter(new Vector4(
                    color.r,
                    color.g,
                    color.b,
                    currentValue
                )))*/;
            }
        }).setOnComplete(f => TweenRecursive());
    }
    
    // Update is called once per frame
    private void Update() {
        if (volum.profile.TryGet<ShadowsMidtonesHighlights>(out shadowMidHigh))
        {
//            Vector4 shadowVal = shadowMidHigh.shadows.value;
            //shadowMidHigh.shadows.overrideState = true;
            
            Debug.Log("_amplitude, frequency, _phase, currentValue");
            Debug.Log(shadowMidHigh.shadows.value);
            
            //Debug.Log( " : " +_amplitude + " " +  frequency + " " +  _phase + " " + currentValue);

            //_hue = (Mathf.Sin(Time.realtimeSinceStartup / _frequency) + 1) / 2 * _amplitude;

            //_hue =  Time.realtimeSinceStartup * _frequency % 1;
            
            Color color = Color.HSVToRGB(_hue, _saturation,_brightness);
            
            shadowMidHigh.shadows.SetValue(new Vector4Parameter(new Vector4(
                color.r,
                color.g,
                color.b,
                currentValue
                )));
        }
    }
}