using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Base class for commands that tweens a Transform's value.
    /// </summary>
    [System.Serializable]
    public class TransformTweenCommand : ModifyTransformCommand {

        public const int IN_PORT_DURATION = 1;

        public const int OUT_PORT_ONCOMPLETE = 1;

        public Tween tween = new Tween();

        public override void Execute(CommandExecutionFlow _flow) {
            Exit();
        }

        protected void StartTweening(CommandExecutionFlow _flow, System.Action<float> _onUpdate, System.Action _onComplete) {
            tween.RunTween(GetTransform(_flow), _onUpdate, _onComplete, GetInput(_flow, IN_PORT_DURATION, tween.duration));
        }

        protected void ExecuteOnComplete(CommandExecutionFlow _flow) {
            RunSubFlow(_flow, OUT_PORT_ONCOMPLETE);
        }

#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            if (tween == null) tween = new Tween();
            base.Editor_InitInPoints();
            if (inputIds.Count <= IN_PORT_DURATION) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint<float>();
        }
        public override void Editor_InitOutPoints() {
            base.Editor_InitOutPoints();
            if (nextIds.Count <= OUT_PORT_ONCOMPLETE) {
                nextIds.Add(new List<ConnectionTarget>());
            }
            CommandGUI.AddMainOutPoint();
        }
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            base.Editor_OnDrawContents(_absPosition);
            CommandGUI.DrawInPoint(IN_PORT_DURATION + 1);
            if (inputIds[IN_PORT_DURATION].targetId != 0) {
                CommandGUI.DrawLabel("Duration", "Duration to apply the tween in seconds.");
            }
            else {
                CommandGUI.DrawFloatField("Duration", "Duration to apply the tween in seconds.", ref tween.duration);
            }
            CommandGUI.DrawToggleField("Timescale Independent", ref tween.timescaleIndependent);
            CommandGUI.DrawEnumField("Easing", ref tween.easing);
        }
#endif

    }

}