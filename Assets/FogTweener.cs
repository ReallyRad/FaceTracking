using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;

public class FogTweener : MonoBehaviour
{
    [SerializeField] private VolumetricFog fog;

    private void OnEnable()
    {
        VideoPanel.VideoCompleted += TweenFog;
    }

    private void OnDisable()
    {
        VideoPanel.VideoCompleted -= TweenFog;
    }

    private void TweenFog()
    {
        LeanTween.value(0, 1, 20).setOnUpdate( val =>
        {
            fog.settings.density = val;
        });
    }
}
