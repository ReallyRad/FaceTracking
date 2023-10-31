using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// An attribute that binds a subclass of TextBehaviourComponent to draw using custom property drawer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CustomTextBehaviourEditorAttribute : Attribute {

        public static Dictionary<Type, Type> editorTypes = new Dictionary<Type, Type>();

        public Type textBehaviourComponentType;

        public CustomTextBehaviourEditorAttribute(Type _textBehaviourComponentType) {
            textBehaviourComponentType = _textBehaviourComponentType;
        }

    }

}
