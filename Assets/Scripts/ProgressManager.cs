using System.Diagnostics;
using UnityEngine;

 public class ProgressManager : MonoBehaviour //handles face expression events to detect whether they should be interpreted as progress
{
    public delegate void OnProgress(float progress);
    public static OnProgress Progress;
    
    [SerializeField] private float _startProgressAt;
    [SerializeField] private float _endProgressAt;
    [SerializeField] private float _currentProgress;
    [SerializeField] private AnimationCurve _progressCurve;

    private Stopwatch _puckerStopwatch;
    
    private int _progressTween;
    private float _previousElapsed;
    private float _progressDuration;


    private void OnEnable()
    {
        FaceTrackingManager.PuckerTrigger += PuckerTrigger;
        Sequenceable.Completed += ResetProgress;
    }

    private void OnDisable()
    {
        FaceTrackingManager.PuckerTrigger -= PuckerTrigger;
        Sequenceable.Completed -= ResetProgress;
    }

    private void Start()
    {
        _puckerStopwatch = new Stopwatch();
        _progressDuration = _endProgressAt - _startProgressAt;
    }

    private void Update()
    {
        if (_puckerStopwatch.ElapsedMilliseconds / 1000f > _startProgressAt && 
            _previousElapsed / 1000f < _startProgressAt) //we just passed the min duration threshold, continue progress
        {

            _progressTween = LeanTween.value(gameObject,
                                            _currentProgress,
                                            _currentProgress + 1,
                                            _progressDuration)
                .setOnUpdate(val =>
                {
                    _currentProgress = val;
                    Progress(_currentProgress);
                })
                .setEase(_progressCurve)
                .id;
        }
        else if (_puckerStopwatch.ElapsedMilliseconds / 1000f > _endProgressAt &&
                 _previousElapsed / 1000f < _endProgressAt) //we just passed the max duration threshold, stop progress
        {
            if (_progressTween != 0) LeanTween.pause(_progressTween);
        }  
        
        _previousElapsed = _puckerStopwatch.ElapsedMilliseconds;
    }

    private void PuckerTrigger(bool pucker)
    {
        if (pucker)
        {
            _puckerStopwatch.Start();
        }
        else
        {
            _puckerStopwatch.Stop();
            if (_progressTween != 0) LeanTween.pause(_progressTween);
            _puckerStopwatch.Reset(); //only reset stopwatch once we passed 0
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