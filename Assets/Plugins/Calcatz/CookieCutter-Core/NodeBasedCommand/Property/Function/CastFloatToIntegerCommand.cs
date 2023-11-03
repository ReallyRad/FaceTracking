using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Casts float value to integer.
    /// </summary>
    public class CastFloatToIntegerCommand : PropertyCommand {

        public enum RoundMode {
            DefaultCast,
            Ceil,
            Floor
        }

        public float m_value = 0;
        public RoundMode roundMode = RoundMode.DefaultCast;

        public override float nodeWidth => 200;

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            float val = GetInput<float>(_flow, 0, m_value);
            int result;
            switch (roundMode) {
                case RoundMode.DefaultCast:
                    result = (int)val;
                    break;
                case RoundMode.Ceil:
                    result = Mathf.CeilToInt(val);
                    break;
                case RoundMode.Floor:
                    result = Mathf.FloorToInt(val);
                    break;
                default:
                    result = (int)val;
                    break;
            }
            return (T)(object)result;
        }

#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            if (inputIds.Count < 1) inputIds.Add(new ConnectionTarget());
            CommandGUI.AddPropertyInPoint<float>();
        }
        public override void Editor_InitOutPoints() {
            if (nextIds.Count < 1) nextIds.Add(new List<ConnectionTarget>());
            CommandGUI.AddPropertyOutPoint<int>();
        }
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            CommandGUI.DrawInPoint(0);
            if (inputIds[0].targetId == 0) {
                CommandGUI.DrawFloatField("Float", ref m_value);
            }
            else {
                CommandGUI.DrawLabel("Float");
            }
            CommandGUI.DrawEnumField("Round Mode", ref roundMode);
            CommandGUI.DrawOutPoint(0);
            CommandGUI.DrawLabel("Integer", true);
        }
#endif

    }

}
