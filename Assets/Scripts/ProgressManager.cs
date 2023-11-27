using System.Diagnostics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Debug = UnityEngine.Debug;

 public class ProgressManager : MonoBehaviour //handles face expression events to detect whether they should be interpreted as progress
{
    public delegate void OnProgress(float progress);
    public static OnProgress Progress;
    
    [SerializeField] private float _startProgressAt;
    [SerializeField] private float _endProgressAt;
    [SerializeField] private float _currentProgress;

    private Stopwatch _puckerStopwatch;
    
    private int _progressTween;
    private float _previousElapsed;

    public GameObject Fluid;
    public Material FluidMaterial;
    public Color lowerThanMinColor = Color.yellow;
    public Color betweenMinAndMaxColor = Color.green;
    public Color higherThanMaxColor = Color.red;

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
    }

    private void Update()
    {
        float puckerProgress = _puckerStopwatch.ElapsedMilliseconds / 1000f;
        float fraction = puckerProgress / _endProgressAt;
        SetYScale(Fluid, fraction);
        if (puckerProgress < _startProgressAt) FluidMaterial.color = lowerThanMinColor;
        else if (puckerProgress > _startProgressAt && puckerProgress<_endProgressAt) FluidMaterial.color = betweenMinAndMaxColor;
        else if (puckerProgress >_endProgressAt) FluidMaterial.color = higherThanMaxColor;


        if (_puckerStopwatch.ElapsedMilliseconds / 1000f > _startProgressAt && 
            _previousElapsed / 1000f < _startProgressAt) //we just passed the min duration threshold, continue progress
        {
            _progressTween = LeanTween.value(gameObject, _currentProgress, _currentProgress + 1, 4)
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

    void SetYScale(GameObject fluid, float yScale)
    {
        Vector3 newScale = fluid.transform.localScale;
        newScale.y = yScale;
        fluid.transform.localScale = newScale;
    }
}