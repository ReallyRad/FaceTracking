using System;
using UnityEngine;

public enum QuestionnaireAnswerType {
    mood,
    relaxation,
    stress,
    anxiety
}

public enum Experience {PsychedelicGarden, Control, HoleGrowthGarden}

[CreateAssetMenu]
public class ExperimentData : ScriptableObject
{
    private void OnEnable() => hideFlags = HideFlags.DontUnloadUnusedAsset;

    [Header("Experiment Data")]
    public QuestionnaireAnswerType answerType;
    public string answerValue;
    
}
