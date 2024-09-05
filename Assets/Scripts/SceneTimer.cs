using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using VolumetricFogAndMist2;

public class SceneTimer : MonoBehaviour
{
    private string currentSceneName;
    public string nextSceneName; 
    
    public float timeToEndScene;
    public float timeToFadeFogIn;
    public float fadeInDuration;

    public VolumetricFog fog;
    public float initialDensity = 0;
    public float finalDensity = 1;

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
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator StartFogFadingInWithDelay()
    {
        yield return new WaitForSeconds(timeToFadeFogIn);
        if (currentSceneName == "PsychedelicGarden")
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
}
