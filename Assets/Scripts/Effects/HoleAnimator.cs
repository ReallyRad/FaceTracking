using UnityEngine;

public class HoleAnimator : ProgressiveSequenceable
{
    public float holeRadius;
    public Color surfaceColor; 
    private Material material;

    [SerializeField] private AudioSource _progressiveMusic;
    [SerializeField] private AnimationCurve _musicProgressCurve;


    void Start()
    {
        material = GetComponent<Renderer>().material;
        material.SetColor("_SurfaceColor", surfaceColor);
        _progressiveMusic.volume = 0f;
    }
    
    public override void Initialize()
    {
        _active = true;
        _localProgress = 0;
        material.SetFloat("_HoleRadius", _initialValue);
    }
    
    protected override void Progress(float progress)
    {
        if (_active)
        {
            _localProgress += progress;

            _transitioning = _localProgress > _startNextPhaseAt;

            if (_localProgress >= _completedAt) 
            {
                _active = false;
                _transitioning = false;
                gameObject.GetComponent<MeshRenderer>().enabled = false;
                StartNextPhase(this); 
            }
            else
            {
                var val = _localProgress / _completedAt;
                holeRadius = Utils.Map(val, 0, 1, _initialValue, _finalValue);
                material.SetFloat("_HoleRadius", holeRadius);
            }
        }
    }
}
