using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscreteWaveChangingColorControl : MonoBehaviour
{
    private ParticleSystem ps;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        FaceTrackingManager.NewFullBreath += WholeColorChangeVFX;
        DiscreteBreathingControl.NewFullBreath += WholeColorChangeVFX;
    }

    private void OnDisable()
    {
        FaceTrackingManager.NewFullBreath -= WholeColorChangeVFX;
        DiscreteBreathingControl.NewFullBreath -= WholeColorChangeVFX;
    }

    private void WholeColorChangeVFX()
    {
        float redChange = DiscreteVfxControl.waveChangingColorFinalColor.r - DiscreteVfxControl.waveChangingColorInitialColor.r;
        float greenChange = DiscreteVfxControl.waveChangingColorFinalColor.g - DiscreteVfxControl.waveChangingColorInitialColor.g;
        float blueChange = DiscreteVfxControl.waveChangingColorFinalColor.b - DiscreteVfxControl.waveChangingColorInitialColor.b;
        float transChange = DiscreteVfxControl.waveChangingColorFinalColor.a - DiscreteVfxControl.waveChangingColorInitialColor.a;
        
        float colorChangeSteps = DiscreteVfxControl.waveChangingColorEndBN - DiscreteVfxControl.waveChangingColorStartBN;
        
        float redChangeSpeed = redChange / colorChangeSteps;
        float greenChangeSpeed = greenChange / colorChangeSteps;
        float blueChangeSpeed = blueChange / colorChangeSteps;
        float transChangeSpeed = transChange / colorChangeSteps; 
        
        Color speed = new Color(redChangeSpeed, greenChangeSpeed, blueChangeSpeed, transChangeSpeed);

        Debug.Log("speed: " + speed);

        if (gameObject.activeSelf) 
        {
            //ParticleSystemRenderer psRenderer = ps.GetComponent<ParticleSystemRenderer>();
            //Color currentCol = psRenderer.material.color;
            //float newR = currentCol.r + redChangeSpeed;
            //float newG = currentCol.g + greenChangeSpeed;
            //float newB = currentCol.b + blueChangeSpeed;
            //float newT = currentCol.a + transChangeSpeed;

            //Color newColor = new Color(newR, newG, newB, newT);
            //psRenderer.material.color = newColor;
            //psRenderer.material.SetFloat("_Alpha", newT);

            StartCoroutine(IncrementalColorChangeVFX(DiscreteVfxControl.increment, speed));
        }
    }

    private IEnumerator IncrementalColorChangeVFX(float increment, Color speed)
    {
        float startTime = Time.time;
        float endTime = startTime + increment;

        while (Time.time < endTime)
        {
            float deltaTime = Time.deltaTime;

            ParticleSystemRenderer psRenderer = ps.GetComponent<ParticleSystemRenderer>();
            Color currentCol = psRenderer.material.color;
            float newR = currentCol.r + ((speed.r) / increment) * deltaTime;
            float newG = currentCol.g + ((speed.g) / increment) * deltaTime;
            float newB = currentCol.b + ((speed.b) / increment) * deltaTime;
            float newT = currentCol.a + ((speed.a) / increment) * deltaTime;

            Color newColor = new Color(newR, newG, newB, newT);
            psRenderer.material.color = newColor;
            psRenderer.material.SetFloat("_Alpha", newT);

            yield return null;
        }
    }
}
