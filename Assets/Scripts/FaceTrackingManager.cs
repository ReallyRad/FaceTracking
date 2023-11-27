using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Metaface.Debug;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class FaceTrackingManager : MonoBehaviour
{
    [SerializeField] private OVRFaceExpressions faceExpressions;

    [Range(-0.065f, 0.0f)]
    [SerializeField] private float _mouthValue;
    
    [SerializeField] private float _puckerThreshold;

    [SerializeField] private bool _manualSmileControl;

    [SerializeField] private bool _autoDebugBreathing;
    [SerializeField] private float _debugBreathRate;

    public delegate void OnMouthValue(float mouthValue);
    public static OnMouthValue MouthValue;

    public delegate void OnPuckerTrigger(bool pucker);
    public static OnPuckerTrigger PuckerTrigger;

    private bool _sendMouthValue;
    private float _previousMouthValue;
    private void Update()
    {
        Vector2 lipPucker = new Vector2();
       
        if (_autoDebugBreathing)
        {
            _mouthValue = Mathf.Sin(Time.time * _debugBreathRate) * 0.03f - 0.03f;
        }
        else if (!_manualSmileControl) //using face tracking
        {
            lipPucker = GetExpressionValue(
                OVRFaceExpressions.FaceExpression.LipPuckerL,
                OVRFaceExpressions.FaceExpression.LipPuckerR);
            
            _mouthValue = - (lipPucker.x + lipPucker.y) / 2;
        }

        if (_sendMouthValue) MouthValue(_mouthValue);

        bool wasPucker = _previousMouthValue < _puckerThreshold;
        bool pucker = _mouthValue < _puckerThreshold;
        
        //if we just started pucker
        if (pucker && !wasPucker)
            PuckerTrigger(true);
        
        //if we just stopped pucker
        else if (wasPucker && !pucker) 
            PuckerTrigger(false); 
        
        _previousMouthValue = _mouthValue;
    }

    private Vector2 GetExpressionValue(OVRFaceExpressions.FaceExpression key1,
        OVRFaceExpressions.FaceExpression key2)
    {
        float w;
        Vector2 expressionVector = new Vector2();
        faceExpressions.TryGetFaceExpressionWeight(key1, out w);
        expressionVector.x = w;
        faceExpressions.TryGetFaceExpressionWeight(key2, out w);
        expressionVector.y = w;

        return expressionVector;
    }
    
}
