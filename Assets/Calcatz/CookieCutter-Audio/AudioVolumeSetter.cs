using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter.Audio {
    /// <summary>
    /// An audio volume setter in a form of MonoBehaviour component as a helper so that it can be called through UnityEvent set from inspector.
    /// </summary>
	[AddComponentMenu("CookieCutter/Audio/Audio Volume Setter")]
    public class AudioVolumeSetter : MonoBehaviour {

        public void ChangeMasterVolume(float _volume) {
            AudioManager.SetVolume(_volume, AudioManager.Channel.Master);
        }

        public void ChangeBGMVolume(float _volume) {
            AudioManager.SetVolume(_volume, AudioManager.Channel.BGM);
        }

        public void ChangeAmbienceVolume(float _volume) {
            AudioManager.SetVolume(_volume, AudioManager.Channel.Ambience);
        }

        public void ChangeMEVolume(float _volume) {
            AudioManager.SetVolume(_volume, AudioManager.Channel.ME);
        }

        public void ChangeSFXVolume(float _volume) {
            AudioManager.SetVolume(_volume, AudioManager.Channel.SFX);
        }

    }
}
