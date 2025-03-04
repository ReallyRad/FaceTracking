using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RotateMultibandHueShift : MonoBehaviour
{
    [SerializeField] private Volume _volume;
    [SerializeField] private ShadowsMidtonesHighlights _shadowsMidtonesHighlights;

    [SerializeField] private float _amplitude;
    [SerializeField] private float frequency;
    [SerializeField] private float _phase;

    [SerializeField] private float x, y, z, w;
    
    // Start is called before the first frame update
    void Start()
    {
        _volume.profile.TryGet(typeof(ColorCurves), out _shadowsMidtonesHighlights);
    }

    // Update is called once per frame
    void Update()
    {
        _shadowsMidtonesHighlights.shadows.overrideState = true;
        _shadowsMidtonesHighlights.midtones.value = new Vector4(x, y, z, w);
    }
}
