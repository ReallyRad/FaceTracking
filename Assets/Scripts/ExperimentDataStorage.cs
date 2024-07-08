using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ExperimentDataStorage : ScriptableObject
{
    public Dictionary<string, string> experimentDataDictionary = new Dictionary<string, string>(); 
}
