using ScriptableObjectArchitecture;
using UnityEngine;
using VolumetricFogAndMist2;

public class FogTweener : MonoBehaviour
{
    [SerializeField] private VolumetricFog _fog;
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private IntVariable _selectedExperience;

    [SerializeField] private ExperimentStateSO experimentStateSO;

    private bool shouldTwen;

    public void ExperienceSelected() //only tween if we're going to intervention scenes
    {
        shouldTwen = _selectedExperience.Value == (int) Experience.Snow;
    }
    
    public void TweenFog()
    {
        if (experimentStateSO.experimentState == ExperimentState.pre && shouldTwen)
        {
            LeanTween.value(0, 1, 10).setOnUpdate( val =>
            {
                _fog.settings.density = val;
            });    
        }
    }
}
