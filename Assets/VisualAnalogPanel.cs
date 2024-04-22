using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.UI;

public class VisualAnalogPanel : MonoBehaviour
{
    [SerializeField] private int _modifiedValues;

    [SerializeField] private Button _nextButton;
    
    public void IncrementModifiedValues()
    {
        _modifiedValues++;
        if (_modifiedValues >= 3) _nextButton.interactable = true;
    }
}
