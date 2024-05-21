using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Oculus.Interaction;
using VolumetricFogAndMist2;
using UnityEngine.SceneManagement;
using System.Data;
using UnityEngine.Video;

public class WaitingRoomTransition : MonoBehaviour
{
    public VolumetricFog fog;
    public VideoPlayer videoPlayter;
    public AnimationCurve curve;
    public float minDensityVolume;
    public float maxDensityVolume;
    public AudioSource fogBackgroundNoise;

    private float timeLeft;
    private bool timerRunning = false;
    private float waitingDuration = 10;

    public delegate void OnNotifyPrePostState(ExperimentState prePost);
    public static OnNotifyPrePostState NotifyPrePostState;
    
    void Start()
    {
        fog.settings.density = minDensityVolume;
        fogBackgroundNoise.volume = minDensityVolume;
    }
    
    public void StartTimer(float duration)
    {
        timeLeft = duration;
        timerRunning = true;
    }

    void Update()
    {
        if (timerRunning)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = 0;
                timerRunning = false;
                SceneManager.LoadScene("SceneGardenGrowth");
            }
            
            videoPlayter.Stop();
            var normalVal = curve.Evaluate((waitingDuration - timeLeft) / waitingDuration);
            var realVal = Utils.Map(normalVal, 0, 1, minDensityVolume, maxDensityVolume);
            fog.settings.density = realVal;
            fogBackgroundNoise.volume = realVal;
        }
    }

    public void OnSlideshowFinished()
    {
        StartTimer(waitingDuration);
    }

}
