using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class CommandExecutor {

        public System.Action onAllFlowsEnded;

        private object m_targetObject;
        public CommandExecutor(object _targetObject) {
            m_targetObject = _targetObject;
        }
        public object targetObject => m_targetObject;


        private CommandData m_mainCommandData;
        public CommandData mainCommandData { get => m_mainCommandData; set => m_mainCommandData = value; }


        private List<CommandExecutionFlow> m_flows = new List<CommandExecutionFlow>();

        private System.Action<Command, CommandExecutionFlow> onCommandFinishedOverride = null;

        public void OverrideOnCommandFinished(System.Action<Command, CommandExecutionFlow> _callback) {
            onCommandFinishedOverride = _callback;
        }

        public CommandExecutionFlow RunFlow(CommandData _rootCommandData, Command _startingCommand, System.Action _onComplete = null) {
            CommandExecutionFlow flow = new CommandExecutionFlow(this, _onComplete);
            m_flows.Add(flow);
            flow.OverrideOnCommandFinished(onCommandFinishedOverride);
            flow.rootCommandData = _rootCommandData;
            flow.ExecuteCommand(_startingCommand);
            return flow;
        }

        public CommandExecutionFlow RunFlow(Command _startingCommand, System.Action _onComplete = null) {
            if (mainCommandData == null) {
                Debug.Log("If RunFlow is called without specifiying root command data, then main command data should be specified first.");
                return null;
            }
            if (_startingCommand == null) return null;
            return RunFlow(mainCommandData, _startingCommand, _onComplete);
        }

        public void KillFlow(CommandExecutionFlow _flow) {
            _flow.Kill();
        }

        public void KillAllFlows() {
            foreach (var flow in m_flows.ToArray()) {
                flow.Kill();
            }
        }

        /// <summary>
        /// Called internally when the flow ends.
        /// </summary>
        /// <param name="_flow"></param>
        public void EndFlow(CommandExecutionFlow _flow) {
            m_flows.Remove(_flow);
            if (m_flows.Count == 0 && onAllFlowsEnded != null) onAllFlowsEnded.Invoke();
        }

    }
}
