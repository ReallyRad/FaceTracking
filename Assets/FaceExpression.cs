using System;
using System.Collections.Generic;
using UnityEngine;

namespace Metaface.Debug
{
    public class FaceExpression : MonoBehaviour
    {
        [SerializeField] private OVRFaceExpressions faceExpressions;
        
        [SerializeField] private float _smileThreshold;
        [SerializeField] private float _breatheOutThreshold;

        [SerializeField] private Vector2 _lipCornerPuller;
        [SerializeField] private Vector2 _lipPucker;
        
    
        void Start()
        {
            _lipCornerPuller = new Vector2();
            _lipPucker = new Vector2();
        }

        void Update()
        {
            float w;
            faceExpressions.TryGetFaceExpressionWeight(OVRFaceExpressions.FaceExpression.LipPuckerL, out w);
            _lipPucker.x = w;
            faceExpressions.TryGetFaceExpressionWeight(OVRFaceExpressions.FaceExpression.LipPuckerR, out w);
            _lipPucker.y = w;
            
            faceExpressions.TryGetFaceExpressionWeight(OVRFaceExpressions.FaceExpression.LipCornerPullerL, out w);
            _lipCornerPuller.x = w;
            faceExpressions.TryGetFaceExpressionWeight(OVRFaceExpressions.FaceExpression.LipCornerPullerR, out w);
            _lipCornerPuller.y = w;
            
            
        }

    }
}