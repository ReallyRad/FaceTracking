using Calcatz.CookieCutter;
using Calcatz.CookieCutter.Audio;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Plays an audio clip directly in Master channel.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Audio/Play Audio Clip", typeof(SequineFlowCommandData))]
    public class PlayAudioClipCommand : PlayOneShotSoundCommand {

        public override void Execute(CommandExecutionFlow _flow) { 
            var audioClip_value = GetInput(_flow, IN_PORT_CLIPINPUT, audioClip);
            var volumeMultiplier_value = GetInput(_flow, IN_PORT_VOLUME_MULTIPLIER, volumeMultiplier);

            if (playAtPosition) {
                var position_value = GetInput(_flow, IN_PORT_POSITION, position);
                AudioManager.PlaySoundAtPosition(audioClip, position_value, volumeMultiplier_value * AudioConfig.masterVolume, ()=> {
                    RunSubFlow(_flow, OUT_PORT_ONCOMPLETE);
                });
            }
            else {
                AudioManager.PlaySound2D(audioClip, volumeMultiplier_value * AudioConfig.masterVolume, () => {
                    RunSubFlow(_flow, OUT_PORT_ONCOMPLETE);
                });
            }

            Exit();
        }


        #region PROPERTIES
        public override float nodeWidth => 275f;

        public AudioClip audioClip;
        #endregion PROPERTIES

        #region NODE-DRAWER
#if UNITY_EDITOR
        protected override string tooltip => "Plays an audio clip directly in Master channel.";

        protected override void OnDrawClipInputField() {
            CommandGUI.DrawObjectField("Audio Clip", "", audioClip, _audioClip => audioClip = _audioClip, true);
        }
#endif
        #endregion NODE-DRAWER

    }

}