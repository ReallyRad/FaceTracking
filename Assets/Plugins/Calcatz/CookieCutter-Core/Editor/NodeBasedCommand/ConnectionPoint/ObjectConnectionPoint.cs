using System;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class ObjectConnectionPoint : PropertyConnectionPoint {

        public ObjectConnectionPoint(Node _node, ConnectionPointType _type, GUIStyle _style, Action<ConnectionPoint> _onClickConnectionPoint)
            : base (_node, _type, _style, _onClickConnectionPoint){
            
        }

        public override Color GetColor() {
            return new Color(0.302f, 0.416f, 1, 1);
        }

    }
}
