using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Pauses an animation (if the action is currently playing) by specifying the action animation.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Animation/Clip Control/Pause Action Animation", typeof(SequineFlowCommandData))]
    public class PauseActionAnimationCommand : SequineTargetCommand {

        public int actionId;

        public override float nodeWidth => 350f;

        public override void Execute(CommandExecutionFlow _flow) {
            SequinePlayer sequine = GetSequinePlayer(_flow);
            if (sequine.TryGetActionClip(actionId, out var clip)) {
                if (sequine.TryGetState(clip, out var state)) {
                    state.Pause();
                }
            }
            Exit();
        }


#if UNITY_EDITOR
        public override void Editor_OnDrawTitle(out string _tooltip) {
            base.Editor_OnDrawTitle(out _tooltip);
            _tooltip = "Pauses an animation (if the action is currently playing) by specifying the action animation.";
        }
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            base.Editor_OnDrawContents(_absPosition);
            var labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
            UnityEditor.EditorGUIUtility.labelWidth = 100;
            DrawActionsField(actionId, OnActionIDChanged);
            UnityEditor.EditorGUIUtility.labelWidth = labelWidth;
        }

        private void OnActionIDChanged(int _actionId) {
            actionId = _actionId;
        }
#endif

    }

}