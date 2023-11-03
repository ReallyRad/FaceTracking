using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Sets a value of a certain global variable.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Data/Set Global Variable", typeof(SequineFlowCommandData))]
    public class SetGlobalVariableCommand : Command {

        public int variableId;
        public object value;

        public SetGlobalVariableCommand() {
            inputIds.Add(new ConnectionTarget());
            nextIds.Add(new List<ConnectionTarget>());
        }

        public override void Execute(CommandExecutionFlow _flow) {
            if (SequineGlobalData.GetTransient().variables.TryGetValue(variableId, out var variable)) {
                var val = GetInput<object>(_flow, 0, value);
                variable.value = val;
            }
            base.Execute(_flow);
        }

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            if (SequineGlobalData.GetTransient().variables.TryGetValue(variableId, out var variable)) {
                return (T)variable.value;
            }
            else {
                Debug.LogError("Global variable with id " + variableId + " not found.");
                return default(T);
            }
        }

        public override void SetValue<T>(CommandExecutionFlow _flow, int _outputPointIndex, T _value) {
            if (SequineGlobalData.GetTransient().variables.TryGetValue(variableId, out var variable)) {
                variable.value = value;
            }
        }

    }
}
