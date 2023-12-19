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

    private void Start()
    {
        groupNames[0] = "Piano";
        groupNames[1] = "Drone";
        groupNames[2] = "ThreeBells";
        groupNames[3] = "Beat";
        groupNames[4] = "Choir";
        groupNames[5] = "Tonic";
        groupNames[6] = "WarblySynth";
        groupNames[7] = "Bass";

        _audioMixer.SetFloat("Piano", 0.7f);
    }

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
                    
                    //_seamlessLoops[_currentLoopIndex].SetVolume(_currentLoopVolume);



                    _audioMixer.SetFloat(groupNames[_currentLoopIndex], Mathf.Log10(_currentLoopVolume));
                    float currentVolume;
                    _audioMixer.GetFloat(groupNames[_currentLoopIndex], out currentVolume);
                    Debug.Log(groupNames[_currentLoopIndex] + ":  " + currentVolume);
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
