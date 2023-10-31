using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter.Audio {
    /// <summary>
    /// A component that requires an Audio Source, that makes it listen to the Audio Manager, to update the volume when it's affected.
    /// </summary>
	[AddComponentMenu("CookieCutter/Audio/Audio Source Auto Adjust Volume")]
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceAutoAdjustVolume : MonoBehaviour {

        [SerializeField][HideInInspector] private AudioSource audioSource;
        private float initialVolume;

        private bool initialized;

        private void OnEnable() {
            if (AudioManager.Instance != null) {
                if (initialized) return;
                initialized = true;
                OnValidate();
                initialVolume = audioSource.volume;
                AudioManager.AddAudioSourceInstance(this);
            }
        }

        private void OnValidate() {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
        }

        private void OnDestroy() {
            if (!AudioManager.IsDestroyed()) {
                AudioManager.RemoveAudioSourceInstance(this);
            }
        }

        public void SetVolume(float _masteredVolume) {
            audioSource.volume = initialVolume * _masteredVolume;
        }

        public void FadeStop(float _duration = 1) {
            if (fadeCoroutine != null) {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeStopCoroutine(_duration));
        }

        private Coroutine fadeCoroutine;
        private IEnumerator FadeStopCoroutine(float _duration = 1) {
            float currentDuration = 0;
            float volumeCap = audioSource.volume;
            while (currentDuration < _duration) {
                currentDuration += Time.deltaTime;
                audioSource.volume = (_duration - currentDuration) / _duration * volumeCap;
                yield return null;
            }
            audioSource.Stop();
        }
    }
}
