using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ModifyColorTuning : MonoBehaviour //tracks the volume weight so that the color curves can be applied progressively
{
    [SerializeField] private Volume _volume;
    private ColorCurves _colorCurvesEffect;

    [SerializeField]
    private AnimationCurve _initialCurve;
        
    [SerializeField] [Range (0,1)]
    private float _effectStrength;
    
    private void Start()
    {
        _volume.profile.TryGet(typeof(ColorCurves), out _colorCurvesEffect);
        
        for (int i = 0; i <= _colorCurvesEffect.hueVsSat.value.length; i++)
        {
            _initialCurve.AddKey(_colorCurvesEffect.hueVsSat.value[i].time, _colorCurvesEffect.hueVsSat.value[i].value);
        }
    }
    
    private void Update()
    {
        _colorCurvesEffect.SetDirty();
        for (int i = 0; i < _initialCurve.length; i++)
        {
            _colorCurvesEffect.hueVsSat.value.MoveKey(i,  new Keyframe(_initialCurve[i].time, (_initialCurve[i].value  - 0.5f ) * _effectStrength + 0.5f ));
        }
    }

    public void SetEffectStrength(float val)
    {
        _effectStrength = val;
    }
}