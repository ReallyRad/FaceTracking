using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using VolumetricFogAndMist2;

public class SceneTimer : MonoBehaviour //TODO remove and merge with the normal CSV writing methods
{
    private string currentSceneName;
    public string nextSceneName; 
    
    public float timeToEndScene;
    public float timeToFadeFogIn;
    public float fadeInDuration;

    public VolumetricFog fog;
    public float initialDensity = 0;
    public float finalDensity = 1;

    private List<float> exhaleDurationsList = new List<float>();
    public CSVManager csvManager;

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;        

        StartCoroutine(StartEndSceneWithDelay());
        StartCoroutine(StartFogFadingInWithDelay());
    }

    IEnumerator StartEndSceneWithDelay()
    {
        yield return new WaitForSeconds(timeToEndScene);

        if (csvManager != null) 
        {
            //TODO use normal CSV writing method
            csvManager._experimentDataStorage.experimentDataDictionary["exhaleCount"] = exhaleDurationsList.Count.ToString();
            csvManager._experimentDataStorage.experimentDataDictionary["exhaleSum"] = exhaleDurationsList.Sum().ToString();
            csvManager._experimentDataStorage.experimentDataDictionary["exhaleEffective"] = exhaleDurationsList.Select(value => value - 2000)
                                                                                                               .Where(result => result > 0)
                                                                                                               .ToString();
            csvManager.NewDataAvailableForDictionary();
        }

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator StartFogFadingInWithDelay()
    {
        yield return new WaitForSeconds(timeToFadeFogIn);
        if (currentSceneName == "PsychedelicGarden") //TODO switch to snow
        {
            StartCoroutine(FogFadingInAtTheEnd());
        }
    }
    IEnumerator FogFadingInAtTheEnd()
    {
        fog.settings.density = initialDensity;
        float startTime = Time.time;

        while (Time.time < startTime + fadeInDuration)
        {
            fog.settings.density = Mathf.Lerp(initialDensity, finalDensity, (Time.time - startTime) / fadeInDuration);
            yield return null;
        }

        fog.settings.density = finalDensity;
    }

    void OnEnable()
    {
        ProgressManager.PuckerStopwatchReset += HandleStopwatchReset;
    }

    void OnDisable()
    {
        ProgressManager.PuckerStopwatchReset -= HandleStopwatchReset;
    }

    private void HandleStopwatchReset(float progressInMilliseconds)
    {
        exhaleDurationsList.Add(progressInMilliseconds);
    }
}
