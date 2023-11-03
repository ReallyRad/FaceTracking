using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

#if UNITY_EDITOR
    public class StarredCommandPair {
        public Type propertyType;
        public Type commandType;
    }
#endif

    /// <summary>
    /// Add starred commands (recommended commands) in add-command context menu when currently holding an output point connection of an existing command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StarredCommandMenuAttribute : Attribute {

#if !UNITY_EDITOR
        public StarredCommandMenuAttribute(Type[] _commandDataTypes, Type[] _recommendedCommandTypes) { }
        public StarredCommandMenuAttribute(Type _propertyType, Type[] _commandDataTypes, params Type[] _recommendedCommandTypes) { }
#else

        public Type[] commandDataTypes;
        public StarredCommandPair[] recommendedCommandTypes;

        public StarredCommandMenuAttribute(Type[] _commandDataTypes, params Type[] _recommendedCommandTypes) {
            Init(null, _commandDataTypes, _recommendedCommandTypes);
        }

        public StarredCommandMenuAttribute(Type _propertyType, Type[] _commandDataTypes, params Type[] _recommendedCommandTypes) {
            Init(_propertyType, _commandDataTypes, _recommendedCommandTypes);
        }

        private void Init(Type _propertyType, Type[] _commandDataTypes, Type[] _recommendedCommandTypes) {
            List<Type> types = new List<Type>();
            foreach (Type type in _commandDataTypes) {
                if (type.IsSubclassOf(typeof(CommandData))) {
                    types.Add(type);
                }
                else {
                    Debug.LogError("RegisterCommandAttribute: Invalid command data type: " + type + " is not inherited from CommandData.");
                }
            }
            commandDataTypes = types.ToArray();

            List<StarredCommandPair> pairs = new List<StarredCommandPair>();

            foreach (Type type in _recommendedCommandTypes) {
                if (type.IsSubclassOf(typeof(Command))) {
                    pairs.Add(new StarredCommandPair() {
                        propertyType = _propertyType == null? typeof(Nullable) : _propertyType,
                        commandType = type
                    });
                }
                else {
                    Debug.LogError("RegisterCommandAttribute: Invalid command type: " + type + " is not inherited from Command.");
                }
            }
            recommendedCommandTypes = pairs.ToArray();
        }
#endif

    }
}
