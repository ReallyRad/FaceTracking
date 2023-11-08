using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sequenceable : MonoBehaviour //this abstract class defines the methods that must be implemented by the vfx so they can be sequenced
{
    public bool active; //whether the item is currently receiving progress updates
    [SerializeField] protected float _maxProgress;

    public delegate void OnCompleted(Sequenceable item); //triggered to notify the current element is done
    public OnCompleted Completed;

    public void OnEnable()
    {
        ProgressManager.Progress += Progress;
    }

    public void OnDisable()
    {
        ProgressManager.Progress -= Progress;
    }

    protected abstract void Progress(float progress); //The handler for when progress is ongoing

    public abstract void Initialize(); //Initialize the values before starting to interpolate.
}
