using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private string _currentSceneName;

    public delegate void OnSceneFinished();
    public static OnSceneFinished SceneFinished;

    private void Start()
    {
        _currentSceneName = SceneManager.GetActiveScene().name;        

        StartCoroutine(StartEndSceneWithDelay());
        StartCoroutine(StartFogFadingInWithDelay());
    }

    private IEnumerator StartEndSceneWithDelay()
    {
        yield return new WaitForSeconds(_timeToEndScene);
        SceneFinished();
        SceneManager.LoadScene(_nextSceneName);
    }

    private IEnumerator StartFogFadingInWithDelay()
    {
        yield return new WaitForSeconds(_timeToFadeFogIn);
        if (_currentSceneName == Experience.Snow.ToString()) StartCoroutine(FogFadingInAtTheEnd());
    }
    
    private IEnumerator FogFadingInAtTheEnd() //TODO use tweens
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

}
