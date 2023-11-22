using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ProgressiveSequenceable : Sequenceable //this abstract class defines the methods that must be implemented by the vfx so they can be sequenced
{
    [SerializeField] protected float _initialValue;
    [SerializeField] protected float _finalValue; 

    private void OnEnable()
    {
        ProgressManager.Progress += Progress;
    }

    private void OnDisable()
    {
        ProgressManager.Progress -= Progress;
    }

    protected abstract void Progress(float progress); //The handler for when progress is ongoing
    
}
