using Calcatz.CookieCutter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Multiplies Vector3 with Float.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Transform/Vector3 * Float", typeof(SequineFlowCommandData))]
    [GlobalStarredCommandMenu(typeof(Vector3), new Type[] { typeof(SequineFlowCommandData) })]
    public class Vector3MultipleByFloatCommand : PropertyCommand {

        public Vector3 a;
        public float b;

        public override float nodeWidth => 250;

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            Vector3 _a;
            if (inputIds[0].targetId == 0) {
                _a = a;
            }
            else {
                _a = GetInput<Vector3>(_flow, 0);
            }
            float _b;
            if (inputIds[1].targetId == 0) {
                _b = b;
            }
            else {
                _b = GetInput<float>(_flow, 1);
            }
            return (T)(object)(_a * _b);
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
            CommandGUI.AddPropertyInPoint<Vector3>();
            CommandGUI.AddPropertyInPoint<float>();
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
            if (inputIds[0].targetId == 0) {
                CommandGUI.DrawVector3Field("A", ref a);
            }
            else {
                CommandGUI.DrawLabel("A");
            }
            CommandGUI.DrawLabel("*");
            CommandGUI.DrawInPoint(1);
            if (inputIds[1].targetId == 0) {
                CommandGUI.DrawFloatField("B", ref b);
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