using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadingSoundIn : MonoBehaviour //TODO cleanup
{
    [SerializeField] private AudioSource _sound;
    [SerializeField] private float initialVolume;
    [SerializeField] private float finalVolume;
    [SerializeField] private float risingDuration;
    [SerializeField] private float risingStart;

    private void Start()
    {
        _sound.volume = initialVolume;
        StartCoroutine(StartCoroutineWithDelay());
    }
    IEnumerator StartCoroutineWithDelay()
    {
        yield return new WaitForSeconds(risingStart);
        StartCoroutine(SoundFadingIn());
    }
    IEnumerator SoundFadingIn()
    {
        _sound.Play();
        _sound.volume = initialVolume;
        float startTime = Time.time;

        while (Time.time < startTime + risingDuration)
        {
            _sound.volume = Mathf.Lerp(initialVolume, finalVolume, (Time.time - startTime) / risingDuration);
            yield return null;
        }
        _sound.volume = finalVolume;
    }
}
