using Oculus.Interaction;
using SCPE;
using UnityEngine;

public class HoleAnimator : InteractiveSequenceable //TODO should be progressive
{
    //public float maxHoleRadius;
    //public float duration;
    public float holeRadius;

    //public Texture2D surfaceTexture; 
    public Color surfaceColor; 

    private Material material;
    //private float timer = 0.0f;

    [SerializeField] private AudioSource _progressiveMusic;
    [SerializeField] private AnimationCurve _musicProgressCurve;


    void Start()
    {
        material = GetComponent<Renderer>().material;
        //material.SetTexture("_MainTex", surfaceTexture);
        material.SetColor("_SurfaceColor", surfaceColor);
        //material.SetFloat("_HoleRadius", _initialValue);
        _progressiveMusic.volume = 0f;
    }

    
    public override void Initialize()
    {
        _active = true;
        _localProgress = 0;
        material.SetFloat("_HoleRadius", _initialValue);
    }

    protected override void Interact()
    {   }

    protected override void Decay()
    {   }
    
    protected override void Progress(float progress)
    {
        if (_active)
        {
            _localProgress += progress;

            var wasTransitioning = _transitioning;
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

                //var musicVal = _musicProgressCurve.Evaluate(val);
                //_progressiveMusic.volume = Utils.Map(musicVal, 0, 1, _initialValue, _finalValue);
                
            }
        }
    }
}
