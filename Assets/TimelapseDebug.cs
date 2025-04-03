using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimelapseDebug : MonoBehaviour
{
  public void ResetScene()
  {
    SceneManager.LoadScene("Sunset");
  }
}
