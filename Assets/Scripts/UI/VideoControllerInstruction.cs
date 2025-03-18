using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoControllerInstruction : MonoBehaviour //TODO cleanup
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private string _videoFileName;
    [SerializeField] private Button _nextButton;

    public delegate void OnVideoInstructionsShown();
    public static OnVideoInstructionsShown VideoInstructionsShown;
    
    private void OnEnable()
    {
        _videoPlayer.loopPointReached += EnableNextButton;
    }

    private void Start()
    {
        string videoPath = System.IO.Path.Combine(Application.persistentDataPath, _videoFileName);
        if (System.IO.File.Exists(videoPath)) _videoPlayer.url = videoPath;
    }

    
    public void PlayVideo()
    {
        VideoInstructionsShown();
        _videoPlayer.Play();
    }

    private void EnableNextButton(VideoPlayer vp)
    {
        _nextButton.interactable = true;
    }
}
