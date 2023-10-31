using System;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class Connection {
        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;
        public Action<Connection> OnClickRemoveConnection;

        public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection) {
            this.inPoint = inPoint;
            this.outPoint = outPoint;
            this.OnClickRemoveConnection = OnClickRemoveConnection;
        }

        public void Draw() {
            Handles.DrawBezier(
                inPoint.rect.center,
                outPoint.rect.center,
                inPoint.rect.center + Vector2.left * 50f,
                outPoint.rect.center - Vector2.left * 50f,
                outPoint.GetColor(),
                null,
                outPoint.GetConnectionWidth()
            );

            //Color col = Handles.color;
            Color col = GUI.color;
            //Handles.color = outPoint.GetColor();
            var newColor = outPoint.GetColor();
            newColor.a = 0.75f;
            GUI.color = newColor;
            if (GUI.Button(new Rect(((inPoint.rect.center + outPoint.rect.center) * 0.5f) - new Vector2(5, 5), new Vector2(10, 10)), "")) {
            //if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap)) {
                if (Event.current.button == 0) {
                    if (OnClickRemoveConnection != null) {
                        OnClickRemoveConnection(this);
                    }
                }
            }
            //Handles.color = col;
            GUI.color = col;
        }
    }
}