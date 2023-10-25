using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Metaface.Debug;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FaceTrackingManager : MonoBehaviour
{
    [SerializeField] private OVRFaceExpressions faceExpressions;

    [Range(-0.05f, 0.05f)]
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
            if (_mouthValue < 0)
            {
                puckerValue = -_mouthValue;
                smileValue = 0f;
            }
            //smile value is the positive half of mouthvalue
            else if (_mouthValue > 0)
            {
                puckerValue = 0f;
                smileValue = _mouthValue;
            }
        }
        else if (!_manualSmileControl)
        {
            smileValue = (lipCornerPuller.x + lipCornerPuller.y) / 2;
            puckerValue = (lipPucker.x + lipPucker.y) / 2;
            _mouthValue = smileValue - puckerValue;
        }
        else
        {
            if (_mouthValue < 0)
            {
                puckerValue = -_mouthValue;
                smileValue = 0;
            }
            else if (_mouthValue > 0)
            {
                smileValue = _mouthValue;
                puckerValue = 0;
            }
        }

        MouthValue(_mouthValue);

        _smiling = smileValue >= _smileThreshold;
        _slightSmile = smileValue < _smileThreshold && _mouthValue > _neutralThreshold;
        _pucker = puckerValue >= _puckerThreshold;
        _slightPucker = puckerValue < _puckerThreshold && _mouthValue < _neutralThreshold;

        if (!_faceData.previouslyPucker && _pucker)
        {
            _faceData.SetData(false, false, false, true);
            FaceExpression();
        }

        else if (!_faceData.previouslySmiling && _smiling)
        {
            _faceData.SetData(true, false, false, false);
            FaceExpression();
        }

        else if (!_faceData.previouslySlightPucker && _slightPucker)
        {
            _faceData.SetData(false, false, true, false);
            FaceExpression();
        }

        else if (!_faceData.previouslySlightSmile && _slightSmile)
        {
            _faceData.SetData(false, true, false, false);
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
}
