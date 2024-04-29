using System;
using UnityEngine;

public enum QuestionnaireAnswerType {
    mood,
    relaxation,
    stress,
    anxiety
}

public enum ExperimentState { pre, post }

public enum Experience { beach, garden, night, winter, control }

[CreateAssetMenu]
public class ExperimentData : ScriptableObject
{
    [Header("Experiment Data")]
    public string subjectID;
    public ExperimentState experimentState;
    public QuestionnaireAnswerType answerType;
    public string answerValue;
    public DateTime timestamp;
}
