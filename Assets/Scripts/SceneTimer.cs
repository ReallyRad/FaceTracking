using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using VolumetricFogAndMist2;

public class SceneTimer : MonoBehaviour //TODO remove and merge with the normal CSV writing methods
{
    [SerializeField] private string _nextSceneName; 
    
    [SerializeField] private float _timeToEndScene; 
    [SerializeField] private float _timeToFadeFogIn;
    [SerializeField] private float _fadeInDuration;

    [SerializeField] private VolumetricFog _fog; //TODO shouldn't be accessing fog from somewhere eles in hierarchy
    [SerializeField] private float initialDensity = 0;
    [SerializeField] private float finalDensity = 1;

    [SerializeField] private  CSVManager _csvManager; //TODO don't reference an object within the scene
    
    private string _currentSceneName;
    private List<float> _exhaleDurationsList = new List<float>();
    
    private void OnEnable()
    {
        ProgressManager.PuckerStopwatchReset += HandleStopwatchReset;
    }

    private void OnDisable()
    {
        ProgressManager.PuckerStopwatchReset -= HandleStopwatchReset;
    }

    private void Start()
    {
        _currentSceneName = SceneManager.GetActiveScene().name;        

        StartCoroutine(StartEndSceneWithDelay());
        StartCoroutine(StartFogFadingInWithDelay());
    }

    private IEnumerator StartEndSceneWithDelay()
    {
        yield return new WaitForSeconds(_timeToEndScene);

        if (_csvManager != null) 
        {
            //TODO use normal CSV writing method
            _csvManager._experimentDataStorage.experimentDataDictionary["exhaleCount"] = _exhaleDurationsList.Count.ToString();
            _csvManager._experimentDataStorage.experimentDataDictionary["exhaleSum"] = _exhaleDurationsList.Sum().ToString();
            _csvManager._experimentDataStorage.experimentDataDictionary["exhaleEffective"] = _exhaleDurationsList.Select(value => value - 2000)
                                                                                                               .Where(result => result > 0)
                                                                                                               .ToString();
            _csvManager.NewDataAvailableForDictionary();
        }

        SceneManager.LoadScene(_nextSceneName);
    }

    private IEnumerator StartFogFadingInWithDelay()
    {
        yield return new WaitForSeconds(_timeToFadeFogIn);
        if (_currentSceneName == "PsychedelicGarden") //TODO switch to snow
        {
            StartCoroutine(FogFadingInAtTheEnd());
        }
    }
    private IEnumerator FogFadingInAtTheEnd()
    {
        _fog.settings.density = initialDensity;
        float startTime = Time.time;

        while (Time.time < startTime + _fadeInDuration)
        {
            _fog.settings.density = Mathf.Lerp(initialDensity, finalDensity, (Time.time - startTime) / _fadeInDuration);
            yield return null;
        }

        _fog.settings.density = finalDensity;
    }

    private void HandleStopwatchReset(float progressInMilliseconds)
    {
        _exhaleDurationsList.Add(progressInMilliseconds);
    }
}
