using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Casts integer value to float.
    /// </summary>
    public class CastIntegerToFloatCommand : PropertyCommand {

        public int m_value = 0;

        public override float nodeWidth => 110;

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            int val = GetInput<int>(_flow, 0, m_value);
            return (T)(object)(float)val;
        }

#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            if (inputIds.Count < 1) inputIds.Add(new ConnectionTarget());
            CommandGUI.AddPropertyInPoint<int>();
        }
        public override void Editor_InitOutPoints() {
            if (nextIds.Count < 1) nextIds.Add(new List<ConnectionTarget>());
            CommandGUI.AddPropertyOutPoint<float>();
        }
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            CommandGUI.DrawInPoint(0);
            if (inputIds[0].targetId == 0) {
                CommandGUI.DrawIntField("Integer", ref m_value);
            }
            else {
                CommandGUI.DrawLabel("Integer");
            }
            CommandGUI.DrawOutPoint(0);
            CommandGUI.DrawLabel("Float", true);
        }
#endif

    }

}
