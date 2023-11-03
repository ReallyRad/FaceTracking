using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter
{

    public partial class BuildableCommandNode : CommandNode {

        private void SetupCommandGUI(Config config) {
            CommandGUI.internal_Actions.addMainInPoint = () => {
                AddMainInPoint(config);
            };
            CommandGUI.internal_Actions.addMainOutPoint = () => {
                AddMainOutPoint(config);
            };
            CommandGUI.internal_Actions.addPropertyInPoint = _type => {
                AddPropertyInPoint(_type, config);
            };
            CommandGUI.internal_Actions.addPropertyOutPoint = _type => {
                AddPropertyOutPoint(_type, config);
            };
            ResetupCommandGUI();

            if (command.id < ReservedNodeNames.Length) {
                nodeName = ReservedNodeNames[command.id];
            }
            else {
                command.Editor_InitInPoints();
            }

            command.Editor_InitOutPoints();
        }

        private void ResetupCommandGUI() {
            CommandGUI.internal_Actions.addRectHeight = _height => {
                AddRectHeight(_height);
            };
            CommandGUI.internal_Actions.labelStyle = styles.label;
            CommandGUI.internal_Actions.labelRightStyle = styles.labelRight;
            CommandGUI.internal_Actions.currentY = () => currentY;
            CommandGUI.internal_Actions.nodeWidth = () => rect.width;
            CommandGUI.internal_Actions.targetObject = commandData.targetObject;
            CommandGUI.internal_Actions.selected = isSelected;
        }

        protected override void HandleFirstInPointCreation(Config _config) {
            if (GetCommand() is BuildableCommand) {
                base.HandleFirstInPointCreation(_config);
            }
        }

        protected override void HandleFirstOutPointCreation(Config _config) {
            if (command is BuildableCommand) {
                base.HandleFirstOutPointCreation(_config);
            }
        }

        protected override void OnDrawTitle(Vector2 _absolutePosition) {
            if (command is BuildableCommand) {
                base.OnDrawTitle(_absolutePosition);
            }
            else {
                ResetupCommandGUI();
                CommandGUI.internal_Actions.absolutePosition = _absolutePosition;
                CommandGUI.internal_Actions.drawInPoint = _index => {
                    DrawInPoint(_absolutePosition, _index, currentY);
                };
                CommandGUI.internal_Actions.drawOutPoint = _index => {
                    DrawOutPoint(_absolutePosition, _index, currentY);
                };
                string commandTooltip;
                command.Editor_OnDrawTitle(out commandTooltip);
                tooltip = commandTooltip;
            }
        }

        private void OnDrawContentsCustomCommand(Vector2 _absolutePosition) {
            float prevWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = rect.width * 0.4f;
            if (EditorGUIUtility.labelWidth > prevWidth) {
                EditorGUIUtility.labelWidth = prevWidth;
            }
            command.Editor_OnDrawContents(_absolutePosition);
            EditorGUIUtility.labelWidth = prevWidth;
        }

    }
}
