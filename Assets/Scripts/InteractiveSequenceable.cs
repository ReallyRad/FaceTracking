using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class InteractiveSequenceable : Sequenceable //this abstract class defines the methods that must be implemented by the vfx so they can be sequenced
{
   
    [SerializeField] private FaceData _faceData;
    
    private void OnEnable()
    {
        FaceTrackingManager.FaceExpression += NewFaceExpressionAvailable;
    }

    private void OnDisable()
    {
        FaceTrackingManager.FaceExpression += NewFaceExpressionAvailable;
    }

    
    private void NewFaceExpressionAvailable()
    {
        if (_faceData.slightSmile)
        {
            //reset
        }
        else if (_faceData.slightPucker)
        {
            //stop
        }
        else if (_faceData.pucker)
        {
            //start
        }
    }
    
}
