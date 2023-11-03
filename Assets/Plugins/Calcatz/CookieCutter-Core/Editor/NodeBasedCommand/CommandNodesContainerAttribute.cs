using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CommandNodesContainerAttribute : Attribute {

        public Type commandDataType;

        public CommandNodesContainerAttribute(Type _commandDataType) {
            if (IsCommandDataType(_commandDataType)) {
                commandDataType = _commandDataType;
            }
            else {
                Debug.LogError("CommandNodesContainerAttribute: Invalid command data type: " + _commandDataType + " is not inherited from CommandData.");
            }
        }

        private static bool IsCommandDataType(Type _commandDataType) {
            return _commandDataType.IsSubclassOf(typeof(CommandData));
        }

        public static bool RefreshAvailableCommands() {
            var commandDataTypes = TypeCache.GetTypesDerivedFrom<CommandData>().ToList();

            TypeCache.TypeCollection containerTypes = TypeCache.GetTypesWithAttribute<CommandNodesContainerAttribute>();
            foreach (Type type in containerTypes) {
                CommandNodesContainerAttribute attribute = type.GetCustomAttribute<CommandNodesContainerAttribute>();
                foreach (MethodInfo method in type.GetRuntimeMethods()) {
                    if (method.Name == "ValidateAvailableCommands") {
                        object tempInstance = Activator.CreateInstance(type);
                        method.Invoke(tempInstance, new object[] { attribute.commandDataType });
                        commandDataTypes.Remove(attribute.commandDataType);
                        break;
                    }
                }
            }

            if (commandDataTypes.Count > 0) {
                ResolveCommandDataWithoutNodeContainers(commandDataTypes);
            }

            return true;
        }

        private static void ResolveCommandDataWithoutNodeContainers(List<Type> commandDataTypes) {
            MethodInfo validateMethod = null;
            foreach (MethodInfo method in typeof(CommandNodesContainer).GetRuntimeMethods()) {
                if (method.Name == "ValidateAvailableCommands") {
                    validateMethod = method;
                }
            }
            if (validateMethod != null) {
                object temp = Activator.CreateInstance(typeof(CommandNodesContainer));
                foreach (var commandDataType in commandDataTypes) {
                    validateMethod.Invoke(temp, new object[] { commandDataType });
                }
            }
        }
    }
}
