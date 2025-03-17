using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ScriptableObjectArchitecture;

public class WaitingRoomTransition : MonoBehaviour //TODO rename to transition manager? 
{
    [SerializeField] private IntVariable _selectedExperience;
    [SerializeField] private ExperimentStateSO experimentStateSO;
    [SerializeField] private ExperimentDataStorage _experimentDataStorage; //reference it here just to make sure it's persisted when switching scenes
    
    private int _waitingDuration;

    public void ExperienceSelected()
    {
        if (_selectedExperience.Value == 0) _waitingDuration = 10;
        else _waitingDuration = 2;
    }
    
    public void OnSlideshowFinished()
    {
        if (experimentStateSO.experimentState == ExperimentState.post) Application.Quit();
        else StartCoroutine(WaitAndLoadNextScene());
    }
    
    private IEnumerator WaitAndLoadNextScene()
    {
        yield return new WaitForSeconds(10);
        string loadScene = ((Experience)_selectedExperience.Value).ToString();
        SceneManager.LoadScene(loadScene);
    }
}
