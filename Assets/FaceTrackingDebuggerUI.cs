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

        private bool _wasSlightSmile; //to differentiate between "stopped smiling and "stopped slight pucker"
        private bool _wasSlightPucker; //to differentiate between "stopped pucker" and "stopped slight smile"

        //TODO replace with data structure holding all 4?
        [SerializeField] private bool _smiling;
        [SerializeField] private bool _slightSmile;
        [SerializeField] private bool _pucker;
        [SerializeField] private bool _slightPucker;
        
        private void Start()
        {
            _smileStopwatch = new Stopwatch();
            _puckerStopwatch = new Stopwatch();
            _progressStopwatch = new Stopwatch();
            UnityEngine.Debug.Log("beotch");
        }
        
        private void OnEnable()
        {
            FaceTrackingManager.MouthValue += SetMouthValue;
            FaceTrackingManager.Pucker += Pucker;
            FaceTrackingManager.SlightPucker += SlightPucker;
            FaceTrackingManager.SlightSmile += SlightSmile;
            FaceTrackingManager.Smile += Smile;
        }

        private void OnDisable()
        {
            FaceTrackingManager.MouthValue -= SetMouthValue;
            FaceTrackingManager.Pucker -= Pucker;
            FaceTrackingManager.SlightPucker -= SlightPucker;
            FaceTrackingManager.SlightSmile -= SlightSmile;
            FaceTrackingManager.Smile -= Smile;
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
                !_slightPucker &&
                !_slightSmile)
            {
                if (_progressOnBreatheIn && _smiling || _progressOnBreatheOut && _pucker)
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

        private void Pucker()
        {
            _pucker = true;
            _puckerStopwatch.Start();
            StartCoroutine(BreathVibration());
            _wasSlightPucker = false;
        }
        
        private void Smile()
        {
            _smiling = true;
            _smileStopwatch.Start();
            _wasSlightSmile = false;
        }

        private void SlightPucker()
        {
            _pucker = false;
            OVRInput.SetControllerVibration(0, 0f, OVRInput.Controller.RTouch);
            UnityEngine.Debug.Log("stop breath vibration");
            StopCoroutine("BreathVibration");
            _puckerStopwatch.Stop();
            _progressStopwatch.Stop();
            _wasSlightPucker = true;
            if (_wasSlightSmile) _smileStopwatch.Reset(); //only reset stopwatch once we passed 0
        }
        
        private void SlightSmile()
        {
            _smiling = false;
            _smileStopwatch.Stop();
            _progressStopwatch.Stop();
            _wasSlightSmile = true;
            if (_wasSlightPucker) _puckerStopwatch.Reset(); //only reset stopwatch once we passed 0
        }

        private IEnumerator BreathVibration()
        {
            UnityEngine.Debug.Log("breath vibes");
            OVRInput.SetControllerVibration(1, 0.2f, OVRInput.Controller.RTouch);
            yield return new WaitForSeconds(2f);
            if (_pucker) StartCoroutine(BreathVibration());
        }
    }
}