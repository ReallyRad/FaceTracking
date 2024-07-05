using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;
using UnityEngine.SceneManagement;
using ScriptableObjectArchitecture;

public class WaitingRoomTransition : MonoBehaviour
{
    public VolumetricFog fog;
    public AnimationCurve curve;
    public float minDensityVolume;
    public float maxDensityVolume;

    private float timeLeft;
    private bool timerRunning = false;
    private float waitingDuration = 10;

    [SerializeField] private IntVariable _selectedExperience;
    [SerializeField] private ExperimentStateSO experimentStateSO;
    
    void Start()
    {
        fog.settings.density = minDensityVolume;
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
                SceneManager.LoadScene(((Experience) _selectedExperience.Value).ToString());
            }
            
            var normalVal = curve.Evaluate((waitingDuration - timeLeft) / waitingDuration);
            var realVal = Utils.Map(normalVal, 0, 1, minDensityVolume, maxDensityVolume);
            fog.settings.density = realVal;
        }
    }

    public void OnSlideshowFinished()
    {
        StartTimer(waitingDuration); //TODO use coroutine instead of Update method
        experimentStateSO.experimentState = ExperimentState.post;
    }

}
