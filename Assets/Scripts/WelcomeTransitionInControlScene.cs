using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeTransitionInControlScene : MonoBehaviour
{
    public GameObject CanvControlVideo;
    public GameObject CanvWelcome;

    public void WelcomeToVideoTransition()
    {
        CanvWelcome.SetActive(false);
        CanvControlVideo.SetActive(true);
    }
}
