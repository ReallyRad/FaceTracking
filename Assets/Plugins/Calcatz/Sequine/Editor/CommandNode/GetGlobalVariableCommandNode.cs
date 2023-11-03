using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Calcatz.Sequine {

    [CustomCommandNodeDrawer(typeof(GetGlobalVariableCommand))]
    class GetGlobalVariableCommandNode : CommandNode {

        private static string searchTerm = "";

        private CommandNodeConfig config;
        private System.Action<ConnectionPoint> onResetOutPoint;

        protected virtual Dictionary<int, CommandData.Variable> variables => SequineGlobalData.GetPersistent().variables;
        protected int variableId {
            get { return GetCommand<GetGlobalVariableCommand>().variableId; }
            set {
                GetCommand<GetGlobalVariableCommand>().variableId = value;
            }
        }

        public GetGlobalVariableCommandNode(CommandData _commandData, Command _command, Vector2 position, Config _config)
            : base(_commandData, _command, position, 175, 100, _config) {
            nodeName = "Global Variable";
            config = (CommandNodeConfig)_config;
            onResetOutPoint = config.onResetOutPoint;
        }

        protected override void HandleFirstInPointCreation(Config _config) {

        }

        protected override void HandleFirstOutPointCreation(Config _config) {
            if (variables != null && variables.ContainsKey(variableId)) {
                AddPropertyOutPoint(variables[variableId].value.GetType(), _config);
            }
            else {
                AddPropertyOutPoint(typeof(PropertyConnectionPoint), _config);
            }
        }

        protected override void OnDrawContents(Vector2 _absolutePosition) {
            base.OnDrawContents(_absolutePosition);
            float height = EditorGUIUtility.singleLineHeight;

            Rect popupRect = new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, height);
            if (variables != null && variables.Count > 0) {
                string currentValue;
                if (variables.TryGetValue(variableId, out var currentVariable)) {
                    currentValue = variableId + ":  " + currentVariable.variableName;
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
                            varId == variableId,
                            ()=> {
                                Undo.RecordObject(commandData.targetObject, "Modified variable target");
                                variableId = varId;
                                System.Type valType = variables[variableId].value.GetType();
                                if (PropertyConnectionPoint.connectionTypes[valType] != outPoints[0].GetType()) {
                                    onResetOutPoint(outPoints[0]);
                                    outPoints.Clear();
                                    AddPropertyOutPoint(valType, config);
                                }
                                EditorUtility.SetDirty(commandData.targetObject);
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

            //DatabaseEditorUtility.VariablePopup(popupRect, commandData.targetObject, ref target.variableId);
            AddRectHeight(height);

        }

        public static string EnsureUniqueName(string _name, IList<string> _list) {
            if (_list.Contains(_name)) {
                int copyIndex = 1;
                while (_list.Contains(_name + "(" + copyIndex + ")")) {
                    copyIndex++;
                }
                return _name + "(" + copyIndex + ")";
            }
            else {
                return _name;
            }
        }

    }
}
