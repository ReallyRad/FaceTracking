using System;
using UnityEngine;

public class MusicManager : ProgressiveSequenceable
{
    private float _maxVolume = 1f;
    private float _currentLoopVolume = 0f;
    
    [SerializeField] private SeamlessLoop[] _seamlessLoops;
    [SerializeField] private int _currentLoopIndex;

    // this is based on these assumptions: _initialValue and _finalValue are
    // the lowest and highest volume of each seamlessloop ... progress changes
    // between 0 and _completedProgressAt ... the whole range of progress is divided
    // to 8 (_seamlessLoops.Length) parts, and each part is mapped to the range of
    // volume change of one of the seamlessLoops. 

    protected override void Progress(float progress) 
    {
        if (_active)
        {
            _localProgress += progress;
            
            var wasTransitioning = _transitioning;
            _transitioning = _localProgress > _startNextPhaseAt;

            if (_currentLoopIndex >= _completedAt) //end of this sequence step 
            {
                _active = false;
                _transitioning = false;
            }
            else 
            {
                if (_transitioning && !wasTransitioning) StartNextPhase(this); //notify progressmanager to starting next phase

                if (_currentLoopVolume >= _maxVolume) 
                {
                    _currentLoopIndex++;
                    _currentLoopVolume = 0f;
                }
                else
                {
                    float loopMinProgress = _currentLoopIndex * _completedAt / _seamlessLoops.Length;
                    float loopMaxProgress = (_currentLoopIndex + 1) * _completedAt / _seamlessLoops.Length;

                    _currentLoopVolume = Utils.Map(_localProgress,
                                                   loopMinProgress,
                                                   loopMaxProgress,
                                                   0f,
                                                   1f);

                    _seamlessLoops[_currentLoopIndex].SetVolume(_currentLoopVolume);
                }
            }
        }
    }

    public override void Initialize() 
    {
        _active = true;
        _currentLoopIndex = 0;
        foreach (SeamlessLoop seamlessLoop in _seamlessLoops) seamlessLoop.SetVolume(0);
    }

}
