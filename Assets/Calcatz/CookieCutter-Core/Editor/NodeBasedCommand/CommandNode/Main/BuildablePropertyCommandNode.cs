using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {

    public class BuildablePropertyCommandNode : CommandNode {

        #region INITIALIZATION
        public BuildablePropertyCommandNode(CommandData _commandData, Command _command, Vector2 position, Config config)
            : base(_commandData, _command, position, _command.nodeWidth, 50, config) {
            CommandRegistry.Registry reg = CommandRegistry.GetRegistry(_commandData.GetType(), _command.GetType());
            string[] pathSplit = reg.pathName.Split('/');
            nodeName = pathSplit[pathSplit.Length - 1];

            if (GetCommand() is BuildablePropertyCommand buildablePropertyCommand) {
                if (buildablePropertyCommand.io == null) buildablePropertyCommand.io = new BuildablePropertyCommand.IO();
                AddPoints(config, buildablePropertyCommand);
            }
            else {
                Debug.LogError(GetCommandData().GetType().Name + " is not derived from BuildablePropertyCommand and has no custom drawer.");
            }
        }

        private void AddPoints(Config _config, BuildablePropertyCommand _buildablePropertyCommand) {
            BuildablePropertyCommand.IO io = _buildablePropertyCommand.io;
            if (io.inputLabels != null) {
                for (int i = 0; i < io.inputValues.Count; i++) {
                    object val = io.inputValues[i];
                    if (io.unityObjectInputs != null && io.unityObjectInputs.ContainsKey(io.inputLabels[i])) {
                        val = typeof(UnityEngine.Object);
                        AddPropertyInPoint((System.Type)val, _config);
                    }
                    else if (val is System.Type) {
                        AddPropertyInPoint((System.Type)val, _config);
                    }
                    else if (val == null) {
                        AddPropertyInPoint(typeof(PropertyConnectionPoint), _config);
                    }
                    else {
                        AddPropertyInPoint(val.GetType(), _config);
                    }
                }
            }

            if (io.outputLabels != null) {
                for (int i = 0; i < io.outputValues.Count; i++) {
                    object val = io.outputValues[i];
                    if (io.unityObjectOutputs != null && io.unityObjectOutputs.ContainsKey(io.outputLabels[i])) {
                        val = typeof(UnityEngine.Object);
                        AddPropertyOutPoint((System.Type)val, _config);
                    }
                    else if (val is System.Type) {
                        AddPropertyOutPoint((System.Type)val, _config);
                    }
                    else if (val == null) {
                        AddPropertyOutPoint(typeof(PropertyConnectionPoint), _config);
                    }
                    else {
                        AddPropertyOutPoint(val.GetType(), _config);
                    }
                }
            }
        }

        protected override void HandleFirstInPointCreation(Config _config) { }

        protected override void HandleFirstOutPointCreation(Config _config) { }

        protected override void OnDrawTitle(Vector2 _offset) {
            
        }
        #endregion

        protected override void OnDrawContents(Vector2 _absolutePosition) {
            base.OnDrawContents(_absolutePosition);
            BuildablePropertyCommand buildablePropertyCommand = (BuildablePropertyCommand)GetCommand();
            float fieldHeight = EditorGUIUtility.singleLineHeight;

            AddRectHeight(0);

            Color labelColor = EditorStyles.label.normal.textColor;
            EditorStyles.label.normal.textColor = Color.white;

            if (buildablePropertyCommand.io.inputLabels != null) {
                for (int i = 0; i < buildablePropertyCommand.io.inputLabels.Count; i++) {
                    DrawInPoint(_absolutePosition, i, currentY);
                    string label = buildablePropertyCommand.io.inputLabels[i];
                    Rect fieldRect = new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, fieldHeight);
                    if (buildablePropertyCommand.inputIds[i] == null || buildablePropertyCommand.inputIds[i].targetId == 0) {
                        fieldRect = HandleDefaultInputValue(buildablePropertyCommand, fieldHeight, i, label, fieldRect);
                    }
                    else {
                        EditorGUI.LabelField(fieldRect, label, styles.label);
                    }
                    AddRectHeight(fieldRect.height + verticalSpacing);
                }
            }

            if (buildablePropertyCommand.io.outputLabels != null) {
                for (int i = 0; i < buildablePropertyCommand.io.outputLabels.Count; i++) {
                    DrawOutPoint(_absolutePosition, i, currentY);
                    EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, fieldHeight), buildablePropertyCommand.io.outputLabels[i], styles.labelRight);
                    AddRectHeight(fieldHeight + verticalSpacing);
                }
            }

            EditorStyles.label.normal.textColor = labelColor;
        }

        private Rect HandleDefaultInputValue(BuildablePropertyCommand buildableCommand, float fieldHeight, int i, string label, Rect fieldRect) {
            object val = buildableCommand.io.inputValues[i];

            float prevWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = fieldRect.width * LABEL_WIDTH;

            if (buildableCommand.io.unityObjectInputs != null && buildableCommand.io.unityObjectInputs.ContainsKey(label)) {
                object newVal = EditorGUI.ObjectField(fieldRect, label, (UnityEngine.Object)val, buildableCommand.io.unityObjectInputs[label], commandData.targetObject != null && commandData.targetObject is MonoBehaviour);
                if (newVal != val) {
                    RegisterUndo("Modified buildable proprerty command unity object input");
                    buildableCommand.io.inputValues[i] = newVal;
                    SetDirty();
                }
            }
            else if (val == null) {

            }
            else if (BuildableCommandNode.propertyHandler.ContainsKey(val.GetType())) {

                if (val is Vector3) {
                    fieldRect.height = fieldHeight * 2;
                }
                object newVal = BuildableCommandNode.propertyHandler[val.GetType()](fieldRect, label, val);
                if (newVal != val) {
                    RegisterUndo("Modified buildable proprerty command input");
                    buildableCommand.io.inputValues[i] = newVal;
                    SetDirty();
                }

            }
            else if (val.GetType().IsEnum) {
                object newVal = EditorGUI.EnumPopup(fieldRect, label, (System.Enum)val);
                if (newVal != val) {
                    RegisterUndo("Modified buildable proprerty command enum input");
                    buildableCommand.io.inputValues[i] = newVal;
                    SetDirty();
                }
            }
            else {
                EditorGUI.LabelField(fieldRect, label + ": " + val, styles.label);
            }

            EditorGUIUtility.labelWidth = prevWidth;
            return fieldRect;
        }

        private void RegisterUndo(string _message) {
            if (!Application.isPlaying) {
                Undo.RecordObject(commandData.targetObject, _message);
            }
        }

        private void SetDirty() {
            if (!Application.isPlaying) {
                EditorUtility.SetDirty(commandData.targetObject);
            }
        }
    }
}
