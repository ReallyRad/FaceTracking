using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Resumes an animation (if the action is previously paused) by specifying the animation clip asset.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Animation/Clip Control/Resume Action Animation", typeof(SequineFlowCommandData))]
    public class ResumeActionAnimationCommand : SequineTargetCommand {

        public int actionId;

        public override float nodeWidth => 350f;

        public override void Execute(CommandExecutionFlow _flow) {
            SequinePlayer sequine = GetSequinePlayer(_flow);
            if (sequine.TryGetActionClip(actionId, out var clip)) {
                if (sequine.TryGetState(clip, out var state)) {
                    state.Resume();
                }
            }
            Exit();
        }


#if UNITY_EDITOR
        public override void Editor_OnDrawTitle(out string _tooltip) {
            base.Editor_OnDrawTitle(out _tooltip);
            _tooltip = "Resumes an animation (if the action is previously paused) by specifying the animation clip asset.";
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