using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Legacy.
    /// The old way of making a property command that doesn't require a new sub-class of CommandNode.
    /// </summary>
    public class BuildablePropertyCommand : PropertyCommand {

        [System.Serializable]
        public class IO {

            public List<object> inputValues;
            public List<object> outputValues;

#if UNITY_EDITOR
            public List<string> inputLabels;
            public List<string> outputLabels;
            public Dictionary<string, System.Type> unityObjectInputs;
            public Dictionary<string, System.Type> unityObjectOutputs;
#endif
        }

        /// <summary>
        /// This should only be accessed by BuildablePropertyCommandNode
        /// </summary>
        public IO io = new IO();

        /// <summary>
        /// Add an input point for property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_label"></param>
        /// <param name="_defaultValue"></param>
        /// <returns></returns>
        protected int AddPropertyInput<T>(string _label, T _defaultValue = default(T)) {
            inputIds.Add(new ConnectionTarget());
#if UNITY_EDITOR
            if (io.inputLabels == null) {
                io.inputLabels = new List<string>();
            }
            io.inputLabels.Add(_label);
#endif
            if (io.inputValues == null) {
                io.inputValues = new List<object>();
            }
            io.inputValues.Add(_defaultValue);
            
#if UNITY_EDITOR
            if (typeof(T) == typeof(UnityEngine.Object) || typeof(T).IsSubclassOf(typeof(UnityEngine.Object))) {
                if (io.unityObjectInputs == null) io.unityObjectInputs = new Dictionary<string, System.Type>();
                io.unityObjectInputs.Add(_label, typeof(T));
            }
#endif
            return io.inputValues.Count - 1;
        }

        /// <summary>
        /// Add on output point for property. The index shares the same list with main outputs as well.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_label"></param>
        /// <param name="_defaultValue"></param>
        /// <returns></returns>
        protected int AddPropertyOutput<T>(string _label, T _defaultValue = default(T)) {
            nextIds.Add(new List<ConnectionTarget>() { new ConnectionTarget() });
#if UNITY_EDITOR
            if (io.outputLabels == null) {
                io.outputLabels = new List<string>();
            }
            io.outputLabels.Add(_label);
#endif
            if (io.outputValues == null) {
                io.outputValues = new List<object>();
            }
            int index = io.outputValues.Count - 1;
            io.outputValues.Add(_defaultValue);

#if UNITY_EDITOR
            if (typeof(T) == typeof(UnityEngine.Object) || typeof(T).IsSubclassOf(typeof(UnityEngine.Object))) {
                if (io.unityObjectOutputs == null) io.unityObjectOutputs = new Dictionary<string, System.Type>();
                io.unityObjectOutputs.Add(_label, typeof(T));
            }
#endif
            return index;
        }

        protected override T GetInput<T>(CommandExecutionFlow _flow, int _index) {
            if (inputIds[_index].targetId == 0) {
                return (T)io.inputValues[_index];
            }
            else {
                return _flow.GetCommandOutput<T>(inputIds[_index].targetId, inputIds[_index].pointIndex);
            }
        }

        protected override T GetInput<T>(CommandExecutionFlow _flow, int _index, T _defaultValue) {
            if (inputIds[_index].targetId == 0) {
                return (T)io.inputValues[_index];
            }
            else {
                return _flow.GetCommandOutput<T>(inputIds[_index].targetId, inputIds[_index].pointIndex);
            }
        }

        protected void SetOutput<T>(int _index, T _value) {
            io.outputValues[_index] = _value;
        }

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _index) {
            return (T)io.outputValues[_index];
        }

    }
}