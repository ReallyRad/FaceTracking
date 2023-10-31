using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Pauses an animation by specifying the action animation, and sample a certain pose at the specified time.
    /// This will also automatically plays the action animation if it's not yet played.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Animation/Clip Control/Pause Action Animation At Time", typeof(SequineFlowCommandData))]
    public class PauseActionAnimationAtTimeCommand : SequineTargetCommand {

        public int actionId;
        public float time = 0f;
        public bool useNormalizedTime = false;

        public int layer = 0;
        public float transitionDuration = 0.25f;
        public bool normalizedTransition = false;

        public override float nodeWidth => 350f;

        public override void Execute(CommandExecutionFlow _flow) {
            SequinePlayer sequine = GetSequinePlayer(_flow);
            if (sequine.TryGetActionClip(actionId, out var clip)) {
                sequine.PauseAnimationClipAtTime(clip, time, useNormalizedTime, new AnimationConfig(1, transitionDuration, normalizedTransition, 1));
            }
            Exit();
        }


#if UNITY_EDITOR
        public bool editor_showConfig = false;

        public override void Editor_OnDrawTitle(out string _tooltip) {
            base.Editor_OnDrawTitle(out _tooltip);
            _tooltip = @"Pauses an animation by specifying the action animation, and sample a certain pose at the specified time.

This will also automatically plays the action animation if it's not yet played.";
        }

        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            base.Editor_OnDrawContents(_absPosition);
            var labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
            UnityEditor.EditorGUIUtility.labelWidth = 100;
            DrawActionsField(actionId, OnActionIDChanged);
            CommandGUI.DrawFloatField("Time", "The time of the clip to sample.", ref time, 0);
            CommandGUI.DrawToggleLeftField("Use Normalized Time", ref useNormalizedTime);
            UnityEditor.EditorGUIUtility.labelWidth = labelWidth;
            CommandGUI.AddRectHeight(5);
            var tipRect = CommandGUI.GetRect(2);
            UnityEditor.EditorGUI.HelpBox(tipRect, "The configuration is only used if the clip is not already playing.", UnityEditor.MessageType.Info);
            CommandGUI.AddRectHeight(tipRect.height);
            CommandGUI.DrawFoldoutGroup("Configurations", ref editor_showConfig, () => {
                CommandGUI.DrawIntField("Layer", "The layer index to play the animation. Value 0 means Base Layer.", ref layer, 0);
                CommandGUI.DrawFloatField("Transition Duration", ref transitionDuration, 0);
                CommandGUI.DrawToggleField("Normalized Transition", "Normalize the transition duration based on the percentage of the total duration instead of using actual seconds.", ref normalizedTransition);
            });
        }

        private void OnActionIDChanged(int _actionId) {
            actionId = _actionId;
        }
#endif

    }

}