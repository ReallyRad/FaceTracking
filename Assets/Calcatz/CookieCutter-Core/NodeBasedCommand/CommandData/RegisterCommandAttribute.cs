using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// An alternative way to register command without registering through Command Nodes Container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RegisterCommandAttribute : Attribute {

#if !UNITY_EDITOR
        public RegisterCommandAttribute(string _pathName, params Type[] _commandDataTypes) { }
        public RegisterCommandAttribute(bool _allowCreateNode, string _pathName, params Type[] _commandDataTypes) { }
#else

        public bool allowCreateNode;
        public string pathName;
        public Type[] commandDataTypes;

        public RegisterCommandAttribute(string _pathName, params Type[] _commandDataTypes) {
            allowCreateNode = true;
            Init(_pathName, _commandDataTypes);
        }

        public RegisterCommandAttribute(bool _allowCreateNode, string _pathName, params Type[] _commandDataTypes) {
            allowCreateNode = _allowCreateNode;
            Init(_pathName, _commandDataTypes);
        }

        private void Init(string _pathName, Type[] _commandDataTypes) {
            pathName = _pathName;
            List<Type> types = new List<Type>();
            foreach (Type type in _commandDataTypes) {
                if (IsCommandDataType(type)) {
                    types.Add(type);
                }
                else {
                    Debug.LogError("RegisterCommandAttribute: Invalid command data type: " + type + " is not inherited from CommandData.");
                }
            }
            commandDataTypes = types.ToArray();
        }

        public static bool IsCommandDataType(Type _commandDataType) {
            return _commandDataType.IsSubclassOf(typeof(CommandData));
        }
#endif

    }
}
