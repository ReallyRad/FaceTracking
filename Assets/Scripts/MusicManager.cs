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
            if (_currentLoopIndex >= _seamlessLoops.Length)
            {
                Completed(this);
                _active = false;
            }
            else 
            {
                if (_currentLoopVolume >= _maxVolume) 
                {
                    _currentLoopIndex++;
                    _currentLoopVolume = 0f;
                }
                else
                {
                    float loopMinProgress = _currentLoopIndex * _completedProgressAt / _seamlessLoops.Length;
                    float loopMaxProgress = (_currentLoopIndex + 1) * _completedProgressAt / _seamlessLoops.Length;

                    _currentLoopVolume = Utils.Map(progress,
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

    //protected override void Progress(float progress) //should fade in a new clip every two full breath outs (8 seconds) 
    //{
    //    if (_active)
    //    {
    //        if (progress >= _finalValue)
    //        {
    //            Completed(this);
    //            _active = false;
    //        }
    //        else
    //        {
    //            //map one breath to half the volume increase of a track
    //            _currentLoopIndex = (int)Math.Truncate(progress) / 2; //switch loopindex every multiple of 2

    //            _seamlessLoops[_currentLoopIndex].SetVolume(Utils.Map(
    //                progress,
    //                2 * _currentLoopIndex,
    //                2 * _currentLoopIndex + 2,
    //                0,
    //                1));
    //        }
    //    }
    //}

}
