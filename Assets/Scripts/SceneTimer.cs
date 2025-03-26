using System;
using System.Collections;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTimer : MonoBehaviour  //count 10 minutes intervention time and go back to Waiting scene
{
    [SerializeField] private float _timeToEndScene; 
    [SerializeField] private ExperimentStateSO _experimentState;
    [SerializeField] private bool _startTimerOnStart;
    
    public delegate void OnSceneFinished();
    public static OnSceneFinished SceneFinished;

    private void Start()
    {
        if (_startTimerOnStart) StartCoroutine(StartEndSceneWithDelay());
    }

    public void StartTimer()
    {
        StartCoroutine(StartEndSceneWithDelay());
    }

    public void EndScene()
    {
        LeanTween.cancelAll();
        _experimentState.experimentState = ExperimentState.post;
        SceneFinished();
        SceneManager.LoadScene("Waiting");
    }
    
    private IEnumerator StartEndSceneWithDelay()
    {
        yield return new WaitForSeconds(_timeToEndScene);
        EndScene();
    }
}
