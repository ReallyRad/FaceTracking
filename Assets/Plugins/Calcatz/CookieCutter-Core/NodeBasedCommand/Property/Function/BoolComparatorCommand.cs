using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Base class of boolean comparator commands.
    /// </summary>
    public class BoolComparatorCommand : PropertyCommand {

        public bool a = true;
        public bool b = true;

        public override float nodeWidth => 75;

#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            if (inputIds.Count < 1) inputIds.Add(new ConnectionTarget());
            if (inputIds.Count < 2) inputIds.Add(new ConnectionTarget());
            CommandGUI.AddPropertyInPoint<bool>();
            CommandGUI.AddPropertyInPoint<bool>();
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
                CommandGUI.DrawToggleField("A", ref a);
            }
            else {
                CommandGUI.DrawLabel("A");
            }
            CommandGUI.DrawInPoint(1);
            if (inputIds[1].targetId == 0) {
                CommandGUI.DrawToggleField("B", ref b);
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