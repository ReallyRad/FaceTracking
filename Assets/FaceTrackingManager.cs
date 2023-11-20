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

    [Range(-0.35f, 0.05f)]
    [SerializeField] private float _mouthValue; //from -1 to +1

    [SerializeField] private float _puckerThreshold;
    [SerializeField] private float _neutralThreshold;
    [SerializeField] private float _smileThreshold;

    private bool _smiling;
    private bool _slightSmile;
    private bool _pucker;
    private bool _slightPucker;

    [SerializeField] private FaceData _faceData;

    [SerializeField] private bool _manualSmileControl;

    [SerializeField] private bool _autoDebugBreathing;
    [SerializeField] private float _debugBreathRate;

    public delegate void OnMouthValue(float mouthValue);
    public static OnMouthValue MouthValue;

    public delegate void OnFaceExpression();
    public static OnFaceExpression FaceExpression;

    private int _previousProgressValue;
    private int _progressValue;

    private void Update()
    {
        float smileValue = 0f;
        float puckerValue = 0f;

        Vector2 lipPucker = new Vector2();
        Vector2 lipCornerPuller = new Vector2();

        lipPucker = GetExpressionValue(OVRFaceExpressions.FaceExpression.LipPuckerL, OVRFaceExpressions.FaceExpression.LipPuckerR);
        lipCornerPuller = GetExpressionValue(OVRFaceExpressions.FaceExpression.LipCornerPullerL, OVRFaceExpressions.FaceExpression.LipCornerPullerR);

        if (_autoDebugBreathing)
        {
            _mouthValue = Mathf.Sin(Time.time * _debugBreathRate) * 0.075f;
            //pucker value is the negative half of mouthvalue
            if (_mouthValue < _neutralThreshold)
            {
                puckerValue = -_mouthValue;
                smileValue = 0f;
            }
            //smile value is the positive half of mouthvalue
            else if (_mouthValue > _neutralThreshold)
            {
                puckerValue = 0f;
                smileValue = _mouthValue;
            }
        }
        else if (!_manualSmileControl) //using face tracking
        {
            smileValue = (lipCornerPuller.x + lipCornerPuller.y) / 2;
            puckerValue = (lipPucker.x + lipPucker.y) / 2;
            _mouthValue = smileValue - puckerValue; //pucker is the negative half, smile is the positive half 
        }
        else //use the inspector slider
        {
            if (_mouthValue < _neutralThreshold)
            {
                puckerValue = -_mouthValue;
                smileValue = 0;
            }
            else if (_mouthValue > _neutralThreshold)
            {
                smileValue = _mouthValue;
                puckerValue = 0;
            }
        }

        Debug.Log("mouthValue " + _mouthValue);
        
        MouthValue(_mouthValue);

        _smiling = _mouthValue >= _smileThreshold; //smile is a value bigger than smile threshold
        _slightSmile = _mouthValue < _smileThreshold && _mouthValue > _neutralThreshold;
        _pucker = _mouthValue <= _puckerThreshold;
        _slightPucker = _mouthValue > _puckerThreshold && _mouthValue < _neutralThreshold;

        if (!_faceData.previouslyPucker && _pucker || 
            !_faceData.previouslySmiling && _smiling ||
            !_faceData.previouslySlightPucker && _slightPucker || 
            !_faceData.previouslySlightSmile && _slightSmile)
        {
            _faceData.SetData(_smiling, _slightSmile, _slightPucker, _pucker);
            FaceExpression();
        }
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
    
    // c#
    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }
}
