using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class InstructionVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button playButton;
    public Button stopButton;

    void Start()
    {
        videoPlayer.Stop();
        playButton.onClick.AddListener(PlayVideo);
        stopButton.onClick.AddListener(PauseVideo);
    }

    public void PlayVideo()
    {
        videoPlayer.Play();
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
    }
}
