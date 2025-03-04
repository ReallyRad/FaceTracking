using UnityEngine;

public class ProgressiveMusic : ProgressiveSequenceable
{
    [SerializeField] private AudioSource _progressiveMusic;
    [SerializeField] private AnimationCurve _musicProgressionCurve;
    
    public override void Initialize()
    {
        _active = true;
        _localProgress = 0;
    }

    protected override void Progress(float progress)
    {
        if (_active)
        {
            _localProgress += progress;

            var wasTransitioning = _transitioning;
            _transitioning = _localProgress > _startNextPhaseAt;

            if (_localProgress >= _completedAt) //end of this sequence step
            {
                _active = false;
                _transitioning = false;
            }
            else
            {
                if (_transitioning && !wasTransitioning) StartNextPhase(this);

                var val = _musicProgressionCurve.Evaluate(_localProgress / _completedAt);
                _progressiveMusic.volume = Utils.Map(val, 0, 1, _initialValue, _finalValue);
            }
        }
    }


}
