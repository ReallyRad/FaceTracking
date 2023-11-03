using System.Collections.Generic;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Sets a local variable.
    /// </summary>
    [System.Serializable]
    public class SetVariableCommand : Command {

        public int variableId;
        public object value;

        public SetVariableCommand() {
            inputIds.Add(new ConnectionTarget());
            nextIds.Add(new List<ConnectionTarget>());
        }

        public override void Execute(CommandExecutionFlow _flow) {
            var val = GetInput<object>(_flow, 0, value);
            _flow.SetVariable(variableId, val);
            base.Execute(_flow);
        }

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            return _flow.GetVariable<T>(variableId);
        }

        public override void SetValue<T>(CommandExecutionFlow _flow, int _outputPointIndex, T _value) {
            _flow.SetVariable(variableId, _value);
        }

    }

}
