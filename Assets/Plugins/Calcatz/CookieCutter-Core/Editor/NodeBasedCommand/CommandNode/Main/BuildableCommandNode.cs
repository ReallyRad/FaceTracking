using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if TIMELINE_AVAILABLE
using UnityEngine.Playables;
#endif

namespace Calcatz.CookieCutter {

    public partial class BuildableCommandNode : CommandNode {

        public static readonly Dictionary<System.Type, System.Func<Rect, string, object, object>> propertyHandler = new Dictionary<System.Type, System.Func<Rect, string, object, object>>() {
            {typeof(bool), (_rect, _label, _object) => EditorGUI.Toggle(_rect, _label, (bool)_object) },
            {typeof(int), (_rect, _label, _object) => EditorGUI.IntField(_rect, _label, (int)_object) },
            {typeof(float), (_rect, _label, _object) => EditorGUI.DelayedFloatField(_rect, _label, (float)_object) },
            {typeof(string), (_rect, _label, _object) => EditorGUI.DelayedTextField(_rect, _label, (string)_object) },
            {typeof(Vector3), (_rect, _label, _object) => EditorGUI.Vector3Field(_rect, _label, (Vector3)_object) }

        };

        #region INITIALIZATION
        public BuildableCommandNode(CommandData _commandData, Command _command, Vector2 position, Config config)
            : base(_commandData, _command, position, _command.nodeWidth, 50, config) {
            CommandRegistry.Registry reg = CommandRegistry.GetRegistry(_commandData.GetType(), _command.GetType());
            string[] pathSplit = reg.pathName.Split('/');
            nodeName = pathSplit[pathSplit.Length - 1];

            if (GetCommand() is BuildableCommand buildableCommand) {

                if (buildableCommand.io == null) buildableCommand.io = new BuildableCommand.IO();
                AddPoints(config, buildableCommand);

            }
            else {
                SetupCommandGUI(config);
            }
        }

        private void AddPoints(Config _config, BuildableCommand _buildableCommand) {
            BuildableCommand.IO io = _buildableCommand.io;
            if (io.inputValues != null) {
                for (int i = 0; i < io.inputValues.Count; i++) {
                    object val = io.inputValues[i];
                    if (val is System.Type) {
                        AddPropertyInPoint((System.Type)val, _config);
                    }
                    else if (val != null && val.GetType().IsValueType) {
                        AddPropertyInPoint(val.GetType(), _config);
                    }
                    else if (io.unityObjectInputs != null && io.unityObjectInputs.ContainsKey(io.inputLabels[i])) {
                        AddPropertyInPoint(typeof(UnityEngine.Object), _config);
                    }
                    else {
                        AddPropertyInPoint(typeof(PropertyConnectionPoint), _config);
                    }
                }
            }

            if (io.outputValues != null) {
                for (int i = 1; i < command.nextIds.Count; i++) {
                    if (io.outputValues.ContainsKey(i)) {
                        object val = io.outputValues[i];
                        if (val is System.Type) {
                            AddPropertyOutPoint((System.Type)val, _config);
                        }
                        else if (val == null) {
                            AddPropertyOutPoint(typeof(PropertyConnectionPoint), _config);
                        }
                        else {
                            AddPropertyOutPoint(val.GetType(), _config);
                        }
                    }
                    else {
                        AddMainOutPoint(_config);
                    }
                }
            }
            else {
                for (int i = 1; i < command.nextIds.Count; i++) {
                    AddMainOutPoint(_config);
                }
            }
        }
        #endregion

        protected override void OnDrawContents(Vector2 _absolutePosition) {
            base.OnDrawContents(_absolutePosition);
            float fieldHeight = EditorGUIUtility.singleLineHeight;

            BuildableCommand buildableCommand = GetCommand<BuildableCommand>();
            if (buildableCommand == null) {
                OnDrawContentsCustomCommand(_absolutePosition);
                return;
            }

            AddRectHeight(0);

            Color labelColor = EditorStyles.label.normal.textColor;
            EditorStyles.label.normal.textColor = Color.white;

            if (buildableCommand.io.inputLabels != null) {
                for (int i = 0; i < buildableCommand.io.inputLabels.Count; i++) {
                    DrawInPoint(_absolutePosition, i + 1, currentY);
                    string label = buildableCommand.io.inputLabels[i];
                    Rect fieldRect = new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, fieldHeight);
                    if (buildableCommand.inputIds[i] == null || buildableCommand.inputIds[i].targetId == 0) {
                        fieldRect = HandleDefaultInputValue(buildableCommand, fieldHeight, i, label, fieldRect);
                    }
                    else {
                        EditorGUI.LabelField(fieldRect, label, styles.label);
                    }
                    AddRectHeight(fieldRect.height + verticalSpacing);
                }
            }

            if (buildableCommand.io.outputLabels != null) {
                int currentPropertyOutputLabelIndex = 0;
                int currentMainOutputLabelIndex = 0;
                for (int i = 1; i < buildableCommand.nextIds.Count; i++) {
                    DrawOutPoint(_absolutePosition, i, currentY);
                    string label;
                    if (buildableCommand.io.outputValues != null && buildableCommand.io.outputValues.ContainsKey(i)) {
                        label = buildableCommand.io.outputLabels[currentPropertyOutputLabelIndex];
                        currentPropertyOutputLabelIndex++;
                    }
                    else {
                        if (buildableCommand.io.mainOutputLabels != null) {
                            label = buildableCommand.io.mainOutputLabels[currentMainOutputLabelIndex];
                        }
                        else {
                            label = "";
                        }
                        currentMainOutputLabelIndex++;
                    }
                    EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, fieldHeight), label, styles.labelRight);
                    AddRectHeight(fieldHeight + verticalSpacing);
                }
            }
            else if (buildableCommand.io.mainOutputLabels != null) {
                for (int i = 0; i < buildableCommand.io.mainOutputLabels.Count; i++) {
                    DrawOutPoint(_absolutePosition, i + 1, currentY);
                    EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, fieldHeight), buildableCommand.io.mainOutputLabels[i], styles.labelRight);
                    AddRectHeight(fieldHeight + verticalSpacing);
                }
            }

            EditorStyles.label.normal.textColor = labelColor;
        }

        private Rect HandleDefaultInputValue(BuildableCommand buildableCommand, float fieldHeight, int i, string label, Rect fieldRect) {
            object val = buildableCommand.io.inputValues[i];

            float prevWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = fieldRect.width * LABEL_WIDTH;

            if (buildableCommand.io.unityObjectInputs != null && buildableCommand.io.unityObjectInputs.ContainsKey(label)) {
                bool allowSceneObjects = commandData.targetObject != null && commandData.targetObject is MonoBehaviour;
                var objectType = buildableCommand.io.unityObjectInputs[label];
                DropBinderAreaGUI(fieldRect, objectType, _newVal => {
                    Undo.RecordObject(commandData.targetObject, "Modified buildable command unity object input");
                    buildableCommand.io.inputValues[i] = _newVal;
                    EditorUtility.SetDirty(commandData.targetObject);
                });
                object newVal = EditorGUI.ObjectField(fieldRect, label, (UnityEngine.Object)val, objectType, allowSceneObjects);
                if (newVal != val) {
                    Undo.RecordObject(commandData.targetObject, "Modified buildable command unity object input");
                    buildableCommand.io.inputValues[i] = newVal;
                    EditorUtility.SetDirty(commandData.targetObject);
                }
            }
            else if (val == null) {
                EditorGUI.LabelField(fieldRect, label, styles.label);
            }
            else if (propertyHandler.ContainsKey(val.GetType())) {

                if (val is Vector3) {
                    fieldRect.height = fieldHeight * 2;
                }
                object newVal = propertyHandler[val.GetType()](fieldRect, label, val);
                if (newVal != val) {
                    Undo.RecordObject(commandData.targetObject, "Modified buildable command input");
                    buildableCommand.io.inputValues[i] = newVal;
                    EditorUtility.SetDirty(commandData.targetObject);
                }

            }
            else if (val.GetType().IsEnum) {
                object newVal = EditorGUI.EnumPopup(fieldRect, label, (System.Enum)val);
                if (newVal != val) {
                    Undo.RecordObject(commandData.targetObject, "Modified buildable command enum input");
                    buildableCommand.io.inputValues[i] = newVal;
                    EditorUtility.SetDirty(commandData.targetObject);
                }
            }
            else {
                EditorGUI.LabelField(fieldRect, label + ": " + val, styles.label);
            }

            EditorGUIUtility.labelWidth = prevWidth;
            return fieldRect;
        }

        private static void DropBinderAreaGUI(Rect _dropArea, System.Type _objectType, System.Action<UnityEngine.Object> _onObjectChanged) {
            Event evt = Event.current;
            switch (evt.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!_dropArea.Contains(evt.mousePosition) || DragAndDrop.objectReferences.Length > 1)
                        return;

                    UnityEngine.Object draggedObject = DragAndDrop.objectReferences[0];
                    if (_objectType.IsAssignableFrom(draggedObject.GetType())) return;

                    if (!EditorUtility.IsPersistent(draggedObject)) {
                        if (draggedObject is CrossSceneBinder binderComponent) {
                            draggedObject = binderComponent.binderAsset;
                        }
                        else if (draggedObject is GameObject go) {
                            if (go.TryGetComponent<CrossSceneBinder>(out var binderComp)) {
                                draggedObject = binderComp.binderAsset;
                            }
#if TIMELINE_AVAILABLE
                            else if (go.TryGetComponent<PlayableDirector>(out var playableDirector)) {
                                binderComp = Undo.AddComponent<CrossSceneBinder>(go);
                                typeof(CrossSceneBinder).GetField("m_binderAsset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                    .SetValue(binderComp, playableDirector.playableAsset);
                                typeof(CrossSceneBinder).GetField("m_componentToBind", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                    .SetValue(binderComp, playableDirector);
                                draggedObject = playableDirector.playableAsset;
                            }
#endif
                            else {
                                draggedObject = null;
                            }
                        }
                        else if (draggedObject is Component comp) {
                            if (comp.TryGetComponent<CrossSceneBinder>(out var binderComp)) {
                                draggedObject = binderComp.binderAsset;
                            }
#if TIMELINE_AVAILABLE
                            else if (comp.TryGetComponent<PlayableDirector>(out var playableDirector)) {
                                binderComp = Undo.AddComponent<CrossSceneBinder>(comp.gameObject);
                                typeof(CrossSceneBinder).GetField("m_binderAsset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                    .SetValue(binderComp, playableDirector.playableAsset);
                                typeof(CrossSceneBinder).GetField("m_componentToBind", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                    .SetValue(binderComp, playableDirector);
                                draggedObject = playableDirector.playableAsset;
                            }
#endif
                            else {
                                draggedObject = null;
                            }
                        }
                    }

                    if (!_objectType.IsAssignableFrom(draggedObject.GetType())) {
                        draggedObject = null;
                    }

                    if (draggedObject == null) {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        return;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    if (evt.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();
                        _onObjectChanged(draggedObject);
                    }
                    evt.Use();
                    break;
            }
        }

    }
}
