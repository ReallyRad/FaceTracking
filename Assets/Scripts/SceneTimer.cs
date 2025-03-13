using System.Collections;
using ScriptableObjectArchitecture;
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

    private string _currentSceneName;

    [SerializeField] private ExperimentStateSO experimentStateSO;
    [SerializeField] private StringVariable _subjectIDVariable;
    [SerializeField] private ExperimentDataStorage _experimentDataStorage; //reference it here just to make sure it's persisted when switching scenes
    [SerializeField] private IntVariable _selectedExperience; //reference it here just to make sure it's persisted when switching scenes

    
    public delegate void OnSceneFinished();
    public static OnSceneFinished SceneFinished;

    //TODO fade fog in for 20 seconds so that next scene is loaded after 10 minutes of video
    
    private void Start()
    {
        _currentSceneName = SceneManager.GetActiveScene().name;        

        StartCoroutine(StartEndSceneWithDelay());
    }

    private IEnumerator StartEndSceneWithDelay()
    {
        yield return new WaitForSeconds(_timeToEndScene);
        SceneFinished();
        SceneManager.LoadScene(_nextSceneName);
    }

    //TODO only fade in fog if we are in snow scene
    
}
