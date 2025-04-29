using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlaybackRateControl : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    
    private void OnEnable()
    {
        TimeSpeeding.SpeedTime += SetRate;
    }
    
    private void OnDisable()
    {
        TimeSpeeding.SpeedTime -= SetRate;
    }

    private void SetRate(float rate)
    {
        _audioSource.pitch = rate * 2 + 0.5f;
    }
}
