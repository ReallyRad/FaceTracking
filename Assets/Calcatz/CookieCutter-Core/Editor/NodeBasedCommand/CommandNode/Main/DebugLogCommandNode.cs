using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {

    [CustomCommandNodeDrawer(typeof(DebugLogCommand))]
    public class DebugLogCommandNode : TextFormatterCommandNode {

        public DebugLogCommandNode(CommandData _commandData, Command _command, Vector2 _position, Config _nodeConfig) 
            : base(_commandData, _command, _position, _nodeConfig) {
            nodeName = "Log";
        }

        protected override void DrawTextFieldLabel(Vector2 _absolutePosition) {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, singleLineHeight), "Message:", styles.label);
            AddRectHeight(singleLineHeight);
        }

    }
}
