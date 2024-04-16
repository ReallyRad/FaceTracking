using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundSoundFadingIn : MonoBehaviour
{
    [SerializeField] private ParticleSystem _snowPS;
    [SerializeField] private AudioSource _backgroundSound;

    private float initialBackgroundSoundVolume = 0f;
    private float finalBackgroundSoundVolume = 0.3f;
    private float backgroundSoundRaisingDuration = 7f;

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "SceneSnow")
        {
            var emission = _snowPS.emission;
            emission.rateOverTime = 0;
        }

        StartCoroutine(SoundFadingIn());
    }
    IEnumerator SoundFadingIn()
    {
        _backgroundSound.volume = initialBackgroundSoundVolume;
        float startTime = Time.time;

        while (Time.time < startTime + backgroundSoundRaisingDuration)
        {
            _backgroundSound.volume = Mathf.Lerp(initialBackgroundSoundVolume, finalBackgroundSoundVolume, (Time.time - startTime) / backgroundSoundRaisingDuration);
            yield return null;
        }
        _backgroundSound.volume = finalBackgroundSoundVolume;
    }
}
