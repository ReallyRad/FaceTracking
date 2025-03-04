using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoControllerControl : MonoBehaviour //TODO cleanup
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private string _videoFileName;
    [SerializeField] private Button _nextButton;

    private void Start()
    {
        string videoPath = System.IO.Path.Combine(Application.persistentDataPath, _videoFileName);
  
        if (System.IO.File.Exists(videoPath)) _videoPlayer.url = videoPath;

        Debug.Log("videoPath: " + videoPath);

        PlayVideo();
    }

    public void PlayVideo()
    {
        _videoPlayer.Play();
    }

    private void EnableNextButton(VideoPlayer vp)
    {
        _nextButton.interactable = true;
    }
}
