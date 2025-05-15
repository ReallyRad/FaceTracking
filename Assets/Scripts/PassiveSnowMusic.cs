using SCPE;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;

public class PassiveSnowMusic : MonoBehaviour
{

    [SerializeField] private AudioSource progressiveMusic;
    [SerializeField] private ParticleSystem snowPS;

    [SerializeField] private float initialSnowHeaviness = 0f;
    [SerializeField] private float finalSnowHeaviness = 700f;

    [SerializeField] private float initialMusicVolume = 0f;
    [SerializeField] private float finalMusicVolume = 1f;

    [SerializeField] private float changeStartTime = 60f;
    [SerializeField] private float changeDuration = 90f;

    private void Start()
    {
        var emission = snowPS.emission;
        StartCoroutine(
                LerpFloat(
                    initialSnowHeaviness, finalSnowHeaviness,
                    changeStartTime, changeDuration,
                    value => emission.rateOverTime = value
                )
            );

        StartCoroutine(
                LerpFloat(
                    initialMusicVolume, finalMusicVolume,
                    changeStartTime, changeDuration,
                    value => progressiveMusic.volume = value
                )
            );
    }

    public IEnumerator LerpFloat(
        float v1, float v2,
        float t1, float d1,
        Action<float> applyValue)
    {
        yield return new WaitForSeconds(t1);

        float elapsed = 0f;
        while (elapsed < d1)
        {
            float t = elapsed / d1;
            float value = Mathf.Lerp(v1, v2, t);
            applyValue(value);

            elapsed += Time.deltaTime;
            yield return null;
        }

        applyValue(v2);
    }
}
