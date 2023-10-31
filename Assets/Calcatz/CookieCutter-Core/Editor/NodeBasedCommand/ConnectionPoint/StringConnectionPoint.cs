using System;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class StringConnectionPoint : PropertyConnectionPoint {

        public StringConnectionPoint(Node _node, ConnectionPointType _type, GUIStyle _style, Action<ConnectionPoint> _onClickConnectionPoint)
            : base (_node, _type, _style, _onClickConnectionPoint){
            
        }

        public override Color GetColor() {
            return new Color(0.988f, 0.494f, 0.874f, 1);
        }

    }
}
