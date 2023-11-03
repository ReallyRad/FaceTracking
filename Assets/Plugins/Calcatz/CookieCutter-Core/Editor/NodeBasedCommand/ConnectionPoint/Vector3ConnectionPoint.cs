using System;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class Vector3ConnectionPoint : PropertyConnectionPoint {

        public Vector3ConnectionPoint(Node _node, ConnectionPointType _type, GUIStyle _style, Action<ConnectionPoint> _onClickConnectionPoint)
            : base (_node, _type, _style, _onClickConnectionPoint){
            
        }

        public override Color GetColor() {
            return new Color(0.988f, 0.85f, 0.396f, 1);
        }

    }
}
