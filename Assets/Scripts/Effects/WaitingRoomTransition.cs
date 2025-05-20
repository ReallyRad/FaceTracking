using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ScriptableObjectArchitecture;

public enum Experience {VRBreathingMystical, VRMystical, VRControl}  //TODO find better place to put this

public class WaitingRoomTransition : MonoBehaviour //TODO rename to transition manager? 
{
    [SerializeField] private IntVariable _selectedExperience;
    
    private int _waitingDuration;

    public void ExperienceSelected()
    {
        if (_selectedExperience.Value != (int) Experience.VRControl) _waitingDuration = 10;
        else _waitingDuration = 2;
    }
    
    public void OnSlideshowFinished()
    { 
        StartCoroutine(WaitAndLoadNextScene());
    }
    
    private IEnumerator WaitAndLoadNextScene()
    {
        yield return new WaitForSeconds(10);
        string loadScene = ((Experience)_selectedExperience.Value).ToString();
        SceneManager.LoadScene(loadScene);
    }
}
