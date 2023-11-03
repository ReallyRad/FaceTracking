using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;

namespace Calcatz.CookieCutter {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CustomCommandNodeDrawerAttribute : Attribute {

        private static readonly Dictionary<Type, Type> commandNodes = new Dictionary<Type, Type>();

        public Type commandType;

        public CustomCommandNodeDrawerAttribute(Type _commandType) {
            if (IsCommandType(_commandType)) {
                commandType = _commandType;
            }
            else {
                Debug.LogError("Invalid command type for custom drawer: " + _commandType + " is not inherited from Command.");
            }
        }

        private static bool IsCommandType(Type _commandType) {
            return _commandType.IsSubclassOf(typeof(Command)) || _commandType == typeof(Command);
        }

        public static Type GetCommandNode(Command _command) {
            return GetCommandNode(_command.GetType());
        }

        public static Type GetCommandNode(Type _commandType) {
            if (IsCommandType(_commandType)) {
                Type nodeType = null;
                if (commandNodes.TryGetValue(_commandType, out nodeType)) {
                    return nodeType;
                }
                /*else if (_commandType == typeof(VariableCommand)/%_commandType.IsSubclassOf(typeof(VariableCommand))%/) {
                    return typeof(VariableCommandNode);
                }*/
                else if (_commandType.IsSubclassOf(typeof(BuildablePropertyCommand))) {
                    return typeof(BuildablePropertyCommandNode);
                }
                else {
                    return typeof(BuildableCommandNode);
                }
            }
            else {
                Debug.LogError("Invalid command type: " + _commandType + " is not inherited from Command.");
                return null;
            }

        }

        public static bool RefreshHandlers() {
            TypeCache.TypeCollection nodeTypes = TypeCache.GetTypesWithAttribute<CustomCommandNodeDrawerAttribute>();
            foreach (Type nodeType in nodeTypes) {
                CustomCommandNodeDrawerAttribute customDrawerAttribute = nodeType.GetCustomAttribute<CustomCommandNodeDrawerAttribute>();
                if (customDrawerAttribute.commandType != null) {
                    if (!commandNodes.ContainsKey(customDrawerAttribute.commandType)) {
                        if (customDrawerAttribute.commandType.IsSubclassOf(typeof(Command)) || customDrawerAttribute.commandType == typeof(Command)) {
                            if (nodeType.IsSubclassOf(typeof(CommandNode))) {
                                commandNodes.Add(customDrawerAttribute.commandType, nodeType);
                            }
                            else {
                                Debug.LogError("Custom command node drawer " + nodeType + " is not inherited from CommandNode.");
                            }

                        }
                    }
                }
            }
            return true;
        }


    }
}
