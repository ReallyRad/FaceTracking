using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour
{
    [SerializeField] private bool _setBlockerTransparency;
    [SerializeField] private Material _occluderMaterial;

    [SerializeField] private bool _setSkyboxExposure;

    private void OnEnable()
    {
        FaceTrackingManager.BlockValue += BlockerValue;
        FaceTrackingManager.SkyboxExposure += SkyboxExposure;
    }

    private void OnDisable()
    {
        FaceTrackingManager.BlockValue -= BlockerValue;
        FaceTrackingManager.SkyboxExposure -= SkyboxExposure;
    }

    private void BlockerValue(float progressTime)
    {
        if (_setBlockerTransparency)
        {
            float mappedValue = Map(progressTime, 0f, 35000f, 1f, 0f);
            Color color = new Color(1,1,1,mappedValue);
            _occluderMaterial.SetColor("_Color", color);
        }
    }

    private void SkyboxExposure(float progress)
    {
        if (_setSkyboxExposure)
        {
            float mappedValue = Map(progress, 0f, 35000f, 4f, 0.5f);
            RenderSettings.skybox.SetFloat("_Exposure", mappedValue);
        }
    }
    
    private float Map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }

}
