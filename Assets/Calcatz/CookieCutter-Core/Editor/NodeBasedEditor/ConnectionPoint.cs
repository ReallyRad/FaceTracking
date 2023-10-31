using System;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public enum ConnectionPointType { In, Out }

    public class ConnectionPoint {
        public Rect rect;

        public ConnectionPointType type;

        public Node node;

        public GUIStyle style;

        public Action<ConnectionPoint> onClickConnectionPoint;

        #region IN-OUT-STYLES
        private static Texture2D m_btnLeftTex;
        protected static Texture2D btnLeftTex {
            get {
                if (m_btnLeftTex == null) {
                    m_btnLeftTex = EditorGUIUtility.Load("builtin skins/lightskin/images/btn left.png") as Texture2D;
                }
                return m_btnLeftTex;
            }
        }
        private static Texture2D m_btnLeftOnTex;
        protected static Texture2D btnLeftOnTex {
            get {
                if (m_btnLeftOnTex == null) {
                    m_btnLeftOnTex = EditorGUIUtility.Load("builtin skins/lightskin/images/btn left on.png") as Texture2D;
                }
                return m_btnLeftOnTex;
            }
        }
        private static Texture2D m_btnRightTex;
        protected static Texture2D btnRightTex {
            get {
                if (m_btnRightTex == null) {
                    m_btnRightTex = EditorGUIUtility.Load("builtin skins/lightskin/images/btn right.png") as Texture2D;
                }
                return m_btnRightTex;
            }
        }
        private static Texture2D m_btnRightOnTex;
        protected static Texture2D btnRightOnTex {
            get {
                if (m_btnRightOnTex == null) {
                    m_btnRightOnTex = EditorGUIUtility.Load("builtin skins/lightskin/images/btn right on.png") as Texture2D;
                }
                return m_btnRightOnTex;
            }
        }
        #endregion

        public ConnectionPoint(Node _node, ConnectionPointType _type, GUIStyle _style, Action<ConnectionPoint> _onClickConnectionPoint) {
            node = _node;
            type = _type;
            style = _style;
            onClickConnectionPoint = _onClickConnectionPoint;
            rect = new Rect(0, 0, 10f, 20f);
        }

        public void Draw(Vector2 _nodePosition, float _yPos) {
            rect.y = _yPos;

            Event e = Event.current;

            switch (type) {
                case ConnectionPointType.In:
                    rect.x = _nodePosition.x - rect.width + 8f;
                    if (e.type == EventType.MouseUp) {
                        if (e.button == 0 && rect.Contains(e.mousePosition)) {
                            if (onClickConnectionPoint != null) {
                                onClickConnectionPoint(this);
                            }
                            Event.current.Use();
                        }
                    }
                    else if (e.type == EventType.MouseDown) {
                        if (e.button == 0 && rect.Contains(e.mousePosition)) {
                            Event.current.Use();
                        }
                    }
                    break;

                case ConnectionPointType.Out:
                    rect.x = _nodePosition.x + node.rect.width - 8f;
                    if (e.type == EventType.MouseDown) {
                        if (e.button == 0 && rect.Contains(e.mousePosition)) {
                            if (onClickConnectionPoint != null) {
                                onClickConnectionPoint(this);
                            }
                            Event.current.Use();
                        }
                    }
                    break;
            }

            Color col = GUI.color;
            GUI.color = GetColor();
            //if (GUI.Button(rect, "", style)) {
            /*if (OnClickConnectionPoint != null) {
                OnClickConnectionPoint(this);
            }*/
            //}
            GUI.Box(rect, GUIContent.none, style);
            GUI.color = col;
        }

        public virtual Color GetColor() {
            return Color.white;
        }

        public virtual float GetConnectionWidth() {
            return 2f;
        }
    }
}