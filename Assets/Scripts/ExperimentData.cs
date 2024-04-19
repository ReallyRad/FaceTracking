using System;
using UnityEngine;

public enum QuestionnaireAnswerType {
    stress,
    mood,
    anxiety,
    happiness
}

public enum ExperimentState { pre, post }

[CreateAssetMenu]
public class ExperimentData : ScriptableObject
{
    [Header("Experiment Data")]
    public string subjectID;
    
    public QuestionnaireAnswerType answerType;
    public ExperimentState experimentState;

    public string answerValue;

    public DateTime timestamp;
}
