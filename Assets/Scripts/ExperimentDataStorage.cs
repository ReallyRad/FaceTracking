using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ExperimentDataStorage : ScriptableObject
{
    private void OnEnable() => hideFlags = HideFlags.DontUnloadUnusedAsset;

    public Dictionary<string, string> experimentDataDictionary = new Dictionary<string, string>(); 
}
