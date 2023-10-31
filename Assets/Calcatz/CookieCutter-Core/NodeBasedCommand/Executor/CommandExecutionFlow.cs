using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    public class CommandExecutionFlow {

        private CommandExecutor m_executor;
        private System.Action onFlowComplete;

        public CommandExecutionFlow(CommandExecutor _executor, System.Action _onFlowComplete = null) {
            m_executor = _executor;
            onFlowComplete = _onFlowComplete;
        }

        public CommandExecutor executor => m_executor;

        private class ParentCommandDataState {
            public int resumeId;
            public CommandData commandData;
        }

        private CommandData m_rootCommandData;
        public CommandData rootCommandData { get => m_rootCommandData; set => m_rootCommandData = value; }
        private Stack<ParentCommandDataState> subDataStack = new Stack<ParentCommandDataState>();

        private System.Action<Command, CommandExecutionFlow> onCommandFinishedOverride = null;
        private bool m_killed;

        private Command m_lastExecutedCommand;

        public bool isKilled => m_killed;
        public Command lastExecutedCommand => m_lastExecutedCommand;

        public void OverrideOnCommandFinished(System.Action<Command, CommandExecutionFlow> _callback) {
            onCommandFinishedOverride = _callback;
        }

        /// <summary>
        /// Get current command data, either it's root or sub data.
        /// </summary>
        public CommandData currentCommandData {
            get {
                if (subDataStack.Count == 0) return m_rootCommandData;
                else return subDataStack.Peek().commandData;
            }
        }

        public virtual void ExecuteCommand(Command _command) {
            m_lastExecutedCommand = _command;
            _command.Validate();
#if UNITY_EDITOR
            _command.executed = true;

            System.Action<UnityEditor.PlayModeStateChange> onPlayModeStateChanged = null;
            onPlayModeStateChanged = _state => {
                if (_state == UnityEditor.PlayModeStateChange.ExitingEditMode) {
                    if (UnityEditor.EditorPrefs.GetBool("resetCommandNodeExecutionFlowOnPlay", true)) {
                        _command.executed = false;
                    }
                    if (_command.executedOutputs != null) {
                        _command.executedOutputs.Clear();
                    }
                    UnityEditor.EditorApplication.playModeStateChanged -= onPlayModeStateChanged;
                }
            };
            UnityEditor.EditorApplication.playModeStateChanged += onPlayModeStateChanged;
            if (_command.executedOutputs == null) {
                _command.executedOutputs = new List<int>();
            }
            currentCommandData.lastExecutedCommand = _command;
#endif
            _command.onFinished = ()=>OnCommandFinished(_command, this);
            _command.Execute(this);
        }

        private Command GetRelativeNextCommand(Command _command, bool _peekOnly) {
#if UNITY_EDITOR
            int nextIndex = _command.cachedNextIndex;
#else
            int nextIndex = _command.GetNextOutputIndex();

#endif
            int nextId = nextIndex < 0 ? 0 : _command.nextIds[nextIndex].Count == 0? 0 : _command.nextIds[nextIndex][0].targetId;
            Command nextCommand = currentCommandData.GetCommand(nextId);
            if (nextCommand == null) {
                nextCommand = CheckStackRecursive(_peekOnly);
            }
            return nextCommand;
        }

        private Command CheckStackRecursive(bool _peekOnly) {
            Command nextCommand = null;
            if (subDataStack.Count > 0) {
                int resumeId = subDataStack.Peek().resumeId;
                if (_peekOnly) {
                    ParentCommandDataState popped = subDataStack.Pop();
                    Command resumedCommand = currentCommandData.GetCommand(resumeId);
                    int nextIndex = resumedCommand.GetNextOutputIndex();
                    int nextId = nextIndex < 0 ? 0 : resumedCommand.nextIds[nextIndex].Count == 0? 0 : resumedCommand.nextIds[nextIndex][0].targetId;
                    nextCommand = currentCommandData.GetCommand(nextId);
                    if (nextCommand == null) {
                        nextCommand = CheckStackRecursive(true);
                    }
                    subDataStack.Push(popped);
                }
                else {
                    subDataStack.Pop();
                    Command resumedCommand = currentCommandData.GetCommand(resumeId);
#if UNITY_EDITOR
                    if (resumedCommand.executedOutputs != null) {
                        if (!resumedCommand.executedOutputs.Contains(resumedCommand.cachedNextIndex)) {
                            resumedCommand.executedOutputs.Add(resumedCommand.cachedNextIndex);
                            currentCommandData.needsRuntimeRepaint = true;
                        }
                    }
#endif
                    nextCommand = GetRelativeNextCommand(resumedCommand);
                    if (nextCommand == null) {
                        nextCommand = CheckStackRecursive(false);
                    }
                }
            }

            return nextCommand;
        }

        public Command GetRelativeNextCommand(Command _command) {
            return GetRelativeNextCommand(_command, true);
        }

        private void OnCommandFinished(Command _command, CommandExecutionFlow _flow) {
#if UNITY_EDITOR
            _flow.currentCommandData.needsRuntimeRepaint = true;
#endif
            if (m_killed) {
                End();
                return;
            }
            if (onCommandFinishedOverride == null) {
                _flow.FinishCommand(_command);
            }
            else {
                onCommandFinishedOverride.Invoke(_command, _flow);
            }
        }

        public void FinishCommand(Command _command) {
            Command nextCommand = GetRelativeNextCommand(_command, false);
            if (nextCommand != null) {
                ExecuteCommand(nextCommand);
            }
            else {
                End();
            }
        }

        private void End() {
            m_lastExecutedCommand = null;
            onFlowComplete?.Invoke();
            executor.EndFlow(this);
        }

        public T GetCommandOutput<T>(int _commandId, int _outPointIndex) {
            return currentCommandData.GetCommand(_commandId).GetOutput<T>(this, _outPointIndex);
        }

        public T GetCommandOutput<T>(Command.ConnectionTarget _connectionTarget) {
            return currentCommandData.GetCommand(_connectionTarget.targetId).GetOutput<T>(this, _connectionTarget.pointIndex);
        }

        public void SetCommandValue<T>(int _commandId, int _outputIndex, T _value) {
            currentCommandData.GetCommand(_commandId).SetValue(this, _outputIndex, _value);
        }

        public void RunSubData(int _resumeId, CommandData _subData) {
            subDataStack.Push(new ParentCommandDataState() { resumeId = _resumeId, commandData = _subData });
            ExecuteCommand(_subData.GetStartingCommand());
        }

        public T GetVariable<T>(int _variableId) {
            CommandData commandData = currentCommandData;
            if (commandData.variables != null && commandData.variables.ContainsKey(_variableId)) {
                if (commandData.variables[_variableId].value is T val) {
                    return val;
                }
                else {
#if UNITY_EDITOR
                    Debug.LogError("Broken Connection: Variable " + commandData.variables[_variableId].variableName + " is no longer type of " + typeof(T).Name, commandData.targetObject);
#else
                    Debug.LogError("Broken Connection: Variable " + _variableId + " is no longer type of " + typeof(T).Name, commandData.targetObject);
#endif
                    return default(T);
                }
            }
            Debug.LogError("Variables property is either null or the variable id didn't exist: " + _variableId, commandData.targetObject);
            return default(T);
        }

        public void SetVariable<T>(int _variableId, T _value) {
            CommandData commandData = currentCommandData;
            if (commandData.variables != null && commandData.variables.ContainsKey(_variableId)) {
                commandData.variables[_variableId].value = _value;
            }
            else {
                Debug.LogError("Variables property is either null or the variable id didn't exist: " + _variableId, commandData.targetObject);
            }
        }

        public void Kill() {
            m_killed = true;
            if (m_lastExecutedCommand != null) {
                m_lastExecutedCommand.Kill();
            }
        }

    }

}
