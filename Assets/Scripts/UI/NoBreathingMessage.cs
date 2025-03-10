using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class NoBreathingMessage : MonoBehaviour
{
    [SerializeField] private int _timeBeforePopup;
    [SerializeField] private PanelDimmer _panelDimmer;

    private Stopwatch _stopwatch;
    private bool _shown;

    private void Start()
    {
        _stopwatch = new Stopwatch();
        _panelDimmer.Hide();
    }

    private void OnEnable()
    {
        FaceTrackingManager.PuckerTrigger += PuckerTrigger;
    }

    private void OnDisable()
    {
        FaceTrackingManager.PuckerTrigger -= PuckerTrigger;
    }

    private void Update()
    {
        if (_stopwatch.ElapsedMilliseconds > _timeBeforePopup * 1000 && !_shown) //show panel if too long since last breath
        {
            _panelDimmer.Show();
            GetComponent<VisionFollower>().isCentered = true;
            _shown = true;
        }
    }

    private void PuckerTrigger(bool pucker)
    {
        if (!pucker)  //if stopped puckering
        {
            _stopwatch.Start(); //stat stopwatch
        }
        else
        {
            GetComponent<VisionFollower>().isCentered = false;
            _stopwatch.Stop();
            _stopwatch.Reset();
            _panelDimmer.Hide();
            _shown = false;
        }
    }
    
}
