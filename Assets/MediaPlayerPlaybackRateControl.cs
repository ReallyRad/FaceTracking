using System.Linq;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

public class MediaPlayerPlaybackRateControl : MonoBehaviour
{
    [SerializeField] private MediaPlayer _mediaPlayer;
    [SerializeField] private float _playSpeed; //for displaying the value in the inspector

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
        _mediaPlayer.Control.SetPlaybackRate(GetClosestSnapValue(rate));
        _playSpeed = rate;
    }
    
    private float GetClosestSnapValue(float value) // Method to find the closest snap value
    {
        float[] snapValues = { 0, 0.25f, 0.5f, 1.0f, 1.25f, 1.5f, 1.75f, 2f};
        return snapValues.OrderBy(x => Mathf.Abs(x - value)).First(); // Find the closest value from the snapValues array
    }
}
