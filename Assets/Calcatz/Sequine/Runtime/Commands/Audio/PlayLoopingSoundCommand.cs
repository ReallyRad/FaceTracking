
using Calcatz.CookieCutter;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Base class of commands that play a looping audio clip.
    /// </summary>
    [System.Serializable]
    public class PlayLoopingSoundCommand : Command {

        #region PROPERTIES
        public override float nodeWidth => 275f;

        protected const int IN_PORT_CLIPINPUT = 0;
        protected const int IN_PORT_RESTART = 1;
        protected const int IN_PORT_FADE_DURATION = 2;
        protected const int IN_PORT_VOLUME_MULTIPLIER = 3;


        public System.Boolean restart;
        public System.Single fadeDuration = 1f;
        public System.Single volumeMultiplier = 1f;

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
            if (inputIds.Count <= IN_PORT_RESTART) {
                inputIds.Add(new ConnectionTarget());
            }
            if (inputIds.Count <= IN_PORT_FADE_DURATION) {
                inputIds.Add(new ConnectionTarget());
            }
            if (inputIds.Count <= IN_PORT_VOLUME_MULTIPLIER) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint(clipInputType);
            CommandGUI.AddPropertyInPoint<System.Boolean>();
            CommandGUI.AddPropertyInPoint<System.Single>();
            CommandGUI.AddPropertyInPoint<System.Single>();
        }

        public override void Editor_InitOutPoints() {
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
                CommandGUI.DrawLabel("Audio Clip");
            }
            CommandGUI.DrawInPoint(IN_PORT_RESTART + 1);
            if (inputIds[IN_PORT_RESTART].targetId == 0) {                    CommandGUI.DrawToggleField("Restart", "", ref restart);
            }
            else {
                CommandGUI.DrawLabel("Restart");
            }

            CommandGUI.DrawInPoint(IN_PORT_FADE_DURATION + 1);
            if (inputIds[IN_PORT_FADE_DURATION].targetId == 0) {                    CommandGUI.DrawFloatField("Fade Duration", "Crossfade duration in seconds", ref fadeDuration, 0f);
            }
            else {
                CommandGUI.DrawLabel("Fade Duration");
            }

            CommandGUI.DrawInPoint(IN_PORT_VOLUME_MULTIPLIER + 1);
            if (inputIds[IN_PORT_VOLUME_MULTIPLIER].targetId == 0) {                    CommandGUI.DrawFloatSliderField("Volume Multiplier", "", ref volumeMultiplier, 0f, 1f);
            }
            else {
                CommandGUI.DrawLabel("Volume Multiplier");
            }
        }

        protected virtual void OnDrawClipInputField() {

        }

#endif
        #endregion NODE-DRAWER

    }

}