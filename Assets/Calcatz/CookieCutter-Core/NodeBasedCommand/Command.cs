using System.Collections.Generic;
using UnityEngine;
using System;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Base class of all command types.
    /// </summary>
    [Serializable]
    public class Command {

#if UNITY_EDITOR
        /// <summary>
        /// Don't use this in non-editor code
        /// </summary>
        public Vector2 nodePosition;
        [NonSerialized] internal bool executed = false;
        /// <summary>
        /// Used to leave trace in the editor when the flow reaches the output point.
        /// </summary>
        [NonSerialized] internal List<int> executedOutputs;
        [NonSerialized] internal int cachedNextIndex = 0;
#endif

        [Serializable]
        public class ConnectionTarget {
            public int targetId = 0;
            public int pointIndex = 0;

            public void Reset() {
                targetId = 0;
                pointIndex = 0;
            }
            public void Set(int _targetId, int _inputIndex) {
                targetId = _targetId;
                pointIndex = _inputIndex;
            }
        }

        public int id;
        [NonSerialized] public Action onFinished;
        /// <summary>
        /// First: Output point index. Second: Connection line index.
        /// </summary>
        public List<List<ConnectionTarget>> nextIds = new List<List<ConnectionTarget>>() { new List<ConnectionTarget>() { new ConnectionTarget() } };

        //Parameters
        public List<ConnectionTarget> inputIds = new List<ConnectionTarget>();

        /// <summary>
        /// Get the output index to get the next command ID upon exiting this command.
        /// </summary>
        /// <returns></returns>
        public virtual int GetNextOutputIndex() {
            if (nextIds.Count > 0) return 0;
            else return -1;
        }

        /// <summary>
        /// Get output value at out-point index.
        /// </summary>
        /// <typeparam name="T">The expected value type</typeparam>
        /// <param name="_flow"></param>
        /// <param name="_pointIndex"></param>
        /// <returns>The output value in T</returns>
        public virtual T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            return default(T);
        }

        /// <summary>
        /// Get input value at in-point index.
        /// </summary>
        /// <typeparam name="T">The expected value type</typeparam>
        /// <param name="_flow"></param>
        /// <param name="_index"></param>
        /// <param name="_defaultValue"></param>
        /// <returns>The input value in T</returns>
        protected virtual T GetInput<T>(CommandExecutionFlow _flow, int _index, T _defaultValue) {
            if (inputIds[_index].targetId > 0) {
                return _flow.GetCommandOutput<T>(inputIds[_index]);
            }
            else {
                return _defaultValue;
            }
        }

        /// <summary>
        /// Get input value at in-point index.
        /// </summary>
        /// <typeparam name="T">The expected value type</typeparam>
        /// <param name="_flow"></param>
        /// <param name="_index"></param>
        /// <returns>The input value in T</returns>
        protected virtual T GetInput<T>(CommandExecutionFlow _flow, int _index) {
            if (inputIds[_index].targetId > 0) {
                return _flow.GetCommandOutput<T>(inputIds[_index]);
            }
            else {
                return default(T);
            }
        }

        /// <summary>
        /// obsolete
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_flow"></param>
        /// <param name="_outputPointIndex"></param>
        /// <param name="_value"></param>
        public virtual void SetValue<T>(CommandExecutionFlow _flow, int _outputPointIndex, T _value) { }

        /// <summary>
        /// The node width used for the command node.
        /// </summary>
        public virtual float nodeWidth {
            get {
                return 165f;
            }
        }

        /// <summary>
        /// The process to execute upon visiting the main in-point (white in-point).
        /// </summary>
        /// <param name="_flow"></param>
        public virtual void Execute(CommandExecutionFlow _flow) {
            Exit();
        }

        /// <summary>
        /// Exit this command, and continue to next command if available.
        /// </summary>
        protected void Exit() {
#if UNITY_EDITOR
            if (executedOutputs != null) {
                cachedNextIndex = GetNextOutputIndex();
                if (!executedOutputs.Contains(cachedNextIndex))
                    executedOutputs.Add(cachedNextIndex);
            }
#endif
            if (onFinished != null) {
                onFinished.Invoke();
            }
        }

        /// <summary>
        /// Validate() is called when opening the nodes editor (if in Edit Mode), or right before the command execution (if in Play Mode).
        /// The main purpose is to adapt or validate any data inside the Command, if its structure had been changed.
        /// </summary>
        public virtual void Validate() {
            if (inputIds == null) inputIds = new List<ConnectionTarget>();
            if (nextIds == null) nextIds = new List<List<ConnectionTarget>>() { new List<ConnectionTarget>() { new ConnectionTarget() } };
        }

        internal void Kill() {
            HandleInterruption();
        }

        /// <summary>
        /// Called when the current flow is force killed, while the currently executed command is exactly this command, and has not exited.
        /// Note that "exited" here means that Exit() method has been called.
        /// </summary>
        protected virtual void HandleInterruption() { }

        public static ConnectionTarget GetOutConnection(Command _command, int _outPointIndex, Command _targetCommand, int _inPointIndex) {
            for (int i = 0; i < _command.nextIds[_outPointIndex].Count; i++) {
                ConnectionTarget outConnection = _command.nextIds[_outPointIndex][i];
                if (outConnection.targetId == _targetCommand.id && outConnection.pointIndex == _inPointIndex) {
                    return outConnection;
                }
            }
            return null;
        }

        public static void RemoveNextIdAtConnection(Command _command, int _outPointIndex, Command _targetCommand, int _inPointIndex) {
            ConnectionTarget outConnection = GetOutConnection(_command, _outPointIndex, _targetCommand, _inPointIndex);
            if (outConnection != null) {
                _command.nextIds[_outPointIndex].Remove(outConnection);
            }
        }
        public static void RemoveInputIdAtConnection(Command _command, int _outPointIndex, Command _targetCommand, int _inPointIndex) {
            ConnectionTarget inConnection = _targetCommand.inputIds[_inPointIndex];
            if (inConnection != null) {
                _targetCommand.inputIds[_inPointIndex].Reset();
            }
        }

        public static void ChangeNextIdInputIndexAtConnection(Command _command, int _outPointIndex, Command _targetCommand, int _inPointIndex, int _newInPointIndex) {
            ConnectionTarget outConnection = GetOutConnection(_command, _outPointIndex, _targetCommand, _inPointIndex);
            outConnection.pointIndex = _newInPointIndex;
        }

#if UNITY_EDITOR
        //Shortcuts for drawing custom node through Command (without using Command Node)
        //Can only be used if the Command has no Command Node

        /// <summary>
        /// Called when a Command Node is initialized, specifically the in-points.
        /// Use this method to ensure inputIds as well.
        /// </summary>
        public virtual void Editor_InitInPoints() {
            CommandGUI.AddMainInPoint();
        }

        /// <summary>
        /// Called when a Command Node is initialized, specifically the out-points.
        /// Use this method to ensure nextIds as well.
        /// </summary>
        public virtual void Editor_InitOutPoints() {
            CommandGUI.AddMainOutPoint();
        }

        /// <summary>
        /// Called when a Command Node's title is drawn.
        /// Use this to change the tooltip of the command.
        /// Also use this to decide whether the Title should draw in-point and/or out-point.
        /// </summary>
        /// <param name="_tooltip"></param>
        public virtual void Editor_OnDrawTitle(out string _tooltip) {
            CommandGUI.DrawInPoint(0);
            CommandGUI.DrawOutPoint(0);
            _tooltip = null;
        }

        /// <summary>
        /// Called when a Command Node is drawn.
        /// </summary>
        /// <param name="_absPosition"></param>
        public virtual void Editor_OnDrawContents(Vector2 _absPosition) {

        }
#endif


        #region PARALLEL-FLOW-HELPER
        /// <summary>
        /// Run a flow, starting from the command at output index.
        /// </summary>
        /// <param name="_flow"></param>
        /// <param name="_outputIndex"></param>
        protected void RunSubFlow(CommandExecutionFlow _flow, int _outputIndex) {
            if (TryGetCommandAtOutputIndex(_flow, _outputIndex, out var nextCommand)) {
                _flow.executor.RunFlow(_flow.currentCommandData, nextCommand);
#if UNITY_EDITOR
                if (executedOutputs != null && !executedOutputs.Contains(_outputIndex)) {
                    executedOutputs.Add(_outputIndex);
                    _flow.currentCommandData.needsRuntimeRepaint = true;
                }
#endif
            }
        }

        /// <summary>
        /// Try get the next command at the specified output index.
        /// </summary>
        /// <param name="_flow"></param>
        /// <param name="_outputIndex"></param>
        /// <param name="_nextCommand"></param>
        /// <returns></returns>
        protected bool TryGetCommandAtOutputIndex(CommandExecutionFlow _flow, int _outputIndex, out Command _nextCommand) {
            if (_outputIndex < 0 || _outputIndex >= nextIds.Count) {
                _nextCommand = null;
                return false;
            }
            if (nextIds[_outputIndex].Count > 0) {
                _nextCommand = _flow.currentCommandData.GetCommand(nextIds[_outputIndex][0].targetId);
                return _nextCommand != null;
            }
            _nextCommand = null;
            return false;
        }
        #endregion
    }

}
