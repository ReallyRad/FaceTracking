using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Reflection;
#if TIMELINE_AVAILABLE
using UnityEngine.Timeline;
#endif
using System.Linq;

namespace Calcatz.CookieCutter {

    [InitializeOnLoad]
    public class CommandInspectorWindow : CommandNodesWindow, IHasCustomMenu {

        private static UnityEngine.Object previousSelection = null;
        private static System.Type previousCommandDataType;
        private int previousInstanceId;

        private GUIStyle lockButtonStyle;
        private bool locked = false;

        private int goToNodeCommandIDDelayed = -1;

        [MenuItem("Window/Calcatz/CookieCutter/Command Inspector")]
        public static void OpenWindow() {
            CommandInspectorWindow window = GetWindow<CommandInspectorWindow>();
            window.titleContent = new GUIContent("Command Inspector");
            window.ValidateSelection(true);
            window.Show();
        }

        public override void GoToNode(int _commandId) {
            if (nodesContainer.commandData == null) {
                goToNodeCommandIDDelayed = _commandId;
            }
            else {
                base.GoToNode(_commandId);
            }
        }

        protected override void OnNodesContainerCreated() {
            base.OnNodesContainerCreated();
            nodesContainer.leftPaneWidth = 300;
            nodesContainer.useVariables = true;
            nodesContainer.useLoadSave = true;
            nodesContainer.onValidateCommandDataReference = ValidateCommandDataReference;
            nodesContainer.onCreateAssetSelection = OnCreateAssetSelection;
            nodesContainer.onCreateBeforeCommandList = OnCreateBeforeCommandList;
        }

        /// <summary>
        /// Magic method which Unity detects automatically.
        /// </summary>
        /// <param name="position">Position of button.</param>
        private void ShowButton(Rect position) {
            if (this.lockButtonStyle == null) {
                this.lockButtonStyle = "IN LockButton";
            }
            bool newLocked = GUI.Toggle(position, this.locked, GUIContent.none, this.lockButtonStyle);
            if (newLocked != locked) {
                locked = newLocked;
                if (!locked) ValidateSelection();
            }
        }

        /// <summary>
        /// Adds custom items to editor window context menu.
        /// </summary>
        /// <param name="menu">Context menu.</param>
        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu) {
            menu.AddItem(new GUIContent("Lock"), this.locked, () => {
                locked = !locked;
                if (!locked) ValidateSelection();
            });
        }

        private void ValidateSelection(bool _forceReset = false) {
            if (nodesContainer == null) return;
            bool proceed = false;
            if (locked) {
                if (_forceReset) {
                    proceed = true;
                }
            }
            else {
                if (previousSelection != Selection.activeObject || _forceReset) {
                    proceed = true;
                }
            }

            if (proceed) {
                nodesContainer.commandData = null;
                previousSelection = Selection.activeObject;
                previousInstanceId = previousSelection == null? 0 : previousSelection.GetInstanceID();
                nodesContainer.RepaintContainer();
            }
        }

        protected override void OnEnable() {
            base.OnEnable();
            ValidateSelection();
            EditorApplication.playModeStateChanged += _stateChange => {
                Selection.activeObject = EditorUtility.InstanceIDToObject(previousInstanceId);
                ValidateSelection(true);
            };
        }

        private void OnSelectionChange() {
            ValidateSelection();
        }

        private static bool IsPrefab(ICommandData _component) {
            MonoBehaviour mb = (MonoBehaviour)_component.targetObject;
            return IsPrefab(mb.gameObject);
        }

        private static bool IsPrefab(GameObject _go) {
            if (_go.scene.IsValid()) {
                return false;
            }
            return true;
        }

        protected virtual void ValidateCommandDataReference() {
            if (nodesContainer.commandData == null) {
                nodesContainer.SetCommandDataWithoutRepaint(FindCommandData());
                System.Type currentCommandDataType = nodesContainer.commandData == null ? null : nodesContainer.commandData.GetType();
                if (previousCommandDataType != currentCommandDataType) {
                    if (nodesContainer.commandData != null) {
                        var reference = CommandNodesContainer.CreateFromCommandData(nodesContainer.commandData);
                        nodesContainer.useVariables = reference.useVariables;
                        nodesContainer.useLoadSave = reference.useLoadSave;
                        nodesContainer.leftPaneWidth = reference.leftPaneWidth;
                    }
                    else {
                        nodesContainer.useVariables = false;
                        nodesContainer.useLoadSave = false;
                        nodesContainer.leftPaneWidth = 0;
                    }
                    previousCommandDataType = currentCommandDataType;
                    nodesContainer.RepaintContainer();
                }
                else if (nodesContainer.commandData != null) {
                    nodesContainer.RepaintContainer();
                }
            }

            if (nodesContainer.commandData != null) {
                if (goToNodeCommandIDDelayed >= 0) {
                    nodesContainer.GoToNode(goToNodeCommandIDDelayed);
                    goToNodeCommandIDDelayed = -1;
                }
            }
        }

        private static CommandData FindCommandData() {
            CommandData foundCommandData = null;
            if (Selection.activeObject != null) {
                if (Selection.activeObject is GameObject go) {
                    ICommandData commandDataComponent = go.GetComponent<ICommandData>();
                    if (commandDataComponent != null) {
                        foundCommandData = commandDataComponent.commandData;
                    }
                }
                else if (Selection.activeObject is ICommandData commandDataAsset) {
                    foundCommandData = commandDataAsset.commandData;
                }
#if TIMELINE_AVAILABLE
                else {
                    //Use this if some time later Unity changes m_Clip variable name
                    /*FieldInfo[] fields = Selection.activeObject.GetType().GetRuntimeFields().ToArray();
                    foreach (FieldInfo f in fields) {
                        Debug.Log(f.Name);
                    }*/
                    FieldInfo field = Selection.activeObject.GetType().GetField("m_Clip", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    if (field != null) {
                        TimelineClip clip = field.GetValue(Selection.activeObject) as TimelineClip;
                        if (clip != null) {
                            if (clip.asset is ICommandData commandDataClip) {
                                foundCommandData = commandDataClip.commandData;
                            }
                        }
                    }
                }
#endif
            }

            return foundCommandData;
        }


#region DRAW
        protected virtual void OnCreateAssetSelection(VisualElement _leftPane) {
            _leftPane.Add(new Label("Object:") {
                style = {
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            });
            ObjectField commandDataField = new ObjectField();
            commandDataField.value = nodesContainer.commandData == null ? null : nodesContainer.commandData.targetObject;
            commandDataField.objectType = typeof(ICommandData);
            commandDataField.SetEnabled(false);
            _leftPane.Add(commandDataField);
        }

        protected virtual void OnCreateBeforeCommandList(VisualElement _leftPane) {
            _leftPane.Add(new Button(HandleLastExecutedCommandButton) {
                text = "Last Executed Command",
                style = {
                    marginTop = new StyleLength(new Length(CommandNode.verticalSpacing, LengthUnit.Pixel)),
                    marginBottom = new StyleLength(new Length(CommandNode.verticalSpacing, LengthUnit.Pixel))
                }
            });
        }

        private void HandleLastExecutedCommandButton() {
            if (nodesContainer.commandData.lastExecutedCommand == null) return;
            CommandData currentCommandData = nodesContainer.commandData;
            CommandNode commandNode;
            while (true) {
                commandNode = nodesContainer.GetCommandNode(currentCommandData.lastExecutedCommand.id);
                if (commandNode != null) {
                    if (currentCommandData.lastExecutedCommand is ISubDataCommand) {
                        currentCommandData = ((ISubDataCommand)currentCommandData.lastExecutedCommand).subData;
                        Selection.activeObject = currentCommandData.targetObject;
                        ValidateSelection();
                    }
                    else {
                        nodesContainer.GoToNode(commandNode);
                        break;
                    }
                }
                else {
                    break;
                }
            }
        }
#endregion
    }
}
