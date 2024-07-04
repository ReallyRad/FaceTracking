using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SequenceEnd : Sequenceable
{
    public delegate void OnNotifyPrePostState(ExperimentState prePost);
    public static OnNotifyPrePostState NotifyPrePostState;
    
    public override void Initialize()
    {
        _active = true;
    }

    private IEnumerator TransitionToWaitingScene()
    {
        //TODO add fog transition here if necessary 
        yield return new WaitForSeconds(5);
        NotifyPrePostState(ExperimentState.pre);
        SceneManager.LoadScene("Waiting");
    }
}
