using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
#else
using Calcatz.OdinSerializer;
#endif

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Legacy.
    /// The old way of making a command that doesn't require a new sub-class of CommandNode.
    /// </summary>
    public class BuildableCommand : Command {

        [System.Serializable]
        public class IO {

            public List<object> inputValues;
            public Dictionary<int, object> outputValues;

#if UNITY_EDITOR
            public List<string> inputLabels;
            public List<string> outputLabels;
            public List<string> mainOutputLabels;
            /// <summary>
            /// Mistakenly named. Supposed to be unityObjectInputs.
            /// </summary>
            public Dictionary<string, System.Type> unityObjectOutputs;
            public Dictionary<string, System.Type> unityObjectInputs { get => unityObjectOutputs; set => unityObjectOutputs = value; }
#endif
        }

        private int nextIdIndex = 0;

        /// <summary>
        /// This should only be accessed by BuildableCommandNode
        /// </summary>
        public IO io = new IO();

        /// <summary>
        /// Add an output point for main flow. The index shares the same list with property outputs as well.
        /// </summary>
        /// <param name="_label"></param>
        /// <returns>Returns the output index that can be accessed using nextIds.</returns>
        protected int AddMainOutput(string _label) {
            nextIds.Add(new List<ConnectionTarget>() { new ConnectionTarget() });
#if UNITY_EDITOR
            if (io.mainOutputLabels == null) io.mainOutputLabels = new List<string>();
            io.mainOutputLabels.Add(_label);
#endif
            return nextIds.Count - 1;
        }

        /// <summary>
        /// Add an input point for property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_label"></param>
        /// <param name="_defaultValue"></param>
        /// <returns>Returns the input index of the property.</returns>
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
        /// <returns>Returns the output index of the property.</returns>
        protected int AddPropertyOutput<T>(string _label, T _defaultValue = default(T)) {
            nextIds.Add(new List<ConnectionTarget>() { new ConnectionTarget() });
#if UNITY_EDITOR
            if (io.outputLabels == null) {
                io.outputLabels = new List<string>();
            }
            io.outputLabels.Add(_label);
#endif
            if (io.outputValues == null) {
                io.outputValues = new Dictionary<int, object>();
            }
            int index = nextIds.Count - 1;
            io.outputValues.Add(index, _defaultValue);

            return index;
        }

        /*
        protected T GetInput<T>(ICommandExecutor _executor, string _label) {
            int index = io.inputLabels.IndexOf(_label);
            return GetInput<T>(_executor, index);
        }
        */

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

        protected void SetMainDestination(int _index) {
            //nextIdIndex = _index == 0 ? 0 : 1 + io.outputValues.Count; //0 is always added as main output by default
            nextIdIndex = _index;
        }

        public override int GetNextOutputIndex() {
            return nextIdIndex;
        }

    }
}