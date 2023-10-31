using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Compares 2 strings with certain comparator.
    /// </summary>
    public class StringComparatorCommand : PropertyCommand {

        public string a = "";
        public string b = "";
        public Comparator comparator = Comparator.Equals;

        public override float nodeWidth => 200;

        public enum Comparator {
            Equals,
            NotEquals
        }

        private static readonly Dictionary<Comparator, Func<string, string, bool>> comparatorHandlers = new Dictionary<Comparator, Func<string, string, bool>>() {
            {Comparator.Equals,         (_a, _b) => { return _a == _b; } },
            {Comparator.NotEquals,      (_a, _b) => { return _a != _b; } }
        };

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            return (T)(object)comparatorHandlers[comparator](GetInput(_flow, 0, a), GetInput(_flow, 1, b));
        }

#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            if (inputIds.Count < 1) inputIds.Add(new ConnectionTarget());
            if (inputIds.Count < 2) inputIds.Add(new ConnectionTarget());
            CommandGUI.AddPropertyInPoint<string>();
            CommandGUI.AddPropertyInPoint<string>();
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
                CommandGUI.DrawTextField("A", ref a);
            }
            else {
                CommandGUI.DrawLabel("A");
            }
            CommandGUI.DrawEnumField("", ref comparator);
            CommandGUI.DrawInPoint(1);
            if (inputIds[1].targetId == 0) {
                CommandGUI.DrawTextField("B", ref b);
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