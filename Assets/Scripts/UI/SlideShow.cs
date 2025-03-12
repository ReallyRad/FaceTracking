using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;

public class SlideShow : MonoBehaviour
{
    [SerializeField] protected GameEvent _slideShowFinished;
    [SerializeField] protected List<GameObject> _slides;
    private int _slideIndex;

    private void Start()
    {
        UpdateActiveSlides();
    }

    public void UpdateActiveSlides()
    {
        _slides = new List<GameObject>();
        foreach (Transform slide in transform)
        {
            if (slide.gameObject.activeSelf) _slides.Add(slide.gameObject);
            slide.GetComponent<PanelDimmer>().Hide();
        }
        _slides[_slideIndex].GetComponentInChildren<PanelDimmer>().Show(); //TODO check if should use slide index or 0       
    }
    
    public void NextButton()
    {   
        if (_slideIndex == _slides.Count - 1) //we reached last slide 
        {   
            Hide();
            _slideShowFinished.Raise();
        }
        else //we go to next slide
        {
            _slides[_slideIndex].GetComponent<PanelDimmer>().Hide();
            if (_slideIndex + 1 < _slides.Count)
                _slides[_slideIndex + 1].GetComponent<PanelDimmer>().Show();
            _slideIndex++;
        }
    }

    private void Hide() 
    {
        _slides[_slideIndex].GetComponent<PanelDimmer>().Hide();
        _slideIndex = 0;
    }
}
