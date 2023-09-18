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
    [SerializeField] private float _smileThreshold;

    [SerializeField] private bool _smiling;
    [SerializeField] private bool _slightSmile;
    [SerializeField] private bool _pucker;
    [SerializeField] private bool _slightPucker;

    [SerializeField] private float _smileTimingThreshold;
    [SerializeField] private float _puckerTimingThreshold;

    [SerializeField] private bool _manualSmileControl;
    
    [SerializeField] private bool _progressOnBreatheIn;
    [SerializeField] private bool _progressOnBreatheOut;
    
    private Stopwatch _smileStopwatch;
    private Stopwatch _puckerStopwatch;
    private Stopwatch _progressStopwatch;

    public delegate void OnLevelUp();
    public static OnLevelUp LevelUp;
    
    public delegate void OnMouthValue(float mouthValue);
    public static OnMouthValue MouthValue;
    
    public delegate void OnProgressValue(float progressValue);
    public static OnProgressValue ProgressValue;

    public delegate void OnPuckerTime(float puckerTime);
    public static OnPuckerTime PuckerTime;
    
    public delegate void OnSmileTime(float puckerTime);
    public static OnSmileTime SmileTime;

    public delegate void OnBLockValue(float progressValue);
    public static OnBLockValue BlockValue;
    
    public delegate void OnSkyboxExposure(float progressValue);
    public static OnSkyboxExposure SkyboxExposure;

    private int _previousProgressValue;
    private int _progressValue;
    
    private void Start()
    {
        _smileStopwatch = new Stopwatch();
        _puckerStopwatch = new Stopwatch();
        _progressStopwatch = new Stopwatch();
    }

    private void Update()
    {
        Vector2 lipPucker = new Vector2();
        Vector2 lipCornerPuller = new Vector2();

        lipPucker = GetExpressionValue(OVRFaceExpressions.FaceExpression.LipPuckerL, OVRFaceExpressions.FaceExpression.LipPuckerR);
        lipCornerPuller = GetExpressionValue(OVRFaceExpressions.FaceExpression.LipCornerPullerL, OVRFaceExpressions.FaceExpression.LipCornerPullerR);

        float smileValue = 0f;
        float puckerValue = 0f;
        
        if (!_manualSmileControl)
        {
            smileValue = (lipCornerPuller.x + lipCornerPuller.y) / 2;
            puckerValue = (lipPucker.x + lipPucker.y) / 2;
            _mouthValue = smileValue - puckerValue;
        }
        else
        {
            if (_mouthValue < 0)
            {
                puckerValue = - _mouthValue;
                smileValue = 0;
            }
            else if (_mouthValue > 0)
            {
                smileValue = _mouthValue;
                puckerValue = 0;
            }
            MouthValue(_mouthValue);
        }

        bool wasSlightSmile = _slightSmile;
        bool wasSlightPucker = _slightPucker;
        
        _smiling = smileValue >= _smileThreshold;
        _slightSmile = smileValue < _smileThreshold && _mouthValue > 0;
        _pucker = puckerValue >= _puckerThreshold;
        _slightPucker = puckerValue < _puckerThreshold && _mouthValue < 0;

        PuckerTime(_puckerStopwatch.ElapsedMilliseconds / 1000f);
        SmileTime(_smileStopwatch.ElapsedMilliseconds / 1000f);

        if (_slightSmile) { 
            _smileStopwatch.Stop();
            _progressStopwatch.Stop();
            if (wasSlightPucker) _puckerStopwatch.Reset();
        } 
        else if (_slightPucker) {
            _puckerStopwatch.Stop();
            _progressStopwatch.Stop();
            if (wasSlightSmile) _smileStopwatch.Reset();
        }
        else if (_smiling)
            _smileStopwatch.Start();
        else if (_pucker) 
            _puckerStopwatch.Start();
        
        if ((_puckerStopwatch.ElapsedMilliseconds > _smileTimingThreshold ||
             _smileStopwatch.ElapsedMilliseconds > _puckerTimingThreshold) &&
            !_slightPucker &&
            !_slightSmile)
        {
            if (_progressOnBreatheIn && _smiling || _progressOnBreatheOut && _pucker)
                _progressStopwatch.Start();
        }
       
        _previousProgressValue = _progressValue;
        _progressValue = (int) _progressStopwatch.ElapsedMilliseconds/1000;
        ProgressValue(_progressValue);
        
        if (_progressValue > _previousProgressValue)
            LevelUp();

        BlockValue(_progressStopwatch.ElapsedMilliseconds);
        SkyboxExposure(_progressStopwatch.ElapsedMilliseconds);
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
