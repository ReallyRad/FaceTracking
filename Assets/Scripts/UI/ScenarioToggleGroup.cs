using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScenarioToggleGroup : MonoBehaviour
{
    [SerializeField] private Button _nextButton;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private IntEvent _experienceSetEvent;

    public void SetExperience(int experience)
    {
        _experienceSetEvent.Invoke(experience);
        
        if (_canvasGroup.alpha == 1) _nextButton.interactable = true; //TODO ????
    }
    
}
