using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CanvasSelection : MonoBehaviour //TODO merge with slideshow slide selection mechanism
{
    [SerializeField] private GameObject _canvInstruction; 
    [SerializeField] private GameObject _canvWelcome; 

    [SerializeField] private GameObject _experienceTitle;
    [SerializeField] private GameObject _controlTitle;
    
    [SerializeField] private WaitingRoomSlideShow _waitingRoomSlideShow; 

    [SerializeField] private IntVariable _experienceVariable;

    public void ExperienceSelected() //select the slides that need to be shown in the waiting scene pre
    {
        if (_experienceVariable.Value == (int) Experience.Control) 
        {
            //instructions and welcome are enabled by default
            _canvInstruction.SetActive(false);
            _canvWelcome.SetActive(false);
            //experience title is enabled by default
            _controlTitle.SetActive(true); 
            _experienceTitle.SetActive(false);
            
            _waitingRoomSlideShow.UpdateActiveSlides();
        }
    }
}
