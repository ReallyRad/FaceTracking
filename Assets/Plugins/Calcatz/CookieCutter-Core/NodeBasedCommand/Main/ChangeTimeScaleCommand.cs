using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Modifies the value of the global time scale using Time.timeScale.
    /// </summary>
    public class ChangeTimeScaleCommand : Command {

        public override float nodeWidth => 180;

        public float timeScale = 1f;

        public ChangeTimeScaleCommand() {
            inputIds.Add(new ConnectionTarget());   //timeScale
        }

        public override void Execute(CommandExecutionFlow _flow) {
            float val = GetInput<float>(_flow, 0, timeScale);
            Time.timeScale = val;
            Exit();
        }

#if UNITY_EDITOR
        public override void Editor_OnDrawTitle(out string _tooltip) {
            base.Editor_OnDrawTitle(out _);
            _tooltip = "Change the global time scale.";
        }

        public override void Editor_InitInPoints() {
            base.Editor_InitInPoints();
            CommandGUI.AddPropertyInPoint<float>();
        }

        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            base.Editor_OnDrawContents(_absPosition);
            CommandGUI.DrawInPoint(1);
            if (inputIds[0].targetId == 0) {
                CommandGUI.DrawFloatField("Time Scale", "Time scale value.", ref timeScale, 0);
            }
            else {
                CommandGUI.DrawLabel("Time Scale");
            }
        }
#endif
    }
}
