using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Calcatz.CookieCutter {

    public class NodesContainer {

        protected VisualElement nodesArea;
        public System.Func<Rect> containerAreaGetter;

        protected List<Node> nodes = new List<Node>();
        protected List<Connection> connections = new List<Connection>();
        protected HashSet<Node> selectedNodes = new HashSet<Node>();

        private GUIStyle nodeStyle;
        private GUIStyle selectedNodeStyle;

        private GUIStyle inPointStyle;
        private GUIStyle outPointStyle;

        private ConnectionPoint selectedInPoint;
        protected ConnectionPoint selectedOutPoint;

        protected Vector2 realPositionOffset = Vector2.zero;

        private float zoomScale = 1f;
        private bool isUsingSelectionBox;
        private string contextMenuSearchTerm = "";

        public static Color backgroundColor => EditorGUIUtility.isProSkin ? new Color32(56, 56, 56, 255) : new Color32(194, 194, 194, 255);

        public Vector2 RealPositionOffset { get => realPositionOffset; set => realPositionOffset = value; }
        public float ZoomScale { get => zoomScale; set => zoomScale = value; }

        public NodesContainer() {
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            selectedNodeStyle = new GUIStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0 on.png") as Texture2D;
            selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

            inPointStyle = new GUIStyle();
            inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            inPointStyle.border = new RectOffset(4, 4, 12, 12);

            outPointStyle = new GUIStyle();
            outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            outPointStyle.border = new RectOffset(4, 4, 12, 12);
        }
        
        protected void Repaint() {
            //repaintHandler.Invoke();
            if (nodesArea != null) {
                //attached on UIElements
                nodesArea?.MarkDirtyRepaint();
            }
            else {
                //attached on IMGUI
                GUI.changed = true;
            }
        }

        protected Rect GetContainerArea() {
            return containerAreaGetter();
        }

        protected virtual Rect GetNodesArea() {
            Rect container = GetContainerArea();
            if (nodesArea != null) {
                //using UIElements
                container.x = 0; container.y = 0;
            }
            return container;
        }

        protected virtual Node.Config GetNodeConfig() {
            Node.Config nodeConfig = new Node.Config(
                    nodeStyle, selectedNodeStyle,
                    inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
            return nodeConfig;
        }

        public void CreateGUI(VisualElement _root, System.Func<Rect> _containerAreaGetter) {
            containerAreaGetter = _containerAreaGetter;
            OnCreateGUI(_root);
        }

        protected virtual void OnCreateGUI(VisualElement _root) {
            nodesArea = CreateNodesArea();
            _root.Add(nodesArea);
        }

        protected void RecreateGUI() {
            if (nodesArea != null) {
                VisualElement root = nodesArea.parent;
                if (root != null) {
                    root.Clear();
                    CreateGUI(root, containerAreaGetter);
                }
            }
        }

        public virtual void OnDrawNodesArea() {
            if (nodesArea != null && !nodesArea.visible) return;
            Rect zoomArea = GetNodesArea();
            if (nodesArea != null) {
                //using UIElements
                zoomArea.position = new Vector2(0, 0);
            }
            EditorZoomArea.Begin(zoomScale, zoomArea, true);
            {
                DrawGrid(20, 0.2f, Color.gray);
                DrawGrid(100, 0.4f, Color.gray);

                DrawNodes();
                DrawConnections();

                DrawConnectionLine(Event.current);

                ProcessNodeEvents(Event.current);
                ProcessEvents(Event.current);
                HandleSelectionBox();

                //EditorGUILayout.LabelField("Total Selected: " + selectedNodes.Count);
                if (GUI.changed) {
                    Repaint();
                }
            }
            EditorZoomArea.End(true);
        }

        private VisualElement CreateNodesArea() {
            Rect nodesArea = GetNodesArea();
            return new IMGUIContainer(OnDrawNodesArea) {
                style = {
                    position = new StyleEnum<Position>(Position.Absolute),
                    left = new StyleLength(new Length(nodesArea.x, LengthUnit.Pixel)),
                    right =  new StyleLength(new Length(0, LengthUnit.Pixel)),
                    top = new StyleLength(new Length(0, LengthUnit.Pixel)),
                    bottom = new StyleLength(new Length(0, LengthUnit.Pixel))
                }/*,
                cullingEnabled = true*/
            };
        }

        protected void UpdateNodesAreaSize(bool _repaint = false) {
            nodesArea.style.left = new StyleLength(new Length(GetNodesArea().x, LengthUnit.Pixel));
            if (_repaint) {
                Repaint();
            }
        }

        public void GoToNode(CommandNode node) {
            Vector2 offset = GetNodesArea().size / 2;
            foreach (Node selectedNode in selectedNodes) {
                selectedNode.isSelected = false;
                selectedNode.style = selectedNode.defaultNodeStyle;
            }
            selectedNodes.Clear();
            selectedNodes.Add(node);
            node.isSelected = true;
            node.style = node.selectedNodeStyle;

            Vector2 targetPos = -node.rect.position - node.rect.size / 2 + offset;
            if (nodesArea != null) {
                ValueAnimation<Vector2> offsetAnim = nodesArea.experimental.animation.Start(realPositionOffset, targetPos, 250, OnGoToNodeAnimChange).Ease(Easing.InOutCubic).KeepAlive();
            }
            else {
                //using pure IMGUI
                OnGoToNodeAnimChange(null, targetPos);
            }
        }

        private void OnGoToNodeAnimChange(VisualElement _e, Vector2 _value) {
            realPositionOffset = _value;
            GUI.changed = true;
            Repaint();
        }

        private void HandleSelectionBox() {
            if (selectedOutPoint == null) {
                bool anyNodeSelected = false;
                if (!isUsingSelectionBox) {
                    foreach (Node node in nodes) {
                        if (node.isSelected) {
                            anyNodeSelected = true;
                            break;
                        }
                    }
                }

                if (!anyNodeSelected) {
                    SelectionBox.Draw(Event.current,
                        _dragRect => {
                            isUsingSelectionBox = true;
                            foreach (Node node in nodes) {
                                Rect rectContainer = new Rect(node.rect.position + realPositionOffset, node.rect.size);
                                if (_dragRect.Overlaps(rectContainer)) {
                                    node.isSelected = true;
                                    node.style = node.selectedNodeStyle;
                                }
                                else {
                                    node.isSelected = false;
                                    node.style = node.defaultNodeStyle;
                                }
                            }
                        },
                        _mouseUpRect => {
                            isUsingSelectionBox = false;
                            selectedNodes.Clear();
                            foreach (Node node in nodes) {
                                Rect rectContainer = new Rect(node.rect.position + realPositionOffset, node.rect.size);
                                if (_mouseUpRect.Overlaps(rectContainer)) {
                                    selectedNodes.Add(node);
                                }
                            }
                        }
                    );
                }
            }
        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor) {
            Vector2 size = GetContainerArea().size;
            int widthDivs = Mathf.CeilToInt(size.x / 0.2f / gridSpacing);
            int heightDivs = Mathf.CeilToInt(size.y / 0.2f / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            Vector3 newOffset = new Vector3(realPositionOffset.x % gridSpacing, realPositionOffset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++) {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, size.y / 0.2f, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++) {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(size.x / 0.2f, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void DrawNodes() {
            if (nodes != null) {
                for (int i = 0; i < nodes.Count; i++) {
                    nodes[i].Draw(realPositionOffset);
                }
            }
        }

        private void DrawConnections() {
            if (connections != null) {
                for (int i = 0; i < connections.Count; i++) {
                    connections[i].Draw();
                }
            }
        }

        private void DrawConnectionLine(Event e) {
            if (selectedInPoint != null && selectedOutPoint == null) {
                Handles.DrawBezier(
                    selectedInPoint.rect.center,
                    e.mousePosition,
                    selectedInPoint.rect.center + Vector2.left * 50f,
                    e.mousePosition - Vector2.left * 50f,
                    selectedInPoint.GetColor(),
                    null,
                    2f
                );

                GUI.changed = true;
            }

            if (selectedOutPoint != null && selectedInPoint == null) {
                Handles.DrawBezier(
                    selectedOutPoint.rect.center,
                    e.mousePosition,
                    selectedOutPoint.rect.center - Vector2.left * 50f,
                    e.mousePosition + Vector2.left * 50f,
                    selectedOutPoint.GetColor(),
                    null,
                    2f
                );

                GUI.changed = true;
            }
        }

        protected virtual void ProcessEvents(Event e) {
            if (e.type == EventType.MouseDown /*&& position.Contains(e.mousePosition)*/) {
                GUI.FocusControl(null);
            }

            switch (e.type) {
                case EventType.MouseDown:
                    if (e.button == 1) {
                        if (selectedOutPoint == null) {
                            if (selectedNodes.Count == 1) {
                                selectedNodes.Clear();
                            }
                            ProcessContextMenu(e.mousePosition);
                        }
                        else {
                            ClearConnectionSelection();
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 2) {
                        OnDrag(e.delta);
                    }
                    else if (e.button == 0) {
                        if (e.alt) {
                            OnDrag(e.delta);
                        }
                    }
                    break;

                case EventType.KeyDown:
                    if (e.control) {
                        if (e.keyCode == KeyCode.S) {
                            Save();
                        }
                    }
                    else if (e.alt) {
                        if (e.keyCode == KeyCode.A) {
                            OnDrag(Vector2.right * 20);
                        }
                        else if (e.keyCode == KeyCode.D) {
                            OnDrag(Vector2.right * -20);
                        }
                        if (e.keyCode == KeyCode.W) {
                            OnDrag(Vector2.up * 20);
                        }
                        else if (e.keyCode == KeyCode.S) {
                            OnDrag(Vector2.up * -20);
                        }
                    }
                    else {
                        if (e.keyCode.ToString().Length == 1 && char.IsLetter(e.keyCode.ToString()[0])) {
                            GenericMenu genericMenu = new GenericMenu();
                            HandleContextMenu(e.mousePosition, genericMenu);
                            List<string> starredPaths = GetStarredContextMenu();
                            List<string> excludedNames = new List<string>() { "Copy", "Paste", "Remove Node", "Remove Selected Nodes" };
                            ScrollWheelSearchPopup.Show(genericMenu, e.mousePosition, e.keyCode, starredPaths, excludedNames);
                        }
                    }

                    if (e.keyCode == KeyCode.Delete) {
                        if (selectedNodes.Count > 1) {
                            OnClickRemoveSelectedNodes();
                        }
                    }
                    break;

                case EventType.ScrollWheel:
                    if (Event.current.delta.y < 0) {
                        zoomScale += 0.05f;
                        if (zoomScale > 1) zoomScale = 1f;
                    }
                    else if (Event.current.delta.y > 0) {
                        zoomScale -= 0.05f;
                        if (zoomScale < 0.2f) zoomScale = 0.2f;
                    }
                    Repaint();
                    break;
            }
        }

        private void ProcessNodeEvents(Event e) {
            if (nodes != null) {
                if (selectedNodes.Count <= 1) {
                    ProcessUnselectedNodeEvents(e);
                }
                else {
                    bool anyNodeClicked = false;

                    foreach (Node selectedNode in selectedNodes) {
                        if (selectedNode.ProcessSelectedNodesEvents(realPositionOffset, e, selectedNodes)) {
                            anyNodeClicked = true;
                        }
                    }
                    if (!anyNodeClicked) {
                        foreach (Node selectedNode in selectedNodes) {
                            selectedNode.isSelected = false;
                            selectedNode.style = selectedNode.defaultNodeStyle;
                        }
                        selectedNodes.Clear();

                        ProcessUnselectedNodeEvents(e);
                    }
                }
            }
        }

        private void ProcessUnselectedNodeEvents(Event e) {
            bool anyNodeClicked = false;
            for (int i = nodes.Count - 1; i >= 0; i--) {
                bool previouslyClicked = anyNodeClicked;
                bool guiChanged = nodes[i].ProcessEvents(realPositionOffset, e, zoomScale, ref anyNodeClicked);

                if (!previouslyClicked && anyNodeClicked) {
                    selectedNodes.Clear();
                    selectedNodes.Add(nodes[i]);
                    nodes[i].isSelected = true;
                    nodes[i].style = nodes[i].selectedNodeStyle;
                    Repaint();
                }

                if (guiChanged) {
                    GUI.changed = true;
                }
            }
        }

        protected virtual void Save() {

        }

        protected void ProcessContextMenu(Vector2 mousePosition) {
            GenericMenu genericMenu = new GenericMenu();
            HandleContextMenu(mousePosition, genericMenu);

            Matrix4x4 m4 = GUI.matrix;
            GUI.matrix = GUI.matrix * Matrix4x4.Scale(new Vector3(1 / zoomScale, 1 / zoomScale, 1 / zoomScale));
            List<string> starredPaths = GetStarredContextMenu();
            GenericMenuPopup popup = GenericMenuPopup.Show(genericMenu, "Create Command", mousePosition * zoomScale, starredPaths, contextMenuSearchTerm, _search => {
                contextMenuSearchTerm = _search;
            });
            GUI.matrix = m4;
        }

        protected virtual List<string> GetStarredContextMenu() {
            return null;
        }

        protected virtual void HandleContextMenu(Vector2 _mousePosition, GenericMenu _genericMenu) {
            if (selectedNodes.Count > 0) {
                _genericMenu.AddItem(new GUIContent("Remove Selected Nodes"), false, OnClickRemoveSelectedNodes);
            }
            //_genericMenu.AddItem(new GUIContent("Add node"), false, () => onClickAddNode(_mousePosition));
        }

        protected void OnDrag(Vector2 delta) {
            realPositionOffset += delta;
            GUI.changed = true;
        }

        protected virtual void OnClickRemoveNode(Node node) {
            if (connections != null) {
                List<Connection> connectionsToRemove = new List<Connection>();

                for (int i = 0; i < connections.Count; i++) {
                    if (node.inPoints.Contains(connections[i].inPoint) || node.outPoints.Contains(connections[i].outPoint)) {
                        connectionsToRemove.Add(connections[i]);
                    }
                }

                HandleRemovedConnections(node, connectionsToRemove);

                for (int i = 0; i < connectionsToRemove.Count; i++) {
                    connections.Remove(connectionsToRemove[i]);
                }

                connectionsToRemove = null;
            }

            nodes.Remove(node);
        }

        private void OnClickRemoveSelectedNodes() {
            if (EditorUtility.DisplayDialog("Remove Selected Nodes", "Are you sure you want to remove selected nodes?", "Yes", "No")) {
                foreach (Node node in selectedNodes) {
                    OnClickRemoveNode(node);
                }
                OnRemoveSelectedNodes();
                selectedNodes.Clear();
            }
        }

        internal virtual void OnRemoveSelectedNodes() {

        }

        protected virtual void HandleRemovedConnections(Node _removedNode, List<Connection> _connectionsToRemove) {

        }

        protected virtual void OnClickInPoint(ConnectionPoint inPoint) {
            selectedInPoint = inPoint;

            if (selectedOutPoint != null) {
                if (selectedOutPoint.node != selectedInPoint.node) {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else {
                    ClearConnectionSelection();
                }
            }
        }

        protected virtual void OnClickOutPoint(ConnectionPoint outPoint) {
            selectedNodes.Clear();

            selectedOutPoint = outPoint;

            if (selectedInPoint != null) {
                if (selectedOutPoint.node != selectedInPoint.node) {
                    CreateConnection();
                    ClearConnectionSelection();
                }
                else {
                    ClearConnectionSelection();
                }
            }
        }

        protected virtual void OnClickRemoveConnection(Connection connection) {
            connections.Remove(connection);
        }

        private void CreateConnection() {
            connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
        }

        protected void ClearConnectionSelection() {
            selectedInPoint = null;
            selectedOutPoint = null;
        }

        public virtual void RepaintIfDirty() {
            
        }

    }
}
