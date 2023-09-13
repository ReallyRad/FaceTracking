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

        [Range(-0.05f, 0.05f)]
        [SerializeField] private float _mouthValue; //from -1 to +1

        [SerializeField] private float _puckerThreshold;
        [SerializeField] private float _smileThreshold;

        [SerializeField] private bool _smiling;
        [SerializeField] private bool _slightSmile;
        [SerializeField] private bool _pucker;
        [SerializeField] private bool _slightPucker;

        [SerializeField] private float _smileTimingThreshold;
        [SerializeField] private float _puckerTimingThreshold;

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
        
        [SerializeField] private bool _manualSmileControl;
        [SerializeField] private bool _setSkyboxExposure;
        [SerializeField] private bool _setBlockerTransparency;
        [SerializeField] private Material _occluderMaterial;
        
        [SerializeField] private bool _progressOnBreatheIn;
        [SerializeField] private bool _progressOnBreatheOut;

        private Stopwatch _smileStopwatch;
        private Stopwatch _puckerStopwatch;
        private Stopwatch _progressStopwatch;

        public delegate void OnLevelUp();
        public static OnLevelUp LevelUp;
        private int _previousProgressValue;
        private int _progressValue;
        
        void Start()
        {
            _lipCornerPuller = new Vector2();
            _lipPucker = new Vector2();

            _smileStopwatch = new Stopwatch();
            _puckerStopwatch = new Stopwatch();
            _progressStopwatch = new Stopwatch();
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

            float smileValue = 0f;
            float puckerValue = 0f;
            
            if (!_manualSmileControl)
            {
                smileValue = (_lipCornerPuller.x + _lipCornerPuller.y) / 2;
                puckerValue = (_lipPucker.x + _lipPucker.y) / 2;
                _mouthValue = smileValue - puckerValue;    
            }
            else
            {
                if (_mouthValue < 0)
                {
                    puckerValue = - _mouthValue;
                    smileValue = 0;
                }
                else if (_mouthValue > 0)
                {
                    smileValue = _mouthValue;
                    puckerValue = 0;
                }
            }

            bool wasSlightSmile = _slightSmile;
            bool wasSlightPucker = _slightPucker;
            
            _smiling = smileValue >= _smileThreshold;
            _slightSmile = smileValue < _smileThreshold && _mouthValue > 0;
            _pucker = puckerValue >= _puckerThreshold;
            _slightPucker = puckerValue < _puckerThreshold && _mouthValue < 0;

            _puckerTime = _puckerStopwatch.ElapsedMilliseconds / 1000f;
            _smileTime = _smileStopwatch.ElapsedMilliseconds / 1000f;

            if (_slightSmile) { 
                _smileStopwatch.Stop();
                _progressStopwatch.Stop();
                if (wasSlightPucker) _puckerStopwatch.Reset();
            } 
            else if (_slightPucker) {
                _puckerStopwatch.Stop();
                _progressStopwatch.Stop();
                if (wasSlightSmile) _smileStopwatch.Reset();
            }
            else if (_smiling) {
                _smileStopwatch.Start();
            } else if (_pucker) {
                _puckerStopwatch.Start();
            }

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
            
            _mouthValueSlider.value = _mouthValue;
            _mouthValueText.text = _mouthValue.ToString();

            _smileTimeSlider.value = _smileTime;
            _smileTimeText.text = _smileTime + "s";

            _previousProgressValue = _progressValue;
            _progressValue = (int) _progressStopwatch.ElapsedMilliseconds/1000; 
                
            _progressValueSlider.value = _progressStopwatch.ElapsedMilliseconds/1000;
            _progressValueText.text = (_progressStopwatch.ElapsedMilliseconds/1000).ToString();

            if (_progressValue > _previousProgressValue)
                LevelUp();
            
            _puckerTimeSlider.value = _puckerTime;
            _puckerTimeText.text = _puckerTime + "s";

            if (_setSkyboxExposure)
            {
                float mappedValue = Map(_progressStopwatch.ElapsedMilliseconds, 0f, 35000f, 4f, 0.5f);
                RenderSettings.skybox.SetFloat("_Exposure", mappedValue);
            }

            if (_setBlockerTransparency)
            {
                float mappedValue = Map(_progressStopwatch.ElapsedMilliseconds, 0f, 35000f, 1f, 0f);
                Color color = new Color(1,1,1,mappedValue);
                _occluderMaterial.SetColor("_Color", color);
            }
        }
        
        private float Map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s-a1)*(b2-b1)/(a2-a1);
        }

    }
}