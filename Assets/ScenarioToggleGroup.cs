using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioToggleGroup : MonoBehaviour
{
    [SerializeField] private Button _nextButton;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private IntVariable _experienceVariable;

    [SerializeField] private GameObject _canvInstruction;
    [SerializeField] private GameObject _canvWelcome;

    [SerializeField] private TextMeshProUGUI transitionTitle;
    [SerializeField] private SlideShow slideShow;

    [SerializeField] private GameObject vibrationManager;


    public static int waitingDuration;

    public void SetExperience(int experience)
    {
        _experienceVariable.Value = experience;
        
        if (_canvasGroup.alpha == 1) _nextButton.interactable = true;
    }
    public void SetExperienceOkButtonClicked() 
    {
        if (_experienceVariable.Value == 0)
        {
            waitingDuration = 10;
            transitionTitle.text = "Super! Jetzt weisst du, wie du langsames Atmen richtig anwendest.\r\n\r\n\r\nWenn du bereit bist, drücke auf OK, um mit dem 10-min. Slow-Paced Breathing Trainingsprogramm zu starten.\r\n\r\n";
            DontDestroyOnLoad(vibrationManager);
        }
        else if (_experienceVariable.Value == 1)
        {
            _canvInstruction.SetActive(false);
            _canvWelcome.SetActive(false);
            waitingDuration = 2;
            transitionTitle.text = "Wenn Sie bereit sind, drücken Sie „OK“, um mit der Wiedergabe einer Dokumentation zu beginnen.";
            slideShow.UpdateActiveSlides();
        }
        else if (_experienceVariable.Value == 2)
        {
            waitingDuration = 2;
            transitionTitle.text = "Super! Jetzt weisst du, wie du langsames Atmen richtig anwendest.\r\n\r\n\r\nWenn du bereit bist, drücke auf OK, um mit dem 10-min. Slow-Paced Breathing Trainingsprogramm zu starten.\r\n\r\n";
            DontDestroyOnLoad(vibrationManager);
        }
    }
}
