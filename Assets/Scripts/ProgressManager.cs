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
    
    private int _progressTween;
    private float _previousElapsed;

    private void OnEnable()
    {
        FaceTrackingManager.FaceExpression += NewFaceExpressionAvailable;
        ProgressiveSequenceable.Completed += ResetProgress;
    }

    private void OnDisable()
    {
        FaceTrackingManager.FaceExpression -= NewFaceExpressionAvailable;
        ProgressiveSequenceable.Completed -= ResetProgress;
    }

    private void Start()
    {
        _puckerStopwatch = new Stopwatch();
    }

    private void Update()
    {
        if (_puckerStopwatch.ElapsedMilliseconds / 1000f > _startProgressAt && 
            _previousElapsed / 1000f < _startProgressAt) //we just passed the min duration threshold, continue progress
        {
            _progressTween = LeanTween.value(gameObject, _currentProgress, _currentProgress + 1, _endProgressAt - _startProgressAt)
                .setOnUpdate(val =>
                {
                    _currentProgress = val;
                    Progress(_currentProgress);
                })
                .setEaseInCirc()
                .id;
        }
        else if (_puckerStopwatch.ElapsedMilliseconds / 1000f > _endProgressAt &&
                 _previousElapsed / 1000f < _endProgressAt) //we just passed the max duration threshold, stop progress
        {
            LeanTween.pause(_progressTween);
        }  
        
        _previousElapsed = _puckerStopwatch.ElapsedMilliseconds;
    }

    private void NewFaceExpressionAvailable()
    {
        if (_faceExpression.slightSmile) //only reset stopwatch once we passed 0
        {
            Debug.Log("Slight smile");
            if (_faceExpression.previouslySlightPucker) _puckerStopwatch.Reset(); 
        }
        else if (_faceExpression.slightPucker) //pause the tween. We might resume it if we detect pucker again
        {
            Debug.Log("Slight pucker");
            if (Progressing()) LeanTween.pause(_progressTween); 
            _puckerStopwatch.Stop();
        }
        else if (_faceExpression.pucker) //if we resumed pucker while we were already doing progress
        {
            Debug.Log("pucker");
            if (Progressing()) LeanTween.resume(_progressTween);
            _puckerStopwatch.Start();
        }
    }

    private bool Progressing()
    {
        return _puckerStopwatch.ElapsedMilliseconds / 1000 > _startProgressAt &&
               _puckerStopwatch.ElapsedMilliseconds / 1000 < _endProgressAt;
    }

    private void ResetProgress(Sequenceable item)
    {
        LeanTween.pause(_progressTween); //TODO is it necessary to pause?
        _currentProgress = 0;
    }
}