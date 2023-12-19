using System;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : ProgressiveSequenceable
{
    private float _maxVolume = 1f;
    private float _currentLoopVolume = 0f;
    
    [SerializeField] private SeamlessLoop[] _seamlessLoops;
    [SerializeField] private int _currentLoopIndex;
    [SerializeField] private AudioMixer _audioMixer;

    private AudioMixerGroup ttt;

    private string[] groupNames = new string[8];
    
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
                                                   0.0001f,
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
