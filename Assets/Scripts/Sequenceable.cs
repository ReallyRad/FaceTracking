using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Sequenceable : MonoBehaviour //this abstract class defines the methods that must be implemented by the vfx so they can be sequenced
{
    public delegate void OnCompleted(Sequenceable item); //triggered to notify the current element is done
    public static OnCompleted Completed;

    [SerializeField] protected bool _active; //whether the item is currently receiving progress updates
    [SerializeField] protected float _initialValue;
    [SerializeField] protected float _finalValue; 
    [SerializeField] protected float _completedProgressAt;

    private void OnEnable()
    {
        ProgressManager.Progress += Progress;
    }

    private void OnDisable()
    {
        ProgressManager.Progress -= Progress;
    }

    protected abstract void Progress(float progress); //The handler for when progress is ongoing

    public abstract void Initialize(); //Initialize the values before starting to interpolate.
}
