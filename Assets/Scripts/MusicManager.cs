using System;
using UnityEngine;

public class MusicManager : Sequenceable
{
    private float _maxVolume = 1f;
    
    [SerializeField] private SeamlessLoop[] _seamlessLoops;
    [SerializeField] private int _currentLoopIndex;

    protected override void Progress(float progress) //should fade in a new clip every two full breath outs (8 seconds) 
    {
        if (_active)
        {
            if (progress >= _finalValue)
            {
                Completed(this);
                _active = false;
            }
            else 
            {
                //map one breath to half the volume increase of a track
                _currentLoopIndex = (int) Math.Truncate(progress) / 2; //switch loopindex every multiple of 2
            
                _seamlessLoops[_currentLoopIndex].SetVolume(Utils.Map(
                    progress,
                    2 * _currentLoopIndex,
                    2 * _currentLoopIndex + 2,
                    0,
                    1 ));
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
