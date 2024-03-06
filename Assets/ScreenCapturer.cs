using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCapturer : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(WaitAndScreenShot());
    }

    private IEnumerator WaitAndScreenShot()
    {
        yield return new WaitForSeconds(4);
        ScreenCapture.CaptureScreenshot("Assets/Textures/BlurredMap.png");
    }
}
    