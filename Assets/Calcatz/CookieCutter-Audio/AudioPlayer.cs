using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter.Audio {
    /// <summary>
    /// An audio player in a form of MonoBehaviour component as a helper so that audio can be played through UnityEvent set from inspector.
    /// </summary>
	[AddComponentMenu("CookieCutter/Audio/Audio Player")]
    public class AudioPlayer : MonoBehaviour {

        [Tooltip("Restart the BGM/Ambience if the clip has already been played.")]
        public bool restart = true;

        public void PlaySound2D(AudioClip _clip) {
            AudioManager.PlaySound2D(_clip);
        }

        public void PlaySoundAtPosition(AudioClip _clip, Vector3 _position) {
            AudioManager.PlaySoundAtPosition(_clip, _position);
        }
        
        public void PlaySFX2D(string _id) {
            //Debug.Log(name + " -> " + _id);
            AudioManager.PlaySFX2D(_id);
        }

        public void PlaySFXAtPosition(string _id, Vector3 _position) {
            AudioManager.PlaySFXAtPosition(_id, _position);
        }

        public void PlayME2D(string _id) {
            AudioManager.PlayME2D(_id);
        }

        public void PlayMEAtPosition(string _id, Vector3 _position) {
            AudioManager.PlayMEAtPosition(_id, _position);
        }

        public void PlayBGM(string _id) {
            AudioManager.PlayBGM(_id, restart);
        }

        public void PlayBGM(string _id, float _fadeDuration) {
            AudioManager.PlayBGM(_id, restart, _fadeDuration);
        }

        public void PlayAmbience(string _id) {
            AudioManager.PlayAmbience(_id, restart);
        }

        public void PlayAmbience(string _id, float _fadeDuration) {
            AudioManager.PlayAmbience(_id, restart, _fadeDuration);
        }

        public void StopBGM() {
            AudioManager.StopBGM();
        }

        public void StopAmbience() {
            AudioManager.StopAmbience();
        }

    }
}
