using System;
using System.Collections;
using System.Diagnostics;
using Oculus.Movement;
using Oculus.Platform;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Metaface.Debug
{
    public class FaceTrackingDebuggerUI : MonoBehaviour
    {
        [SerializeField] private Slider _mouthValueSlider;
        [SerializeField] private TMP_Text _mouthValueText;
        
        [SerializeField] private Slider _smileTimeSlider;
        [SerializeField] private TMP_Text _smileTimeText;

        [SerializeField] private Slider _progressValueSlider;
        [SerializeField] private TMP_Text _progressValueText;
        
        [SerializeField] private Slider _puckerTimeSlider;
        [SerializeField] private TMP_Text _puckerTimeText;

        [SerializeField] private bool _progressOnBreatheIn;
        [SerializeField] private bool _progressOnBreatheOut;

        [SerializeField] private float _smileTimingThreshold;
        [SerializeField] private float _puckerTimingThreshold;
        
        private Stopwatch _smileStopwatch;
        private Stopwatch _puckerStopwatch;
        private Stopwatch _progressStopwatch;

        [SerializeField] private FaceData _faceExpression;
        
        private void OnEnable()
        {
            FaceTrackingManager.MouthValue += SetMouthValue;
            //FaceTrackingManager.PuckerTrigger += NewFaceExpressionAvailable;
        }

        private void OnDisable()
        {
            FaceTrackingManager.MouthValue -= SetMouthValue;
            //FaceTrackingManager.PuckerTrigger -= NewFaceExpressionAvailable;
        }
        
        private void Start()
        {
            _smileStopwatch = new Stopwatch();
            _puckerStopwatch = new Stopwatch();
            _progressStopwatch = new Stopwatch();
        }
        
        private void Update()
        {
            _puckerTimeSlider.value = (float) _puckerStopwatch.ElapsedMilliseconds/1000;
            _puckerTimeText.text = _puckerStopwatch.ElapsedMilliseconds/1000 + "s";

            _smileTimeSlider.value = (float) _smileStopwatch.ElapsedMilliseconds/1000;
            _smileTimeText.text = _smileStopwatch.ElapsedMilliseconds / 1000 +"s";

            _progressValueSlider.value = _progressStopwatch.ElapsedMilliseconds/1000;
            _progressValueText.text = _progressStopwatch.ElapsedMilliseconds / 1000 + "s";
            
            if ((_puckerStopwatch.ElapsedMilliseconds > _smileTimingThreshold ||
                 _smileStopwatch.ElapsedMilliseconds > _puckerTimingThreshold) &&
                !_faceExpression.slightPucker &&
                !_faceExpression.slightSmile)
            {
                if (_progressOnBreatheIn && _faceExpression.smiling || _progressOnBreatheOut && _faceExpression.pucker)
                {
                    _progressStopwatch.Start();
                }
            }
        }

        private void SetMouthValue(float mouthValue)
        {
            _mouthValueSlider.value = mouthValue;
            _mouthValueText.text = mouthValue.ToString();
        }

        private void NewFaceExpressionAvailable()
        {
            if (_faceExpression.smiling) Smile();
            else if (_faceExpression.slightSmile) SlightSmile();
            else if (_faceExpression.slightPucker) SlightPucker();
            else if (_faceExpression.pucker) Pucker();
        }
        
        private void Pucker()
        {
            _puckerStopwatch.Start();
            StartCoroutine(BreathVibration());
        }
        
        private void Smile()
        {
            _smileStopwatch.Start();
        }

        private void SlightPucker()
        {
            OVRInput.SetControllerVibration(0, 0f, OVRInput.Controller.RTouch);
            //UnityEngine.Debug.Log("stop breath vibration");
            StopCoroutine("BreathVibration");
            _puckerStopwatch.Stop();
            _progressStopwatch.Stop();
            if (_faceExpression.previouslySlightSmile) _smileStopwatch.Reset(); //only reset stopwatch once we passed 0
        }
        
        private void SlightSmile()
        {
            _smileStopwatch.Stop();
            _progressStopwatch.Stop();
            if (_faceExpression.previouslySlightPucker) _puckerStopwatch.Reset(); //only reset stopwatch once we passed 0
        }

        private IEnumerator BreathVibration()
        {
            OVRInput.SetControllerVibration(1, 0.2f, OVRInput.Controller.RTouch);
            yield return new WaitForSeconds(2f);
            if (_faceExpression.pucker) StartCoroutine(BreathVibration());
        }
    }
}