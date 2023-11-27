using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TrackingUI : MonoBehaviour
{
    private float _startProgressAt;
    private float _endProgressAt;

    private Stopwatch _puckerStopwatch;

    public Material FluidMaterial;
    public Color lowerThanMinColor = Color.yellow;
    public Color betweenMinAndMaxColor = Color.green;
    public Color higherThanMaxColor = Color.red;

    private void OnEnable()
    {
        FaceTrackingManager.PuckerTrigger += PuckerTrigger;
    }

    private void OnDisable()
    {
        FaceTrackingManager.PuckerTrigger -= PuckerTrigger;
    }

    private void Start()
    {
        _startProgressAt = 3f;
        _endProgressAt = 7f;
        _puckerStopwatch = new Stopwatch();
    }

    private void Update()
    {
        float puckerProgress = _puckerStopwatch.ElapsedMilliseconds / 1000f;
        float fraction = puckerProgress / _endProgressAt;
        SetYScale(gameObject, fraction);
        if (puckerProgress < _startProgressAt) FluidMaterial.color = lowerThanMinColor;
        else if (puckerProgress > _startProgressAt && puckerProgress < _endProgressAt) FluidMaterial.color = betweenMinAndMaxColor;
        else if (puckerProgress > _endProgressAt) FluidMaterial.color = higherThanMaxColor;
    }

    private void PuckerTrigger(bool pucker)
    {
        if (pucker)
        {
            _puckerStopwatch.Start();
        }
        else
        {
            _puckerStopwatch.Reset(); 
        }
    }

    void SetYScale(GameObject fluid, float yScale)
    {
        Vector3 newScale = fluid.transform.localScale;
        newScale.y = yScale;
        fluid.transform.localScale = newScale;
    }
}
