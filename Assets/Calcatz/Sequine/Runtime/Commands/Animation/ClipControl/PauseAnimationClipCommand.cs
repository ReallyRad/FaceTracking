using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Pauses an animation (if the clip is currently playing) by specifying the animation clip asset.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Animation/Clip Control/Pause Animation Clip", typeof(SequineFlowCommandData))]
    public class PauseAnimationClipCommand : SequineTargetCommand {

        public const int IN_PORT_CLIP = 1;

        public AnimationClip clip;

        public override float nodeWidth => 350f;

        public override void Execute(CommandExecutionFlow _flow) {
            SequinePlayer sequine = GetSequinePlayer(_flow);
            var clipValue = GetInput<AnimationClip>(_flow, IN_PORT_CLIP, clip);
            if (sequine.TryGetState(clipValue, out var state)) {
                state.Pause();
            }
            Exit();
        }


#if UNITY_EDITOR
        public override void Editor_OnDrawTitle(out string _tooltip) {
            base.Editor_OnDrawTitle(out _tooltip);
            _tooltip = "Pauses an animation (if the clip is currently playing) by specifying the animation clip asset.";
        }

        public override void Editor_InitInPoints() {
            base.Editor_InitInPoints();
            if (inputIds.Count <= IN_PORT_CLIP) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint<UnityEngine.Object>();
        }

        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            base.Editor_OnDrawContents(_absPosition);
            var labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
            UnityEditor.EditorGUIUtility.labelWidth = 100;
            CommandGUI.DrawInPoint(IN_PORT_CLIP + 1);
            if (inputIds[IN_PORT_CLIP].targetId == 0) {
                CommandGUI.DrawObjectField("Clip", clip, _clip => clip = _clip);
            }
            else {
                CommandGUI.DrawLabel("Clip");
            }
            UnityEditor.EditorGUIUtility.labelWidth = labelWidth;
        }
#endif

    }

}