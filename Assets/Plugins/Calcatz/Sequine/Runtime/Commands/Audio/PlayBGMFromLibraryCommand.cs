
using Calcatz.CookieCutter;
using Calcatz.CookieCutter.Audio;
using System;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Plays a music from BGM Library in BGM channel.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Audio/Play BGM From Library", typeof(SequineFlowCommandData))]
    public class PlayBGMFromLibraryCommand : PlayLoopingSoundCommand {

        public override void Execute(CommandExecutionFlow _flow) { 
            var soundId_value = GetInput(_flow, IN_PORT_CLIPINPUT, soundId);
            var restart_value = GetInput(_flow, IN_PORT_RESTART, restart);
            var fadeDuration_value = GetInput(_flow, IN_PORT_FADE_DURATION, fadeDuration);
            var volumeMultiplier_value = GetInput(_flow, IN_PORT_VOLUME_MULTIPLIER, volumeMultiplier);

            AudioManager.PlayBGM(soundId_value, restart_value, fadeDuration_value, volumeMultiplier_value);

            Exit();
        }


        #region PROPERTIES
        public string soundId;
        #endregion PROPERTIES

        #region NODE-DRAWER
#if UNITY_EDITOR
        protected override Type clipInputType => typeof(string);
        protected override string clipInputLabel => "Sound ID";
        protected override string tooltip => "Plays a music from BGM Library in BGM channel.";

        protected override void OnDrawClipInputField() {
            CommandGUI.GetSingleLineLabeledRect(out var labelRect, out var buttonRect);
            CommandGUI.DrawLabel(labelRect, clipInputLabel);
            if (GUI.Button(buttonRect, soundId, UnityEditor.EditorStyles.popup)) {
                var index = -1;
                var allIds = AudioConfig.GetOrCreateSettings().m_bgmLibrary.GetAllIDs();
                var contextMenu = new UnityEditor.GenericMenu();
                for (int i = 0; i < allIds.Length; i++) {
                    if (allIds[i] == soundId) {
                        index = i;
                    }
                    string newId = allIds[i];
                    contextMenu.AddItem(new GUIContent(newId), index == i, () => {
                        UnityEditor.Undo.RecordObject(CommandGUI.currentTargetObject, "change sound ID");
                        soundId = newId;
                        UnityEditor.EditorUtility.SetDirty(CommandGUI.currentTargetObject);
                    });
                }
                contextMenu.DropDown(buttonRect);
            }
        }

#endif
        #endregion NODE-DRAWER

    }

}