using ScriptableObjectArchitecture;
using UnityEngine;
using VolumetricFogAndMist2;

public class FogTweener : MonoBehaviour
{
    [SerializeField] private VolumetricFog _fog;
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private IntVariable _selectedExperience;

    private bool _shouldTween;

    public void ExperienceSelected() //only tween if we're going to intervention scenes
    {
        _shouldTween = _selectedExperience.Value != (int) Experience.VRControl;
    }
    
    public void TweenFog()
    {
        if (_shouldTween)
        {
            LeanTween.value(0, 1, 10).setOnUpdate( val =>
            {
                _fog.settings.density = val;
            });    
        }
    }
}
