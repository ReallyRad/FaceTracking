using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Base class for commands with operator.
    /// </summary>
    /// <typeparam name="TValueType"></typeparam>
    public class OperatorCommand<TValueType> : PropertyCommand {

        public TValueType a = default(TValueType);
        public TValueType b = default(TValueType);
        public Operator operatorType = Operator.Add;

        public override float nodeWidth => 145;

        public enum Operator {
            Add,
            Subtract,
            Multiply,
            Divide,
            Pow
        }

        private static readonly Dictionary<Operator, Func<float, float, float>> operatorHandlers = new Dictionary<Operator, Func<float, float, float>>() {
            {Operator.Add,      (_a, _b) => { return _a + _b; } },
            {Operator.Subtract,(_a, _b) => { return _a - _b; } },
            {Operator.Multiply, (_a, _b) => { return _a * _b; } },
            {Operator.Divide,   (_a, _b) => { return _a / _b; } },
            {Operator.Pow,      (_a, _b) => { return Mathf.Pow(_a, _b); } }
        };

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            return (T)(object)operatorHandlers[operatorType](
                (float)(object)GetInput(_flow, 0, a), 
                (float)(object)GetInput(_flow, 1, b));
        }

#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            if (inputIds.Count < 1) inputIds.Add(new ConnectionTarget());
            if (inputIds.Count < 2) inputIds.Add(new ConnectionTarget());
            CommandGUI.AddPropertyInPoint(typeof(TValueType));
            CommandGUI.AddPropertyInPoint(typeof(TValueType));
        }
        public override void Editor_InitOutPoints() {
            if (nextIds.Count < 1) nextIds.Add(new List<ConnectionTarget>());
            CommandGUI.AddPropertyOutPoint(typeof(TValueType));
        }
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            var labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
            UnityEditor.EditorGUIUtility.labelWidth = 20;
            CommandGUI.DrawInPoint(0);
            if (inputIds[0].targetId == 0) {
                OnDrawAField();
            }
            else {
                CommandGUI.DrawLabel("A");
            }
            CommandGUI.DrawEnumField("", ref operatorType);
            CommandGUI.DrawInPoint(1);
            if (inputIds[1].targetId == 0) {
                OnDrawBField();
            }
            else {
                CommandGUI.DrawLabel("B");
            }
            CommandGUI.DrawOutPoint(0);
            CommandGUI.DrawLabel("Out", true);
            UnityEditor.EditorGUIUtility.labelWidth = labelWidth;
        }
        protected virtual void OnDrawAField() {

        }
        protected virtual void OnDrawBField() {

        }
#endif
    }
}