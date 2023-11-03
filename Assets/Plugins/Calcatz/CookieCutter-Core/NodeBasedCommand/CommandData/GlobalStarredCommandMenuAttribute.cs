using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Add this command as one of the starred commands (recommended commands) in add-command context menu when currently holding an output point connection of an existing command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GlobalStarredCommandMenuAttribute : Attribute {

#if !UNITY_EDITOR
        public GlobalStarredCommandMenuAttribute(Type[] _commandDataTypes) { }
        public GlobalStarredCommandMenuAttribute(Type _propertyType, Type[] _commandDataTypes) { }
#else

        //1st key: CommandData type, 2nd key: property type
        private static readonly Dictionary<Type, Dictionary<Type, List<Type>>> starredCommands = new Dictionary<Type, Dictionary<Type, List<Type>>>();

        public Type[] commandDataTypes;
        public Type propertyType;

        public GlobalStarredCommandMenuAttribute(Type[] _commandDataTypes) {
            Init(null, _commandDataTypes);
        }

        public GlobalStarredCommandMenuAttribute(Type _propertyType, Type[] _commandDataTypes) {
            Init(_propertyType, _commandDataTypes);
        }

        private void Init(Type _propertyType, Type[] _commandDataTypes) {
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
            propertyType = _propertyType;
        }

        public static bool RefreshCollections() {
            TypeCache.TypeCollection commandTypes = TypeCache.GetTypesWithAttribute<GlobalStarredCommandMenuAttribute>();
            foreach (Type commandType in commandTypes) {
                if (commandType.IsSubclassOf(typeof(Command)) || commandType == typeof(Command)) {
                    var starredCommandAttribute = commandType.GetCustomAttribute<GlobalStarredCommandMenuAttribute>();
                    foreach (var commandDataType in starredCommandAttribute.commandDataTypes) {
                        Type prop = starredCommandAttribute.propertyType;
                        AddStarredCommand(commandDataType, prop, commandType);
                    }
                }
                else {
                    Debug.LogError("Global Starred Command " + commandType + " is not inherited from Command.");
                }
            }
            return true;
        }

        public static void AddStarredCommand(Type _commandDataType, Type _commandType) {
            AddStarredCommand(_commandDataType, null, _commandType);
        }

        public static void AddStarredCommand(Type _commandDataType, Type _propertyType, Type _commandType) {
            if (!starredCommands.ContainsKey(_commandDataType)) starredCommands.Add(_commandDataType, new Dictionary<Type, List<Type>>());
            var dict = starredCommands[_commandDataType];
            if (_propertyType == null) _propertyType = typeof(Nullable);
            if (!dict.ContainsKey(_propertyType)) dict.Add(_propertyType, new List<Type>());
            dict[_propertyType].Add(_commandType);
        }

        public static bool TryGetStarredCommands(Type _commandDataType, Type _propertyType, out Type[] result) {
            if (starredCommands.TryGetValue(_commandDataType, out var r1)) {
                if (r1.TryGetValue(_propertyType, out var r2)) {
                    result = r2.ToArray();
                    return true;
                }
            }
            result = null;
            return false;
        }
#endif

    }
}
