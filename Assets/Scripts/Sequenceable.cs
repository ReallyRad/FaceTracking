using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sequenceable : MonoBehaviour
{
    public delegate void OnStartNextPhase(Sequenceable item); //triggered to notify the next sequence phase should start fading inx
    public static OnStartNextPhase StartNextPhase;
    
    [SerializeField] protected bool _active; //whether the item is currently receiving progress updates
    [SerializeField] protected bool _transitioning;

    [SerializeField] protected float _startNextPhaseAt; //number of full breaths necessary to complete a sequence phase
    [SerializeField] protected float _completedAt; //the number of full breaths at which we start transitioning to the next sequence phase

    [SerializeField] protected float _localProgress;

    public abstract void Initialize(); //Initialize the values before starting to interpolate.
    
}
    