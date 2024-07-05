using System;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;

public class SlideShow : MonoBehaviour
{
    [SerializeField] private GameEvent _slideShowFinished;
    [SerializeField] private List<GameObject> _slides;
    
    [SerializeField] private List<GameObject> _preSlides;
    [SerializeField] private List<GameObject> _postSlides;
    [SerializeField] private ExperimentStateSO _experimentStateSO;
    
    private int _slideIndex;
    private bool _showing;

    private void Awake()
    {
        if (_experimentStateSO.experimentState == ExperimentState.post)
             foreach (GameObject preSlide in _preSlides) Destroy(preSlide);
        else if (_experimentStateSO.experimentState == ExperimentState.pre)
            foreach (GameObject postSlide in _postSlides) Destroy(postSlide);
        
        _slides = new List<GameObject>();
        foreach (Transform slide in transform) 
        {
            _slides.Add(slide.gameObject);
            slide.GetComponent<PanelDimmer>().Hide();
        }

        _slides[0].GetComponentInChildren<PanelDimmer>().Show();
    }

    private void Start()
    {
     
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
        _showing = false;
        _slideIndex = 0;
    }
}