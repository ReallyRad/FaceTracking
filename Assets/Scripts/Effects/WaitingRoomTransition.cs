using UnityEngine;
using VolumetricFogAndMist2;
using UnityEngine.SceneManagement;
using ScriptableObjectArchitecture;

public class WaitingRoomTransition : MonoBehaviour //TODO cleanup
{
    public VolumetricFog fog;
    public AnimationCurve curve;
    public float minDensityVolume;
    public float maxDensityVolume;

    private float timeLeft;
    private bool timerRunning = false;

    [SerializeField] private IntVariable _selectedExperience;
    [SerializeField] private ExperimentStateSO experimentStateSO;
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
           
            var normalVal = curve.Evaluate((ScenarioToggleGroup.waitingDuration - timeLeft) / ScenarioToggleGroup.waitingDuration);
            var realVal = Utils.Map(normalVal, 0, 1, minDensityVolume, maxDensityVolume);
            if (_selectedExperience == (int) Experience.PsychedelicGarden) 
                fog.settings.density = realVal;
        }
    }

    public void OnSlideshowFinished()
    {
        if (experimentStateSO.experimentState == ExperimentState.pre)
        {
            StartTimer(ScenarioToggleGroup.waitingDuration);
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
