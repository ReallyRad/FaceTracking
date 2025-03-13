using System.Collections;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTimer : MonoBehaviour 
{
    [SerializeField] private string _nextSceneName; 
    
    [SerializeField] private float _timeToEndScene; 
    
    [SerializeField] private ExperimentStateSO experimentStateSO;
    [SerializeField] private StringVariable _subjectIDVariable;
    [SerializeField] private ExperimentDataStorage _experimentDataStorage; //reference it here just to make sure it's persisted when switching scenes
    [SerializeField] private IntVariable _selectedExperience; //reference it here just to make sure it's persisted when switching scenes

    public delegate void OnSceneFinished();
    public static OnSceneFinished SceneFinished;
    
    private void Start()
    {
        StartCoroutine(StartEndSceneWithDelay());
    }

    private IEnumerator StartEndSceneWithDelay()
    {
        yield return new WaitForSeconds(_timeToEndScene);
        SceneFinished();
        SceneManager.LoadScene(_nextSceneName);
    }

}
