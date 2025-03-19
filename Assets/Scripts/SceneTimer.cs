using System.Collections;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTimer : MonoBehaviour  //count 10 minutes intervention time and go back to Waiting scene
{
    [SerializeField] private float _timeToEndScene; 
    [SerializeField] private ExperimentState _experimentState;

    public delegate void OnSceneFinished();
    public static OnSceneFinished SceneFinished;
    
    private void Start()
    {
        StartCoroutine(StartEndSceneWithDelay());
    }

    private IEnumerator StartEndSceneWithDelay()
    {
        yield return new WaitForSeconds(_timeToEndScene);
        _experimentState = ExperimentState.post;
        SceneFinished();
        SceneManager.LoadScene("Waiting");
    }
}
