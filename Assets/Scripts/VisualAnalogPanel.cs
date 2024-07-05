using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.UI;

public class VisualAnalogPanel : MonoBehaviour //Used to enable Next button when 2 sliders are changed on a apnale
{
    [SerializeField] private int _modifiedValues;

    [SerializeField] private Button _nextButton;
    
    public void IncrementModifiedValues()
    {
        _modifiedValues++;
        if (_modifiedValues >= 2) _nextButton.interactable = true;
    }
}
