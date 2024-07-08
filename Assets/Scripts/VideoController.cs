using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections.Specialized;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button playButton;
    public Button pauseButton;
    private string videoPath;
    public string videoFileName;



    void Start()
    {
        //#if UNITY_STANDALONE || UNITY_EDITOR
        //        videoPath = System.IO.Path.Combine(Application.dataPath, "StreamingAssets", "yourVideoFile.mp4");
        //#elif UNITY_ANDROID
        //        videoPath = System.IO.Path.Combine(Application.persistentDataPath, "yourVideoFile.mp4");
        //#endif

        videoPath = System.IO.Path.Combine(Application.persistentDataPath, videoFileName);
        if (System.IO.File.Exists(videoPath))
        {
            videoPlayer.url = videoPath;
        }

        videoPlayer.Stop();
        playButton.onClick.AddListener(PlayVideo);
        pauseButton.onClick.AddListener(PauseVideo);
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
