using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField] private Slider _mouthValueSlider;
        [SerializeField] private TMP_Text _mouthValueText;
        
        [SerializeField] private Slider _smileTimeSlider;
        [SerializeField] private TMP_Text _smileTimeText;

        [SerializeField] private Slider _progressValueSlider;
        [SerializeField] private TMP_Text _progressValueText;
        
        [SerializeField] private Slider _puckerTimeSlider;
        [SerializeField] private TMP_Text _puckerTimeText;
        
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

            _smiling = smileValue > _smileThreshold; 
            _pucker = puckerValue > _puckerThreshold;

            _puckerTime = _puckerStopwatch.ElapsedMilliseconds / 1000f;
            _smileTime = _smileStopwatch.ElapsedMilliseconds / 1000f;
            
            bool wasNeutral = _neutral;
            _neutral = smileValue < _smileThreshold && puckerValue < _puckerThreshold;
            
            if (!wasNeutral && _neutral) {
                _puckerStopwatch.Stop();
                _smileStopwatch.Stop();
            }

            _mouthValueSlider.value = _mouthValue;
            _mouthValueText.text = _mouthValue.ToString();

            _smileTimeSlider.value = _smileTime;
            _smileTimeText.text = _smileTime + "s";

            _progressValueSlider.value = _progress;
            _progressValueText.text = _progress.ToString();

            _puckerTimeSlider.value = _puckerTime;
            _puckerTimeText.text = _puckerTime + "s";
        }

    }
}