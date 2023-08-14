using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Metaface.Debug
{
    public class FaceExpression : MonoBehaviour
    {
        [SerializeField] private OVRFaceExpressions faceExpressions;
        
        [SerializeField] private Vector2 _lipCornerPuller;
        [SerializeField] private Vector2 _lipPucker;

        [SerializeField] private float _mouthValue; //from -1 to +1

        [SerializeField] private float _puckerThreshold;
        [SerializeField] private float _smileThreshold;

        [SerializeField] private bool _smiling;
        [SerializeField] private bool _neutral;
        [SerializeField] private bool _pucker;

        [SerializeField] private float _smileTimingThreshold;
        [SerializeField] private float _puckerTimingThreshold;

        [SerializeField] private float _progress;

        [SerializeField] private float _smileTime;
        [SerializeField] private float _puckerTime;
        
        private Stopwatch _smileStopwatch;
        private Stopwatch _puckerStopwatch;
        
        void Start()
        {
            _lipCornerPuller = new Vector2();
            _lipPucker = new Vector2();

            _smileStopwatch = new Stopwatch();
            _puckerStopwatch = new Stopwatch();
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

            float smileValue = (_lipCornerPuller.x + _lipCornerPuller.y) / 2;
            float puckerValue = (_lipPucker.x + _lipPucker.y) / 2;
            _mouthValue = smileValue - puckerValue;

            bool wasSmiling = _smiling;
            _smiling = smileValue > _smileThreshold;
            if (!wasSmiling && _smiling) _puckerStopwatch.Restart();
            
            bool wasPucker = _pucker;
            _pucker = puckerValue > _puckerThreshold;
            if(!wasPucker && _pucker) _smileStopwatch.Restart();

            _puckerTime = _puckerStopwatch.ElapsedMilliseconds / 1000;
            _smileTime = _smileStopwatch.ElapsedMilliseconds / 1000;
            
            bool wasNeutral = _neutral;
            _neutral = smileValue < _smileThreshold && puckerValue < _puckerThreshold;
            if(!wasNeutral && _neutral) {
                _puckerStopwatch.Stop();
                _smileStopwatch.Stop();
                if (wasPucker && 
                    !_pucker && 
                    _puckerStopwatch.ElapsedMilliseconds > _puckerTimingThreshold &&
                    _smileStopwatch.ElapsedMilliseconds > _smileTimingThreshold)
                    _progress++;
            }
  
            //if (_smileStopwatch.ElapsedMilliseconds == _smileTimingThreshold) 
                
        }

    }
}