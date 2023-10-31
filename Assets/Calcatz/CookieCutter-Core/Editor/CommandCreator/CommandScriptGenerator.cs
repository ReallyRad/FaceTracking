using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Calcatz.CookieCutter.CommandCreatorWindow;

namespace Calcatz.CookieCutter {
    public class CommandScriptGenerator {

        private Dictionary<CommandLine, string> varNames;
        private Dictionary<CommandLine, string> inPorts;
        private Dictionary<CommandLine, string> outPorts;
        private List<string> parallelMethodNames;
        private int alternateExitCount;

        public void Generate(CommandConstructor _constructor, string _filePath) {
            varNames = new Dictionary<CommandLine, string>();
            inPorts = new Dictionary<CommandLine, string>();
            outPorts = new Dictionary<CommandLine, string>();

            string fileContent = template;
            fileContent = fileContent.Replace("<NAMESPACE>", _constructor._namespace);
            fileContent = fileContent.Replace("<DISPLAY-NAME>", _constructor.displayName);

            string includedCommandData = "";
            foreach (var inclusion in _constructor.includedCommandData) {
                if (inclusion.include) {
                    if (includedCommandData != "") includedCommandData += ", ";
                    includedCommandData += "typeof(" + inclusion.type.FullName + ")";
                }
            }
            fileContent = fileContent.Replace("<INCLUDED-COMMAND-DATA>", includedCommandData);
            fileContent = fileContent.Replace("<CLASS-NAME>", _constructor.className);
            fileContent = fileContent.Replace("<BASE-TYPE>", _constructor.isPropertyCommand? "PropertyCommand" : "Command");
            fileContent = fileContent.Replace("<NODE-WIDTH>", _constructor.nodeWidth + "f");
            fileContent = fileContent.Replace("<VARIABLES>", CreateVariables(_constructor));
            fileContent = fileContent.Replace("<CONST-INPUT-PORTS>", CreateConstInPorts(_constructor));
            fileContent = fileContent.Replace("<CONST-OUTPUT-PORTS>", CreateConstOutPorts(_constructor));

            fileContent = fileContent.Replace("<PARALLEL-OUT>", CreateParallelMainOutputs());
            fileContent = fileContent.Replace("<GET-NEXT-ID>", CreateGetNextOutputIndex(_constructor));
            fileContent = fileContent.Replace("<EXIT-PORT-INDEX>", CreateExitPortIndex());

            if (_constructor.isPropertyCommand) {
                fileContent = fileContent.Replace("<EXECUTE>", "");
            }
            else {
                fileContent = fileContent.Replace("<EXECUTE>", CreateExecute(_constructor));
            }

            fileContent = fileContent.Replace("<GET-OUTPUT>", CreateGetOutput(_constructor));

            fileContent = fileContent.Replace("<INIT-IN-POINTS>", CreateInitInPoints(_constructor));
            fileContent = fileContent.Replace("<INIT-OUT-POINTS>", CreateInitOutPoints(_constructor));
            fileContent = fileContent.Replace("<DRAW-TITLE>", CreateOnDrawTitle(_constructor));
            fileContent = fileContent.Replace("<DRAW-CONTENTS>", CreateOnDrawContents(_constructor));

            File.WriteAllText(_filePath, fileContent);
            Debug.Log("Successfully created " + _constructor.className + " command script.");
            AssetDatabase.Refresh();
        }

        private string CreateVariables(CommandConstructor _constructor) {
            string result = "";
            HashSet<string> existingVarNames = new HashSet<string>();
            foreach (var line in _constructor.lines) {
                if (line.contentType == CommandLine.ContentType.InputField) {
                    string varName;
                    if (line.label != null && line.label != "") {
                        varName = line.label.ConvertToCamelCase();
                        if (existingVarNames.Contains(varName)) {
                            int retry = 1;
                            string newVarName;
                            do {
                                newVarName = varName + retry;
                                retry++;
                            } while (existingVarNames.Contains(newVarName));
                            varName = newVarName;
                        }
                    }
                    else {
                        int retry = 1;
                        do {
                            varName = "variable_" + retry;
                            retry++;
                        } while (existingVarNames.Contains(varName));
                    }
                    existingVarNames.Add(varName);
                    varNames.Add(line, varName);
                    result += string.Format(@"
        public {0} {1};", CommandLine.fieldDataTypes[line.fieldType], varName);
                }
            }
            return result;
        }

        private string CreateConstInPorts(CommandConstructor _constructor) {
            string result = "";
            HashSet<string> existingNames = new HashSet<string>();
            int index = 0;
            int currentInPoint = 0;
            foreach (var line in _constructor.lines) {
                bool portAvailable = false;
                if (line.contentType == CommandLine.ContentType.InputField) {
                    if (line.pointLocation == CommandLine.PointLocation.Left) {
                        if (varNames.TryGetValue(line, out string varName)) {
                            portAvailable = true;
                        }
                    }
                }
                else {
                    if (line.pointLocation == CommandLine.PointLocation.Left) {
                        portAvailable = true;
                    }
                }
                if (portAvailable) {
                    string portName;

                    if (line.contentType != CommandLine.ContentType.None && line.label != null && line.label != "") {
                        portName = "IN_PORT_" + line.label.ConvertToMacroCase();
                    }
                    else {
                        portName = "IN_PORT_UNNAMED_" + index;
                    }
                    if (existingNames.Contains(portName)) {
                        string newName;
                        int retry = 1;
                        do {
                            newName = portName + "_" + retry;
                            retry++;
                        } while (existingNames.Contains(newName));
                        portName = newName;
                    }
                    existingNames.Add(portName);
                    inPorts.Add(line, portName);
                    result += string.Format(@"
        private const int {0} = {1};", portName, currentInPoint++);
                }
                index++;
            }
            return result;
        }

        private string CreateConstOutPorts(CommandConstructor _constructor) {
            string result = "";
            HashSet<string> existingNames = new HashSet<string>();
            int index = 0;
            int currentOutPoint = !_constructor.isPropertyCommand && _constructor.titleHasOutPoint ? 1 : 0;
            foreach (var line in _constructor.lines) {
                if (line.contentType == CommandLine.ContentType.InputField) {

                }
                else {
                    if (line.pointLocation == CommandLine.PointLocation.Right) {
                        string portName;
                        if (line.contentType != CommandLine.ContentType.None && line.label != null && line.label != "") {
                            portName = "OUT_PORT_" + line.label.ConvertToMacroCase();
                        }
                        else {
                            portName = "OUT_PORT_UNNAMED_" + index;
                        }
                        if (existingNames.Contains(portName)) {
                            string newName;
                            int retry = 1;
                            do {
                                newName = portName + "_" + retry;
                                retry++;
                            } while (existingNames.Contains(newName));
                            portName = newName;
                        }
                        existingNames.Add(portName);
                        outPorts.Add(line, portName);
                        result += string.Format(@"
        private const int {0} = {1};", portName, currentOutPoint++);
                    }
                }
                index++;
            }
            return result;
        }

        private string CreateInitInPoints(CommandConstructor _constructor) {
            string result = @"        public override void Editor_InitInPoints() {";
            if (!_constructor.isPropertyCommand) {
                result += @"
            CommandGUI.AddMainInPoint();";
            }

            foreach (var kvp in inPorts) {
                result += String.Format(@"
            if (inputIds.Count <= {0}) {{
                inputIds.Add(new ConnectionTarget());
            }}", kvp.Value);
            }

            foreach (var line in _constructor.lines) {
                if (line.contentType == CommandLine.ContentType.InputField) {
                    if (line.pointLocation == CommandLine.PointLocation.Left) {
                        var forcedPointType = CommandLine.fieldPointTypes[line.fieldType];
                        var connectionType = CommandLine.connectionTypes[forcedPointType];
                        var propertyType = PropertyConnectionPoint.variableTypes[connectionType];
                        result += string.Format(@"
            CommandGUI.AddPropertyInPoint<{0}>();", propertyType);
                    }
                }
                else {
                    if (line.pointLocation == CommandLine.PointLocation.Left) {
                        if (line.pointType == CommandLine.PointType.Main) {
                            result += @"
            CommandGUI.AddMainInPoint();";
                        }
                        else {
                            var connectionType = CommandLine.connectionTypes[line.pointType];
                            result += string.Format(@"
            CommandGUI.AddPropertyInPoint<{0}>();", PropertyConnectionPoint.variableTypes[connectionType]);
                        }
                    }
                }
            }
            result += @"
        }";
            return result;
        }

        private string CreateInitOutPoints(CommandConstructor _constructor) {
            string result = @"        public override void Editor_InitOutPoints() {";
            if (!_constructor.isPropertyCommand && _constructor.titleHasOutPoint) {
                result += @"
            CommandGUI.AddMainOutPoint();";
            }
            foreach (var kvp in outPorts) {
                result += String.Format(@"
            if (nextIds.Count <= {0}) {{
                nextIds.Add(new List<ConnectionTarget>());
            }}", kvp.Value);
            }
            foreach (var line in _constructor.lines) {
                if (line.contentType == CommandLine.ContentType.InputField) {

                }
                else {
                    if (line.pointLocation == CommandLine.PointLocation.Right) {
                        if (line.pointType == CommandLine.PointType.Main) {
                            result += @"
            CommandGUI.AddMainOutPoint();";
                        }
                        else {
                            var connectionType = CommandLine.connectionTypes[line.pointType];
                            result += string.Format(@"
            CommandGUI.AddPropertyOutPoint<{0}>();", PropertyConnectionPoint.variableTypes[connectionType]);
                        }
                    }
                }
            }
            result += @"
        }";
            return result;
        }

        private static string CreateOnDrawTitle(CommandConstructor _constructor) {
            string result = string.Format(@"        public override void Editor_OnDrawTitle(out string _tooltip) {{
            _tooltip = @""{0}"";", _constructor.tooltip);
            if (!_constructor.isPropertyCommand) {
                result += @"
            CommandGUI.DrawInPoint(0);";
                if (_constructor.titleHasOutPoint) {
                    result += @"
            CommandGUI.DrawOutPoint(0);";
                }
            }
            result += @"
        }
                ";
            return result;
        }

        private string CreateOnDrawContents(CommandConstructor _constructor) {
            string result = @"        public override void Editor_OnDrawContents(Vector2 _absPosition) {";

            foreach (var line in _constructor.lines) {

                if (line.contentType == CommandLine.ContentType.InputField) {
                    if (line.pointLocation == CommandLine.PointLocation.Left) {
                        if (_constructor.isPropertyCommand) {
                            result += string.Format(@"
            CommandGUI.DrawInPoint({0});", inPorts[line]);
                        }
                        else {
                            result += string.Format(@"
            CommandGUI.DrawInPoint({0} + 1);", inPorts[line]);
                        }
                    }
                }
                else {
                    if (line.pointLocation == CommandLine.PointLocation.Left) {
                        if (_constructor.isPropertyCommand) {
                            result += string.Format(@"
            CommandGUI.DrawInPoint({0});", inPorts[line]);
                        }
                        else {
                            result += string.Format(@"
            CommandGUI.DrawInPoint({0} + 1);", inPorts[line]);
                        }
                    }
                    else if (line.pointLocation == CommandLine.PointLocation.Right) {
                        result += string.Format(@"
            CommandGUI.DrawOutPoint({0});", outPorts[line]);
                    }
                }

                if (line.contentType == CommandLine.ContentType.None) {
                    result += @"
            CommandGUI.AddRectHeight(UnityEditor.EditorGUIUtility.singleLineHeight);";
                }
                else if (line.contentType == CommandLine.ContentType.Label) {
                    result += string.Format(@"
            CommandGUI.DrawLabel(""{0}"", {1});", line.label, line.alignRight.ToString().ToLower());
                }
                else {
                    if (line.pointLocation == CommandLine.PointLocation.Left) {
                        result += string.Format(@"
            if (inputIds[{0}].targetId == 0) {{", inPorts[line]);
                        result += HandleFieldContent(line, 1);
                        result += @"
            }
            else {";
                        result += string.Format(@"
                CommandGUI.DrawLabel(""{0}"");", line.label);
                        result += @"
            }";
                    }
                    else {
                        result += HandleFieldContent(line);
                    }
                }
                if (line.spacing > 0) {
                    result += string.Format(@"
            CommandGUI.AddRectHeight({0}f);", line.spacing);
                }
            }

            result += @"
        }
                ";
            return result;
        }

        private string HandleFieldContent(CommandLine line, int indent = 0) {
            string result = "";
            if (line.fieldType == CommandLine.FieldType.Text) {
                result += string.Format(@"
            CommandGUI.DrawTextField(""{0}"", ""{1}"", ref {2});", line.label, line.tooltip, varNames[line]);
            }
            else if (line.fieldType == CommandLine.FieldType.TextArea) {
                result += string.Format(@"
            CommandGUI.DrawTextAreaField(""{0}"", ""{1}"", ref {2}, {3}, {4});", line.label, line.tooltip, varNames[line], line.lineCount, line.wordWrap);
            }
            else if (line.fieldType == CommandLine.FieldType.RoundedFloat) {
                if (line.useMin && line.useMax) {
                    result += string.Format(@"
            CommandGUI.DrawRoundedFloatField(""{0}"", ""{1}"", ref {2}, {3}f, {4}f);", line.label, line.tooltip, varNames[line], line.min, line.max);
                }
                else if (line.useMin) {
                    result += string.Format(@"
            CommandGUI.DrawRoundedFloatField(""{0}"", ""{1}"", ref {2}, {3}f);", line.label, line.tooltip, varNames[line], line.min);
                }
                else if (line.useMax) {
                    result += string.Format(@"
            CommandGUI.DrawRoundedFloatField(""{0}"", ""{1}"", ref {2}, float.MinValue, {3}f);", line.label, line.tooltip, varNames[line], line.max);
                }
                else {
                    result += string.Format(@"
            CommandGUI.DrawRoundedFloatField(""{0}"", ""{1}"", ref {2});", line.label, line.tooltip, varNames[line]);
                }
            }
            else if (line.fieldType == CommandLine.FieldType.Float) {
                if (line.useMin && line.useMax) {
                    result += string.Format(@"
            CommandGUI.DrawFloatField(""{0}"", ""{1}"", ref {2}, {3}f, {4}f);", line.label, line.tooltip, varNames[line], line.min, line.max);
                }
                else if (line.useMin) {
                    result += string.Format(@"
            CommandGUI.DrawFloatField(""{0}"", ""{1}"", ref {2}, {3}f);", line.label, line.tooltip, varNames[line], line.min);
                }
                else if (line.useMax) {
                    result += string.Format(@"
            CommandGUI.DrawFloatField(""{0}"", ""{1}"", ref {2}, float.MinValue, {3}f);", line.label, line.tooltip, varNames[line], line.max);
                }
                else {
                    result += string.Format(@"
            CommandGUI.DrawFloatField(""{0}"", ""{1}"", {1}, ref {2});", line.label, line.tooltip, varNames[line]);
                }
            }
            else if (line.fieldType == CommandLine.FieldType.FloatSlider) {
                result += string.Format(@"
            CommandGUI.DrawFloatSliderField(""{0}"", ""{1}"", ref {2}, {3}f, {4}f);", line.label, line.tooltip, varNames[line], line.min, line.max);
                }
            else if (line.fieldType == CommandLine.FieldType.Int) {
                if (line.useMin && line.useMax) {
                    result += string.Format(@"
            CommandGUI.DrawIntField(""{0}"", ""{1}"", ref {2}, {3}, {4});", line.label, line.tooltip, varNames[line], (int)line.min, (int)line.max);
                }
                else if (line.useMin) {
                    result += string.Format(@"
            CommandGUI.DrawIntField(""{0}"", ""{1}"", ref {2}, {3});", line.label, line.tooltip, varNames[line], (int)line.min);
                }
                else if (line.useMax) {
                    result += string.Format(@"
            CommandGUI.DrawIntField(""{0}"", ""{1}"", ref {2}, int.MinValue, {3});", line.label, line.tooltip, varNames[line], (int)line.max);
                }
                else {
                    result += string.Format(@"
            CommandGUI.DrawIntField(""{0}"", ""{1}"", ref {2});", line.label, line.tooltip, varNames[line]);
                }
            }
            else if (line.fieldType == CommandLine.FieldType.IntSlider) {
                result += string.Format(@"
            CommandGUI.IntSliderField(""{0}"", ""{1}"", ref {2}, {3}, {4});", line.label, line.tooltip, varNames[line], (int)line.min, (int)line.max);
                }
            else if (line.fieldType == CommandLine.FieldType.Toggle) {
                result += string.Format(@"
            CommandGUI.DrawToggleField(""{0}"", ""{1}"", ref {2});", line.label, line.tooltip, varNames[line]);
                }
            else if (line.fieldType == CommandLine.FieldType.ToggleLeft) {
                result += string.Format(@"
            CommandGUI.DrawToggleLeftField(""{0}"", ""{1}"", ref {2});", line.label, line.tooltip, varNames[line]);
                }
            else if (line.fieldType == CommandLine.FieldType.Object) {
                result += string.Format(@"
            CommandGUI.DrawObjectField(""{0}"", ""{1}"", {2}, _{2} => {2} = _{2}, {3});", line.label, line.tooltip, varNames[line], line.allowSceneObjects.ToString().ToLower());
                }
            else if (line.fieldType == CommandLine.FieldType.Color) {
                result += string.Format(@"
            CommandGUI.DrawColorField(""{0}"", ""{1}"", ref {2}); ", line.label, line.tooltip, varNames[line]);
            }
            else if (line.fieldType == CommandLine.FieldType.Curve) {
                result += string.Format(@"
            CommandGUI.DrawCurveField(""{0}"", ""{1}"", {2}, _{2} => {2} = _{2});", line.label, line.tooltip, varNames[line]);
                }
            else if (line.fieldType == CommandLine.FieldType.Enum) {
                result += string.Format(@"
            CommandGUI.DrawEnumField(""{0}"", ""{1}"", ref {2});", line.label, line.tooltip, varNames[line]);
                }
            else if (line.fieldType == CommandLine.FieldType.Vector3) {
                result += string.Format(@"
            CommandGUI.DrawVector3Field(""{0}"", ""{1}"", ref {2});", line.label, line.tooltip, varNames[line]);
                }
            else if (line.fieldType == CommandLine.FieldType.Vector3Int) {
                result += string.Format(@"
            CommandGUI.DrawVector3IntField(""{0}"", ""{1}"", ref {2});", line.label, line.tooltip, varNames[line]);
                }
            else if (line.fieldType == CommandLine.FieldType.Vector2) {
                result += string.Format(@"
            CommandGUI.DrawVector2Field(""{0}"", ""{1}"", ref {2});", line.label, line.tooltip, varNames[line]);
                }
            else if (line.fieldType == CommandLine.FieldType.Vector2Int) {
                result += string.Format(@"
            CommandGUI.DrawVector2IntField(""{0}"", ""{1}"", ref {2});", line.label, line.tooltip, varNames[line]);
                }
            else if (line.fieldType == CommandLine.FieldType.Vector4) {
                result += string.Format(@"
            CommandGUI.DrawVector4Field(""{0}"", ""{1}"", ref {2});", line.label, line.tooltip, varNames[line]);
                }
            else if (line.fieldType == CommandLine.FieldType.Rect) {
                result += string.Format(@"
            CommandGUI.DrawRectField(""{0}"", ""{1}"", ref {2});", line.label, line.tooltip, varNames[line]);
                }
            else if (line.fieldType == CommandLine.FieldType.RectInt) {
                result += string.Format(@"
            CommandGUI.DrawRectIntField(""{0}"", ""{1}"", ref {2});", line.label, line.tooltip, varNames[line]);
                }
            else if (line.fieldType == CommandLine.FieldType.Popup) {
                result += string.Format(@"
            CommandGUI.DrawPopupField(""{0}"", ""{1}"", {2}, new string[] {{ 
                ""Option0"",
                ""Option1"",
                ""Option2""
            }}, _index => {{ 
                {2} = _index;
            }});", line.label, line.tooltip, varNames[line]);
            }
            string[] splits = result.Split('\n');
            string indentedResult = "";
            string spaceAdditions = "";
            for (int i=0; i<indent; i++) {
                spaceAdditions += "    ";
            }
            foreach (var split in splits) {
                indentedResult += spaceAdditions + split;
            }
            return indentedResult;
        }

        private string CreateStoredInputs(CommandConstructor _constructor) {
            string result = "";
            foreach(var kvp in inPorts) {
                var line = kvp.Key;
                if (line.pointType != CommandLine.PointType.Main || line.contentType == CommandLine.ContentType.InputField) {
                    if (!varNames.TryGetValue(line, out string varName)) {
                        varName = kvp.Value.ConvertToCamelCase();
                    }
                    Type varType;
                    if (line.contentType == CommandLine.ContentType.InputField) {
                        varType = CommandLine.fieldDataTypes[line.fieldType];
                    }
                    else {
                        var connectionType = CommandLine.connectionTypes[line.pointType];
                        varType = PropertyConnectionPoint.variableTypes[connectionType];
                    }

                    if (varNames.ContainsKey(line)) {
                        result += String.Format(@"
            var {0}_value = GetInput<{1}>(_flow, {2}, {3});", varName, varType, kvp.Value, varNames[line]);
                    }
                    else {
                        result += String.Format(@"
            var {0}_value = GetInput<{1}>(_flow, {2});", varName, varType, kvp.Value);
                    }
                }
            }
            result += "\n";
            return result;
        }

        private string CreateExecute(CommandConstructor _constructor) {
            string result = "";

            string parallelMethodCalls = "";
            foreach (var methodName in parallelMethodNames) {
                parallelMethodCalls += string.Format(@"
            {0}", methodName);
            }

            result += string.Format(@"
        public override void Execute(CommandExecutionFlow _flow) {{ {0}
            //....................................
            //...YOUR EXECUTION LOGIC GOES HERE...
            //....................................
{1}
            Exit();
        }}", CreateStoredInputs(_constructor), parallelMethodCalls);
            return result;
        }

        private string CreateParallelMainOutputs() {
            parallelMethodNames = new List<string>();
            string result = "";
            Dictionary<CommandLine, string> parallels = new Dictionary<CommandLine, string>();
            foreach (var kvp in outPorts) {
                var line = kvp.Key;
                if (line.pointType != CommandLine.PointType.Main) continue;
                else if (line.outMainType == CommandLine.OutMainType.Parallel) parallels.Add(line, kvp.Value);
            }
            foreach (var kvp in parallels) {
                string methodName = string.Format("RunSubFlow(_flow, {0});", kvp.Value);
                parallelMethodNames.Add(methodName);
            }
            return result;
        }

        private string CreateGetNextOutputIndex(CommandConstructor _constructor) {
            alternateExitCount = 0;
            string result = "";
            Dictionary<CommandLine, string> alternateExits = new Dictionary<CommandLine, string>();
            foreach (var kvp in outPorts) {
                var line = kvp.Key;
                if (line.pointType != CommandLine.PointType.Main) continue;
                else if (line.outMainType == CommandLine.OutMainType.AlternateExit) alternateExits.Add(line, kvp.Value);
            }
            if (alternateExits.Count == 0) return result;
            alternateExitCount = alternateExits.Count;
            result += @"

        public override int GetNextOutputIndex() {
            //You can change exitPortIndex in Execute method by your desired condition
            return exitPortIndex;
        }";
            return result;
        }

        private string CreateExitPortIndex() {
            string result = "";
            if (alternateExitCount > 0) {
                result += @"

        private int exitPortIndex = 0;
";
            }
            return result;
        }

        private string CreateGetOutput(CommandConstructor _constructor) {
            string result = "";
            if (outPorts.Select(kvp=>kvp.Key.pointType != CommandLine.PointType.Main).ToArray().Length == 0) return result;
            result += string.Format(@"
        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {{ {0}
            switch(_pointIndex) {{ {1}
                default:
                    Debug.LogError(""Output port index does not exist in {2}: "" + _pointIndex);
                    return default(T);
            }}
        }}", _constructor.isPropertyCommand? CreateStoredInputs(_constructor) : "", CreateOutputCases(), _constructor.className);
            return result;
        }

        private string CreateOutputCases() {
            string result = "\n";
            foreach (var kvp in outPorts) {
                var line = kvp.Key;
                if (line.pointType == CommandLine.PointType.Main) continue;
                string val;
                if (line.pointType == CommandLine.PointType.Boolean) val = "(T)(object)false";
                else if (line.pointType == CommandLine.PointType.Float) val = "(T)(object)0.0f";
                else if (line.pointType == CommandLine.PointType.Integer) val = "(T)(object)0";
                else if (line.pointType == CommandLine.PointType.String) val = "(T)(object)\"\"";
                else if (line.pointType == CommandLine.PointType.UnityObject) val = "(T)(object)null";
                else if (line.pointType == CommandLine.PointType.Vector3) val = "(T)(object)Vector3.zero";
                else if (line.pointType == CommandLine.PointType.Unspecified) val = "default(T)";
                else val = "default(T)";
                result += string.Format(@"
                case {0}:
                    //...YOUR EVALUATION LOGIC GOES HERE...
                    return {1};
", kvp.Value, val);
            }
            return result;
        }

        private static string template => @"
using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace <NAMESPACE> {

    [System.Serializable]
    [RegisterCommand(""<DISPLAY-NAME>"", <INCLUDED-COMMAND-DATA>)]
    public class <CLASS-NAME> : <BASE-TYPE> {
<EXECUTE>
<GET-OUTPUT><PARALLEL-OUT><GET-NEXT-ID>

        #region PROPERTIES<EXIT-PORT-INDEX>
        public override float nodeWidth => <NODE-WIDTH>;
<CONST-INPUT-PORTS>
<CONST-OUTPUT-PORTS>
<VARIABLES>
        #endregion PROPERTIES

        #region NODE-DRAWER
#if UNITY_EDITOR
<INIT-IN-POINTS>

<INIT-OUT-POINTS>

<DRAW-TITLE>
<DRAW-CONTENTS>
#endif
        #endregion NODE-DRAWER

    }

}";

    }
}