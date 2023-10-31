using Calcatz.CookieCutter;
using Calcatz.CookieCutter.Audio;
using System;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Plays a sound from SFX library in SFX channel.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Audio/Play SFX From Library", typeof(SequineFlowCommandData))]
    public class PlaySFXFromLibraryCommand : PlayOneShotSoundCommand {

        public override void Execute(CommandExecutionFlow _flow) { 
            var soundId_value = GetInput(_flow, IN_PORT_CLIPINPUT, soundId);
            var volumeMultiplier_value = GetInput(_flow, IN_PORT_VOLUME_MULTIPLIER, volumeMultiplier);

            if (playAtPosition) {
                var position_value = GetInput(_flow, IN_PORT_POSITION, position);
                AudioManager.PlaySFXAtPosition(soundId_value, position_value, ()=> {
                    RunSubFlow(_flow, OUT_PORT_ONCOMPLETE);
                }, volumeMultiplier_value);
            }
            else {
                AudioManager.PlaySFX2D(soundId_value, () => {
                    RunSubFlow(_flow, OUT_PORT_ONCOMPLETE);
                }, volumeMultiplier_value);
            }

            Exit();
        }


        #region PROPERTIES
        public string soundId;

        #endregion PROPERTIES

        #region NODE-DRAWER
#if UNITY_EDITOR
        protected override Type clipInputType => typeof(string);
        protected override string clipInputLabel => "Sound ID";
        protected override string tooltip => "Plays a sound from SFX library in SFX channel.";

        protected override void OnDrawClipInputField() {
            CommandGUI.GetSingleLineLabeledRect(out var labelRect, out var buttonRect);
            CommandGUI.DrawLabel(labelRect, clipInputLabel);
            if (GUI.Button(buttonRect, soundId, UnityEditor.EditorStyles.popup)) {
                var index = -1;
                var allIds = AudioConfig.GetOrCreateSettings().m_sfxLibrary.GetAllIDs();
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