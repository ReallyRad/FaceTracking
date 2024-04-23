using System;
using UnityEngine;

public enum QuestionnaireAnswerType {
    stress,
    mood,
    anxiety,
    happiness
}

public enum ExperimentState { pre, post }

public enum Experience { control, beach, garden, nightSky, snow }

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
