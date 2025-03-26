using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoPanel : MonoBehaviour 
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private string _videoFileName;
    [SerializeField] private Button _nextButton;

    private void Start()
    {
        string videoPath = System.IO.Path.Combine(Application.persistentDataPath, _videoFileName);
        if (System.IO.File.Exists(videoPath)) _videoPlayer.url = videoPath;
        _videoPlayer.loopPointReached += source => GetComponent<PanelDimmer>().Show(false);
    }

    public void PlayVideo()
    {
        _videoPlayer.Play();
    }

}
