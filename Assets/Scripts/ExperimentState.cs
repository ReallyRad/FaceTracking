using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExperimentState { pre, post }

[CreateAssetMenu]
public class ExperimentStateSO : ScriptableObject
{
    public ExperimentState experimentState;
}
