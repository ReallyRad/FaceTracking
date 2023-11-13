using System;
using UnityEditor.Search;
using UnityEngine;

public class MusicManager : Sequenceable
{
    private float _maxVolume = 1f;
    
    [SerializeField] private SeamlessLoop[] _seamlessLoops;
    [SerializeField] private int _currentLoopIndex;

    private float _previousProgress;
    
    protected override void Progress(float progress) //should fade in a new clip every two full breath outs (8 seconds) 
    {
        if (active)
        {
            //map one breath to half the volume increase of a track
            _currentLoopIndex = (int) Math.Truncate(progress) / 2; //switch loopindex every multiple of 2
            
            _seamlessLoops[_currentLoopIndex].SetVolume(Utils.Map(
                progress,
                2 * _currentLoopIndex,
                2 * _currentLoopIndex + 2,
                0,
                1 ));

                Debug.Log("mapping " + progress + " from [" +
                          2 * _currentLoopIndex +   
                          ", " +
                          2 * _currentLoopIndex +
                          2 +
                          "] " + 
                          "to [0, 1]");    
            
            if (progress >= _maxProgress) Completed(this);
        }
    }

    public override void Initialize()
    {
        active = true;
        _currentLoopIndex = 0;
        foreach (SeamlessLoop seamlessLoop in _seamlessLoops) seamlessLoop.SetVolume(0);
    }

}
