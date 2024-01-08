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
    public abstract void Initialize(); //Initialize the values before starting to interpolate.
    [SerializeField] protected float _overlapTime; //the number of full breaths at which we start transitioning to the next sequence phase
    
}
    