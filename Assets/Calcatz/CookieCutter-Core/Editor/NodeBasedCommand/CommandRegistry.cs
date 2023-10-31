using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
#else
using Calcatz.OdinSerializer;
#endif

namespace Calcatz.CookieCutter {
    public class CommandRegistry {

        public class Registry {
            public bool allowCreateNode = true;
            public string pathName;
            public HashSet<StarredCommandPair> starredCommands = null;
        }

        //Type: Command
        private Dictionary<Type, Registry> registries = new Dictionary<Type, Registry>();

        public bool ContainsCommand(Type _commandType) {
            return registries.ContainsKey(_commandType);
        }

        //Type: CommandData
        private static Dictionary<Type, CommandRegistry> instances = new Dictionary<Type, CommandRegistry>();

        public static CommandRegistry GetInstance<T>() where T : CommandData {
            return GetInstance(typeof(T));
        }

        public static CommandRegistry GetInstance(Type _commandDataType) {
            if (!instances.ContainsKey(_commandDataType)) {
                CommandRegistry commandRegistry = new CommandRegistry();
                instances.Add(_commandDataType, commandRegistry);
            }
            return instances[_commandDataType];
        }

        public static void RegisterCommand<TCommandData, TCommand>(bool _allowCreateNode, string _pathName) where TCommandData : CommandData where TCommand : Command {
            RegisterCommand(typeof(TCommandData), typeof(TCommand), _allowCreateNode, _pathName);
        }
        public static void RegisterCommand<T>(Type _commandType, bool _allowCreateNode, string _pathName) where T : CommandData {
            RegisterCommand(typeof(T), _commandType, _allowCreateNode, _pathName);
        }
        public static void RegisterCommand(Type _commandDataType, Type _commandType, bool _allowCreateNode, string _pathName) {
            if (!GetInstance(_commandDataType).registries.ContainsKey(_commandType)) {
                GetInstance(_commandDataType).registries.Add(_commandType, new Registry() { allowCreateNode = _allowCreateNode, pathName = _pathName });
            }
            else {
                GetInstance(_commandDataType).registries[_commandType].allowCreateNode = _allowCreateNode;
                GetInstance(_commandDataType).registries[_commandType].pathName = _pathName;
            }
        }
        public static void UnregisterCommand(Type _commandDataType, Type _commandType) {
            if (GetInstance(_commandDataType).registries.ContainsKey(_commandType)) {
                GetInstance(_commandDataType).registries.Remove(_commandType);
            }
        }
        public static void UnregisterCommand<T>(Type _commandType) {
            if (GetInstance(typeof(T)).registries.ContainsKey(_commandType)) {
                GetInstance(typeof(T)).registries.Remove(_commandType);
            }
        }

        public static CommandNode CreateNode<T>(CommandData _commandData, Command _command, Vector2 _nodePosition, Node.Config _config) where T : CommandData {
            return CreateNode(typeof(T), _commandData, _command, _nodePosition, _config);
        }

        public static CommandNode CreateNode(Type _commandDataType, CommandData _commandData, Command _command, Vector2 _nodePosition, Node.Config _config) {
            Type commandType = _command.GetType();
            Registry registry;
            PostAssemblyReloadHandler.Rebuild();
            if (GetInstance(_commandDataType).registries.TryGetValue(commandType, out registry)) {
                Type commandNodeType = CustomCommandNodeDrawerAttribute.GetCommandNode(commandType);
                return (CommandNode)Activator.CreateInstance(commandNodeType, _commandData, _command, _nodePosition, _config);
                //Debug.LogError("Error creating node of type " + registry.type + " or the type is not derived from CommandNode.");
            }
            else {
                Debug.LogError("Command " + commandType + " is not registered to " + _commandDataType + ".");
                return null;
            }
        }

        public static void GetCommands<T>(out HashSet<KeyValuePair<Type, Registry>> _mainCommands, out HashSet<KeyValuePair<Type, Registry>> _propertyCommands) where T : CommandData {
            GetCommands(typeof(T), out _mainCommands, out _propertyCommands);
        }

        public static void GetCommands(Type _commandDataType, out HashSet<KeyValuePair<Type, Registry>> _mainCommands, out HashSet<KeyValuePair<Type, Registry>> _propertyCommands) {
            _mainCommands = new HashSet<KeyValuePair<Type, Registry>>();
            _propertyCommands = new HashSet<KeyValuePair<Type, Registry>>();
            foreach (KeyValuePair<Type, Registry> kvp in GetInstance(_commandDataType).registries) {
                if (kvp.Key.IsSubclassOf(typeof(PropertyCommand))) _propertyCommands.Add(kvp);
                else _mainCommands.Add(kvp);
            }
        }

        public static Registry GetRegistry<T>( Type _commandType) where T : CommandData {
            return GetRegistry(typeof(T), _commandType);
        }

        public static Registry GetRegistry(Type _commandDataType, Type _commandType) {
            return GetInstance(_commandDataType).registries[_commandType];
        }
    }
}
