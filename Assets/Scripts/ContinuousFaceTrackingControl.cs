using System;
using System.Diagnostics;
using Oculus.Movement;
using Oculus.Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ContinuousFaceTrackingControl : MonoBehaviour
{
    public delegate void OnExhalingLongerThanMinimum(float howLongMore);
    public static OnExhalingLongerThanMinimum NewExhalingLongerThanMinimum;

    private Stopwatch _smileStopwatch;
    private Stopwatch _puckerStopwatch;
    private Stopwatch _progressStopwatch;

    private bool _wasSlightSmile;  
    private bool _wasSlightPucker; 

    [SerializeField] private bool _smiling;
    [SerializeField] private bool _slightSmile;
    [SerializeField] private bool _pucker;
    [SerializeField] private bool _slightPucker;

    private float minAcceptableExhalePeriod = 1f;
    private float maxAcceptableExhalePeriod = 3f;
    private float minAcceptableInhalePeriod = 1f;
    private float maxAcceptableInhalePeriod = 4f;

    public GameObject exhalePeriodFluid;
    public GameObject inhalePeriodFluid;
    public Material exhaleMaterial;
    public Material inhaleMaterial;

    private void Start()
    {
        _smileStopwatch = new Stopwatch();
        _puckerStopwatch = new Stopwatch();
        _progressStopwatch = new Stopwatch();

        SetYScale(inhalePeriodFluid, 0f);
        SetYScale(exhalePeriodFluid, 0f);
    }

    private void OnEnable()
    {
        FaceTrackingManager.Pucker += Pucker;
        FaceTrackingManager.SlightPucker += SlightPucker;
        FaceTrackingManager.SlightSmile += SlightSmile;
        FaceTrackingManager.Smile += Smile;
    }

    private void OnDisable()
    {
        FaceTrackingManager.Pucker -= Pucker;
        FaceTrackingManager.SlightPucker -= SlightPucker;
        FaceTrackingManager.SlightSmile -= SlightSmile;
        FaceTrackingManager.Smile -= Smile;
    }

    private void Update()
    {
        float inhaleTime = (float) _smileStopwatch.ElapsedMilliseconds / 1000.0f;
        float inhaleProportion = inhaleTime / maxAcceptableInhalePeriod;
        SetYScale(inhalePeriodFluid, inhaleProportion);
        if (inhaleTime > minAcceptableInhalePeriod)
        {
            if (inhaleTime < maxAcceptableInhalePeriod)
            {
                inhaleMaterial.color = Color.green;
            }
            else
            {
                inhaleMaterial.color = Color.red;
            }
        }
        else
        {
            inhaleMaterial.color = Color.yellow;
        }


        float exhaleTime = (float) _puckerStopwatch.ElapsedMilliseconds / 1000.0f;
        float exhaleProportion = exhaleTime / maxAcceptableExhalePeriod;
        SetYScale(exhalePeriodFluid, exhaleProportion);
        if (exhaleTime > minAcceptableExhalePeriod)
        {
            if (exhaleTime < maxAcceptableExhalePeriod)
            {
                exhaleMaterial.color = Color.green;

                float fraction = ((exhaleTime - minAcceptableExhalePeriod) / (maxAcceptableExhalePeriod - minAcceptableExhalePeriod)) * Time.deltaTime;
                NewExhalingLongerThanMinimum.Invoke(fraction);
            }
            else
            {
                exhaleMaterial.color = Color.red;
            }
        }
        else
        {
            exhaleMaterial.color = Color.yellow;
        }
    }


    private void Pucker()
    {
        _pucker = true;
        _puckerStopwatch.Start();
        _wasSlightPucker = false;
    }

    private void Smile()
    {
        _smiling = true;
        _smileStopwatch.Start();
        _wasSlightSmile = false;
    }

    private void SlightPucker()
    {
        _pucker = false;
        _puckerStopwatch.Stop();
        _progressStopwatch.Stop();
        _wasSlightPucker = true;
        if (_wasSlightSmile) _smileStopwatch.Reset(); 
    }

    private void SlightSmile()
    {
        _smiling = false;
        _smileStopwatch.Stop();
        _progressStopwatch.Stop();
        _wasSlightSmile = true;
        if (_wasSlightPucker) _puckerStopwatch.Reset(); 
    }

    void SetYScale(GameObject fluid, float yScale)
    {
        Vector3 newScale = fluid.transform.localScale;
        newScale.y = yScale;
        fluid.transform.localScale = newScale;
    }
}