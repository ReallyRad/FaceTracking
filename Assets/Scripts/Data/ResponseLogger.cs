using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;

public class ResponseLogger : MonoBehaviour
{
    [SerializeField] private QuestionnaireAnswerType _answerType;
    [SerializeField] private GameEvent _newDataAvailableEvent;
    [SerializeField] private ExperimentData _experimentData;

    public void NextButtonPressed()
    {
        _experimentData.answerType = _answerType;
        _experimentData.timestamp = DateTime.Now;
        _newDataAvailableEvent.Raise();
    }
    
    public void SetValue(bool value)
    {
        if (value) _experimentData.answerValue = value.ToString();
        else _experimentData.answerValue = value.ToString();
    }

    public void SetValue(float value)
    {
        _experimentData.answerValue = value.ToString();
        _experimentData.answerType = _answerType;
        _experimentData.timestamp = DateTime.Now;
        _newDataAvailableEvent.Raise();
    }

    public void SetValue(string value)
    {
        _experimentData.answerValue = value;
    }
}