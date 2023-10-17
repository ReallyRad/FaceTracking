using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscreteWaveEnlargingControl : MonoBehaviour
{
    private ParticleSystem ps;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        //FaceTrackingManager.NewFullBreath += WholeEnlargementVFX;
        DiscreteBreathingControl.NewFullBreath += WholeEnlargementVFX;
    }

    private void OnDisable()
    {
        //FaceTrackingManager.NewFullBreath -= WholeEnlargementVFX;
        DiscreteBreathingControl.NewFullBreath -= WholeEnlargementVFX;
    }

    private void WholeEnlargementVFX()
    {
        float scaleChange;
        float scaleChangeSteps;
        float scaleChangeSpeed;
        
        scaleChange = DiscreteVfxControl.waveEnlargingFinalScale - DiscreteVfxControl.waveEnlargingIntialScale;
        scaleChangeSteps = DiscreteVfxControl.waveEnlargingEndBN - DiscreteVfxControl.waveEnlargingStartBN;
        scaleChangeSpeed = scaleChange / scaleChangeSteps;

        if (gameObject.activeSelf) 
        {
            //ps.transform.localScale += new Vector3(scaleChangeSpeed, scaleChangeSpeed,scaleChangeSpeed);
            StartCoroutine(IncrementalEnlargementVFX(DiscreteVfxControl.increment, scaleChangeSpeed));
        }

    }

    private IEnumerator IncrementalEnlargementVFX(float increment, float speed)
    {
        float startTime = Time.time;
        float endTime = startTime + increment;

        while (Time.time < endTime)
        {
            float deltaTime = Time.deltaTime;
            ps.transform.localScale += new Vector3((speed / increment) * deltaTime, 
                                                   (speed / increment) * deltaTime, 
                                                   (speed / increment) * deltaTime);
            yield return null;
        }
    }
}
