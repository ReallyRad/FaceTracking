using Calcatz.CookieCutter;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Calcatz.Sequine {

    [CustomCommandNodeDrawer(typeof(SetGlobalVariableCommand))]
    public class SetGlobalVariableCommandNode : CommandNode {

        private static string searchTerm = "";

        private Config config;
        private System.Action<ConnectionPoint, ConnectionPoint> onRemoveConnection;

        public SetGlobalVariableCommandNode(CommandData _commandData, Command _command, Vector2 _position, Config _config)
            : base(_commandData, _command, _position, 175, 50, _config) {
            var variableCommand = GetCommand<SetGlobalVariableCommand>();

            nodeName = "Set Global Variable";
            config = _config;
            onRemoveConnection = ((CommandNodeConfig)config).onRemovePropertyConnection;

            if (variableCommand != null) {
                if (SequineGlobalData.GetPersistent().variables.TryGetValue(variableCommand.variableId, out var variable)) {
                    System.Type type = variable.value == null ? typeof(string) : variable.value.GetType();
                    AddPropertyInPoint(type, _config);
                    AddPropertyOutPoint(type, _config);
                }

            }

        }


        public override void Draw(Vector2 _offset) {
            var variableCommand = (SetGlobalVariableCommand)GetCommand();
            base.Draw(_offset);
            Vector2 pos = rect.position + _offset;
            currentY = pos.y + 10;
            float height = EditorGUIUtility.singleLineHeight;
            //rect.height = height;

            //OnDrawTitle(_offset);

            AddRectHeight(height);

            DrawVariablePopup(variableCommand, pos, height);
            AddRectHeight(height);

            Rect fieldRect = new Rect(pos.x + 10, currentY, rect.width - 20, height);

            if (variableCommand.inputIds[0].targetId == 0) {
                DrawDefaultValueProperty(variableCommand, height, fieldRect);
            }
            else {
                EditorGUI.LabelField(fieldRect, " From Input", styles.label);
            }

            if (1<inPoints.Count) {
                DrawInPoint(pos, 1, currentY);
            }
            if (1<outPoints.Count) {
                DrawOutPoint(pos, 1, currentY);
            }

            //OnDrawContents(pos);
        }

        private void DrawVariablePopup(SetGlobalVariableCommand variableCommand, Vector2 pos, float height) {
            System.Type previousPointType = inPoints[inPoints.Count - 1].GetType();

            var variables = SequineGlobalData.GetPersistent().variables;
            Rect popupRect = new Rect(pos.x + 10, currentY, rect.width - 20, height);
            if (variables != null && variables.Count > 0) {
                string currentValue;
                if (variables.TryGetValue(variableCommand.variableId, out var currentVariable)) {
                    currentValue = variableCommand.variableId + ":  " + currentVariable.variableName;
                }
                else {
                    currentValue = "(None)";
                }
                if (GUI.Button(popupRect, currentValue, EditorStyles.popup)) {
                    int padSize = 0;
                    foreach (var key in variables.Keys) {
                        int newSize = key.ToString().Length;
                        if (newSize > padSize) padSize = newSize;
                    }
                    GenericMenu popupMenu = new GenericMenu();
                    foreach (var kvp in variables) {
                        var varId = kvp.Key;
                        var variable = kvp.Value;
                        popupMenu.AddItem(new GUIContent(varId.ToString().PadLeft(padSize, '0') + ":  " + variable.variableName),
                            varId == variableCommand.variableId,
                            () => {
                                Undo.RecordObject(commandData.targetObject, "Modified variable target");
                                variableCommand.variableId = varId;
                                EditorUtility.SetDirty(commandData.targetObject);
                                if (SequineGlobalData.GetPersistent().variables.TryGetValue(variableCommand.variableId, out var foundVariable)) {
                                    variableCommand.value = foundVariable.value;
                                    System.Type type = foundVariable.value == null ? typeof(string) : foundVariable.value.GetType();
                                    if (!PropertyConnectionPoint.connectionTypes.ContainsKey(type) || previousPointType != PropertyConnectionPoint.connectionTypes[type]) {
                                        onRemoveConnection.Invoke(inPoints[inPoints.Count - 1], outPoints[outPoints.Count - 1]);
                                        inPoints.RemoveAt(inPoints.Count - 1);
                                        outPoints.RemoveAt(outPoints.Count - 1);
                                        AddPropertyInPoint(type, config);
                                        AddPropertyOutPoint(type, config);
                                        if (onRefreshNeeded != null) {
                                            onRefreshNeeded.Invoke();
                                        }
                                    }
                                }
                            });
                    }
                    GenericMenuPopup.Show(popupMenu, currentValue, popupRect.position, null, searchTerm, _searchTerm => {
                        searchTerm = _searchTerm;
                    });
                }
            }
            else {
                GUI.Button(popupRect, "(None)", EditorStyles.popup);
            }
        }


        private void DrawDefaultValueProperty(SetGlobalVariableCommand variableCommand, float height, Rect fieldRect) {
            if (variableCommand.variableId == 0) return;
            object val = variableCommand.value;
            var propertyHandler = BuildableCommandNode.propertyHandler;

            System.Type dataType;
            if (SequineGlobalData.GetPersistent().variables.TryGetValue(variableCommand.variableId, out var variable)) {
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
