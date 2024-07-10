using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExperimentState { pre, post }

[CreateAssetMenu]
public class ExperimentStateSO : ScriptableObject //Data conteiner for pre/post state that can be shared across scenes
{
    private void OnEnable() => hideFlags = HideFlags.DontUnloadUnusedAsset;

    public ExperimentState experimentState;
}
