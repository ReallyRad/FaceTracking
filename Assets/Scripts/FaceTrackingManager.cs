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

        if (_autoDebugBreathing)
        {
            _mouthValue = Mathf.Sin(Time.time * _debugBreathRate) * 0.075f;
        }
        else if (!_manualSmileControl) //using face tracking
        {
            lipPucker = GetExpressionValue(OVRFaceExpressions.FaceExpression.LipPuckerL, OVRFaceExpressions.FaceExpression.LipPuckerR);
            lipCornerPuller = GetExpressionValue(OVRFaceExpressions.FaceExpression.LipCornerPullerL, OVRFaceExpressions.FaceExpression.LipCornerPullerR);

            smileValue = (lipCornerPuller.x + lipCornerPuller.y) / 2;
            puckerValue = (lipPucker.x + lipPucker.y) / 2;
            _mouthValue = smileValue - puckerValue; //pucker is the negative half, smile is the positive half 
        }

        //MouthValue(_mouthValue);

        var smiling = _mouthValue >= _smileThreshold; 
        var slightSmile = _mouthValue < _smileThreshold && _mouthValue > _neutralThreshold;
        var pucker = _mouthValue <= _puckerThreshold;
        var slightPucker = _mouthValue > _puckerThreshold && _mouthValue < _neutralThreshold;

        if (!_faceData.previouslyPucker && pucker || 
            !_faceData.previouslySmiling && smiling ||
            !_faceData.previouslySlightPucker && slightPucker || 
            !_faceData.previouslySlightSmile && slightSmile)
        {
            _faceData.SetData(smiling, slightSmile, slightPucker, pucker);
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
