using Calcatz.CookieCutter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Multiplies Quaternion with Vector3 as quaternion transformation to a vector.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Transform/Quaternion * Vector3", typeof(SequineFlowCommandData))]
    [GlobalStarredCommandMenu(typeof(Quaternion), new System.Type[] { typeof(SequineFlowCommandData) })]
    public class QuaternionMultipleByVector3Command : PropertyCommand {

        public Vector3 b;

        public override float nodeWidth => 250;

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            return (T)(object)(GetInput(_flow, 0, Quaternion.identity) * GetInput(_flow, 1, b));
        }

#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            base.Editor_InitInPoints();
            if (inputIds.Count < 1) {
                inputIds.Add(new ConnectionTarget());
            }
            if (inputIds.Count < 2) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint<Quaternion>();
            CommandGUI.AddPropertyInPoint<Vector3>();
        }
        public override void Editor_InitOutPoints() {
            base.Editor_InitOutPoints();
            if (nextIds.Count < 1) {
                nextIds.Add(new List<ConnectionTarget>());
            }
            CommandGUI.AddPropertyOutPoint<Vector3>();
        }
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            base.Editor_OnDrawContents(_absPosition);
            CommandGUI.DrawInPoint(0);
            CommandGUI.DrawLabel("A");
            CommandGUI.DrawLabel("*");
            CommandGUI.DrawInPoint(1);
            if (inputIds[1].targetId == 0) {
                CommandGUI.DrawVector3Field("B", ref b);
            }
            else {
                CommandGUI.DrawLabel("B");
            }
            CommandGUI.DrawOutPoint(0);
            CommandGUI.DrawLabel("Out", true);
        }
#endif

    }

}