using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioToggleGroup : MonoBehaviour
{
    [SerializeField] private Button _nextButton;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private IntVariable _experienceVariable;
    
    public void SetExperience(int experience)
    {
        _experienceVariable.Value = experience;
        
        if (_canvasGroup.alpha == 1)
            _nextButton.interactable = true;
    }
    
}
