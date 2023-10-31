
namespace Calcatz.CookieCutter {

    /// <summary>
    /// Gets a local variable value.
    /// </summary>
    [System.Serializable]
    public class VariableCommand : PropertyCommand {

        public int variableId;

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            return _flow.GetVariable<T>(variableId);
        }

        public override void SetValue<T>(CommandExecutionFlow _flow, int _outputPointIndex, T _value) {
            _flow.SetVariable(variableId, _value);
        }
    }

}
