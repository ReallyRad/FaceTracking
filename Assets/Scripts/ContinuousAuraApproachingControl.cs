using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousAuraApproachingControl : MonoBehaviour
{
    private ParticleSystem ps;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        ContinuousBreathingControl.NewExhalingLongerThanMinimum += WholeApproachingVFX;
    }
    private void OnDisable()
    {
        ContinuousBreathingControl.NewExhalingLongerThanMinimum -= WholeApproachingVFX;
    }
    private void WholeApproachingVFX(float fraction)
    {
        Vector3 movement = ContinuousVfxControl.auraApproachingFinalPosition - ContinuousVfxControl.auraApproachingInitialPosition;
        Vector3 maxIncrementalMovement = movement / ContinuousVfxControl.auraApproachingNumberOfExhalesIfCompleteToFinish;
        Vector3 currentIncrementalMovement = fraction * maxIncrementalMovement;

        if (Vector3.Distance(ps.transform.position, ContinuousVfxControl.auraApproachingFinalPosition) > 0.05f) 
        {
            ps.transform.position += currentIncrementalMovement;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
