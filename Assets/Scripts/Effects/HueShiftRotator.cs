using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HueShiftRotator : MonoBehaviour
{
    private ShadowsMidtonesHighlights shadowMidHigh;
    [SerializeField] private Volume volume;

    [SerializeField] [Range (0,1)]
    private float _hue;
    
    [SerializeField] [Range (0,1)]
    private float _saturation;
    
    [SerializeField] [Range (0,1)]
    private float _frequency;
    
    // Update is called once per frame
    private void Update() {
        if (volume.profile.TryGet(out shadowMidHigh))
        {
            _hue =  Time.realtimeSinceStartup * _frequency % 1;
            
            Color color = Color.HSVToRGB(_hue, _saturation,1);
            
            shadowMidHigh.midtones.SetValue(new Vector4Parameter(new Vector4(
                color.r,
                color.g,
                color.b,
                0
                )));
        }
    }

    public void SetSaturation(float sat)
    {
        _saturation = sat;
    }
}