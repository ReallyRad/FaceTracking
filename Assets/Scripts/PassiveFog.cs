using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;

public class PassiveFog : MonoBehaviour
{
    [SerializeField] private VolumetricFog fog;

    [SerializeField] private float initialFogDensity = 1f;
    [SerializeField] private float finalFogDensity = 0f;

    [SerializeField] private float changeStartTime = 0f;
    [SerializeField] private float changeDuration = 60f;

    private void Start()
    {
        StartCoroutine(
                LerpFloat(
                    initialFogDensity, finalFogDensity,
                    changeStartTime, changeDuration,
                    value => fog.settings.density = value
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
