using UnityEngine;
using VolumetricFogAndMist2;

public class FogTweener : MonoBehaviour
{
    [SerializeField] private VolumetricFog _fog;
    [SerializeField] private AnimationCurve _curve;

    [SerializeField] private ExperimentStateSO experimentStateSO;
    
    public void TweenFog()
    {
        if (experimentStateSO.experimentState == ExperimentState.pre)
        {
            LeanTween.value(0, 1, 10).setOnUpdate( val =>
            {
                _fog.settings.density = val;
            });    
        }
    }
}
