using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Calcatz.CookieCutter {
    public class NewCommandDataTypeWizard : EditorWindow {

        [MenuItem("Window/Calcatz/CookieCutter/New Command Data Type Wizard")]
        private static void OpenWindow() {
            NewCommandDataTypeWizard window = GetWindow<NewCommandDataTypeWizard>();
            window.titleContent.text = "New Command Data Type Wizard";
            window.Show();
        }

        public enum ObjectType {
            None, Asset, TimelineClip, Component
        }

        [SerializeField] private string path;
        [SerializeField] private string editorPath;
        [SerializeField] private ObjectType objectType = ObjectType.Asset;
        [SerializeField] private string newTypeName;
        [SerializeField] private string targetNamespace = "Calcatz.CookieCutter";

        private bool createdSuccessfully;
        private bool createPressed;

        private void OnGUI() {
            GUILayout.Label("New Command Data Type Wizard", EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.Space();

            string newPath = EditorGUILayout.TextField("Output Directory", path);
            if (newPath != path) {
                Undo.RecordObject(this, "change path");
                path = newPath;
            }

            if (GUILayout.Button("Select Output Directory")) {
                newPath = EditorUtility.OpenFolderPanel("Select Output Directory", path, "");
                if (newPath != path) {
                    Undo.RecordObject(this, "change path");
                    path = newPath;
                }
            }

            EditorGUILayout.Space();

            string newEditorPath = EditorGUILayout.TextField("Editor Output Directory", editorPath);
            if (newEditorPath != editorPath) {
                Undo.RecordObject(this, "change editor path");
                editorPath = newEditorPath;
            }

            if (GUILayout.Button("Select Editor Output Directory")) {
                newEditorPath = EditorUtility.OpenFolderPanel("Select Editor Output Directory", editorPath, "");
                if (newEditorPath != editorPath) {
                    Undo.RecordObject(this, "change editor path");
                    editorPath = newEditorPath;
                }
            }

            EditorGUILayout.Space();

            string newNewTypeName = EditorGUILayout.TextField(new GUIContent("New Type Name", "Should respect to class naming rules (pascal case, no white space)."), newTypeName);
            if (newNewTypeName != newTypeName) {
                Undo.RecordObject(this, "change new type name");
                newTypeName = newNewTypeName;
            }

            ObjectType newObjectType = (ObjectType)EditorGUILayout.EnumPopup("Object Type Template", objectType);
            if (newObjectType != objectType) {
                Undo.RecordObject(this, "change object type");
                objectType = newObjectType;
            }

            string newTargetNamespace = EditorGUILayout.TextField("Target Namespace", targetNamespace);
            if (newTargetNamespace != targetNamespace) {
                Undo.RecordObject(this, "change target namespace");
                targetNamespace = newTargetNamespace;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            bool guiEnabled = GUI.enabled;
            GUI.enabled = guiEnabled && path != null && path != "" && editorPath != null && editorPath != "";
            if (GUILayout.Button("Create")) {
                createPressed = true;
                createdSuccessfully = CreateCommandDataType();
            }
            GUI.enabled = guiEnabled;

            if (createPressed) {
                EditorGUILayout.Space();
                if (createdSuccessfully) {
                    EditorGUILayout.HelpBox("Success", MessageType.Info);
                }
                else {
                    EditorGUILayout.HelpBox("Error", MessageType.Error);
                }
            }
        }

        private bool CreateCommandDataType() {
            try {
                string commandDataObjectFilePath = path + "/";

#if ODIN_INSPECTOR
                string odinSerializerNamespace = "Sirenix.Serialization";
#else
            string odinSerializerNamespace = "Calcatz.OdinSerializer";
#endif
                string commandDataObjectFile = "";
                switch (objectType) {
                    case ObjectType.Asset:
                        commandDataObjectFilePath += newTypeName + "Asset.cs";
                        commandDataObjectFile = string.Format(CreateCommandDataAssetString(),
                            odinSerializerNamespace,
                            targetNamespace,
                            newTypeName);
                        break;
                    case ObjectType.Component:
                        commandDataObjectFilePath += newTypeName + ".cs";
                        commandDataObjectFile = string.Format(CreateCommandDataComponentString(),
                            odinSerializerNamespace,
                            targetNamespace,
                            newTypeName);
                        break;
                    case ObjectType.TimelineClip:
                        commandDataObjectFilePath += newTypeName + "Clip.cs";
                        commandDataObjectFile = string.Format(CreateCommandDataTimelineClipString(),
                            odinSerializerNamespace,
                            targetNamespace,
                            newTypeName);
                        break;
                }
                string commandDataFile = string.Format(CreateCommandDataString(), targetNamespace, newTypeName);
                string nodesContainerFile = string.Format(CreateNodesContainerString(objectType == ObjectType.Component), targetNamespace, newTypeName);
                string blankCommandFile = string.Format(CreateBlankCommandString(), targetNamespace, newTypeName);
                string blankCommandNodeFile = string.Format(CreateBlankCommandNodeString(), targetNamespace, newTypeName);

                if (objectType != ObjectType.None) {
                    string filePath = commandDataObjectFilePath;
                    SaveFile(filePath, commandDataObjectFile);
                }
                SaveFile(path + "/" + newTypeName + "CommandData.cs", commandDataFile);
                SaveFile(path + "/" + newTypeName + "Command.cs", blankCommandFile);

                SaveFile(editorPath + "/" + newTypeName + "NodesContainer.cs", nodesContainerFile);
                SaveFile(editorPath + "/" + newTypeName + "CommandNode.cs", blankCommandNodeFile);

                if (objectType == ObjectType.Asset) {
                    string assetEditorFile = string.Format(CreateAssetEditorString(), targetNamespace, newTypeName);
                    SaveFile(editorPath + "/" + newTypeName + "AssetEditor.cs", assetEditorFile);
                }

                AssetDatabase.Refresh();
                return true;
            }
            catch (System.Exception _e) {
                Debug.LogError(_e);
                return false;
            }
        }

        private static void SaveFile(string filePath, string commandDataObjectFile) {
            if (File.Exists(filePath)) {
                Debug.LogError("File already exists and not replaced for safety: " + filePath);
            }
            else {
                File.WriteAllText(filePath, commandDataObjectFile);
            }
        }

        private string CreateCommandDataAssetString() {
            return @"using UnityEngine;
using Calcatz.CookieCutter;
using System.Collections.Generic;
using {0};

namespace {1} {{

    [CreateAssetMenu(fileName = ""{2}_New"", menuName = ""Command Data Asset/{2} Asset"", order = 1)]
    public class {2}Asset : ScriptableObjectCommandData {{

        public override void ValidateObject() {{
            base.ValidateObject();
            if (commandData == null) {{
                commandData = new {2}CommandData(this);
            }}
        }}

        [OdinSerialize, HideInInspector]
        private {2}CommandData m_commandData;
        public override CommandData commandData {{ get => m_commandData; set => m_commandData = ({2}CommandData)value; }}

    }}
}}";
        }

        private string CreateCommandDataComponentString() {
            return @"using UnityEngine;
using Calcatz.CookieCutter;
using System.Collections.Generic;
using {0};

namespace {1} {{

    public class {2} : MonoBehaviourCommandData {{

        public override void ValidateObject() {{
            base.ValidateObject();
            if (commandData == null) {{
                commandData = new {2}CommandData(this);
            }}
        }}

        [OdinSerialize, HideInInspector]
        private {2}CommandData m_commandData;
        public override CommandData commandData {{ get => m_commandData; set => m_commandData = ({2}CommandData)value; }}

    }}
}}";
        }

        private string CreateCommandDataTimelineClipString() {
            return @"using UnityEngine;
using Calcatz.CookieCutter;
using System.Collections.Generic;
using {0};
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace {1} {{

    public class {2}Clip : TimelineCommandDataClip {{

        public override void ValidateObject() {{
            base.ValidateObject();
            if (commandData == null) {{
                commandData = new {2}CommandData(this);
            }}
        }}

        [OdinSerialize][ShowCommandNodes(450, true)]
#if ODIN_INSPECTOR
        [HideReferenceObjectPicker]
#endif
        private {2}CommandData m_commandData;
        public override CommandData commandData {{ get => m_commandData; set => m_commandData = ({2}CommandData)value; }}

    }}
}}";
        }

        private string CreateCommandDataString() {
            return @"using UnityEngine;
using Calcatz.CookieCutter;
using System;
using System.Collections;
using System.Collections.Generic;

namespace {0} {{
    [Serializable]
    public class {1}CommandData : CommandData {{

        public {1}CommandData(UnityEngine.Object _targetObject) : base(_targetObject) {{ }}

    }}
}}";
        }

        private string CreateNodesContainerString(bool _addCommandNodesPreview) {
            return @"using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Calcatz.CookieCutter;
using System;

namespace {0} {{

    [CommandNodesContainer(typeof({1}CommandData))]
    public class {1}NodesContainer : CommandNodesContainer {{

        public {1}NodesContainer() {{
            leftPaneWidth = 300;
            useVariables = true;
        }}

" + (_addCommandNodesPreview? @"
        [CustomPreview(typeof({1}))]
        public class CommandNodesPreview : CommandDataInspectorPreview {{ }}
" : "") + @"
        protected override void ValidateData() {{
            //base.ValidateData();
            if (!commandData.commands.ContainsKey(1)) {{
                commandData.commands.Add(1, new {1}Command() {{ id = 1, nodePosition = Vector2.zero + Vector2.up * 200 }});
            }}
            /*if (!commandData.commands.ContainsKey(2)) {{
                commandData.commands.Add(2, new {1}Command() {{ id = 2, nodePosition = commandData.commands[1].nodePosition - Vector2.up * 200 }});
            }}*/
        }}

        protected override void ValidateAvailableCommands(Type _commandDataType) {{
            base.ValidateAvailableCommands(_commandDataType);
            CommandRegistry.RegisterCommand<{1}CommandData>(typeof({1}Command), false, """");
        }}

        protected override void OnReloadReservedNodes(CommandNode _node) {{
            switch (_node.GetCommand().id) {{
                case 1:
                    SetNodeStyle(_node, ""5"");
                    break;
                /*case 2:
                    SetNodeStyle(_node, ""2"");
                    break;*/
            }}
        }}

    }}
}}";
        }

        private string CreateBlankCommandString() {
            return @"using Calcatz.CookieCutter;

namespace {0} {{

    /// <summary>
    /// Blank command for {1} command data.
    /// </summary>
    [System.Serializable]
    public class {1}Command : Command {{

        

    }}

}}";
        }

        private string CreateBlankCommandNodeString() {
            return @"using UnityEngine;
using Calcatz.CookieCutter;

namespace {0} {{

    [CustomCommandNodeDrawer(typeof({1}Command))]
    public class {1}CommandNode : BlankCommandNode {{

        private static readonly string[] reservedNodeNames = new string[] {{ ""None"", ""Start"" /*, ""Alternate Start""*/ }};
        public override string[] ReservedNodeNames => reservedNodeNames;

        public {1}CommandNode(CommandData _commandData, Command _command, Vector2 position, Config config)
            : base(_commandData, _command, position, config) {{
        }}

    }}
}}";
        }

        private string CreateAssetEditorString() {
            return @"using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Calcatz.CookieCutter;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace {0} {{

    public class {1}AssetEditor : ScriptableObjectCommandNodesWindow<{1}Asset, {1}AssetEditor> {{

        [CustomPreview(typeof({1}Asset))]
        public class CommandNodesPreview : CommandDataInspectorPreview {{ }}

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line) {{
            return HandleOpenAsset<{1}Asset>(instanceID);
        }}

        protected override CommandNodesContainer CreateNodesContainer() => new {1}NodesContainer();

        #region DRAW
        protected override void OnCreateBeforeCommandList(VisualElement _leftPane) {{
            base.OnCreateBeforeCommandList(_leftPane);
            //{1}Asset asset = ({1}Asset)nodesContainer.commandData.targetObject;
            //Create related GUI if necessary, given the asset
        }}
        #endregion
    }}

}}";
        }

    }
}