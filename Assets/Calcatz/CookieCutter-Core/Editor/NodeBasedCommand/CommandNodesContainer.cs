using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.Reflection;

#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;
#else
using Calcatz.OdinSerializer;
using SerializationUtility = Calcatz.OdinSerializer.SerializationUtility;
#endif

namespace Calcatz.CookieCutter {

    public class CommandNodesContainer : NodesContainer {

        protected virtual void ValidateAvailableCommands(Type _commandDataType) {
            CommandRegistry.RegisterCommand(_commandDataType, typeof(Command), false, "");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(StickyNoteCommand), true, "Sticky Note");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(DebugLogCommand), true, "Debug/Log");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(BranchCommand), true, "Flow/Branch");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(ChangeTimeScaleCommand), true, "Flow/Change Time Scale");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(WaitForSecondsCommand), true, "Flow/Wait For Seconds");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(VariableCommand), false, "Variable");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(SetVariableCommand), false, "Set Variable");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(CrossSceneComponentCommand), true, "Get Cross Scene Component");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(RandomObjectSelectorCommand), true, "Random Object Selector");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(AndCommand), true, "Comparator/AND");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(OrCommand), true, "Comparator/OR");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(FloatComparatorCommand), true, "Comparator/Float Comparator");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(FloatOperatorCommand), true, "Primitive Operator/Float Operator");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(IntegerComparatorCommand), true, "Comparator/Integer Comparator");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(IntegerOperatorCommand), true, "Primitive Operator/Integer Operator");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(CastFloatToIntegerCommand), true, "Cast/Float to Integer");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(CastIntegerToFloatCommand), true, "Cast/Integer to Float");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(StringComparatorCommand), true, "Comparator/String Comparator");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(StringFormatterCommand), true, "Formatter/String Formatter");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(TimelinePlayerCommand), true, "Timeline/Timeline Player");
            CommandRegistry.RegisterCommand(_commandDataType, typeof(TimelineExtrapolationCommand), true, "Timeline/Timeline Extrapolation");

            GlobalStarredCommandMenuAttribute.AddStarredCommand(_commandDataType, typeof(bool), typeof(AndCommand));
            GlobalStarredCommandMenuAttribute.AddStarredCommand(_commandDataType, typeof(bool), typeof(OrCommand));
            GlobalStarredCommandMenuAttribute.AddStarredCommand(_commandDataType, typeof(int), typeof(IntegerOperatorCommand));
            GlobalStarredCommandMenuAttribute.AddStarredCommand(_commandDataType, typeof(int), typeof(IntegerComparatorCommand));
            GlobalStarredCommandMenuAttribute.AddStarredCommand(_commandDataType, typeof(int), typeof(CastIntegerToFloatCommand));
            GlobalStarredCommandMenuAttribute.AddStarredCommand(_commandDataType, typeof(float), typeof(FloatOperatorCommand));
            GlobalStarredCommandMenuAttribute.AddStarredCommand(_commandDataType, typeof(float), typeof(FloatComparatorCommand));
            GlobalStarredCommandMenuAttribute.AddStarredCommand(_commandDataType, typeof(float), typeof(CastFloatToIntegerCommand));
            GlobalStarredCommandMenuAttribute.AddStarredCommand(_commandDataType, typeof(string), typeof(StringComparatorCommand));
        }

        public Action onValidateCommandDataReference;
        public Action<VisualElement> onCreateAssetSelection;
        public Action<VisualElement> onCreateBeforeCommandList;

        private CommandData m_commandData;

        protected Type commandDataType;
        protected Dictionary<int, CommandNode> m_commandNodes = new Dictionary<int, CommandNode>();
        protected Command currentOutPointCommand;

        protected CommandNodesContainerLeftPane leftPaneHandler;
        protected bool m_useVariables;
        protected bool m_showCreateVariableNodeButton = true;
        protected bool m_useLoadSave;

        public CommandNodesContainer() {
            Undo.undoRedoPerformed += OnUndoRedo;
            leftPaneHandler = new CommandNodesContainerLeftPane();
        }

        ~CommandNodesContainer() {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void OnUndoRedo() {
            RepaintContainer();
            m_commandData = null;
        }

        public CommandNode GetCommandNode(int _id) => m_commandNodes.ContainsKey(_id) ? m_commandNodes[_id] : null;

        public CommandData commandData {
            get => m_commandData;
            set {
                SetCommandDataWithoutRepaint(value);
                RepaintContainer();
            }
        }

        internal void SetCommandDataWithoutRepaint(CommandData value) {
            m_commandData = value;
            if (value != null) {
                commandDataType = value.GetType();
                ValidateData();
                if (m_commandData.variables != null) {
                    leftPaneHandler.variableTable.BindVariables(() => m_commandData.variables);
                }
                else {
                    leftPaneHandler.variableTable.BindVariables(null);
                }
            }
            else {
                commandDataType = null;
                if (leftPaneHandler.variableTable != null) {
                    leftPaneHandler.variableTable.BindVariables(null);
                }
            }
        }

        public bool saveAvailable { set => leftPaneHandler.saveAvailable = value; }

        public float leftPaneWidth { get => leftPaneHandler.leftPaneWidth; set => leftPaneHandler.leftPaneWidth = value; }
        public bool useVariables { get => m_useVariables; set => m_useVariables = value; }
        public bool showCreateVariableNodeButton { get => m_showCreateVariableNodeButton; set => m_showCreateVariableNodeButton = value; }
        public bool useLoadSave { get => m_useLoadSave; set => m_useLoadSave = value; }

        protected override Node.Config GetNodeConfig() {
            Node.Config nodeConfigBase = base.GetNodeConfig();
            CommandNode.CommandNodeConfig nodeConfig = new CommandNode.CommandNodeConfig(
                    nodeConfigBase.nodeStyle, nodeConfigBase.selectedStyle,
                    nodeConfigBase.inPointStyle, nodeConfigBase.outPointStyle,
                    OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, OnRemovePropertyConnection, OnSwapInConnections, OnResetOutPoint);
            return nodeConfig;
        }

        protected virtual void ValidateData() {
            if (!commandData.commands.ContainsKey(1)) {
                commandData.commands.Add(1, new Command() { id = 1, nodePosition = Vector2.zero + Vector2.up * 200 });
            }
        }

        public void ReloadNodes(bool _resetNullConnections = true) {
            /*if (Event.current != null) {
                Event.current.Use();
            }*/
            nodes.Clear();
            m_commandNodes.Clear();

            connections.Clear();

            if (m_commandData != null) {
                if (commandDataType == null) {
                    commandDataType = m_commandData.GetType();
                }
                Node.Config config = GetNodeConfig();
                m_commandData.TraverseCommands(_command => {
                    _command.Validate();
                    CommandNode node;
                    node = CommandRegistry.CreateNode(commandDataType, m_commandData, _command, _command.nodePosition, config);

                    if (_command.id < node.ReservedNodeNames.Length) {
                        OnReloadReservedNodes(node);
                    }
                    nodes.Add(node);

                    node.onRefreshNeeded = NodeOnRefreshNeeded;
                    m_commandNodes.Add(_command.id, (CommandNode)nodes[nodes.Count - 1]);
                });

            }

            foreach (CommandNode commandNode in m_commandNodes.Values) {
                Command command = commandNode.GetCommand();
                for (int i = 0; i < command.nextIds.Count; i++) {
                    if (i >= commandNode.outPoints.Count) break;
                    for (int j = 0; j < command.nextIds[i].Count; j++) {
                        if (command.nextIds[i][j].targetId != 0) {
                            ConnectionPoint outPoint = commandNode.outPoints[i];
                            int targetId = command.nextIds[i][j].targetId;
                            int pointIndex = command.nextIds[i][j].pointIndex;

                            if (m_commandNodes.ContainsKey(targetId)) {
                                if (pointIndex < m_commandNodes[targetId].inPoints.Count) {
                                    ConnectionPoint inPoint = m_commandNodes[targetId].inPoints[pointIndex];
                                    connections.Add(new Connection(inPoint, outPoint, OnClickRemoveConnection));
                                }
                                else if (_resetNullConnections) {
                                    command.nextIds[i][j].Reset();
                                }
                            }
                            else if (_resetNullConnections) {
                                command.nextIds[i][j].Reset();
                            }
                        }
                    }
                }
            }
        }

        private void LoadNewlyAddedNodes(List<Command> _commands) {
            if (m_commandData != null) {
                if (commandDataType == null) {
                    commandDataType = m_commandData.GetType();
                }
                Node.Config config = GetNodeConfig();
                foreach (var _command in _commands) {
                    _command.Validate();
                    if (m_commandNodes.ContainsKey(_command.id)) continue;
                    CommandNode node;
                    node = CommandRegistry.CreateNode(commandDataType, m_commandData, _command, _command.nodePosition, config);

                    if (_command.id < node.ReservedNodeNames.Length) {
                        OnReloadReservedNodes(node);
                    }
                    nodes.Add(node);

                    node.onRefreshNeeded = NodeOnRefreshNeeded;
                    m_commandNodes.Add(_command.id, (CommandNode)nodes[nodes.Count - 1]);
                };

            }
        }

        private void LoadNewlyAddedConnections(List<Command> _commands) {
            var newCommandNodes = new List<CommandNode>();
            foreach (var _command in _commands) {
                if (m_commandNodes.TryGetValue(_command.id, out var node)) {
                    newCommandNodes.Add(node);
                }
            };
            foreach (CommandNode commandNode in newCommandNodes) {
                Command command = commandNode.GetCommand();
                for (int i = 0; i < command.nextIds.Count; i++) {
                    if (i >= commandNode.outPoints.Count) break;
                    for (int j = 0; j < command.nextIds[i].Count; j++) {
                        if (command.nextIds[i][j].targetId != 0) {
                            ConnectionPoint outPoint = commandNode.outPoints[i];
                            int targetId = command.nextIds[i][j].targetId;
                            int pointIndex = command.nextIds[i][j].pointIndex;

                            if (m_commandNodes.ContainsKey(targetId)) {
                                if (pointIndex < m_commandNodes[targetId].inPoints.Count) {
                                    ConnectionPoint inPoint = m_commandNodes[targetId].inPoints[pointIndex];
                                    connections.Add(new Connection(inPoint, outPoint, OnClickRemoveConnection));
                                }
                            }
                        }
                    }
                }
            }
        }

        protected virtual void OnReloadReservedNodes(CommandNode _node) {
            if (_node.GetCommand().id == 1) {
                SetNodeStyle(_node, "5");
            }
        }

        protected virtual void NodeOnRefreshNeeded() {
            ReloadNodes();
            saveAvailable = true;
        }

        protected override void Save() {
            if (m_commandData == null) return;

            EditorUtility.SetDirty(m_commandData.targetObject);
            AssetDatabase.SaveAssets();
            saveAvailable = false;
        }

        protected static void SetNodeStyle(CommandNode node, string _nodeTextureIndex) {
            GUIStyle firstEncounterNodeStyle = new GUIStyle();
            firstEncounterNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node" + _nodeTextureIndex + ".png") as Texture2D;
            firstEncounterNodeStyle.border = new RectOffset(12, 12, 12, 12);

            GUIStyle selecetedFirstEncounterNodeStyle = new GUIStyle();
            selecetedFirstEncounterNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node" + _nodeTextureIndex + " on.png") as Texture2D;
            selecetedFirstEncounterNodeStyle.border = new RectOffset(12, 12, 12, 12);

            node.style = firstEncounterNodeStyle;
            node.defaultNodeStyle = firstEncounterNodeStyle;
            node.selectedNodeStyle = selecetedFirstEncounterNodeStyle;
        }

        public virtual void RepaintContainer() {
            ClearConnectionSelection();
            ReloadNodes();
            leftPaneHandler.RefreshCommandList(nodes, GoToNode);
            RecreateGUI();
            Repaint();
        }

        public void SetDirty() {
            if (commandData != null && commandData.targetObject != null) {
                EditorUtility.SetDirty(commandData.targetObject);
            }
        }

        public override void RepaintIfDirty() {
            if (commandData != null && commandData.targetObject != null) {
                if (EditorUtility.IsDirty(commandData.targetObject)) {
                    RepaintContainer();
                }
            }
        }

        public void GoToNode(int _commandId) {
            CommandNode commandNode;
            if (m_commandNodes.TryGetValue(_commandId, out commandNode)) {
                GoToNode(commandNode);
            }
        }

        public static CommandNodesContainer CreateFromCommandData(CommandData _commandData) {
            Type commandDataType = _commandData.GetType();
            Type containerType = typeof(CommandNodesContainer);
            var allContainerTypes = TypeCache.GetTypesWithAttribute(typeof(CommandNodesContainerAttribute));
            foreach(var type in allContainerTypes) {
                if (type.GetCustomAttribute<CommandNodesContainerAttribute>().commandDataType == commandDataType) {
                    containerType = type;
                }
            }
            //Type containerType = TypeCache.GetTypesWithAttribute(typeof(CommandNodesContainerAttribute))
            //    .Single(_type => _type.GetCustomAttribute<CommandNodesContainerAttribute>().commandDataType == commandDataType);
            var nodesContainer = Activator.CreateInstance(containerType) as CommandNodesContainer;
            nodesContainer.commandData = _commandData;
            return nodesContainer;
        }

        #region DRAW

        protected override void OnCreateGUI(VisualElement _root) {
            ReloadNodes();
            base.OnCreateGUI(_root);
            if (leftPaneWidth > 1f) {
                leftPaneHandler.CreateGUI(_root, OnCreateLeftPane, UpdateNodesAreaSize, ()=>commandData, ()=>commandDataType);
                UpdateNodesAreaSize();
            }
        }

        protected virtual void OnCreateLeftPane(VisualElement _leftPane) {
            OnCreateAssetSelection(_leftPane);
            VisualElement saveSettingsArea = leftPaneHandler.CreateSaveSettingsArea(ReloadNodes, Save);
            _leftPane.Add(saveSettingsArea);
            saveSettingsArea.style.display = useLoadSave? DisplayStyle.Flex : DisplayStyle.None;
            OnCreateBeforeCommandList(_leftPane);

            VisualElement commandDataProperties = new VisualElement() {
                style = {
                    flexGrow = 1
                }
            };
            _leftPane.Add(commandDataProperties);
            if (m_useVariables) {
                leftPaneHandler.CreateVariableMenu(commandDataProperties, () => realPositionOffset, AddNewCommand, showCreateVariableNodeButton);
            }
            leftPaneHandler.CreateCommandListMenu(commandDataProperties, nodes, GoToNode);
        }
               

        protected virtual void OnCreateAssetSelection(VisualElement _leftPane) {
            if (onCreateAssetSelection != null) onCreateAssetSelection.Invoke(_leftPane);
        }

        protected virtual void OnCreateBeforeCommandList(VisualElement _leftPane) {
            if (onCreateBeforeCommandList != null) onCreateBeforeCommandList.Invoke(_leftPane);
        }

        public override void OnDrawNodesArea() {
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float leftOffset = leftPaneHandler.leftPaneArea == null ? 0 : leftPaneHandler.leftPaneArea.style.width.value.value;
            Rect container = GetNodesArea();
            if (commandData != null) {
                Rect buttonRect = new Rect(container.x + container.size.x - 85 - leftOffset, container.y + container.size.y - 25, 80, 20);
                if (GUI.Button(buttonRect, "Go to Start")) {
                    Command startingCommand = commandData.GetStartingCommand();
                    if (startingCommand != null) {
                        CommandNode startingNode = GetCommandNode(startingCommand.id);
                        if (startingNode != null) {
                            GoToNode(startingNode);
                        }
                    }
                }
            }

            if (onValidateCommandDataReference != null) {
                onValidateCommandDataReference.Invoke();
            }

            EditorGUI.BeginChangeCheck();
            base.OnDrawNodesArea();
            if (EditorGUI.EndChangeCheck()) {
                saveAvailable = true;
            }

            Color col = GUI.color;
            GUI.color = new Color(col.r, col.g, col.b, 0.2f);
            Rect versionRect = new Rect(container.x + container.size.x - 150 - 85 - leftOffset, container.y + container.size.y - 25, 150, 20);
            EditorGUI.LabelField(versionRect, "Calcatz CookieCutter v" + CookieCutterProvider.version, EditorStyles.miniLabel);
            GUI.color = col;

            EditorGUI.indentLevel = indentLevel;
        }

        protected override Rect GetNodesArea() {
            Rect containerArea = GetContainerArea();
            if (nodesArea != null) {
                //using UIElements
                float leftOffset = leftPaneHandler.leftPaneArea == null ? 0 : leftPaneHandler.leftPaneArea.style.width.value.value;
                return new Rect(leftOffset, 0, containerArea.width - leftOffset, containerArea.height);
            }
            else {
                //using IMGUI
                return containerArea;
            }
        }

        protected override void ProcessEvents(Event e) {
            base.ProcessEvents(e);
            switch (e.type) {
                /*case EventType.ValidateCommand:
                    switch (e.commandName) {
                        //case "UndoRedoPerformed":
                        //case "SelectAll":
                        //    RepaintContainer();
                        //    break;
                    }
                    break;*/

                case EventType.MouseDown:
                    if (e.button == 0 && selectedOutPoint != null /*&& selectedOutPoint is MainConnectionPoint*/) {
                        ProcessContextMenu(e.mousePosition);
                    }
                    break;

                case EventType.KeyDown:
                    if (e.control) {
                        if (e.keyCode == KeyCode.C) {
                            if (IsCopyAvailable()) {
                                Copy();
                            }
                        }
                        else if (e.keyCode == KeyCode.V) {
                            if (IsPasteAvailable()) {
                                Paste(e.mousePosition);
                            }
                        }
                    }
                    break;
            }

        }
        #endregion

        #region CONNECTION-CALLBACK      
        protected virtual void HandleRemoveConnection(ConnectionPoint _inPoint, ConnectionPoint _outPoint) {
            Undo.RecordObject(m_commandData.targetObject, "Removed a connection");
            CommandNode commandNode = (CommandNode)_outPoint.node;
            Command command = commandNode.GetCommand();

            int outPointIndex = CommandNode.GetConnectionPointIndex(_outPoint, commandNode.outPoints);
            int inPointIndex = CommandNode.GetConnectionPointIndex(_inPoint, _inPoint.node.inPoints);

            CommandNode targetCommandNode = (CommandNode)_inPoint.node;
            Command targetCommand = targetCommandNode.GetCommand();

            Command.RemoveNextIdAtConnection(command, outPointIndex, targetCommand, inPointIndex);
            if (targetCommand is PropertyCommand) {
                Command.RemoveInputIdAtConnection(command, outPointIndex, targetCommand, inPointIndex);
            }
            else {
                if (inPointIndex > 0) { //inPointIndex 0 always main connection
                    Command.RemoveInputIdAtConnection(command, outPointIndex, targetCommand, inPointIndex - 1);
                }
            }
            saveAvailable = true;
            EditorUtility.SetDirty(m_commandData.targetObject);
        }

        protected override void OnClickRemoveConnection(Connection connection) {
            if (EditorUtility.DisplayDialog("Remove Connection", "Are you sure you want to remove the connection?", "Yes", "No")) {
                base.OnClickRemoveConnection(connection);
                HandleRemoveConnection(connection.inPoint, connection.outPoint);
            }
        }

        protected void OnRemovePropertyConnection(ConnectionPoint _inPoint, ConnectionPoint _outPoint) {
            ShiftTheRestConnectionIns(_inPoint);

            //Handle In-Point Related
            List<ConnectionPoint> removedOutPoints = new List<ConnectionPoint>();
            foreach (Connection con in connections) {
                if (con.inPoint == _inPoint) {
                    removedOutPoints.Add(con.outPoint);
                }
            }
            foreach (ConnectionPoint outPointToRemove in removedOutPoints) {
                HandleRemoveConnection(_inPoint, outPointToRemove);
            }

            //Handle Out-Point Related
            if (_outPoint != null) {
                int outPointIndex = CommandNode.GetConnectionPointIndex(_outPoint, _outPoint.node.outPoints);
                Command command = ((CommandNode)_outPoint.node).GetCommand();
                command.nextIds[outPointIndex].Clear();
            }

            ReloadNodes();
        }
        private void ShiftTheRestConnectionIns(ConnectionPoint _inPoint) {
            List<Connection> shiftedCons = new List<Connection>();
            bool startCheckingConnection = false;
            for (int i = 0; i < _inPoint.node.inPoints.Count; i++) {
                if (_inPoint.node.inPoints[i] == _inPoint) {
                    startCheckingConnection = true;
                    continue;
                }
                if (startCheckingConnection) {
                    foreach (Connection con in connections) {
                        if (con.inPoint == _inPoint.node.inPoints[i]) {
                            shiftedCons.Add(con);
                        }
                    }
                }
            }
            foreach (Connection conToShift in shiftedCons) {
                CommandNode shiftingCommandNode = (CommandNode)conToShift.outPoint.node;
                CommandNode targetCommandNode = (CommandNode)conToShift.inPoint.node;

                int conInIndex = 0;
                for (int i = 0; i < targetCommandNode.inPoints.Count; i++) {
                    if (targetCommandNode.inPoints[i] == conToShift.inPoint) {
                        conInIndex = i;
                    }
                }

                Command shiftingCommand = shiftingCommandNode.GetCommand();
                for (int i = 0; i < shiftingCommand.nextIds.Count; i++) {
                    for (int j = 0; j < shiftingCommand.nextIds[i].Count; j++) {
                        if (shiftingCommand.nextIds[i][j].pointIndex == conInIndex) {
                            shiftingCommand.nextIds[i][j].pointIndex--;
                        }
                    }
                }

            }
        }

        protected void OnSwapInConnections(ConnectionPoint _lowerInPoint, ConnectionPoint _upperInPoint, int _lowerIndex) {
            List<Connection> lowerConnections = new List<Connection>();
            List<Connection> upperConnections = new List<Connection>();
            foreach (Connection con in connections) {
                if (con.inPoint == _lowerInPoint) {
                    lowerConnections.Add(con);
                }
                if (con.inPoint == _upperInPoint) {
                    upperConnections.Add(con);
                }
            }

            foreach (Connection con in lowerConnections) {
                con.inPoint = _upperInPoint;
                CommandNode commandNode = (CommandNode)con.outPoint.node;
                Command command = commandNode.GetCommand();
                Command targetCommand = ((CommandNode)_upperInPoint.node).GetCommand();
                int outPointIndex = CommandNode.GetConnectionPointIndex(con.outPoint, commandNode.outPoints);

                Command.ChangeNextIdInputIndexAtConnection(command, outPointIndex, targetCommand, _lowerIndex, _lowerIndex - 1);
            }
            foreach (Connection con in upperConnections) {
                con.inPoint = _lowerInPoint;
                CommandNode commandNode = (CommandNode)con.outPoint.node;
                Command command = commandNode.GetCommand();
                Command targetCommand = ((CommandNode)_lowerInPoint.node).GetCommand();
                int outPointIndex = CommandNode.GetConnectionPointIndex(con.outPoint, commandNode.outPoints);

                Command.ChangeNextIdInputIndexAtConnection(command, outPointIndex, targetCommand, _lowerIndex - 1, _lowerIndex);
            }
            ReloadNodes();
        }

        protected override void OnClickInPoint(ConnectionPoint inPoint) {
            if (IsTheSameConnectionType(inPoint)) {

                Undo.RecordObject(m_commandData.targetObject, "Created a connection");

                int outPointIndex = CommandNode.GetConnectionPointIndex(selectedOutPoint, selectedOutPoint.node.outPoints);
                int inPointIndex = CommandNode.GetConnectionPointIndex(inPoint, inPoint.node.inPoints);

                ConnectionPoint outPoint = selectedOutPoint;

                base.OnClickInPoint(inPoint);

                CommandNode commandNode = (CommandNode)inPoint.node;

                if (inPoint is MainConnectionPoint) {
                    if (currentOutPointCommand.nextIds.Count == 0) {
                        currentOutPointCommand.nextIds.Add(new List<Command.ConnectionTarget>());
                        outPointIndex = 0;
                    }
                    if (currentOutPointCommand.nextIds[outPointIndex].Count == 0) {
                        currentOutPointCommand.nextIds[outPointIndex].Add(new Command.ConnectionTarget());
                    }
                    bool previouslyEmpty = currentOutPointCommand.nextIds[outPointIndex][0].targetId == 0;
                    currentOutPointCommand.nextIds[outPointIndex][0].Set(commandNode.GetCommand().id, inPointIndex);

                    if (!previouslyEmpty) { //Node has already connected, but changed
                        ReloadNodes();
                    }
                }
                else if (inPoint is PropertyConnectionPoint) {
                    foreach (Connection con in connections) {
                        if (con.inPoint == inPoint && con.outPoint != outPoint) {
                            HandleRemoveConnection(con.inPoint, con.outPoint);
                            connections.Remove(con);
                            break;
                        }
                    }

                    Command.ConnectionTarget newOriginConnection = null;
                    //Search for an unused connection first
                    foreach (var conn in currentOutPointCommand.nextIds[outPointIndex]) {
                        if (conn.targetId == 0) {
                            newOriginConnection = conn;
                            conn.targetId = commandNode.GetCommand().id;
                            conn.pointIndex = inPointIndex;
                        }
                    }

                    if (newOriginConnection == null) {
                        newOriginConnection = new Command.ConnectionTarget() { targetId = commandNode.GetCommand().id, pointIndex = inPointIndex };
                        currentOutPointCommand.nextIds[outPointIndex].Add(newOriginConnection);
                    }

                    CommandNode targetCommandNode = (CommandNode)outPoint.node;
                    Command.ConnectionTarget newTargetConnection = new Command.ConnectionTarget() { targetId = targetCommandNode.GetCommand().id, pointIndex = outPointIndex };

                    if (((CommandNode)inPoint.node).GetCommand() is PropertyCommand) {
                        commandNode.GetCommand().inputIds[inPointIndex] = newTargetConnection;
                    }
                    else {
                        commandNode.GetCommand().inputIds[inPointIndex - 1] = newTargetConnection; //input 0 always main connection
                    }

                    RepaintContainer();
                }

                saveAvailable = true;
                EditorUtility.SetDirty(m_commandData.targetObject);
            }
        }

        private bool IsTheSameConnectionType(ConnectionPoint inPoint) {
            if (selectedOutPoint == null) return false;
            return selectedOutPoint.GetType() == inPoint.GetType() || inPoint.GetType() == typeof(PropertyConnectionPoint);
        }

        protected override void OnClickOutPoint(ConnectionPoint outPoint) {
            base.OnClickOutPoint(outPoint);
            CommandNode commandNode = (CommandNode)outPoint.node;
            currentOutPointCommand = commandNode.GetCommand();
        }

        private void OnResetOutPoint(ConnectionPoint _outPoint) {
            List<Connection> connectionsToRemove = new List<Connection>();
            foreach (Connection con in connections) {
                if (con.outPoint == _outPoint) {
                    connectionsToRemove.Add(con);
                }
            }
            foreach (Connection con in connectionsToRemove) {
                HandleRemoveConnection(con.inPoint, con.outPoint);
                connections.Remove(con);
            }
        }

        protected override void HandleRemovedConnections(Node _removedNode, List<Connection> _connectionsToRemove) {
            base.HandleRemovedConnections(_removedNode, _connectionsToRemove);
            foreach (Connection connection in _connectionsToRemove) {
                //if (_removedNode.inPoints.Contains(connection.inPoint)) {

                HandleRemoveConnection(connection.inPoint, connection.outPoint);

                saveAvailable = true;
                //}
            }
        }

        protected override void OnClickRemoveNode(Node node) {
            CommandNode commandNode = (CommandNode)node;
            if (commandNode.GetCommand().id < commandNode.ReservedNodeNames.Length) return;
            base.OnClickRemoveNode(node);
            Undo.RecordObject(m_commandData.targetObject, "Removed a node");
            m_commandData.RemoveCommand(commandNode.GetCommand().id);
            EditorUtility.SetDirty(m_commandData.targetObject);
            leftPaneHandler.RefreshCommandList(nodes, GoToNode);
        }

        internal override void OnRemoveSelectedNodes() {
            if (m_commandData.ClearUnusedPooledIds()) {
                EditorUtility.SetDirty(m_commandData.targetObject);
            }
        }
        #endregion

        #region CONTEXT-MENU

        protected override void HandleContextMenu(Vector2 _mousePosition, GenericMenu _genericMenu) {
            if (commandDataType == null) return;
            base.HandleContextMenu(_mousePosition, _genericMenu);
            if (IsCopyAvailable()) {
                _genericMenu.AddItem(new GUIContent("Copy"), false, Copy);
            }
            if (IsPasteAvailable()) {
                _genericMenu.AddItem(new GUIContent("Paste"), false, () => Paste(_mousePosition));
            }
            _genericMenu.AddSeparator("");

            HashSet<KeyValuePair<Type, CommandRegistry.Registry>> mainCommands;
            HashSet<KeyValuePair<Type, CommandRegistry.Registry>> propertyCommands;
            CommandRegistry.GetCommands(commandDataType, out mainCommands, out propertyCommands);
            foreach (KeyValuePair<Type, CommandRegistry.Registry> kvp in mainCommands) {
                if (kvp.Value.allowCreateNode)
                    _genericMenu.AddItem(new GUIContent(kvp.Value.pathName), false, () => OnClickAddNode(kvp.Key, _mousePosition));
            }
            _genericMenu.AddSeparator("");
            foreach (KeyValuePair<Type, CommandRegistry.Registry> kvp in propertyCommands) {
                if (kvp.Value.allowCreateNode)
                    _genericMenu.AddItem(new GUIContent("Property/" + kvp.Value.pathName), false, () => OnClickAddNode(kvp.Key, _mousePosition));
            }
        }

        protected override List<string> GetStarredContextMenu() {
            List<string> starredPaths = new List<string>();
            if (selectedOutPoint != null) {
                var registry = CommandRegistry.GetRegistry(commandDataType, ((CommandNode)selectedOutPoint.node).GetCommand().GetType());
                if (registry != null) {
                    if (registry.starredCommands != null) {
                        foreach (var starredCommand in registry.starredCommands) {
                            if (PropertyConnectionPoint.variableTypes.TryGetValue(selectedOutPoint.GetType(), out Type _type)) {
                                if (_type == starredCommand.propertyType) {
                                    starredPaths.Add(CommandRegistry.GetRegistry(commandDataType, starredCommand.commandType).pathName);
                                }
                            }
                        }
                    }
                }

                if (PropertyConnectionPoint.variableTypes.TryGetValue(selectedOutPoint.GetType(), out Type _propertyType)) {
                    if (GlobalStarredCommandMenuAttribute.TryGetStarredCommands(commandDataType, _propertyType, out var commandTypes)) {
                        foreach(var commandType in commandTypes) {
                            starredPaths.Add(CommandRegistry.GetRegistry(commandDataType, commandType).pathName);
                        }
                    }
                }
            }

            return starredPaths;
        }

        protected void OnClickAddNode(Type _type, Vector2 _mousePosition) {
            if (nodes == null || m_commandData == null) {
                return;
            }
            Command command = (Command)Activator.CreateInstance(_type);
            command.nodePosition = _mousePosition - realPositionOffset;
            AddNewCommand(command);
            ConnectIfSelectedOutExists(command);

        }

        protected void OnClickAddNode<T2>(Vector2 _mousePosition) {
            OnClickAddNode(typeof(T2), _mousePosition);
        }

        private void ConnectIfSelectedOutExists(Command _command) {
            if (selectedOutPoint != null) {
                CommandNode inCommand = m_commandNodes[_command.id];
                if (inCommand.inPoints.Count > 0) {
                    OnClickInPoint(inCommand.inPoints[0]);
                    RepaintContainer();
                }
                else {
                    Repaint();
                }
            }
            else {
                Repaint();
            }
        }

        protected virtual void AddNewCommand(Command _command) {
            Undo.RecordObject(m_commandData.targetObject, "Added a new command");
            m_commandData.AddCommand(_command);
            ReloadNodes();
            leftPaneHandler.RefreshCommandList(nodes, GoToNode);
            saveAvailable = true;
            EditorUtility.SetDirty(m_commandData.targetObject);
        }

        protected virtual void AddNewCommands(List<Command> _commands, bool _resetNullConnections = true) {
            Undo.RecordObject(m_commandData.targetObject, "Added new commands");
            foreach (Command command in _commands) {
                var oldId = command.id;
                m_commandData.AddCommand(command);
            }
            LoadNewlyAddedNodes(_commands);
            leftPaneHandler.RefreshCommandList(nodes, GoToNode);
            saveAvailable = true;
            EditorUtility.SetDirty(m_commandData.targetObject);
        }

        private bool IsCopyAvailable() {
            return selectedNodes.Count > 0;
        }

        private void Copy() {
            CommandNode.clipboardCommands.Clear();
            foreach (CommandNode node in selectedNodes) {
                CommandNode.clipboardCommands.Add(node.GetCommand());
            }
        }

        private bool IsPasteAvailable() {
            try {
                return selectedNodes.Count != 1 && CommandNode.clipboardCommands.Count > 0;
            }
            catch {
                return false;
            }
        }

        private void Paste(Vector2 _mousePosition) {
            //DialogueCommand copiedCommand = ClipboardUtility.Paste<DialogueCommand>();
            List<Command> copiedCommands = (List<Command>)SerializationUtility.CreateCopy(CommandNode.clipboardCommands);
            if (copiedCommands != null) {
                if (copiedCommands.Count > 0) {
                    Vector2 offset = _mousePosition - copiedCommands[0].nodePosition - realPositionOffset;
                    selectedNodes.Clear();
                    Dictionary<int, Command> oldIdCommands = new Dictionary<int, Command>();
                    foreach (Command command in copiedCommands) {
                        command.nodePosition = command.nodePosition + offset;
                        oldIdCommands.Add(command.id, command);
                    }
                    AddNewCommands(copiedCommands, false);
                    AdjustConnectionWithNewIDs(copiedCommands, oldIdCommands);

                    if (copiedCommands.Count == 1) {
                        ConnectIfSelectedOutExists(copiedCommands[0]);
                    }
                }
            }
        }

        private void AdjustConnectionWithNewIDs(List<Command> copiedCommands, Dictionary<int, Command> oldIdCommands) {
            foreach (Command command in copiedCommands) {
                for (int i = 0; i < command.nextIds.Count; i++) {
                    for (int j = 0; j < command.nextIds[i].Count; j++) {
                        int oldNextId = command.nextIds[i][j].targetId;
                        if (oldIdCommands.ContainsKey(oldNextId)) {
                            command.nextIds[i][j].targetId = oldIdCommands[oldNextId].id;
                        }
                        else {
                            command.nextIds[i][j].Set(0, 0);
                        }
                    }
                }
                for (int i = 0; i < command.inputIds.Count; i++) {
                    int oldInputId = command.inputIds[i].targetId;
                    if (oldIdCommands.ContainsKey(oldInputId)) {
                        command.inputIds[i].targetId = oldIdCommands[oldInputId].id;
                    }
                }
            }
            LoadNewlyAddedConnections(copiedCommands);
            foreach (Command command in copiedCommands) {
                CommandNode commandNode = m_commandNodes[command.id];
                selectedNodes.Add(commandNode);
                commandNode.isSelected = true;
                commandNode.style = commandNode.selectedNodeStyle;
            }
            Repaint();
        }
        #endregion
    }
}
