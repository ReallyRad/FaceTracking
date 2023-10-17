using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscreteAuraApproachingControl : MonoBehaviour
{
    private ParticleSystem ps;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        //FaceTrackingManager.NewFullBreath += WholeApproachingVFX;
        DiscreteBreathingControl.NewFullBreath += WholeApproachingVFX;
    }
    private void OnDisable()
    {
        //FaceTrackingManager.NewFullBreath -= WholeApproachingVFX;
        DiscreteBreathingControl.NewFullBreath -= WholeApproachingVFX;
    }
    private void WholeApproachingVFX()
    {
        Vector3 movement = DiscreteVfxControl.auraApproachingFinalPosition - DiscreteVfxControl.auraApproachingInitialPosition;
        float movementSteps = DiscreteVfxControl.auraApproachingEndBN - DiscreteVfxControl.auraApproachingStartBN;
        Vector3 movementSpeed = movement / movementSteps;

        if (gameObject.activeSelf) 
        {
            //ps.transform.position += movementSpeed;
            StartCoroutine(IncrementalApproachingVFX(DiscreteVfxControl.increment, movementSpeed));
        }
    }
    private IEnumerator IncrementalApproachingVFX(float increment, Vector3 speed)
    {
        float startTime = Time.time;
        float endTime = startTime + increment;

        while (Time.time < endTime)
        {
            ps.transform.position += (speed/increment) * Time.deltaTime;
            yield return null;
        }
    }
}
