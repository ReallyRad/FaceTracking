using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Oculus.Interaction;
using VolumetricFogAndMist2;
using UnityEngine.SceneManagement;
using System.Data;

public class WaitingRoomTransition : MonoBehaviour
{
    public VolumetricFog fog;

    private float timeLeft;
    private bool timerRunning = false;
    private float waitingDuration = 10;

    public delegate void OnNotifyPrePostState(ExperimentState prePost);
    public static OnNotifyPrePostState NotifyPrePostState;
    
    void Start()
    {
       
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
                SceneManager.LoadScene("SceneSnow");
            }
            fog.settings.density = (waitingDuration - timeLeft) / waitingDuration;
        }
    }

    public void OnSlideshowFinished()
    {
        StartTimer(waitingDuration);
    }

}
