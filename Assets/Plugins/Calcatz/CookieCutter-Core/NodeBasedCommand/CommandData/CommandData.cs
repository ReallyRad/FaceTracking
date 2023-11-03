using System.Collections.Generic;
using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
#else
using Calcatz.OdinSerializer;
#endif

namespace Calcatz.CookieCutter {

    [Serializable]
    public class CommandData : IVariableUser {

        #region EDITOR
#if UNITY_EDITOR
        private Command m_lastExecutedCommand;
        internal Command lastExecutedCommand {
            get { return m_lastExecutedCommand; }
            set { m_lastExecutedCommand = value; }
        }

        internal bool needsRuntimeRepaint { get; set; }

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [HideInInspector]
        [SerializeField] private List<int> pooledIds = new List<int>();
#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [HideInInspector]
        [SerializeField] private int currentId = 10;   //Reserve 10 ids for whatever purpose

        /// <summary>
        /// EditorOnly
        /// </summary>
        /// <param name="_command"></param>
        internal void AddCommand(Command _command) {
            int id;
            do {
                if (pooledIds.Count > 0) {
                    id = pooledIds[0];
                    pooledIds.Remove(id);
                }
                else {
                    currentId++;
                    id = currentId;
                }
                _command.id = id;
            } while (commands.ContainsKey(id));

            commands.Add(id, _command);
        }


        /// <summary>
        /// Editor Only
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_command"></param>
        internal void SetCommand(int _id, Command _command) {
            commands[_id] = _command;
        }

        internal void RemoveCommand(int _id) {
            pooledIds.Add(_id);
            commands.Remove(_id);
            for (int i = 0; i < pooledIds.Count; i++) {
                if (pooledIds[i] > currentId) {
                    pooledIds.RemoveAt(i);
                    i--;
                }
            }
        }

        internal bool ClearUnusedPooledIds() {
            bool changed = false;
            int highestId = 0;
            foreach (var id in commands.Keys) {
                if (id > highestId) highestId = id;
            }
            for(int i=0; i<pooledIds.Count; i++) {
                if (pooledIds[i] > highestId) {
                    pooledIds.RemoveAt(i);
                    i--;
                    changed = true;
                }
            }
            return changed;
        }
#endif
        #endregion


#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        //[HideInInspector]
        public readonly Dictionary<int, Command> commands = new Dictionary<int, Command>();
        [SerializeField] private UnityEngine.Object m_targetObject;

        [Serializable]
        public class Variable {
#if UNITY_EDITOR
            public string variableName;
            private bool initialized = false;
#endif
            [OdinSerialize] private object m_value;
            public object value {
                get {
#if UNITY_EDITOR
                    if (m_value == null) m_value = 0;
#endif
                    return m_value; 
                }
                set {
#if UNITY_EDITOR
                    if (Application.isPlaying && !initialized) {
                        initialized = true;
                        System.Action<UnityEditor.PlayModeStateChange> onPlayModeStateChanged = null;
                        object initialValue = m_value;
                        onPlayModeStateChanged = _state => {
                            if (_state == UnityEditor.PlayModeStateChange.ExitingPlayMode) {
                                m_value = initialValue;
                                initialized = false;
                                UnityEditor.EditorApplication.playModeStateChanged -= onPlayModeStateChanged;
                            }
                        };
                        UnityEditor.EditorApplication.playModeStateChanged += onPlayModeStateChanged;
                    }
#endif
                    m_value = value;
                }
            }
        }

#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize]
#else
        [Calcatz.OdinSerializer.OdinSerialize]
#endif
        private Dictionary<int, Variable> m_variables = new Dictionary<int, Variable>();
        public virtual Dictionary<int, Variable> variables { get => m_variables; set => m_variables = value; }

        public UnityEngine.Object targetObject { get { return m_targetObject; } }

        internal void SetTargetObject(UnityEngine.Object _targetObject) {
            m_targetObject = _targetObject;
        }

        public CommandData(UnityEngine.Object _targetObject) {
            m_targetObject = _targetObject;
            commands = new Dictionary<int, Command>();
            m_variables = new Dictionary<int, Variable>();
        }

        #region COMMAND
        public virtual Command GetStartingCommand() {
            return GetCommand(1);
        }

        public virtual Command GetCommand(int _id) {
            Command command;
            if (commands.TryGetValue(_id, out command)) {
                return command;
            }
            return null;
        }

        public void TraverseCommands(Action<Command> _commandHandler) {
            if (_commandHandler == null) return;
            foreach (Command command in commands.Values) {
                _commandHandler.Invoke(command);
            }
        }

        public virtual List<int> TraverseFlow(Action<Command> _commandHandler) {
            List<int> traversedIds = new List<int>();
            TraverseFlowRecursive(GetStartingCommand(), _commandHandler, traversedIds);
            return traversedIds;
        }

        protected void TraverseFlowRecursive(Command _command, Action<Command> _commandHandler, List<int> _traversedIds) {
            if (_command == null) return;
            _traversedIds.Add(_command.id);
            _commandHandler.Invoke(_command);
            for(int i=0; i<_command.nextIds.Count; i++) {
                for (int j=0; j<_command.nextIds[i].Count; j++) {
                    int nextId = _command.nextIds[i][j].targetId;
                    if (nextId != 0 && !_traversedIds.Contains(nextId)) {
                        Command nextCommand = GetCommand(nextId);
                        if (nextCommand != null) {
                            TraverseFlowRecursive(nextCommand, _commandHandler, _traversedIds);
                        }
                    }
                }
            }
        }
        #endregion

    }

}
