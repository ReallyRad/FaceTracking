using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Calcatz.CookieCutter;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Calcatz.CookieCutter {
    /// <summary>
    /// Command Node Editor for Command Data contained in ScriptableObject object.
    /// </summary>
    /// <typeparam name="AssetType">Type of the ScriptableObject</typeparam>
    /// <typeparam name="WindowType">Type of the EditorWindow (Pass its self type)</typeparam>
    public class ScriptableObjectCommandNodesWindow<AssetType, WindowType> : CommandNodesWindow where AssetType : ICommandData {

        [SerializeField] private AssetType assetToEdit;

        private static readonly Dictionary<AssetType, ScriptableObjectCommandNodesWindow<AssetType, WindowType>> openedWindows = new Dictionary<AssetType, ScriptableObjectCommandNodesWindow<AssetType, WindowType>>();

        public static T GetAssetWindow<T>(AssetType _asset) where T : ScriptableObjectCommandNodesWindow<AssetType, WindowType> {
            ScriptableObjectCommandNodesWindow<AssetType, WindowType> result;
            if (openedWindows.TryGetValue(_asset, out result)) {
                return result as T;
            }
            return null;
        }

        private static CommandNodesWindow GetAssetWindow(AssetType _asset) {
            ScriptableObjectCommandNodesWindow<AssetType, WindowType> result;
            if (openedWindows.TryGetValue(_asset, out result)) {
                return result;
            }
            return null;
        }

        protected static bool HandleOpenAsset<T>(int _instanceId) {
            Object obj = EditorUtility.InstanceIDToObject(_instanceId);
            if (obj != null && obj is T _data) {
                ICommandData asset = (ICommandData)_data;
                asset.ValidateObject();
                OpenWindow((AssetType)asset);
                return true; //catch open file
            }

            return false; // let unity open the file
        }

        public static WindowType OpenAssetEditor(AssetType _asset) {
            if (ReferenceEquals(_asset, null)) return default;
            int instanceId = (_asset as UnityEngine.Object).GetInstanceID();
            if (HandleOpenAsset<AssetType>(instanceId)) {
                return (WindowType)(object)GetAssetWindow(_asset);
            }
            return default;
        }

        public static ScriptableObjectCommandNodesWindow<AssetType, WindowType> OpenWindow(AssetType _asset) {
            ScriptableObjectCommandNodesWindow<AssetType, WindowType> window;
            if (openedWindows.ContainsKey(_asset)) {
                window = (ScriptableObjectCommandNodesWindow<AssetType, WindowType>)(object)openedWindows[_asset];
            }
            else {
                window = (ScriptableObjectCommandNodesWindow<AssetType, WindowType>)(object)CreateInstance(typeof(WindowType));
                window.nodesContainer.commandData = _asset.commandData;
                window.titleContent = new GUIContent(_asset.Name);
                window.assetToEdit = _asset;
                openedWindows.Add(_asset, window);
            }

            window.nodesContainer.ReloadNodes();
            window.Show();
            window.ShowTab();
            return window;
        }

        protected override void OnEnable() {
            base.OnEnable();
            if (assetToEdit != null) {
                if (!openedWindows.ContainsKey(assetToEdit)) {
                    openedWindows.Add(assetToEdit, this);
                }
            }
        }

        protected virtual void OnDestroy() {
            openedWindows.Remove(assetToEdit);
        }

        protected override void OnNodesContainerCreated() {
            base.OnNodesContainerCreated();
            nodesContainer.onValidateCommandDataReference = ValidateCommandDataReference;
            nodesContainer.onCreateAssetSelection = OnCreateAssetSelection;
            nodesContainer.onCreateBeforeCommandList = OnCreateBeforeCommandList;
        }

        protected virtual void ValidateCommandDataReference() {
            if (nodesContainer.commandData == null) {
                try {
                    nodesContainer.commandData = assetToEdit.commandData;
                    if (nodesContainer.commandData == null) {
                        Close();
                    }
                }
                catch {
                    Close();
                }
            }
        }

        #region DRAW
        protected virtual void OnCreateAssetSelection(VisualElement _leftPane) {
            var imguiContainer = new IMGUIContainer(OnDrawAssetSelection) {
                style = {
                    height = EditorGUIUtility.singleLineHeight * 2
                }
            };
            _leftPane.Add(imguiContainer);
        }

        private void OnDrawAssetSelection() {
            GUILayout.Label("Asset:", EditorStyles.boldLabel);
            bool guiEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.ObjectField(nodesContainer.commandData == null ? null : nodesContainer.commandData.targetObject, typeof(AssetType), false);
            GUI.enabled = guiEnabled;
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
            ScriptableObjectCommandNodesWindow<AssetType, WindowType> window = this;
            CommandData currentCommandData = nodesContainer.commandData;
            CommandNode commandNode;
            while (true) {
                commandNode = window.nodesContainer.GetCommandNode(currentCommandData.lastExecutedCommand.id);
                if (commandNode != null) {
                    window.nodesContainer.GoToNode(commandNode);
                    if (currentCommandData.lastExecutedCommand is ISubDataCommand) {
                        currentCommandData = ((ISubDataCommand)currentCommandData.lastExecutedCommand).subData;
                        window = OpenWindow((AssetType)(ICommandData)currentCommandData.targetObject);
                    }
                    else {
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
