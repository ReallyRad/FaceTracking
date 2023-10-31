using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class PropertyConnectionPoint : ConnectionPoint {

        private static GUIStyle inStyle;
        private static GUIStyle outStyle;

        public static readonly Dictionary<Type, Type> connectionTypes = new Dictionary<Type, Type>() {
            {typeof(bool), typeof(BooleanConnectionPoint) },
            {typeof(int), typeof(IntConnectionPoint) },
            {typeof(float), typeof(FloatConnectionPoint) },
            {typeof(string), typeof(StringConnectionPoint) },
            {typeof(Vector3), typeof(Vector3ConnectionPoint) },
            {typeof(UnityEngine.Object), typeof(ObjectConnectionPoint) },
            {typeof(PropertyConnectionPoint), typeof(PropertyConnectionPoint) }, //Unspecified
            {typeof(Quaternion), typeof(QuaternionConnectionPoint) }
        };

        public static readonly Dictionary<Type, Type> variableTypes = new Dictionary<Type, Type>() {
            {typeof(MainConnectionPoint), typeof(Nullable) },
            {typeof(BooleanConnectionPoint), typeof(bool) },
            {typeof(IntConnectionPoint), typeof(int) },
            {typeof(FloatConnectionPoint), typeof(float) },
            {typeof(StringConnectionPoint), typeof(string)  },
            {typeof(Vector3ConnectionPoint), typeof(Vector3) },
            {typeof(ObjectConnectionPoint), typeof(UnityEngine.Object) },
            {typeof(PropertyConnectionPoint), typeof(object) }, //Unspecified
            {typeof(QuaternionConnectionPoint), typeof(Quaternion) }
        };

        public PropertyConnectionPoint(Node _node, ConnectionPointType _type, GUIStyle _style, Action<ConnectionPoint> _onClickConnectionPoint)
            : base (_node, _type, _style, _onClickConnectionPoint){
            if (inStyle == null) {
                inStyle = new GUIStyle();
                inStyle.normal.background = btnLeftTex;
                inStyle.active.background = btnLeftOnTex;
                inStyle.border = new RectOffset(4, 4, 12, 12);
            }
            if (outStyle == null) {
                outStyle = new GUIStyle();
                outStyle.normal.background = btnRightTex;
                outStyle.active.background = btnRightOnTex;
                outStyle.border = new RectOffset(4, 4, 12, 12);
            }
            if (_type == ConnectionPointType.In) {
                style = inStyle;
            }
            else if (_type == ConnectionPointType.Out) {
                style = outStyle;
            }
        }

        public override Color GetColor() {
            return Color.gray;
        }

    }
}
