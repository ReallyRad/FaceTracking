using System;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class QuaternionConnectionPoint : PropertyConnectionPoint {

        public QuaternionConnectionPoint(Node _node, ConnectionPointType _type, GUIStyle _style, Action<ConnectionPoint> _onClickConnectionPoint)
            : base (_node, _type, _style, _onClickConnectionPoint){
            
        }

        public override Color GetColor() {
            return new Color32(223, 157, 90, 255);
        }

    }
}
