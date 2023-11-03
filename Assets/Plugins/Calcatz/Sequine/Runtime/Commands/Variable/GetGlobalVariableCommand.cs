using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Gets a value of a certain global variable.
    /// </summary>
    [RegisterCommand("Data/Global Variable", typeof(SequineFlowCommandData))]
    public class GetGlobalVariableCommand : PropertyCommand {

        public int variableId;

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            if (SequineGlobalData.GetTransient().variables.TryGetValue(variableId, out var variable)) {
                return (T)variable.value;
            }
            else {
                Debug.LogError("Global variable with id " + variableId + " not found.");
                return default(T);
            }
        }

    }
}
