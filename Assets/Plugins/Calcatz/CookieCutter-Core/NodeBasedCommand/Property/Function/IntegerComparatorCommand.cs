using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Compares 2 integers with certain comparator.
    /// </summary>
    public class IntegerComparatorCommand : PropertyCommand {

        public int a = 0;
        public int b = 0;
        public Comparator comparator = Comparator.Equals;

        public override float nodeWidth => 145;

        public enum Comparator {
            Equals,
            NotEquals,
            LessThan,
            LessThanEquals,
            MoreThan,
            MoreThanEquals
        }

        private static readonly Dictionary<Comparator, Func<int, int, bool>> comparatorHandlers = new Dictionary<Comparator, Func<int, int, bool>>() {
            {Comparator.Equals,         (_a, _b) => { return _a == _b; } },
            {Comparator.NotEquals,      (_a, _b) => { return _a != _b; } },
            {Comparator.LessThan,       (_a, _b) => { return _a <  _b; } },
            {Comparator.LessThanEquals, (_a, _b) => { return _a <= _b; } },
            {Comparator.MoreThan,       (_a, _b) => { return _a >  _b; } },
            {Comparator.MoreThanEquals, (_a, _b) => { return _a >= _b; } }
        };

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            return (T)(object)comparatorHandlers[comparator](GetInput(_flow, 0, a), GetInput(_flow, 1, b));
        }

#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            if (inputIds.Count < 1) inputIds.Add(new ConnectionTarget());
            if (inputIds.Count < 2) inputIds.Add(new ConnectionTarget());
            CommandGUI.AddPropertyInPoint<int>();
            CommandGUI.AddPropertyInPoint<int>();
        }
        public override void Editor_InitOutPoints() {
            if (nextIds.Count < 1) nextIds.Add(new List<ConnectionTarget>());
            CommandGUI.AddPropertyOutPoint<bool>();
        }
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            var labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
            UnityEditor.EditorGUIUtility.labelWidth = 20;
            CommandGUI.DrawInPoint(0);
            if (inputIds[0].targetId == 0) {
                CommandGUI.DrawIntField("A", ref a);
            }
            else {
                CommandGUI.DrawLabel("A");
            }
            CommandGUI.DrawEnumField("", ref comparator);
            CommandGUI.DrawInPoint(1);
            if (inputIds[1].targetId == 0) {
                CommandGUI.DrawIntField("B", ref b);
            }
            else {
                CommandGUI.DrawLabel("B");
            }
            CommandGUI.DrawOutPoint(0);
            CommandGUI.DrawLabel("Out", true);
            UnityEditor.EditorGUIUtility.labelWidth = labelWidth;
        }
#endif
    }
}