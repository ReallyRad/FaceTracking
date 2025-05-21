using System;
using System.Collections;
using UnityEngine;
using System.IO;

public class ProgressManager : MonoBehaviour //handles face expression events to detect whether they should be interpreted as progress
{
    public delegate void OnProgress(float progress);
    public static OnProgress Progress;
    
    public delegate void OnNewBreathOutFinished(float exhaleDuration); 
    public static OnNewBreathOutFinished ExhaleEnded; //TODO is this still needed?
    
    [SerializeField] private float _startProgressAt;
    [SerializeField] private float _endProgressAt;
    [SerializeField] private float _currentProgress;
    [SerializeField] private AnimationCurve _progressCurve;

    private int _progressTween;
    private float _progressDuration;

    private Coroutine PuckerEnd;
    private Coroutine PuckerStart;
    
    private void OnEnable()
    {
        FaceTrackingManager.PuckerTrigger += PuckerTrigger;
        SnowSlowing.SnowSlowingInitialized += SetStartProgressAtToZero; //TODO this should be set back to whatever it was when finished?
    }

    private void OnDisable()
    {
        FaceTrackingManager.PuckerTrigger -= PuckerTrigger;
        SnowSlowing.SnowSlowingInitialized -= SetStartProgressAtToZero;
    }

    private void Start()
    {
        _progressDuration = _endProgressAt - _startProgressAt;
    }

    private IEnumerator PuckerThresholdStart()
    {
        yield return new WaitForSeconds(_startProgressAt);
        Debug.Log("start progress");
        _progressTween = LeanTween.value(gameObject,
                _currentProgress,
                _currentProgress + 1,
                _progressDuration)
            .setOnUpdate(val =>
            {
                var increment = val - _currentProgress;
                _currentProgress = val;
                Progress(increment); //send the difference with previous value so it can be used to increment e
            })
            .setEase(_progressCurve)
            .id;
    }

    private IEnumerator PuckerThresholdEnd()
    {
        yield return new WaitForSeconds(_endProgressAt);
        Debug.Log("end progress");
        if (_progressTween != 0) LeanTween.pause(_progressTween);
    }

    private void PuckerTrigger(bool pucker)
    {
        if (pucker) {
            PuckerStart = StartCoroutine(PuckerThresholdStart());
            PuckerEnd = StartCoroutine(PuckerThresholdEnd());
            Debug.Log("start stopwatch");
        }
        else
        {
            StopCoroutine(PuckerStart);
            StopCoroutine(PuckerEnd);
            Debug.Log("stop stopwatch");
            if (_progressTween != 0)
            {
                LeanTween.pause(_progressTween);
                Debug.Log("pause tween");
            }
        }
    }

    private void SetStartProgressAtToZero()
    {
        _startProgressAt = 0; //This is to make the snow slowing visible faster?
    }
}