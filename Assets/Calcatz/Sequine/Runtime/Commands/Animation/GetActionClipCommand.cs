using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Calcatz.Sequine;
using Calcatz.CookieCutter;

namespace Calcatz.Sequine {

    /// <summary>
    /// Gets the actual animation clip reference of a Sequine Animation Data by specifying the action.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Animation/Get Action Clip", typeof(SequineFlowCommandData))]
    public class GetActionClipCommand : SequineTargetPropertyCommand {

        public int actionId;

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            SequinePlayer sequine = GetSequinePlayer(_flow);
            if (sequine.TryGetActionClip(actionId, out var clip)) {
                return (T)(object)clip;
            }
            return default(T);
        }

#if UNITY_EDITOR
        public override void Editor_OnDrawTitle(out string _tooltip) {
            base.Editor_OnDrawTitle(out _tooltip);
            _tooltip = "Gets the actual animation clip reference of a Sequine Animation Data by specifying the action.";
        }

        public override void Editor_InitOutPoints() {
            if (nextIds.Count < 1) nextIds.Add(new List<ConnectionTarget>());
            CommandGUI.AddPropertyOutPoint<AnimationClip>();
        }

        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            base.Editor_OnDrawContents(_absPosition);
            var labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
            UnityEditor.EditorGUIUtility.labelWidth = 100;
            DrawActionsField(actionId, OnActionIDChanged);
            UnityEditor.EditorGUIUtility.labelWidth = labelWidth;

            CommandGUI.DrawOutPoint(0);
            CommandGUI.DrawLabel("Animation Clip", true);
        }

        private void OnActionIDChanged(int _actionId) {
            actionId = _actionId;
        }
#endif
    }

}