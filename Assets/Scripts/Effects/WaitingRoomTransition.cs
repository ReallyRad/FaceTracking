using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ScriptableObjectArchitecture;

public class WaitingRoomTransition : MonoBehaviour
{
    [SerializeField] private IntVariable _selectedExperience;
    [SerializeField] private ExperimentStateSO experimentStateSO;
    [SerializeField] private ExperimentDataStorage _experimentDataStorage; //reference it here just to make sure it's persisted when switching scenes
    
    private int _waitingDuration; 

    public void OnSlideshowFinished()
    {
        if (experimentStateSO.experimentState == ExperimentState.post) Application.Quit();
        else StartCoroutine(WaitAndLoadNextScene());
    }
    
    private  IEnumerator WaitAndLoadNextScene()
    {
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene(_selectedExperience.ToString());
    }
}
