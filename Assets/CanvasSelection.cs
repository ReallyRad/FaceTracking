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
    
    //TODO don't access objects like that in the editor
    [SerializeField] private GameObject _VRBreathingMysticalTitle;
    [SerializeField] private GameObject _VRMysticalTitle;
    [SerializeField] private GameObject _VRControlTitle;
    
    [SerializeField] private SlideShow _waitingRoomSlideShow; 
    [SerializeField] private IntVariable _experienceVariable;

    public void ExperienceSelected() //select the slides that need to be shown in the waiting scene pre
    {
        if (_experienceVariable.Value == (int) Experience.VRBreathingMystical) 
        {
            //instructions and welcome are disabled by default
            _canvInstruction.SetActive(true);
            _canvWelcome.SetActive(true);
            
            _waitingRoomSlideShow.UpdateActiveSlides();
        }
        
        //experience title is enabled by default
        _VRBreathingMysticalTitle.SetActive(_experienceVariable.Value == (int) Experience.VRBreathingMystical);
        _VRMysticalTitle.SetActive(_experienceVariable.Value == (int) Experience.VRMystical);
        _VRControlTitle.SetActive(_experienceVariable.Value == (int) Experience.VRControl); 
    }
}
