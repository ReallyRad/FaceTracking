using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSnowSlowing : MonoBehaviour
{
    [SerializeField] private ParticleSystem snowPS;

    [SerializeField] private float changeStartTime = 90f;
    [SerializeField] private float changeEndTime = 180f;

    [SerializeField] private float riseDuration = 7f;
    [SerializeField] private float decayDuration = 3f;

    [SerializeField] private float maximumSnowSpeed = 1f;
    [SerializeField] private float minimumSnowSpeed = 0.03f;
    void Start()
    {
        var main = snowPS.main;

        StartCoroutine(
            PingPongLerp(
                v0: minimumSnowSpeed,
                v1: maximumSnowSpeed,
                dT1: riseDuration,
                dT2: decayDuration,
                t1: changeStartTime,
                t2: changeEndTime,
                applyValue: value => main.simulationSpeed = value
            )
        );
    }

    public IEnumerator PingPongLerp(
        float v0, float v1,
        float dT1, float dT2,
        float t1, float t2,
        Action<float> applyValue)
    {
        // Wait until t1
        float timeSinceStart = 0f;
        while (timeSinceStart < t1)
        {
            timeSinceStart += Time.deltaTime;
            yield return null;
        }

        float sceneTime = timeSinceStart;

        while (sceneTime < t2)
        {
            // Phase 1: v0 -> v1 over dT1
            float elapsed = 0f;
            while (elapsed < dT1 && sceneTime < t2)
            {
                float t = elapsed / dT1;
                float value = Mathf.Lerp(v0, v1, t);
                applyValue(value);

                elapsed += Time.deltaTime;
                sceneTime += Time.deltaTime;
                yield return null;
            }
            applyValue(v1); // Snap to exact end value

            // Phase 2: v1 -> v0 over dT2
            elapsed = 0f;
            while (elapsed < dT2 && sceneTime < t2)
            {
                float t = elapsed / dT2;
                float value = Mathf.Lerp(v1, v0, t);
                applyValue(value);

                elapsed += Time.deltaTime;
                sceneTime += Time.deltaTime;
                yield return null;
            }
            applyValue(v0);
        }
    }
}
