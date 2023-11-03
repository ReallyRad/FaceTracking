using System;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class FloatConnectionPoint : PropertyConnectionPoint {

        public FloatConnectionPoint(Node _node, ConnectionPointType _type, GUIStyle _style, Action<ConnectionPoint> _onClickConnectionPoint)
            : base (_node, _type, _style, _onClickConnectionPoint){
            
        }

        public override Color GetColor() {
            return new Color(0.294f, 0.675f, 0.839f, 1);
        }

    }
}
