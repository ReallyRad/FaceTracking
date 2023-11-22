using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sequenceable : MonoBehaviour
{
    public delegate void OnCompleted(Sequenceable item); //triggered to notify the current element is done
    public static OnCompleted Completed;
    
    [SerializeField] protected bool _active; //whether the item is currently receiving progress updates
    [SerializeField] protected float _completedProgressAt;

    public abstract void Initialize(); //Initialize the values before starting to interpolate.
}
    