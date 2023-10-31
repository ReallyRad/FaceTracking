using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ShowCommandNodesAttribute : PropertyAttribute {

        public int height = 0;
        public bool showCommandInspectorButton = false;

        public ShowCommandNodesAttribute(bool _showCommandInspectorButton = false) {
            showCommandInspectorButton = _showCommandInspectorButton;
        }

        public ShowCommandNodesAttribute(int _height, bool _showCommandInspectorButton = false) {
            height = _height;
            showCommandInspectorButton = _showCommandInspectorButton;
        }

    }

}
