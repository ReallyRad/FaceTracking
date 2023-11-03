using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;
#else
using Calcatz.OdinSerializer;
using SerializationUtility = Calcatz.OdinSerializer.SerializationUtility;
#endif


namespace Calcatz.CookieCutter {

    public class CommandNode : Node {

        [System.NonSerialized] public static List<Command> clipboardCommands = new List<Command>(); //copied to clipboard

        private static readonly string[] reservedNodeNames = new string[] { "None", "Start" };
        public virtual string[] ReservedNodeNames {
            get {
                return reservedNodeNames;
            }
        }

        protected const float LABEL_WIDTH = 0.4f;

        public class Styles {
            public GUIStyle name;
            public GUIStyle label;
            public GUIStyle labelRight;

            private GUIStyle m_wrappedTextArea;
            public GUIStyle wrappedTextArea {
                get {
                    if (m_wrappedTextArea == null) {
                        if (EditorStyles.textArea == null) return null;
                        m_wrappedTextArea = new GUIStyle(EditorStyles.textArea);
                        m_wrappedTextArea.wordWrap = true;
                    }
                    return m_wrappedTextArea;
                }
            }

            public Styles() {
                name = new GUIStyle();
                name.fontStyle = FontStyle.Bold;
                name.fontSize = 8;
                name.normal.textColor = Color.white;

                label = new GUIStyle();
                name.fontSize = 10;
                label.normal.textColor = Color.white;

                labelRight = new GUIStyle(label);
                labelRight.alignment = TextAnchor.MiddleRight;
            }
        }

        public class CommandNodeConfig : Config {

            public Action<ConnectionPoint, ConnectionPoint> onRemovePropertyConnection;
            public Action<ConnectionPoint, ConnectionPoint, int> onSwapInConnections;
            public Action<ConnectionPoint> onResetOutPoint;

            public CommandNodeConfig(GUIStyle _nodeStyle, GUIStyle _selectedNodeStyle, GUIStyle _inPointStyle, GUIStyle _outPointStyle,
                                    Action<ConnectionPoint> _onClickInPoint, Action<ConnectionPoint> _onClickOutPoint, Action<Node> _onClickRemoveNode,
                                    Action<ConnectionPoint, ConnectionPoint> _onRemovePropertyConnection, Action<ConnectionPoint, ConnectionPoint, int> _onSwapInConnections,
                                    Action<ConnectionPoint> _onResetPoint)
                : base(_nodeStyle, _selectedNodeStyle, _inPointStyle, _outPointStyle, _onClickInPoint, _onClickOutPoint, _onClickRemoveNode) {
                onRemovePropertyConnection = _onRemovePropertyConnection;
                onSwapInConnections = _onSwapInConnections;
                onResetOutPoint = _onResetPoint;
            }
        }

        public System.Action onRefreshNeeded;


        protected CommandData commandData;
        public CommandData GetCommandData() { return commandData; }

        protected static Styles styles = new Styles();

        protected SerializedProperty serializedProperty;

        protected Command command;
        public Command GetCommand() {
            return command;
        }
        public T GetCommand<T>() where T : Command {
            return command as T;
        }

        public string nodeName = "Command";
        public string tooltip = "";

        protected float currentY;

        public CommandNode(CommandData _commandData, Command _command, Vector2 _position, float _width, float _height, Config _config)
            : base(_position, _width, _height, _config) {
            commandData = _commandData;
            command = _command;
            HandleFirstInPointCreation(_config);
            HandleFirstOutPointCreation(_config);
        }

        protected virtual void HandleFirstInPointCreation(Config _config) {
            if (command.id < ReservedNodeNames.Length) {
                nodeName = ReservedNodeNames[command.id];
            }
            else {
                AddMainInPoint(_config);
            }
        }
        protected virtual void HandleFirstOutPointCreation(Config _config) {
            AddMainOutPoint(_config);
        }


        public ConnectionPoint AddMainInPoint(Config _config) {
            MainConnectionPoint point = new MainConnectionPoint(this, ConnectionPointType.In, _config.inPointStyle, _config.onClickInPoint);
            inPoints.Add(point);
            return point;
        }

        public ConnectionPoint AddMainOutPoint(Config _config) {
            MainConnectionPoint point = new MainConnectionPoint(this, ConnectionPointType.Out, _config.outPointStyle, _config.onClickOutPoint);
            outPoints.Add(point);
            return point;
        }

        private ConnectionPoint CreatePropertyPoint(Type _type, ConnectionPointType _connectionPointType, Config _config) {
            if (_type.IsEnum) return null;
            Type connectionType;
            if (PropertyConnectionPoint.connectionTypes.ContainsKey(_type)) {
                connectionType = PropertyConnectionPoint.connectionTypes[_type];
            }
            else if (_type.IsSubclassOf(typeof(UnityEngine.Object)) || _type == typeof(UnityEngine.Object)) {
                connectionType = PropertyConnectionPoint.connectionTypes[typeof(UnityEngine.Object)];
            }
            else {
                connectionType = PropertyConnectionPoint.connectionTypes[typeof(PropertyConnectionPoint)];
            }
            Action<ConnectionPoint> onClick = _connectionPointType == ConnectionPointType.In ? _config.onClickInPoint : _config.onClickOutPoint;
            GUIStyle pointStyle = _connectionPointType == ConnectionPointType.In ? _config.inPointStyle : _config.outPointStyle;
            ConnectionPoint point = (ConnectionPoint)Activator.CreateInstance(connectionType, this, _connectionPointType, pointStyle, onClick);
            return point;
        }

        public ConnectionPoint AddPropertyInPoint(Type _type, Config _config) {
            ConnectionPoint point = CreatePropertyPoint(_type, ConnectionPointType.In, _config);
            inPoints.Add(point);
            return point;
        }

        public ConnectionPoint AddPropertyInPoint<T>(Config _config) {
            return AddPropertyInPoint(typeof(T), _config);
        }

        public ConnectionPoint AddPropertyOutPoint(Type _type, Config _config) {
            ConnectionPoint point = CreatePropertyPoint(_type, ConnectionPointType.Out, _config);
            outPoints.Add(point);
            return point;
        }

        public ConnectionPoint AddPropertyOutPoint<T>(Config _config) {
            return AddPropertyOutPoint(typeof(T), _config);
        }

        protected override void DrawInPoint(Vector2 _nodePosition, int _index, float _yPos) {
            if (_index >= inPoints.Count) {
                Debug.LogError("InPoint index out of range: " + _index + " | " + command);
                return;
            }
            else if (inPoints[_index] == null) {
                return;
            }
            base.DrawInPoint(_nodePosition, _index, _yPos);
        }

        protected override void DrawOutPoint(Vector2 _nodePosition, int _index, float _yPos) {
            if (_index >= outPoints.Count || outPoints[_index] == null) {
                Debug.LogError("OutPoint index out of range: " + _index + " | " + command);
                return;
            }
            base.DrawOutPoint(_nodePosition, _index, _yPos);
        }

        public override void Draw(Vector2 _offset) {
            base.Draw(_offset);
            Vector2 absolutePosition = rect.position + _offset;
            currentY = absolutePosition.y + 10;
            float height = EditorGUIUtility.singleLineHeight;
            rect.height = height;

            OnDrawTitle(absolutePosition);

            if (command.id > ReservedNodeNames.Length) {
                var nameRect = new Rect(absolutePosition.x + 10, currentY, rect.width - 20, height);
                var labelRect = nameRect;

                var monoScript = GetMonoScript(command.GetType());
                Texture scriptIcon = GetMonoIcon(monoScript);

                GUI.Label(labelRect, new GUIContent("  " + nodeName, scriptIcon, tooltip), styles.name);

                if (monoScript != null) {
                    bool guiEnabled = GUI.enabled;
                    GUI.enabled = false;
                    EditorGUI.ObjectField(new Rect(nameRect.x + nameRect.width - 45, currentY - height - verticalSpacing, 35, height), GetMonoScript(command.GetType()), typeof(MonoScript), false);
                    GUI.enabled = guiEnabled;
                }
            }
            else {
                GUI.Label(new Rect(absolutePosition.x + 10, currentY, rect.width - 20, height),
                    new GUIContent("  " + nodeName, tooltip), styles.label);
            }

            AddRectHeight(height);
            OnDrawContents(absolutePosition);
        }

        protected virtual void OnDrawTitle(Vector2 _absolutePosition) {
            if (inPoints.Count > 0) DrawInPoint(_absolutePosition, 0, currentY);
            if (outPoints.Count > 0) DrawOutPoint(_absolutePosition, 0, currentY);
        }

        protected virtual void OnDrawContents(Vector2 _absolutePosition) {

        }

        protected void AddRectHeight(float _height, bool _autoAddVerticalSpacing = true) {
            currentY += (_height + (_autoAddVerticalSpacing ? verticalSpacing : 0));
            rect.height += (_height + (_autoAddVerticalSpacing ? verticalSpacing : 0));
        }

        protected override void OnEndDragNode() {
            Undo.RecordObject(commandData.targetObject, "Moved a node");
            command.nodePosition = rect.position;
            EditorUtility.SetDirty(commandData.targetObject);
        }

        protected override void HandleContextMenu(GenericMenu _genericMenu) {
            base.HandleContextMenu(_genericMenu);
            _genericMenu.AddSeparator("");

            if (IsCopyAvailable()) {
                _genericMenu.AddItem(new GUIContent("Copy"), false, Copy);
            }

            if (IsPasteAvailable()) {
                _genericMenu.AddItem(new GUIContent("Paste"), false, Paste);
            }
            else {
                _genericMenu.AddDisabledItem(new GUIContent("Paste"), false);
            }
        }

        protected override bool IsCopyAvailable() {
            return command.id >= ReservedNodeNames.Length;
        }

        protected bool IsPasteAvailable() {
            if (command.id >= ReservedNodeNames.Length) {
                if (clipboardCommands.Count == 1) {
                    return clipboardCommands[0].GetType() == command.GetType();
                }
            }
            return false;
        }

        protected override void Copy() {
            clipboardCommands.Clear();
            clipboardCommands.Add(command);
        }

        protected override void Paste() {
            if (IsPasteAvailable()) {
                List<Command> copiedCommands = (List<Command>)SerializationUtility.CreateCopy(clipboardCommands);
                if (copiedCommands != null && copiedCommands.Count == 1) {
                    Undo.RecordObject(commandData.targetObject, "Pasted an existing command");
                    copiedCommands[0].id = command.id;
                    copiedCommands[0].nodePosition = command.nodePosition;
                    copiedCommands[0].nextIds = command.nextIds;
                    command = copiedCommands[0];
                    commandData.SetCommand(copiedCommands[0].id, copiedCommands[0]);
                    onRefreshNeeded.Invoke();
                    EditorUtility.SetDirty(commandData.targetObject);
                }
            }
        }

        public static int GetConnectionPointIndex(ConnectionPoint _point, List<ConnectionPoint> _points) {
            int conPointIndex;
            for (conPointIndex = 0; conPointIndex < _points.Count; conPointIndex++) {
                if (_points[conPointIndex] == _point) break;
            }
            return conPointIndex;
        }

        public virtual bool FilterByContent(string _filter, out string _displayContent) {
            _displayContent = nodeName + " (" + command.id + ")";
            if (_filter != null && _filter != "") {
                if (_displayContent.ToLower().Contains(_filter.ToLower())) {
                    return true;
                }
            }
            else {
                return true;
            }
            return false;
        }

        protected override void OnClickRemoveNode() {
            if (command.id < ReservedNodeNames.Length) {
                return;
            }
            base.OnClickRemoveNode();
        }

        /// <summary>
        /// A wrapper to generate a standard rect for one line (or more) field.
        /// </summary>
        /// <param name="_absolutePosition"></param>
        /// <param name="_lineCount"></param>
        /// <returns></returns>
        protected Rect GenerateLineRect(Vector2 _absolutePosition, int _lineCount = 1) {
            return new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, EditorGUIUtility.singleLineHeight * _lineCount);
        }


        protected void GenerateSingleLineLabeledRect(Vector2 _absolutePosition, out Rect labelRect, out Rect valueRect) {
            valueRect = GenerateLineRect(_absolutePosition);
            labelRect = valueRect;
            labelRect.width = EditorGUIUtility.labelWidth;
            valueRect.x += labelRect.width;
            valueRect.width -= labelRect.width;
        }


        protected static void DropBinderAreaGUI<T>(Rect _dropArea, System.Action<T> _onObjectChanged) {
            Event evt = Event.current;
            switch (evt.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!_dropArea.Contains(evt.mousePosition) || DragAndDrop.objectReferences.Length > 1)
                        return;

                    UnityEngine.Object draggedObject = DragAndDrop.objectReferences[0];

                    if (!EditorUtility.IsPersistent(draggedObject)) {
                        if (draggedObject is CrossSceneBinder binderComponent) {
                            draggedObject = binderComponent.binderAsset;
                        }
                        else if (draggedObject is GameObject go) {
                            if (go.TryGetComponent<CrossSceneBinder>(out var binderComp)) {
                                draggedObject = binderComp.binderAsset;
                            }
#if TIMELINE_AVAILABLE
                            else if (go.TryGetComponent<UnityEngine.Playables.PlayableDirector>(out var playableDirector)) {
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
                            else if (comp.TryGetComponent<UnityEngine.Playables.PlayableDirector>(out var playableDirector)) {
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
                    else if (!(draggedObject is T)) {
                        draggedObject = null;
                    }

                    if (draggedObject == null) {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        return;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    if (evt.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();
                        _onObjectChanged((T)(object)draggedObject);
                    }
                    evt.Use();
                    break;
            }
        }

        #region MONO-SCRIPT
        internal static readonly Dictionary<Type, MonoScript> monoScripts = new Dictionary<Type, MonoScript>();
#if UNITY_2021_2_OR_NEWER
        internal static readonly Dictionary<MonoScript, Texture> monoIcons = new Dictionary<MonoScript, Texture>();
#endif
        internal static void RefreshMonoScriptsReference() {
            monoScripts.Clear();
            var scripts = CCAssetUtility.GetScriptAssetsOfType<Command>();
            foreach(var monoScript in scripts) {
                if (!monoScripts.ContainsKey(monoScript.GetClass())) {
                    monoScripts.Add(monoScript.GetClass(), monoScript);
                }
            }
        }
        public static MonoScript GetMonoScript(Type _commandType) {
            if (monoScripts.ContainsKey(_commandType)) {
                return monoScripts[_commandType];
            }
            return null;
        }
        private static Texture GetMonoIcon(MonoScript _monoScript) {
#if UNITY_2021_2_OR_NEWER
            if (ReferenceEquals(_monoScript, null)) return null;
            if (!monoIcons.ContainsKey(_monoScript)) {
                var monoImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_monoScript)) as MonoImporter;
                monoIcons.Add(_monoScript, monoImporter.GetIcon());
            }
            return monoIcons[_monoScript];
#else
            return null;
#endif
        }
#endregion
    }
}
