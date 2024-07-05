using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SequenceEnd : Sequenceable
{
    [SerializeField] private ExperimentStateSO experimentStateSO;
    
    public override void Initialize()
    {
        _active = true;
        StartCoroutine(TransitionToWaitingScene());
    }

    private IEnumerator TransitionToWaitingScene()
    {
        //TODO add fog transition here if necessary 
        experimentStateSO.experimentState = ExperimentState.pre;
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Waiting");
    }
}
