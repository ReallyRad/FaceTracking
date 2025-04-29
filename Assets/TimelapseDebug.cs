using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimelapseDebug : MonoBehaviour
{
  public TMP_Text _speedText; //TODO make event.

  
  private void OnEnable()
  {
    TimeSpeeding.SpeedTime += SetText;
  }
    
  private void OnDisable()
  {
    TimeSpeeding.SpeedTime -= SetText;
  }
  
  public void ResetScene()
  {
    SceneManager.LoadScene("Sunset");
  }

  private void SetText(float val)
  {
    _speedText.text = "Playback rate : " + val;
  }
}
