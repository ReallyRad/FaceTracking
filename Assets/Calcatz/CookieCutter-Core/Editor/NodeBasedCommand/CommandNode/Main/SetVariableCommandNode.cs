using Calcatz.CookieCutter;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {

    [CustomCommandNodeDrawer(typeof(SetVariableCommand))]
    public class SetVariableCommandNode : CommandNode {

        public SetVariableCommandNode(CommandData _commandData, Command _command, Vector2 _position, Config _config)
            : base(_commandData, _command, _position, 200, 50, _config) {
            SetVariableCommand variableCommand = GetCommand<SetVariableCommand>();

            if (variableCommand != null) {
                string variableName = GetVariableName(variableCommand);
                nodeName = "Set " + variableName;

                if (commandData.variables.ContainsKey(variableCommand.variableId)) {
                    CommandData.Variable variable = commandData.variables[variableCommand.variableId];
                    System.Type type = variable.value == null ? typeof(string) : variable.value.GetType();
                    AddPropertyInPoint(type, _config);
                    AddPropertyOutPoint(type, _config);
                }

            }
            else {
                nodeName = "Set Variable";
            }
        }


        public override void Draw(Vector2 _offset) {
            SetVariableCommand variableCommand = (SetVariableCommand)GetCommand();
            base.Draw(_offset);
            Vector2 pos = rect.position + _offset;
            currentY = pos.y + 10;
            float height = EditorGUIUtility.singleLineHeight;
            //rect.height = height;

            //OnDrawTitle(_offset);

            AddRectHeight(height);

            Rect fieldRect = new Rect(pos.x + 10, currentY, rect.width - 20, height);

            if (variableCommand.inputIds[0].targetId == 0) {
                DrawDefaultValueProperty(variableCommand, height, fieldRect);
            }
            else {
                EditorGUI.LabelField(fieldRect, " From Input", styles.label);
            }

            DrawInPoint(pos, 1, currentY);
            DrawOutPoint(pos, 1, currentY);

            //OnDrawContents(pos);
        }


        private string GetVariableName(SetVariableCommand variableCommand) {
            if (commandData.variables != null && commandData.variables.ContainsKey(variableCommand.variableId)) {
                return commandData.variables[variableCommand.variableId].variableName;
            }
            else {
                return "(Missing)";
            }
        }


        private void DrawDefaultValueProperty(SetVariableCommand variableCommand, float height, Rect fieldRect) {
            object val = variableCommand.value;
            var propertyHandler = BuildableCommandNode.propertyHandler;

            System.Type dataType;
            if (commandData.variables.ContainsKey(variableCommand.variableId)) {
                CommandData.Variable variable = commandData.variables[variableCommand.variableId];
                dataType = variable.value == null ? typeof(string) : variable.value.GetType();
                if (val == null) {
                    val = variableCommand.value = variable.value;
                }
            }
            else {
                dataType = typeof(string);
            }

            if (propertyHandler.ContainsKey(dataType)) {

                if (dataType == typeof(Vector3)) {
                    fieldRect.height = height * 2;
                }
                object newVal = propertyHandler[dataType](fieldRect, "", val);
                if (newVal != val) {
                    Undo.RecordObject(commandData.targetObject, "Modified set variable command input");
                    variableCommand.value = newVal;
                    EditorUtility.SetDirty(commandData.targetObject);
                }

            }
            else if (dataType.IsEnum) {
                object newVal = EditorGUI.EnumPopup(fieldRect, "", (System.Enum)val);
                if (newVal != val) {
                    Undo.RecordObject(commandData.targetObject, "Modified set variable command enum input");
                    variableCommand.value = newVal;
                    EditorUtility.SetDirty(commandData.targetObject);
                }
            }
            else {
                EditorGUI.LabelField(fieldRect, "(Invalid Data Type)", styles.label);
            }

        }

    }
}
