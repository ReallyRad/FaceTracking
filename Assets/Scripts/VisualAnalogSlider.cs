using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualAnalogSlider : MonoBehaviour
{
    private bool _modified;
    [SerializeField] private Slider _slider;
    [SerializeField] private VisualAnalogPanel _panel;

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(delegate {ValueChangeCheck();} );
    }

    public void ValueChangeCheck()
    {
        if (!_modified)
        {
            _modified = true;
            _panel.IncrementModifiedValues();
        }
    }
    
}
