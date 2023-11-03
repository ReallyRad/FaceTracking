using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;

public class ProgressManger : MonoBehaviour
{
    public delegate void OnProgress(float progress);
    public static OnProgress Progress;
    
    private Stopwatch _progressStopwatch;

    [FormerlySerializedAs("_startProgressat")] [SerializeField] private float _startProgressAt;
    [SerializeField] private float _endProgressAt;
    [SerializeField] private FaceData _faceExpression;

    private Stopwatch _puckerStopwatch;
    
    private void OnEnable()
    {
        FaceTrackingManager.FaceExpression += NewFaceExpressionAvailable;
    }

    private void OnDisable()
    {
        FaceTrackingManager.FaceExpression -= NewFaceExpressionAvailable;
    }
 
    private void Update()
    {
        if (_puckerStopwatch.ElapsedMilliseconds / 1000f > _startProgressAt && _puckerStopwatch.ElapsedMilliseconds / 1000f < _endProgressAt)
        { 
            _progressStopwatch.Start();
            Progress(_progressStopwatch.ElapsedMilliseconds);
        }
        else if (_puckerStopwatch.ElapsedMilliseconds / 1000f > _endProgressAt ||  _puckerStopwatch.ElapsedMilliseconds / 1000f < _startProgressAt) 
        { 
            _progressStopwatch.Stop();
        }
    }

    private void NewFaceExpressionAvailable()
    {
        if (_faceExpression.slightSmile) SlightSmile();
        else if (_faceExpression.slightPucker) SlightPucker();
        else if (_faceExpression.pucker) Pucker();
    }

    private void Pucker()
    {
        _puckerStopwatch.Start();
    }
        
    private void SlightPucker()
    {
        _puckerStopwatch.Stop();
        _progressStopwatch.Stop();
    }
        
    private void SlightSmile()
    {
        _progressStopwatch.Stop();
        if (_faceExpression.previouslySlightPucker) _puckerStopwatch.Reset(); //only reset stopwatch once we passed 0
    }
}
