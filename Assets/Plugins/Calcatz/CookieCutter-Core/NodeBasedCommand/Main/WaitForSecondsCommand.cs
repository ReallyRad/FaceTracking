using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Waits for certain duration in seconds before continuing to the next command.
    /// </summary>
    public class WaitForSecondsCommand : Command {

        public static bool pauseAll = false;

        public override float nodeWidth => 180;

        public float duration = 1f;
        public bool timeScaleIndependent = true;

        public WaitForSecondsCommand() {
            inputIds.Add(new ConnectionTarget());   //duration
            inputIds.Add(new ConnectionTarget());   //timeScaleIndependent
        }

        public override void Execute(CommandExecutionFlow _flow) {
            Wait();
        }

        private async void Wait() {
            float startTime = Time.time;
            float currentTime = startTime;
            while (currentTime - startTime < duration) {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    return;
                }
#endif
                if (!pauseAll) {
                    currentTime += timeScaleIndependent ? Time.unscaledDeltaTime : Time.deltaTime;
                }
                await Task.Yield();
            }
            Exit();
        }

#if UNITY_EDITOR
        public override void Editor_OnDrawTitle(out string _tooltip) {
            base.Editor_OnDrawTitle(out _);
            _tooltip = "Wait for seconds before proceeding to the next command.";
        }

        public override void Editor_InitInPoints() {
            base.Editor_InitInPoints();
            CommandGUI.AddPropertyInPoint<float>();
            CommandGUI.AddPropertyInPoint<bool>();
        }

        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            base.Editor_OnDrawContents(_absPosition);
            CommandGUI.DrawInPoint(1);
            if (inputIds[0].targetId == 0) {
                CommandGUI.DrawFloatField("Duration", "In seconds.", ref duration, 0);
            }
            else {
                CommandGUI.DrawLabel("Duration", "In seconds.");
            }
            CommandGUI.DrawInPoint(2);
            if (inputIds[1].targetId == 0) {
                CommandGUI.DrawToggleLeftField("Timescale Independent", ref timeScaleIndependent);
            }
            else {
                CommandGUI.DrawLabel("Timescale Independent");
            }
        }
#endif
    }
}
