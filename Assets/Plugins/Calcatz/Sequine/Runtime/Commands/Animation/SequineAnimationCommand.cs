using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Base class for commands that targets a Sequine Player component and uses animation configuration without implementing how to play the animation yet.
    /// </summary>
    [System.Serializable]
    public class SequineAnimationCommand : SequineTargetCommand {

        public const int OUT_PORT_ONCOMPLETE = 1;
        public const int OUT_PORT_TOTALDURATION = 2;

        public int layer = 0;
        public float speed = 1f;
        public float transitionDuration = 0.25f;
        public bool normalizedTransition = false;
        public float lengthToPlay = 1.0f;
        public bool restart = true;

        protected AnimationConfig GetAnimationConfig() {
            return new AnimationConfig(speed, transitionDuration, normalizedTransition, lengthToPlay);
        }

        protected void ExecuteOnComplete(CommandExecutionFlow _flow) {
            RunSubFlow(_flow, OUT_PORT_ONCOMPLETE);
        }

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            return (T)(object)GetTotalAnimationDuration(_flow);
        }

        protected virtual float GetTotalAnimationDuration(CommandExecutionFlow _flow) => 0f;

#if UNITY_EDITOR
        public bool editor_showConfig = false;

        public override void Editor_InitOutPoints() {
            base.Editor_InitOutPoints();
            if (nextIds.Count <= OUT_PORT_ONCOMPLETE) {
                nextIds.Add(new List<ConnectionTarget>());
            }
            if (nextIds.Count <= OUT_PORT_TOTALDURATION) {
                nextIds.Add(new List<ConnectionTarget>());
            }
            CommandGUI.AddMainOutPoint();
            CommandGUI.AddPropertyOutPoint<float>();
        }

        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            base.Editor_OnDrawContents(_absPosition);
            var labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
            UnityEditor.EditorGUIUtility.labelWidth = 100;
            Editor_OnDrawAnimationContent(_absPosition);
            CommandGUI.DrawToggleField("Restart", ref restart);
            UnityEditor.EditorGUIUtility.labelWidth = labelWidth;
            CommandGUI.DrawFoldoutGroup("Configurations", ref editor_showConfig, () => {
                CommandGUI.DrawIntField("Layer", "The layer index to play the animation. Value 0 means Base Layer.", ref layer, 0);
                CommandGUI.DrawFloatField("Speed", ref speed, 0);
                CommandGUI.DrawFloatField("Transition Duration", ref transitionDuration, 0);
                CommandGUI.DrawToggleField("Normalized Transition", "Normalize the transition duration based on the percentage of the total duration instead of using actual seconds.", ref normalizedTransition);
                CommandGUI.DrawFloatField("Length to Play", "Normalized length to play based on the total duration of the clip", ref lengthToPlay, 0);
            });
            CommandGUI.PropertySpace();
            CommandGUI.DrawOutPoint(OUT_PORT_ONCOMPLETE);
            CommandGUI.DrawLabel("On Complete", "Executed when the elapsed duration reached the specified length to play.", true);
            CommandGUI.DrawOutPoint(OUT_PORT_TOTALDURATION);
            CommandGUI.DrawLabel("Total Duration", "The total duration (in seconds) of the animation based on the length to play.", true);
        }

        protected virtual void Editor_OnDrawAnimationContent(Vector2 _absPosition) {

        }
#endif

    }

}