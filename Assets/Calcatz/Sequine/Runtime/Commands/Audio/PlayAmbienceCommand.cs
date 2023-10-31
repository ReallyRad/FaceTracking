
using Calcatz.CookieCutter;
using Calcatz.CookieCutter.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Plays an ambience using the specified Audio Clip, in Ambience channel.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Audio/Play Ambience", typeof(SequineFlowCommandData))]
    public class PlayAmbienceCommand : PlayLoopingSoundCommand {

        public override void Execute(CommandExecutionFlow _flow) {
            var audioClip_value = GetInput<AudioClip>(_flow, IN_PORT_CLIPINPUT, audioClip);
            var restart_value = GetInput<System.Boolean>(_flow, IN_PORT_RESTART, restart);
            var fadeDuration_value = GetInput<System.Single>(_flow, IN_PORT_FADE_DURATION, fadeDuration);
            var volumeMultiplier_value = GetInput<System.Single>(_flow, IN_PORT_VOLUME_MULTIPLIER, volumeMultiplier);

            AudioManager.PlayAmbience(audioClip_value, restart_value, fadeDuration_value, volumeMultiplier_value);

            Exit();
        }


        #region PROPERTIES
        public AudioClip audioClip;
        #endregion PROPERTIES

        #region NODE-DRAWER
#if UNITY_EDITOR
        protected override string tooltip => "Plays an ambience using the specified Audio Clip, in Ambience channel.";

        protected override void OnDrawClipInputField() {
            CommandGUI.DrawObjectField(clipInputLabel, "", audioClip, _audioClip => audioClip = _audioClip, false);
        }

#endif
        #endregion NODE-DRAWER

    }

}