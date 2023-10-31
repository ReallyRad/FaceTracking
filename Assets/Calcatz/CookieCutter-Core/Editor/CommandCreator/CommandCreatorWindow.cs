using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Calcatz.CookieCutter {

    public class CommandCreatorWindow : EditorWindow {

        private const int scriptConfigLineHeight = 6;

        [SerializeField] private DummyNodesContainer nodesContainer;
        [SerializeField] private CommandConstructor commandConstructor = new CommandConstructor();

        private SerializedObject serializedObject;

        [MenuItem("Window/Calcatz/CookieCutter/Command Creator")]
        public static void OpenWindow() {
            var window = GetWindow<CommandCreatorWindow>();
            window.titleContent = new GUIContent("Command Creator");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }

        private void OnEnable() {
            serializedObject = new SerializedObject(this);
            nodesContainer = new DummyNodesContainer();
            nodesContainer.containerAreaGetter = () => new Rect(400, 0, position.width, position.height - EditorGUIUtility.singleLineHeight * scriptConfigLineHeight + 10);
            nodesContainer.leftPaneWidth = 0;
            nodesContainer.useVariables = false;
            nodesContainer.useLoadSave = false;
            nodesContainer.onValidateCommandDataReference = ValidateCommandDataReference;
            RefreshCommandDataInclusions();
        }

        private void RefreshCommandDataInclusions() {
            var newInclusionList = new List<CommandDataInclusion>();

            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<CommandData>();
            foreach (var commandDataType in types) {
                string commandDataName = ObjectNames.NicifyVariableName(commandDataType.Name);
                var currentInclusion = commandConstructor.includedCommandData.Find(_inclusion => _inclusion.name == commandDataName);
                if (currentInclusion != null) {
                    currentInclusion.type = commandDataType;
                    newInclusionList.Add(currentInclusion);
                }
                else {
                    newInclusionList.Add(new CommandDataInclusion() {
                        name = commandDataName,
                        type = commandDataType,
                        include = false
                    });
                }
            }
            commandConstructor.includedCommandData = newInclusionList;
        }

        protected virtual void ValidateCommandDataReference() {
            if (nodesContainer.commandData == null) {
                var commandData = new CommandData(this);
                if (!commandData.commands.ContainsKey(100)) {
                    commandData.commands.Add(100, new DummyCommand() { id = 100, nodePosition = Vector2.zero + Vector2.up * 10 + Vector2.right * 10 });
                }
                nodesContainer.commandData = commandData;
            }
        }

        private Vector2 linesScrollPos;
        private Vector2 commandDataScrollPos;

        private void OnGUI() {
            EditorGUI.HelpBox(new Rect(0, 0, 400, position.height), "", MessageType.None);

            serializedObject.Update();
            var constructorProp = serializedObject.FindProperty("commandConstructor");
            GUILayout.BeginArea(new Rect(5, 5, 385, position.height - 10));
            {
                EditorGUILayout.PropertyField(constructorProp.FindPropertyRelative("displayName"));
                EditorGUILayout.PropertyField(constructorProp.FindPropertyRelative("tooltip"));
                EditorGUILayout.PropertyField(constructorProp.FindPropertyRelative("nodeWidth"));
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(constructorProp.FindPropertyRelative("isPropertyCommand"));
                if (!commandConstructor.isPropertyCommand) {
                    EditorGUILayout.PropertyField(constructorProp.FindPropertyRelative("titleHasOutPoint"));
                }
                if (EditorGUI.EndChangeCheck()) {
                    RefreshNodesContainer();
                }

                linesScrollPos = GUILayout.BeginScrollView(linesScrollPos);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(constructorProp.FindPropertyRelative("lines"));
                if (EditorGUI.EndChangeCheck()) {
                    RefreshNodesContainer();
                }
                GUILayout.Space(10);
                GUILayout.EndScrollView();

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Load Template...")) {
                        LoadTemplate();
                    }
                    if (GUILayout.Button("Save Template...")) {
                        SaveTemplate();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();

            float scriptAreaHeight = (scriptConfigLineHeight-1) * EditorGUIUtility.singleLineHeight + 10;
            EditorGUI.HelpBox(new Rect(400, position.height- scriptAreaHeight, position.width-400, scriptAreaHeight), "", MessageType.None);

            GUILayout.BeginArea(new Rect(405, position.height - scriptAreaHeight + 5, position.width - 400 - 10, scriptAreaHeight - 10));
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(constructorProp.FindPropertyRelative("className"));
                    EditorGUILayout.PropertyField(constructorProp.FindPropertyRelative("_namespace"));

                    GUILayout.Label("Include in Command Data:");
                    var commandDataList = constructorProp.FindPropertyRelative("includedCommandData");
                    commandDataScrollPos = GUILayout.BeginScrollView(commandDataScrollPos, false, false);
                    GUILayout.BeginHorizontal();
                    {
                        for (int i=0; i<commandDataList.arraySize; i++) {
                            var commandDataProp = commandDataList.GetArrayElementAtIndex(i);
                            var includedProp = commandDataProp.FindPropertyRelative("include");
                            includedProp.boolValue = EditorGUILayout.ToggleLeft(commandDataProp.FindPropertyRelative("name").stringValue, includedProp.boolValue);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndScrollView();

                    GUILayout.EndVertical();
                    if (GUILayout.Button("Generate\nScript...", GUILayout.ExpandHeight(true))) {
                        GenerateScript();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
            serializedObject.ApplyModifiedProperties();

            if (nodesContainer != null) {
                DrawNodesContainer();
            }
        }

        private void RefreshNodesContainer() {
            nodesContainer.commandData = null;
            ValidateCommandDataReference();
        }

        private void GenerateScript() {
            RefreshCommandDataInclusions();
            bool includeAtLeast1CommandData = false;
            foreach(var inclusion in commandConstructor.includedCommandData) {
                if (inclusion.include) {
                    includeAtLeast1CommandData = true;
                    break;
                }
            }
            if (!includeAtLeast1CommandData) {
                EditorUtility.DisplayDialog("Error", "The command must be included in at least 1 command data.", "Ok");
                return;
            }
            var path = EditorUtility.SaveFilePanel("Generate Script File...", Application.dataPath, commandConstructor.className + ".cs", "cs");
            if (path != null && path !="") {
                var generatpr = new CommandScriptGenerator();
                generatpr.Generate(commandConstructor, path);
            }
        }

        private void SaveTemplate() {
            var path = EditorUtility.SaveFilePanel("Save Template...", Application.dataPath, commandConstructor.className + ".json", "json");
            if (path != null && path != "") {
                File.WriteAllText(path, JsonUtility.ToJson(commandConstructor));
                AssetDatabase.Refresh();
            }
        }
        private void LoadTemplate() {
            var path = EditorUtility.OpenFilePanel("Load Template...", Application.dataPath, "json");
            if (path != null && path != "") {
                var content = File.ReadAllText(path);
                commandConstructor = JsonUtility.FromJson<CommandConstructor>(content);
                RefreshNodesContainer();
            }
        }

        #region DRAW
        private void DrawNodesContainer() {
            bool guiEnabled = GUI.enabled;
            GUI.enabled = false;

            if (nodesContainer.commandData != null) {
                if (nodesContainer.commandData.commands != null) {
                    if (nodesContainer.commandData.commands.TryGetValue(100, out var cmd)) {
                        if (cmd is DummyCommand dummyCommand) {
                            DrawDummyCommand(dummyCommand);
                        }
                    }
                }
            }

            nodesContainer.OnDrawNodesArea();
            GUI.enabled = guiEnabled;

        }

        private void DrawDummyCommand(DummyCommand _command) {

            var node = nodesContainer.GetCommandNode(_command.id);
            node.nodeName = commandConstructor.displayName.Split('/').Last();
            node.rect.width = commandConstructor.nodeWidth;

            _command.onDrawTitle = OnDrawTitle;
            _command.onDrawContents = OnDrawContents;
        }

        private string OnDrawTitle() {
            if (!commandConstructor.isPropertyCommand) {
                CommandGUI.AddMainInPoint();
            }

            if (!commandConstructor.isPropertyCommand) {
                if (commandConstructor.titleHasOutPoint) {
                    CommandGUI.AddMainOutPoint();
                }
            }

            foreach(var line in commandConstructor.lines) {
                if (line.contentType == CommandLine.ContentType.InputField) {
                    if (line.pointLocation == CommandLine.PointLocation.Left) {
                        var forcedPointType = CommandLine.fieldPointTypes[line.fieldType];
                        var connectionType = CommandLine.connectionTypes[forcedPointType];
                        var propertyType = PropertyConnectionPoint.variableTypes[connectionType];
                        CommandGUI.AddPropertyInPoint(propertyType);
                    }
                    
                }
                else {
                    if (line.pointLocation == CommandLine.PointLocation.Left) {
                        if (line.pointType == CommandLine.PointType.Main) {
                            CommandGUI.AddMainInPoint();
                        }
                        else {
                            var connectionType = CommandLine.connectionTypes[line.pointType];
                            CommandGUI.AddPropertyInPoint(PropertyConnectionPoint.variableTypes[connectionType]);
                        }
                    }
                    else if (line.pointLocation == CommandLine.PointLocation.Right) {
                        if (line.pointType == CommandLine.PointType.Main) {
                            CommandGUI.AddMainOutPoint();
                        }
                        else {
                            var connectionType = CommandLine.connectionTypes[line.pointType];
                            CommandGUI.AddPropertyOutPoint(PropertyConnectionPoint.variableTypes[connectionType]);
                        }
                    }
                }
            }

            if (!commandConstructor.isPropertyCommand) {
                CommandGUI.DrawInPoint(0);
                if (commandConstructor.titleHasOutPoint) {
                    CommandGUI.DrawOutPoint(0);
                }
            }
            return commandConstructor.tooltip;
        }

        private void OnDrawContents(Vector2 _absPosition) {
            int currentInPoint = commandConstructor.isPropertyCommand? 0 : 1;
            int currentOutPoint = !commandConstructor.isPropertyCommand && commandConstructor.titleHasOutPoint? 1 : 0;
            foreach(var line in commandConstructor.lines) {

                if (line.contentType == CommandLine.ContentType.InputField) {
                    if (line.pointLocation == CommandLine.PointLocation.Left) {
                        CommandGUI.DrawInPoint(currentInPoint++);
                    }
                }
                else {
                    if (line.pointLocation == CommandLine.PointLocation.Left) {
                        CommandGUI.DrawInPoint(currentInPoint++);
                    }
                    else if (line.pointLocation == CommandLine.PointLocation.Right) {
                        CommandGUI.DrawOutPoint(currentOutPoint++);
                    }
                }

                if (line.contentType == CommandLine.ContentType.None) {
                    CommandGUI.AddRectHeight(EditorGUIUtility.singleLineHeight);
                }
                else if (line.contentType == CommandLine.ContentType.Label) {
                    CommandGUI.DrawLabel(line.label, line.alignRight);
                }
                else {
                    HandleFieldContent(line);
                }
                CommandGUI.AddRectHeight(line.spacing);
            }
        }

        private static void HandleFieldContent(CommandLine line) {
            if (line.fieldType == CommandLine.FieldType.Text) {
                string dummyTarget = null;
                CommandGUI.DrawTextField(line.label, line.tooltip, ref dummyTarget);
            }
            else if (line.fieldType == CommandLine.FieldType.TextArea) {
                string dummyTarget = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras at pretium neque. Sed vehicula iaculis ex sit amet fringilla. Aenean nisl risus, auctor sed ullamcorper et, iaculis sed lorem.";
                CommandGUI.DrawTextAreaField(line.label, line.tooltip, ref dummyTarget, line.lineCount, line.wordWrap);
            }
            else if (line.fieldType == CommandLine.FieldType.RoundedFloat) {
                float dummyTarget = 0;
                if (line.useMin && line.useMax) {
                    dummyTarget = (line.min + line.max) / 2f;
                    CommandGUI.DrawRoundedFloatField(line.label, line.tooltip, ref dummyTarget, line.min, line.max);
                }
                else if (line.useMin) {
                    dummyTarget = line.min;
                    CommandGUI.DrawRoundedFloatField(line.label, line.tooltip, ref dummyTarget, line.min);
                }
                else if (line.useMax) {
                    dummyTarget = line.max;
                    CommandGUI.DrawRoundedFloatField(line.label, line.tooltip, ref dummyTarget, float.MinValue, line.max);
                }
                else {
                    CommandGUI.DrawRoundedFloatField(line.label, line.tooltip, ref dummyTarget);
                }
            }
            else if (line.fieldType == CommandLine.FieldType.Float) {
                float dummyTarget = 0;
                if (line.useMin && line.useMax) {
                    dummyTarget = (line.min + line.max) / 2f;
                    CommandGUI.DrawFloatField(line.label, line.tooltip, ref dummyTarget, line.min, line.max);
                }
                else if (line.useMin) {
                    dummyTarget = line.min;
                    CommandGUI.DrawFloatField(line.label, line.tooltip, ref dummyTarget, line.min);
                }
                else if (line.useMax) {
                    dummyTarget = line.max;
                    CommandGUI.DrawFloatField(line.label, line.tooltip, ref dummyTarget, float.MinValue, line.max);
                }
                else {
                    CommandGUI.DrawFloatField(line.label, line.tooltip, ref dummyTarget);
                }
            }
            else if (line.fieldType == CommandLine.FieldType.FloatSlider) {
                float dummyTarget = (line.min + line.max) / 2f;
                CommandGUI.DrawFloatSliderField(line.label, line.tooltip, ref dummyTarget, line.min, line.max);
            }
            else if (line.fieldType == CommandLine.FieldType.Int) {
                int dummyTarget = 0;
                if (line.useMin && line.useMax) {
                    dummyTarget = (int)((float)(line.min + line.max) / 2f);
                    CommandGUI.DrawIntField(line.label, line.tooltip, ref dummyTarget, (int)line.min, (int)line.max);
                }
                else if (line.useMin) {
                    dummyTarget = (int)line.min;
                    CommandGUI.DrawIntField(line.label, line.tooltip, ref dummyTarget, (int)line.min);
                }
                else if (line.useMax) {
                    dummyTarget = (int)line.max;
                    CommandGUI.DrawIntField(line.label, line.tooltip, ref dummyTarget, int.MinValue, (int)line.max);
                }
                else {
                    CommandGUI.DrawIntField(line.label, line.tooltip, ref dummyTarget);
                }
            }
            else if (line.fieldType == CommandLine.FieldType.IntSlider) {
                int dummyTarget = (int)((float)(line.min + line.max) / 2f);
                CommandGUI.IntSliderField(line.label, line.tooltip, ref dummyTarget, (int)line.min, (int)line.max);
            }
            else if (line.fieldType == CommandLine.FieldType.Toggle) {
                bool dummyTarget = false;
                CommandGUI.DrawToggleField(line.label, line.tooltip, ref dummyTarget);
            }
            else if (line.fieldType == CommandLine.FieldType.ToggleLeft) {
                bool dummyTarget = false;
                CommandGUI.DrawToggleLeftField(line.label, line.tooltip, ref dummyTarget);
            }
            else if (line.fieldType == CommandLine.FieldType.Object) {
                UnityEngine.Object dummyTarget = null;
                CommandGUI.DrawObjectField(line.label, line.tooltip, dummyTarget, _dummyTarget => dummyTarget = _dummyTarget, line.allowSceneObjects);
            }
            else if (line.fieldType == CommandLine.FieldType.Color) {
                var dummyTarget = new Color();
                CommandGUI.DrawColorField(line.label, line.tooltip, ref dummyTarget);
            }
            else if (line.fieldType == CommandLine.FieldType.Curve) {
                var dummyTarget = new AnimationCurve();
                CommandGUI.DrawCurveField(line.label, line.tooltip, dummyTarget, _dummyTarget => dummyTarget = _dummyTarget);
            }
            else if (line.fieldType == CommandLine.FieldType.Enum) {
                CommandLine.FieldType dummyTarget = CommandLine.FieldType.Enum;
                CommandGUI.DrawEnumField(line.label, line.tooltip, ref dummyTarget);
            }
            else if (line.fieldType == CommandLine.FieldType.Vector3) {
                var dummyTarget = new Vector3();
                CommandGUI.DrawVector3Field(line.label, line.tooltip, ref dummyTarget);
            }
            else if (line.fieldType == CommandLine.FieldType.Vector3Int) {
                var dummyTarget = new Vector3Int();
                CommandGUI.DrawVector3IntField(line.label, line.tooltip, ref dummyTarget);
            }
            else if (line.fieldType == CommandLine.FieldType.Vector2) {
                var dummyTarget = new Vector2();
                CommandGUI.DrawVector2Field(line.label, line.tooltip, ref dummyTarget);
            }
            else if (line.fieldType == CommandLine.FieldType.Vector2Int) {
                var dummyTarget = new Vector2Int();
                CommandGUI.DrawVector2IntField(line.label, line.tooltip, ref dummyTarget);
            }
            else if (line.fieldType == CommandLine.FieldType.Vector4) {
                var dummyTarget = new Vector4();
                CommandGUI.DrawVector4Field(line.label, line.tooltip, ref dummyTarget);
            }
            else if (line.fieldType == CommandLine.FieldType.Rect) {
                var dummyTarget = new Rect();
                CommandGUI.DrawRectField(line.label, line.tooltip, ref dummyTarget);
            }
            else if (line.fieldType == CommandLine.FieldType.RectInt) {
                var dummyTarget = new RectInt();
                CommandGUI.DrawRectIntField(line.label, line.tooltip, ref dummyTarget);
            }
            else if (line.fieldType == CommandLine.FieldType.Popup) {
                CommandGUI.DrawPopupField(line.label, line.tooltip, 0, new string[] { "Option1" }, _index => { });
            }
        }

        #endregion

        #region CLASSES
        [System.Serializable]
        public class CommandConstructor {

            [Tooltip("You can put '/' in front of the name to put into specific category.")]
            public string displayName = "New Command";
            [TextArea(2, 5)]
            public string tooltip = "This is a new custom command.";
            [Min(50)]
            public float nodeWidth = 250f;
            [Tooltip("Property commands have no Main input/output at all.\nSo, their \"Execute\" method is not used.\nTheir outputs only return properties which then used by another Command.\n")]
            public bool isPropertyCommand = false;
            public bool titleHasOutPoint = true;
            public List<CommandLine> lines = new List<CommandLine>() {
                new CommandLine()
            };
            public string className = "NewCommand";
            public string _namespace = "MyNamespace";

            public List<CommandDataInclusion> includedCommandData = new List<CommandDataInclusion>();
        }

        [System.Serializable]
        public class CommandLine {

            public static readonly Dictionary<PointType, Type> connectionTypes = new Dictionary<PointType, Type>() {
                { PointType.Main, typeof(MainConnectionPoint) },
                { PointType.Boolean, typeof(BooleanConnectionPoint) },
                { PointType.Integer, typeof(IntConnectionPoint) },
                { PointType.Float, typeof(FloatConnectionPoint) },
                { PointType.String, typeof(StringConnectionPoint) },
                { PointType.Vector3, typeof(Vector3ConnectionPoint) },
                { PointType.UnityObject, typeof(ObjectConnectionPoint) },
                { PointType.Unspecified, typeof(PropertyConnectionPoint) }
            };

            public enum PointLocation {
                None, Left, Right
            }
            public enum ContentType {
                InputField, Label, None
            }
            public enum PointType {
                Main, Boolean, Integer, Float, String, Vector3, UnityObject, Unspecified
            }
            public enum OutMainType {
                AlternateExit, Parallel
            }
            public enum FieldType {
                Text, TextArea, RoundedFloat, Float, FloatSlider, Int, IntSlider, Toggle, ToggleLeft, Object,
                Color, Curve, Enum, Vector3, Vector3Int, Vector2, Vector2Int, Vector4, Rect, RectInt, Popup
            }

            public static readonly Dictionary<FieldType, PointType> fieldPointTypes = new Dictionary<FieldType, PointType>() {
                { FieldType.Text, PointType.String },
                { FieldType.TextArea, PointType.String },
                { FieldType.RoundedFloat, PointType.Float },
                { FieldType.Float, PointType.Float },
                { FieldType.FloatSlider, PointType.Float },
                { FieldType.Int, PointType.Integer },
                { FieldType.IntSlider, PointType.Integer },
                { FieldType.Toggle, PointType.Boolean },
                { FieldType.ToggleLeft, PointType.Boolean },
                { FieldType.Object, PointType.UnityObject },
                { FieldType.Color, PointType.Unspecified },
                { FieldType.Curve, PointType.Unspecified },
                { FieldType.Enum, PointType.Unspecified },
                { FieldType.Vector3, PointType.Vector3 },
                { FieldType.Vector3Int, PointType.Vector3 },
                { FieldType.Vector2, PointType.Vector3 },
                { FieldType.Vector2Int, PointType.Unspecified },
                { FieldType.Vector4, PointType.Unspecified },
                { FieldType.Rect, PointType.Unspecified },
                { FieldType.RectInt, PointType.Unspecified },
                { FieldType.Popup, PointType.Integer }
            };

            public static readonly Dictionary<FieldType, Type> fieldDataTypes = new Dictionary<FieldType, Type>() {
                { FieldType.Text, typeof(string) },
                { FieldType.TextArea, typeof(string) },
                { FieldType.RoundedFloat, typeof(float) },
                { FieldType.Float, typeof(float) },
                { FieldType.FloatSlider, typeof(float) },
                { FieldType.Int, typeof(int) },
                { FieldType.IntSlider, typeof(int) },
                { FieldType.Toggle, typeof(bool) },
                { FieldType.ToggleLeft, typeof(bool) },
                { FieldType.Object, typeof(UnityEngine.Object) },
                { FieldType.Color, typeof(Color) },
                { FieldType.Curve, typeof(AnimationCurve) },
                { FieldType.Enum, typeof(TextAlignment) },
                { FieldType.Vector3, typeof(Vector3) },
                { FieldType.Vector3Int, typeof(Vector3Int) },
                { FieldType.Vector2, typeof(Vector2) },
                { FieldType.Vector2Int, typeof(Vector2Int) },
                { FieldType.Vector4, typeof(Vector4) },
                { FieldType.Rect, typeof(Rect) },
                { FieldType.RectInt, typeof(RectInt) },
                { FieldType.Popup, typeof(int) }
            };

            public ContentType contentType = ContentType.InputField;
            public string label = "New Field";
            [TextArea(3, 3)]
            public string tooltip = "";
            [Tooltip("Input Field can't have point location at right.")]
            public PointLocation pointLocation = PointLocation.None;
            public PointType pointType = PointType.Main;
            [Tooltip("Alternate Exit: Use this out point as an alternative exit (override) for the currently running flow.\nParallel: Run a new flow along with the currently running flows.")]
            public OutMainType outMainType = OutMainType.AlternateExit;
            public bool alignRight = false;
            [Tooltip("If field type is Object, then you can change the type declaration later in the script to specify a specific unity object class.")]
            public FieldType fieldType = FieldType.Float;
            [Tooltip("Line count of the TextArea field.")]
            [Min(1)]
            public int lineCount = 2;
            public bool wordWrap = true;
            public bool useMin = false;
            public bool useMax = true;
            public float min = 0f;
            public float max = 0f;
            public bool allowSceneObjects = true;
            [Min(0f)]
            public float spacing = 0f;

        }

        public class DummyCommand : Command {

            public Func<string> onDrawTitle;
            public Action<Vector2> onDrawContents;

            public override void Editor_InitInPoints() { }

            public override void Editor_InitOutPoints() { }

            public override void Editor_OnDrawTitle(out string _tooltip) {
                if (onDrawTitle != null) {
                    _tooltip = onDrawTitle.Invoke();
                }
                else {
                    _tooltip = null;
                }
            }

            public override void Editor_OnDrawContents(Vector2 _absPosition) {
                onDrawContents?.Invoke(_absPosition);
            }

        }

        public class DummyNodesContainer : CommandNodesContainer {

            private static bool initialized = false;

            protected override void ValidateData() {
                if (!initialized) {
                    initialized = true;
                    CommandRegistry.RegisterCommand(typeof(CommandData), typeof(Command), false, "");
                    CommandRegistry.RegisterCommand(typeof(CommandData), typeof(DummyCommand), false, "");
                }
            }
        }

        [System.Serializable]
        public class CommandDataInclusion {
            public string name;
            public Type type;
            public bool include;
        }
        #endregion CLASSES
    }
}
