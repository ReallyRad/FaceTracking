using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitingRoomSlideShow : SlideShow
{
    
    [SerializeField] private List<GameObject> _preSlides; //slides to keep in pre
    [SerializeField] private List<GameObject> _postSlides; //slides to keep in post
    [SerializeField] private ExperimentStateSO _experimentStateSO;


    private void Start()
    {
        //pick slides, VAS slides are always kept
        if (_experimentStateSO.experimentState == ExperimentState.pre)
            foreach (GameObject postSlide in _postSlides)
                postSlide.SetActive(false);
            
        if (_experimentStateSO.experimentState == ExperimentState.post)
            foreach (GameObject preSlide in _preSlides)
                    preSlide.SetActive(false);
        
        UpdateActiveSlides();
    }

    

}