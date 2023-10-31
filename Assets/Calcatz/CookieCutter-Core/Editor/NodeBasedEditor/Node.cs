using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class Node {
        public const float verticalSpacing = 5;

        public Rect rect;
        public bool isDragged;
        public bool isSelected;

        public List<ConnectionPoint> inPoints;
        public List<ConnectionPoint> outPoints;

        public GUIStyle style;
        public GUIStyle defaultNodeStyle;
        public GUIStyle selectedNodeStyle;

        public Action<Node> OnRemoveNode;


        public class Config {
            public GUIStyle nodeStyle;
            public GUIStyle selectedStyle;
            public GUIStyle inPointStyle;
            public GUIStyle outPointStyle;
            public Action<ConnectionPoint> onClickInPoint;
            public Action<ConnectionPoint> onClickOutPoint;
            public Action<Node> onClickRemoveNode;
            public Config(GUIStyle _nodeStyle, GUIStyle _selectedNodeStyle, GUIStyle _inPointStyle, GUIStyle _outPointStyle, Action<ConnectionPoint> _onClickInPoint, Action<ConnectionPoint> _onClickOutPoint, Action<Node> _onClickRemoveNode) {
                nodeStyle = _nodeStyle;
                selectedStyle = _selectedNodeStyle;
                inPointStyle = _inPointStyle;
                outPointStyle = _outPointStyle;
                onClickInPoint = _onClickInPoint;
                onClickOutPoint = _onClickOutPoint;
                onClickRemoveNode = _onClickRemoveNode;
            }
        }

        public Node(Vector2 position, float width, float height, Config config) {
            rect = new Rect(position.x, position.y, width, height);
            inPoints = new List<ConnectionPoint>();
            outPoints = new List<ConnectionPoint>();
            SetConfig(config);
        }

        public ConnectionPoint AddInPoint(Config _config) {
            ConnectionPoint point = new ConnectionPoint(this, ConnectionPointType.In, _config.inPointStyle, _config.onClickInPoint);
            inPoints.Add(point);
            return point;
        }

        public ConnectionPoint AddOutPoint(Config _config) {
            ConnectionPoint point = new ConnectionPoint(this, ConnectionPointType.Out, _config.outPointStyle, _config.onClickOutPoint);
            outPoints.Add(point);
            return point;
        }

        public void SetConfig(Config _config) {
            style = _config.nodeStyle;
            foreach(ConnectionPoint point in inPoints) {
                point.style = _config.inPointStyle;
            }
            foreach (ConnectionPoint point in outPoints) {
                point.style = _config.outPointStyle;
            }
            defaultNodeStyle = _config.nodeStyle;
            selectedNodeStyle = _config.selectedStyle;
            OnRemoveNode = _config.onClickRemoveNode;
        }

        public void Drag(Vector2 delta) {
            rect.position += delta;
        }

        public virtual void Draw(Vector2 _offset) {
            GUI.Box(new Rect(rect.position + _offset, rect.size), GUIContent.none, style);
        }

        protected virtual void DrawOutPoint(Vector2 _nodePosition, int _index, float _yPos) {
            outPoints[_index].Draw(_nodePosition, _yPos);
        }

        protected virtual void DrawInPoint(Vector2 _nodePosition, int _index, float _yPos) {
            inPoints[_index].Draw(_nodePosition, _yPos);
        }

        public bool ProcessEvents(Vector2 _offset, Event e, float _zoomScale, ref bool _anyNodeClicked) {
            Rect rectContainer = new Rect(rect.position + _offset, rect.size);
            switch (e.type) {
                case EventType.MouseDown:
                    if (e.button == 0 || e.button == 1) {
                        if (rectContainer.Contains(e.mousePosition) && !e.alt && !_anyNodeClicked) {
                            if (e.button == 0) {    //only drag with left click
                                isDragged = true;
                            }
                            GUI.changed = true;
                            isSelected = true;
                            style = selectedNodeStyle;
                            _anyNodeClicked = true;
                        } else {
                            GUI.changed = true;
                            isSelected = false;
                            style = defaultNodeStyle;
                        }
                    }
                    if (e.button == 1 && isSelected && rectContainer.Contains(e.mousePosition)) {
                        ProcessContextMenu(_zoomScale);
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (isDragged) {
                        OnEndDragNode();
                    }
                    isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged) {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;

                case EventType.KeyDown:
                    if (isSelected) {
                        if (e.control) {
                            if (e.keyCode == KeyCode.C) {
                                if (IsCopyAvailable()) {
                                    Copy();
                                }
                            }
                            else if (e.keyCode == KeyCode.V) {
                                Paste();
                            }
                        }
                        else if (e.keyCode == KeyCode.Delete) {
                            OnClickRemoveNode();
                        }
                    }
                    break;
            }
            return false;
        }

        public bool ProcessSelectedNodesEvents(Vector2 _offset, Event e, HashSet<Node> _selectedNodes) {
            Rect rectContainer = new Rect(rect.position + _offset, rect.size);
            switch (e.type) {
                case EventType.MouseDown:
                    if (e.button == 0) {
                        if (rectContainer.Contains(e.mousePosition)) {
                            if (!e.alt) {
                                foreach (Node node in _selectedNodes) {
                                    node.isDragged = true;
                                }
                            }
                        }
                        else {
                            return false;
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (isDragged) {
                        OnEndDragNode();
                    }
                    isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged) {
                        foreach (Node node in _selectedNodes) {
                            node.Drag(e.delta);
                        }
                        e.Use();
                    }
                    break;
            }
            return true;
        }

        protected virtual void OnEndDragNode() {

        }

        private void ProcessContextMenu(float _zoomScale) {
            GenericMenu genericMenu = new GenericMenu();
            HandleContextMenu(genericMenu);

            Matrix4x4 m4 = GUI.matrix;
            GUI.matrix = GUI.matrix * Matrix4x4.Scale(new Vector3(1 / _zoomScale, 1 / _zoomScale, 1 / _zoomScale));
            genericMenu.ShowAsContext();
            GUI.matrix = m4;
        }

        protected virtual bool IsCopyAvailable() {
            return false;
        }

        protected virtual void Copy() { }

        protected virtual void Paste() { }

        protected virtual void HandleContextMenu(GenericMenu _genericMenu) {
            _genericMenu.AddItem(new GUIContent("Remove Node"), false, OnClickRemoveNode);
        }

        protected virtual void OnClickRemoveNode() {
            if (EditorUtility.DisplayDialog("Remove a Node", "Are you sure you want to remove this node?", "Yes", "No")) {
                OnRemoved();
                if (OnRemoveNode != null) {
                    OnRemoveNode(this);
                }
            }
        }

        protected virtual void OnRemoved() {

        }
    }
}