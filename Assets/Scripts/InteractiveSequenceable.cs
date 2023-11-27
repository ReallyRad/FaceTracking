using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class InteractiveSequenceable : Sequenceable //this abstract class defines the methods that must be implemented by the vfx so they can be sequenced
{
    [SerializeField] protected float _riseTime;
    [SerializeField] protected float _decayTime;

    private void OnEnable()
    {
        FaceTrackingManager.PuckerTrigger += NewFaceExpressionAvailable;
        //TODO add Progress handler to trigger OnCOmplete()
    }

    private void OnDisable()
    {
        FaceTrackingManager.PuckerTrigger += NewFaceExpressionAvailable;
    }
    
    private void NewFaceExpressionAvailable(bool pucker)
    {
        if (!pucker && _active)
        {
            Decay();
            Debug.Log("decay");    
        }
        else if (pucker && _active)
        {
            Interact();
            Debug.Log("interact");    
        }
    }

    public abstract void Interact();
    
    public abstract void Decay();
}
