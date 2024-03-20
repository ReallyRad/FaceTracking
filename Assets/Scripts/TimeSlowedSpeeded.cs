using Oculus.Interaction;
using RenderHeads.Media.AVProVideo;
using SCPE;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class TimeSlowedSpeeded : InteractiveSequenceable
{
    public MediaPlayer mediaPlayer;
    //public VideoPlayer mediaPlayer;
    public float playSpeed;


    public override void Initialize()
    {
        _active = true;
        _localProgress = 0;
        ChnagePlaySpeed(_initialValue);
    }

    protected override void Interact()
    {
        ChnagePlaySpeed(_finalValue);
    }

    protected override void Decay()
    {
        ChnagePlaySpeed(_initialValue);
    }

    protected override void Progress(float progress)
    {

    }
    private void ChnagePlaySpeed(float speed) 
    {
        mediaPlayer.Control.SetPlaybackRate(speed);
        playSpeed = mediaPlayer.PlaybackRate;
    }
}
