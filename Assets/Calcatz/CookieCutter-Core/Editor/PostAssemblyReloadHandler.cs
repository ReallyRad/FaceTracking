using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {
    [InitializeOnLoad]
    internal static class PostAssemblyReloadHandler {
        static PostAssemblyReloadHandler() {
            AssemblyReloadEvents.afterAssemblyReload += Rebuild;
        }

        private static bool rebuilt = false;

        public static void Rebuild() {
            if (rebuilt) return;
            rebuilt = true;

            CustomCommandNodeDrawerAttribute.RefreshHandlers();
            CommandNodesContainerAttribute.RefreshAvailableCommands();
            CommandNode.RefreshMonoScriptsReference();

            RegisterCommandsByAttribute();
            AddStarredCommandsByAttribute();
            GlobalStarredCommandMenuAttribute.RefreshCollections();
        }

        private static void RegisterCommandsByAttribute() {
            TypeCache.TypeCollection commandTypes = TypeCache.GetTypesWithAttribute<RegisterCommandAttribute>();
            foreach (Type commandType in commandTypes) {
                if (!commandType.IsSubclassOf(typeof(Command))) {
                    Debug.LogError("RegisterCommandAttribute: Invalid command type: " + commandType + " is not inherited from Command.");
                    continue;
                }
                RegisterCommandAttribute attribute = commandType.GetCustomAttribute<RegisterCommandAttribute>();
                foreach (Type commandDataType in attribute.commandDataTypes) {
                    if (RegisterCommandAttribute.IsCommandDataType(commandDataType)) {
                        CommandRegistry.RegisterCommand(commandDataType, commandType, attribute.allowCreateNode, attribute.pathName);
                    }
                    else {
                        Debug.LogError("RegisterCommandAttribute: Invalid command data type: " + commandDataType + " is not inherited from CommandData.");
                    }
                }
            }
        }

        private static void AddStarredCommandsByAttribute() {
            TypeCache.TypeCollection commandTypes = TypeCache.GetTypesWithAttribute<StarredCommandMenuAttribute>();
            foreach (Type commandType in commandTypes) {
                if (!commandType.IsSubclassOf(typeof(Command))) {
                    Debug.LogError("StarredCommandMenuAttribute: Invalid command type: " + commandType + " is not inherited from Command.");
                    continue;
                }
                var attributes = commandType.GetCustomAttributes<StarredCommandMenuAttribute>();
                foreach (var attribute in attributes) {
                    foreach (Type commandDataType in attribute.commandDataTypes) {
                        if (RegisterCommandAttribute.IsCommandDataType(commandDataType)) {
                            var reg = CommandRegistry.GetRegistry(commandDataType, commandType);
                            foreach (var starredCommandPair in attribute.recommendedCommandTypes) {
                                if (reg.starredCommands == null) reg.starredCommands = new HashSet<StarredCommandPair>();
                                reg.starredCommands.Add(starredCommandPair);
                            }
                        }
                        else {
                            Debug.LogError("RegisterCommandAttribute: Invalid command data type: " + commandDataType + " is not inherited from CommandData.");
                        }
                    }
                }
            }
        }

    }
}
