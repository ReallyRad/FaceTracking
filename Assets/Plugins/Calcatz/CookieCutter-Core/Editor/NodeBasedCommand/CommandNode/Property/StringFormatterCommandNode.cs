using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {

    [CustomCommandNodeDrawer(typeof(StringFormatterCommand))]
    public class StringFormatterCommandNode : TextFormatterCommandNode {

        public StringFormatterCommandNode(CommandData _commandData, Command _command, Vector2 _position, Config _nodeConfig) 
            : base(_commandData, _command, _position, _nodeConfig) {
            nodeName = "String Formatter";
        }

        protected override void HandleFirstInPointCreation(Config _config) {

        }

        protected override void HandleFirstOutPointCreation(Config _config) {
            AddPropertyOutPoint<string>(_config);
        }

        protected override void OnDrawTitle(Vector2 _absolutePosition) {
            DrawOutPoint(_absolutePosition, 0, currentY);
        }

        protected override void DrawTextFieldLabel(Vector2 _absolutePosition) {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, singleLineHeight), "Format:", styles.label);
            AddRectHeight(singleLineHeight);
        }

    }
}
