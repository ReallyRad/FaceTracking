using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class InteractiveSequenceable : Sequenceable //this abstract class defines the methods that must be implemented by the vfx so they can be sequenced
{
    [SerializeField] protected float _riseTime;
    [SerializeField] protected float _decayTime;

    [SerializeField] private FaceData _faceData;

    //TODO HACK to handle double triggers coming from FaceTracking events. Find way to have single triggers
    private bool _skipDecayTrigger;
    private bool _skipInteractTrigger;
    
    private void OnEnable()
    {
        FaceTrackingManager.PuckerTrigger += NewFaceExpressionAvailable;
    }

    private void OnDisable()
    {
        FaceTrackingManager.PuckerTrigger += NewFaceExpressionAvailable;
    }
    
    private void NewFaceExpressionAvailable(bool pucker)
    {
        if (_faceData.slightPucker)
        {
            if (!_skipDecayTrigger) //skip first trigger
            {
                Decay();
                _skipDecayTrigger = true;
                _skipInteractTrigger = false;
                Debug.Log("decay");    
            }
            
        }
        else if (_faceData.pucker)
        {
            if (!_skipInteractTrigger) //skip first trigger
            {
                Interact();
                _skipInteractTrigger = true;
                _skipDecayTrigger = false;
                Debug.Log("interact");    
            }
        }
    }

    public abstract void Interact();
    
    public abstract void Decay();
}
