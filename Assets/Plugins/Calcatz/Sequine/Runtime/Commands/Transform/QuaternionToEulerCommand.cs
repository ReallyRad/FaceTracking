using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Casts Quaternion to Euler Angles.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Cast/Quaternion to Euler", typeof(SequineFlowCommandData))]
    [GlobalStarredCommandMenu(typeof(Quaternion), new System.Type[] { typeof(SequineFlowCommandData) })]
    public class QuaternionToEulerCommand : PropertyCommand {

        public override float nodeWidth => 130;

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            if (inputIds[0].targetId != 0) {
                return (T)(object)GetInput<Quaternion>(_flow, 0).eulerAngles;
            }
            Debug.LogError("Quaternion to Euler input is not set.");
            return (T)(object)Vector3.zero;
        }

#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            base.Editor_InitInPoints();
            if (inputIds.Count < 1) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint<Quaternion>();
        }

        public override void Editor_InitOutPoints() {
            base.Editor_InitOutPoints();
            if (nextIds.Count < 1) {
                nextIds.Add(new List<ConnectionTarget>());
            }
            CommandGUI.AddPropertyOutPoint<Vector3>();
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