
using Calcatz.CookieCutter;
using Calcatz.CookieCutter.Audio;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Plays a music using the specified Audio Clip, in BGM channel.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Audio/Play BGM", typeof(SequineFlowCommandData))]
    public class BGMControlCommand : PlayLoopingSoundCommand {

        public override void Execute(CommandExecutionFlow _flow) {
            var audioClip_value = GetInput<AudioClip>(_flow, IN_PORT_CLIPINPUT, audioClip);
            var restart_value = GetInput<System.Boolean>(_flow, IN_PORT_RESTART, restart);
            var fadeDuration_value = GetInput<System.Single>(_flow, IN_PORT_FADE_DURATION, fadeDuration);
            var volumeMultiplier_value = GetInput<System.Single>(_flow, IN_PORT_VOLUME_MULTIPLIER, volumeMultiplier);

            AudioManager.PlayBGM(audioClip_value, restart_value, fadeDuration_value, volumeMultiplier_value);

            Exit();
        }


        #region PROPERTIES
        public AudioClip audioClip;
        #endregion PROPERTIES

        #region NODE-DRAWER
#if UNITY_EDITOR
        protected override string tooltip => "Plays a music using the specified Audio Clip, in BGM channel.";

        protected override void OnDrawClipInputField() {
            CommandGUI.DrawObjectField(clipInputLabel, "", audioClip, _audioClip => audioClip = _audioClip, false);
        }

#endif
        #endregion NODE-DRAWER

    }

}