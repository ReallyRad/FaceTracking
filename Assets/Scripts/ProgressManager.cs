using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ProgressManager : MonoBehaviour //handles face expression events to detect whether they should be interpreted as progress
{
    public delegate void OnProgress(float progress);
    public static OnProgress Progress;
    
    [SerializeField] private float _startProgressAt;
    [SerializeField] private float _endProgressAt;
    [SerializeField] private FaceData _faceExpression;
    [SerializeField] private float _currentProgress;

    private Stopwatch _puckerStopwatch;
    
    private bool _progressing;
    private bool _wasProgressing;

    private int _progressTween;
    
    private void OnEnable()
    {
        FaceTrackingManager.FaceExpression += NewFaceExpressionAvailable;
    }

    private void OnDisable()
    {
        FaceTrackingManager.FaceExpression -= NewFaceExpressionAvailable;
    }

    private void Start()
    {
        _puckerStopwatch = new Stopwatch();
    }

    private void Update()
    {
        _wasProgressing = _progressing;

        _progressing = _puckerStopwatch.IsRunning && //stopwatch is running
                       _puckerStopwatch.ElapsedMilliseconds / 1000f > _startProgressAt && //for more than 3 sec
                       _puckerStopwatch.ElapsedMilliseconds / 1000f < _endProgressAt; // and less than 7
                      
        if (!_wasProgressing && _progressing) //if we just started progressing
        {
            //start progress tween
            _progressTween = LeanTween.value(gameObject, _currentProgress, _currentProgress + 1, 4)
                .setOnUpdate(val =>
                {
                    Debug.Log("progress : " + val);
                    _currentProgress = val;
                    Progress(_currentProgress);
                })
                .setEaseInCirc()
                .id;
        }
        else if (_wasProgressing && !_progressing) //if we just stopped progressing
        {
            Debug.Log("progress stopped : ");
            LeanTween.pause(_progressTween);
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
    }
        
    private void SlightSmile()
    {
        if (_faceExpression.previouslySlightPucker) _puckerStopwatch.Reset(); //only reset stopwatch once we passed 0
    }
}