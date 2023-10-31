using Calcatz.CookieCutter;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Base class of commands that play a one shot audio clip.
    /// </summary>
    [System.Serializable]
    public class PlayOneShotSoundCommand : Command {

        #region PROPERTIES
        public override float nodeWidth => 275f;

        protected const int IN_PORT_CLIPINPUT = 0;
        protected const int IN_PORT_VOLUME_MULTIPLIER = 1;
        protected const int IN_PORT_POSITION = 2;

        protected const int OUT_PORT_ONCOMPLETE = 1;


        public float volumeMultiplier = 1f;
        public bool playAtPosition;
        public Vector3 position;
        #endregion PROPERTIES

        #region NODE-DRAWER
#if UNITY_EDITOR
        protected virtual System.Type clipInputType => typeof(AudioClip);
        protected virtual string clipInputLabel => "Audio Clip";
        protected virtual string tooltip => "";

        public override void Editor_InitInPoints() {
            CommandGUI.AddMainInPoint();
            if (inputIds.Count <= IN_PORT_CLIPINPUT) {
                inputIds.Add(new ConnectionTarget());
            }
            if (inputIds.Count <= IN_PORT_VOLUME_MULTIPLIER) {
                inputIds.Add(new ConnectionTarget());
            }
            if (inputIds.Count <= IN_PORT_POSITION) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint(clipInputType);
            CommandGUI.AddPropertyInPoint<System.Single>();
            CommandGUI.AddPropertyInPoint<UnityEngine.Vector3>();
        }

        public override void Editor_InitOutPoints() {
            CommandGUI.AddMainOutPoint();
            if (nextIds.Count <= OUT_PORT_ONCOMPLETE) {
                nextIds.Add(new System.Collections.Generic.List<ConnectionTarget>());
            }
            CommandGUI.AddMainOutPoint();
        }

        public override void Editor_OnDrawTitle(out string _tooltip) {
            _tooltip = tooltip;
            CommandGUI.DrawInPoint(0);
            CommandGUI.DrawOutPoint(0);
        }
                
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            CommandGUI.DrawInPoint(IN_PORT_CLIPINPUT + 1);
            if (inputIds[IN_PORT_CLIPINPUT].targetId == 0) {
                OnDrawClipInputField();
            }
            else {
                CommandGUI.DrawLabel(clipInputLabel);
            }
            CommandGUI.DrawInPoint(IN_PORT_VOLUME_MULTIPLIER + 1);
            if (inputIds[IN_PORT_VOLUME_MULTIPLIER].targetId == 0) {                    CommandGUI.DrawFloatSliderField("Volume Multiplier", "", ref volumeMultiplier, 0f, 1f);
            }
            else {
                CommandGUI.DrawLabel("Volume Multiplier");
            }            CommandGUI.DrawToggleLeftField("Play At Position", "", ref playAtPosition);

            bool guiEnabled = GUI.enabled;
            GUI.enabled = guiEnabled && playAtPosition;
            CommandGUI.DrawInPoint(IN_PORT_POSITION + 1);
            if (inputIds[IN_PORT_POSITION].targetId == 0) {                    CommandGUI.DrawVector3Field("Position", "", ref position);
            }
            else {
                CommandGUI.DrawLabel("Position");
            }
            GUI.enabled = guiEnabled;

            CommandGUI.AddRectHeight(5);
            CommandGUI.DrawOutPoint(OUT_PORT_ONCOMPLETE);
            CommandGUI.DrawLabel("On Complete", true);
        }

        protected virtual void OnDrawClipInputField() {

        }
                
#endif
        #endregion NODE-DRAWER

    }

}