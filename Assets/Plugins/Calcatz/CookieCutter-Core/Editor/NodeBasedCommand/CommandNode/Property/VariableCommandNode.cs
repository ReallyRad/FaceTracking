using Calcatz.CookieCutter;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {

    [CustomCommandNodeDrawer(typeof(VariableCommand))]
    public class VariableCommandNode : CommandNode {

        public VariableCommandNode(CommandData _commandData, Command _command, Vector2 position, Config config)
            : base(_commandData, _command, position, 150, 50, config) {
            VariableCommand variableCommand = GetCommand<VariableCommand>();
            nodeName = "Get " + GetVariableName(variableCommand);
        }

        protected override void HandleFirstInPointCreation(Config _config) {
            
        }

        protected override void HandleFirstOutPointCreation(Config _config) {
            VariableCommand variableCommand = (VariableCommand)GetCommand();
            if (commandData.variables != null && commandData.variables.ContainsKey(variableCommand.variableId)) {
                CommandData.Variable variable = commandData.variables[variableCommand.variableId];
                System.Type type = variable.value == null ? typeof(string) : variable.value.GetType();
                AddPropertyOutPoint(type, _config);
            }
        }

        private string GetVariableName(VariableCommand variableCommand) {
            if (commandData.variables != null && commandData.variables.ContainsKey(variableCommand.variableId)) {
                return commandData.variables[variableCommand.variableId].variableName;
            }
            else {
                return "(Missing)";
            }
        }

        /*public override void Draw(Vector2 _offset) {
            VariableCommand variableCommand = (VariableCommand)GetCommand();
            string variableName;
            if (commandData.variables != null && commandData.variables.ContainsKey(variableCommand.variableId)) {
                variableName = commandData.variables[variableCommand.variableId].variableName;
            }
            else {
                variableName = "";
            }
            base.Draw(_offset);
            Vector2 pos = rect.position + _offset;
            currentY = pos.y + 10;
            float height = EditorGUIUtility.singleLineHeight;
            //rect.height = height;

            //OnDrawTitle(_offset);

            AddRectHeight(height);
            GUI.Label(new Rect(pos.x + 10, currentY, rect.width - 20, height), "  " + variableName, styles.label);
            //AddRectHeight(height);

            //OnDrawContents(_offset);
        }*/
    }
}
