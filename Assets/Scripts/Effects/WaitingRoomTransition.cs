using UnityEngine;
using VolumetricFogAndMist2;
using UnityEngine.SceneManagement;
using ScriptableObjectArchitecture;

public class WaitingRoomTransition : MonoBehaviour //TODO cleanup
{
    public VolumetricFog fog; //TODO let fog handle itself
    public AnimationCurve curve;
    public float minDensityVolume;
    public float maxDensityVolume;

    private float timeLeft; //TODO use stopwatches or tweens instead
    private bool timerRunning = false;

    [SerializeField] private IntVariable _selectedExperience;
    [SerializeField] private ExperimentStateSO experimentStateSO;
    
    private int _waitingDuration; 

    private void Start()
    {
        fog.settings.density = minDensityVolume;
    }

    private void Update()
    {
        if (timerRunning)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = 0;
                timerRunning = false;
                SceneManager.LoadScene(((Experience) _selectedExperience.Value).ToString());
            }
           
            var normalVal = curve.Evaluate((_waitingDuration - timeLeft) / _waitingDuration);
            var realVal = Utils.Map(normalVal, 0, 1, minDensityVolume, maxDensityVolume);
            
            if (_selectedExperience == (int) Experience.Snow) fog.settings.density = realVal;
        }
    }

    public void ExperienceSelected() 
    {
        if (_selectedExperience.Value == (int) Experience.Control) _waitingDuration = 10;
        else _waitingDuration = 2;
    }

    public void OnSlideshowFinished()
    {
        if (experimentStateSO.experimentState == ExperimentState.pre)
        {
            StartTimer(_waitingDuration);
            experimentStateSO.experimentState = ExperimentState.post;
        }
        else if (experimentStateSO.experimentState == ExperimentState.post) 
        {
            Application.Quit();
        }
    }
    
    private void StartTimer(float duration)
    {
        timeLeft = duration;
        timerRunning = true;
    }
}
