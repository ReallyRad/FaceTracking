using System;
using System.Diagnostics;
using Oculus.Movement;
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

        private void OnEnable()
        {
            FaceTrackingManager.MouthValue += SetMouthValue;
            FaceTrackingManager.ProgressValue += SetProgressValue;
            FaceTrackingManager.PuckerTime += SetPuckerTime;
            FaceTrackingManager.SmileTime += SetSmileTime;
        }

        private void OnDisable()
        {
            FaceTrackingManager.MouthValue -= SetMouthValue;
            FaceTrackingManager.ProgressValue -= SetProgressValue;
            FaceTrackingManager.PuckerTime -= SetPuckerTime;
            FaceTrackingManager.SmileTime -= SetSmileTime;
        }

        private void SetMouthValue(float mouthValue)
        {
            _mouthValueSlider.value = mouthValue;
            _mouthValueText.text = mouthValue.ToString();
        }

        private void SetProgressValue(float progressValue)
        {
            _progressValueSlider.value = progressValue;
            _progressValueText.text = (progressValue).ToString();
        }

        private void SetPuckerTime(float puckerTime)
        {
            _puckerTimeSlider.value = puckerTime;
            _puckerTimeText.text = puckerTime + "s";
        }
        
        private void SetSmileTime(float smileTime)
        {
            _smileTimeSlider.value = smileTime;
            _smileTimeText.text = smileTime + "s";
        }
    }
}