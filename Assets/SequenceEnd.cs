using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SequenceEnd : Sequenceable
{
    [SerializeField] private ExperimentStateSO experimentStateSO;
    [SerializeField] private StringVariable _subjectIDVariable;
    [SerializeField] private ExperimentDataStorage _experimentDataStorage; //reference it here just to make sure it's persisted when switching scenes
    [SerializeField] private IntVariable _selectedExperience; //reference it here just to make sure it's persisted when switching scenes
 
    public override void Initialize()
    {
        _active = true;
        StartCoroutine(TransitionToWaitingScene());
    }

    private IEnumerator TransitionToWaitingScene()
    {
        //TODO add fog transition here if necessary 
        yield return new WaitForSeconds(5);
        experimentStateSO.experimentState = ExperimentState.post; //load the new scene, this time as post questionnaire
        SceneManager.LoadScene("Waiting");
    }
}
