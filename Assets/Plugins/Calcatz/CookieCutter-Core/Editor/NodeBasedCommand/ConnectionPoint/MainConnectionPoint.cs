using System;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class MainConnectionPoint : ConnectionPoint {

        private static GUIStyle inStyle;
        private static GUIStyle outStyle;

        public MainConnectionPoint(Node _node, ConnectionPointType _type, GUIStyle _style, Action<ConnectionPoint> _onClickConnectionPoint)
            : base (_node, _type, _style, _onClickConnectionPoint){
            if (inStyle == null) {
                inStyle = new GUIStyle();
                inStyle.normal.background = btnLeftTex;
                inStyle.active.background = btnLeftOnTex;
                inStyle.border = new RectOffset(4, 4, 12, 12);
            }
            if (outStyle == null) {
                outStyle = new GUIStyle();
                outStyle.normal.background = btnRightTex;
                outStyle.active.background = btnRightOnTex;
                outStyle.border = new RectOffset(4, 4, 12, 12);
            }
            if (_type == ConnectionPointType.In) {
                style = inStyle;
            }
            else if (_type == ConnectionPointType.Out) {
                style = outStyle;
            }
        }

        public override Color GetColor() {
            if (type == ConnectionPointType.Out) {
                CommandNode commandNode = (CommandNode)node;
                Command command = commandNode.GetCommand();
                if (command.executed) {
                    if (command.executedOutputs != null) {
                        int pointIndex = CommandNode.GetConnectionPointIndex(this, commandNode.outPoints);
                        if (command.executedOutputs.Contains(pointIndex)) {
                            return new Color(1, 0.992f, 0.5f, 1);
                        }
                    }
                }
            }
            return Color.white;
        }

        public override float GetConnectionWidth() {
            if (type == ConnectionPointType.Out) {
                CommandNode commandNode = (CommandNode)node;
                Command command = commandNode.GetCommand();
                if (commandNode.GetCommand().executed) {
                    if (command.executedOutputs != null) {
                        int pointIndex = CommandNode.GetConnectionPointIndex(this, commandNode.outPoints);
                        if (command.executedOutputs.Contains(pointIndex)) {
                            return 6f;
                        }
                    }
                }
            }
            return 3f;
        }
    }
}
