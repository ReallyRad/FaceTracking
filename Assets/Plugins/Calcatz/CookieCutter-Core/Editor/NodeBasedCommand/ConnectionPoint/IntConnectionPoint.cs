using System;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class IntConnectionPoint : PropertyConnectionPoint {

        public IntConnectionPoint(Node _node, ConnectionPointType _type, GUIStyle _style, Action<ConnectionPoint> _onClickConnectionPoint)
            : base (_node, _type, _style, _onClickConnectionPoint){
            
        }

        public override Color GetColor() {
            return new Color(0.33f, 0.84f, 0.294f, 1);
        }

    }
}
