using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathParticleControl : MonoBehaviour
{
    private ParticleSystem ps;
    public float hSliderValue = 5.0f;

    private void OnEnable()
    {
        FaceTrackingManager.PuckerTrigger += ParticleForce;
    }

    private void OnDisable()
    {
        FaceTrackingManager.PuckerTrigger -= ParticleForce;
    }

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }


    private void ParticleForce(bool pucker)
    {
        var emission = ps.emission;
        if (pucker) emission.rateOverTime = 15;
        else emission.rateOverTime = 0;
    }
}
