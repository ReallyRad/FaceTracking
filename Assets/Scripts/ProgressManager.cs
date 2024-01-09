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
    }

    private void OnDisable()
    {
        FaceTrackingManager.PuckerTrigger -= PuckerTrigger;
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
                    var increment = val - _currentProgress;
                    _currentProgress = val;
                    Progress(increment); //send the difference with previous value so it can be used to increment e
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
}