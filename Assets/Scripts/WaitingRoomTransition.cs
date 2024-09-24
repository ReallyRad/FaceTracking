using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;
using UnityEngine.SceneManagement;
using ScriptableObjectArchitecture;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class WaitingRoomTransition : MonoBehaviour
{
    public VolumetricFog fog;
    public AnimationCurve curve;
    public float minDensityVolume;
    public float maxDensityVolume;

    private float timeLeft;
    private bool timerRunning = false;
    //private float waitingDuration;

    [SerializeField] private IntVariable _selectedExperience;
    [SerializeField] private ExperimentStateSO experimentStateSO;
    [SerializeField] private GameObject vibrationManager;
    void Start()
    {
        fog.settings.density = minDensityVolume;

        //if (_selectedExperience == (int)Experience.PsychedelicGarden) waitingDuration = 15;
        //else waitingDuration = 1;
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

                Destroy(vibrationManager);
                SceneManager.LoadScene(((Experience) _selectedExperience.Value).ToString());

                //Debug.Log("YYYYYYY" + ((Experience)_selectedExperience.Value).ToString());
                //StartCoroutine(LoadNewScene(((Experience)_selectedExperience.Value).ToString()));
            }
           
            var normalVal = curve.Evaluate((ScenarioToggleGroup.waitingDuration - timeLeft) / ScenarioToggleGroup.waitingDuration);
            var realVal = Utils.Map(normalVal, 0, 1, minDensityVolume, maxDensityVolume);
            if (_selectedExperience == (int) Experience.PsychedelicGarden) 
                fog.settings.density = realVal;
        }
    }

    public void OnSlideshowFinished()
    {
        if (experimentStateSO.experimentState == ExperimentState.pre)
        {
            StartTimer(ScenarioToggleGroup.waitingDuration);
            experimentStateSO.experimentState = ExperimentState.post;
        }
        else if (experimentStateSO.experimentState == ExperimentState.post) 
        {
            Application.Quit();
        }
    }

    private IEnumerator LoadNewScene(string newSceneName)
    {
        // Load the new scene additively (it won't unload the current scene).
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        // Wait until the scene is fully loaded (progress >= 0.9 indicates nearly done).
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Activate the new scene when it's ready.
        asyncLoad.allowSceneActivation = true;

        // Wait for the new scene to activate.
        yield return new WaitUntil(() => asyncLoad.isDone);

        Debug.Log("AFTER ASYNC");

        // Set the newly loaded scene as active.
        //Scene newScene = SceneManager.GetSceneByName(newSceneName);
        //SceneManager.SetActiveScene(newScene);

        // Now unload the current (old) scene.
        //Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync("Waiting");
    }
}
