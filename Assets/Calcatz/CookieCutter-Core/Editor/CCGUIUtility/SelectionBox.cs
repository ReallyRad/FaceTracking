using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class SelectionBox {
        private static Vector2 anchorPoint;

        public static void Draw(Event _event, System.Action<Rect> _onMouseDrag, System.Action<Rect> _onMouseUp) {
            //Rect position = new Rect(0, 0, Screen.width, Screen.height);

            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            switch (_event.GetTypeForControl(controlID)) {
                case EventType.MouseDown:
                    if (_event.button == 0 /*&& position.Contains(_event.mousePosition)*/ && !_event.alt) {
                        anchorPoint = _event.mousePosition;

                        // Lock input focus onto the selection area.
                        GUIUtility.hotControl = controlID;
                        _event.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (_event.button == 0 && GUIUtility.hotControl == controlID) {
                        // Force repaint as mouse is dragged.
                        if (_onMouseDrag != null) {
                            _onMouseDrag.Invoke(GetRectFromSelection(_event));
                        }
                        _event.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (_event.button == 0 && GUIUtility.hotControl == controlID) {
                        // Very important, we need to release our lock on the mouse!
                        GUIUtility.hotControl = 0;

                        if (_onMouseUp != null) {
                            _onMouseUp.Invoke(GetRectFromSelection(_event));
                        }
                        _event.Use();
                    }
                    break;

                case EventType.Repaint:
                    if (GUIUtility.hotControl == controlID) {
                        Color col = GUI.color;
                        GUI.color = new Color(0.3f, 0.584f, 1f, 0.5f);
                        GUI.Box(GetRectFromSelection(_event), GUIContent.none);
                        GUI.color = col;
                    }
                    break;
            }
        }

        private static Rect GetRectFromSelection(Event _event) {
            var rect = new Rect(
                anchorPoint.x,
                anchorPoint.y,
                _event.mousePosition.x - anchorPoint.x,
                _event.mousePosition.y - anchorPoint.y
            );

            // Normalize bounds of rectangle (don't want negative area).
            if (rect.width < 0) {
                rect.x += rect.width;
                rect.width = -rect.width;
            }
            if (rect.height < 0) {
                rect.y += rect.height;
                rect.height = -rect.height;
            }

            return rect;
        }
    }
}