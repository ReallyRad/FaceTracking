using System;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class BooleanConnectionPoint : PropertyConnectionPoint {

        public BooleanConnectionPoint(Node _node, ConnectionPointType _type, GUIStyle _style, Action<ConnectionPoint> _onClickConnectionPoint)
            : base (_node, _type, _style, _onClickConnectionPoint){
            
        }

        public override Color GetColor() {
            return new Color(1, 0.13f, 0.13f, 1);
        }

    }
}
