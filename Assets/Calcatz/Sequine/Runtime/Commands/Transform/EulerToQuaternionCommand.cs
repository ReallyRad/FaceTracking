using Calcatz.CookieCutter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Casts Euler Angles to Quaternion.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Cast/Euler to Quaternion", typeof(SequineFlowCommandData))]
    [GlobalStarredCommandMenu(typeof(Vector3), new Type[] { typeof(SequineFlowCommandData) })]
    public class EulerToQuaternionCommand : PropertyCommand {

        public override float nodeWidth => 130;

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            if (inputIds[0].targetId != 0) {
                return (T)(object)Quaternion.Euler(GetInput<Vector3>(_flow, 0));
            }
            Debug.LogError("Euler to Quaternion input is not set.");
            return (T)(object)Quaternion.identity;
        }

#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            base.Editor_InitInPoints();
            if (inputIds.Count < 1) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint<Vector3>();
        }

        public override void Editor_InitOutPoints() {
            base.Editor_InitOutPoints();
            if (nextIds.Count < 1) {
                nextIds.Add(new List<ConnectionTarget>());
            }
            CommandGUI.AddPropertyOutPoint<Quaternion>();
        }

        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            CommandGUI.DrawInPoint(0);
            CommandGUI.DrawLabel("In");
            CommandGUI.DrawOutPoint(0);
            CommandGUI.DrawLabel("Out", true);
        }
#endif

    }

}