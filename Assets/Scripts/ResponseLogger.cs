using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;

public class ResponseLogger : MonoBehaviour
{
    [SerializeField] private ExperimentState _experimentState;
    [SerializeField] private QuestionnaireAnswerType _answerType;
    [SerializeField] private ExperimentDataGameEvent _newDataAvailableEvent;

    private ExperimentData _experimentData;

    private void OnEnable()
    {
        WaitingRoomTransition.NotifyPrePostState += SetExperimentState;
    }

    private void OnDisable()
    {
        WaitingRoomTransition.NotifyPrePostState -= SetExperimentState;
    }

    private void Start()
    {
        _experimentData = ScriptableObject.CreateInstance<ExperimentData>();
    }

    public void NextButtonPressed()
    {
        _experimentData.answerType = _answerType;
        _experimentData.experimentState = _experimentState;
        _experimentData.timestamp = DateTime.Now;
        _newDataAvailableEvent.Raise(_experimentData);
    }
    
    public void SetValue(bool value)
    {
        if (value) _experimentData.answerValue = value.ToString();
        else _experimentData.answerValue = value.ToString();
    }

    public void SetValue(float value)
    {
        _experimentData.answerValue = value.ToString();
    }

    public void SetValue(string value)
    {
        _experimentData.answerValue = value;
    }

    private void SetExperimentState(ExperimentState prePost)
    {
        _experimentState = prePost;
    }
}