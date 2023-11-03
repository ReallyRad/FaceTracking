using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;
using System.IO;

namespace Calcatz.Sequine {

    internal class SequineScriptTemplate {

        private static string commandTemplate = @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Calcatz.Sequine;
using Calcatz.CookieCutter;

    #ROOTNAMESPACEBEGIN#
[System.Serializable]
[RegisterCommand(""Custom/#SCRIPTNAME#"", typeof(SequineFlowCommandData))]
public class #SCRIPTNAME# : Command
{
    public override void Execute(CommandExecutionFlow _flow)
    {
        //Your logic here... (Before Exit Method)
        Exit();
    }
    #NOTRIM#
#if UNITY_EDITOR
    public override void Editor_OnDrawContents(Vector2 _absPosition)
    {
        
    }
#endif
}
#ROOTNAMESPACEEND#
";

        private static string propertyCommandTemplate = @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Calcatz.Sequine;
using Calcatz.CookieCutter;

    #ROOTNAMESPACEBEGIN#
[System.Serializable]
[RegisterCommand(""Custom/#SCRIPTNAME#"", typeof(SequineFlowCommandData))]
public class #SCRIPTNAME# : PropertyCommand
{
    public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex)
    {
        switch(_pointIndex) {
            case 0:
                bool valueToReturn = false;
                //Your logic here...
                return (T)(object)valueToReturn;
            default:
                return (T)(object)false;
        }
    }
    #NOTRIM#
#if UNITY_EDITOR
    public override void Editor_InitOutPoints() {
        if (nextIds.Count < 1) nextIds.Add(new List<ConnectionTarget>());
        CommandGUI.AddPropertyOutPoint<bool>();
    }
    public override void Editor_OnDrawContents(Vector2 _absPosition) {
        CommandGUI.DrawOutPoint(0);
        CommandGUI.DrawLabel(""Out"", true);
    }
#endif
}
#ROOTNAMESPACEEND#
";

        private static string textBehaviourComponentTemplate = @"using TMPro;
using UnityEngine;
using Calcatz.Sequine;

public class #SCRIPTNAME# : TextBehaviourComponent
{

    public override bool overrideGeometry => false;
    public override bool overrideVertexData => false;

    public override void HandleCharacterBehaviour(CharacterData _characterData,
                                                    TMP_Text _textComponent,
                                                    TMP_TextInfo _textInfo,
                                                    TMP_MeshInfo[] _meshInfo,
                                                    Bounds _meshBounds,
                                                    float _segmentNormalizedTime)
    {

    }

}";

        private static void CreateScript(string _templateContent, string _templateFileName, string _fileName) {
            string libraryPath = Path.Combine(Application.dataPath, "../Library/Sequine");
            string scriptName = Path.Combine(libraryPath, _templateFileName);

            if (!System.IO.Directory.Exists(libraryPath))
                System.IO.Directory.CreateDirectory(libraryPath);

            if (!System.IO.File.Exists(scriptName)) {
                System.IO.File.WriteAllText(scriptName, _templateContent);
            }

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(scriptName, _fileName);
        }

        [MenuItem("Assets/Create/Sequine/C# Command Script", false, 150)]
        private static void CreateCommandScript() {
            CreateScript(commandTemplate, "150-C# Script-NewSequineCommand.cs.txt", "NewCommand.cs");
        }

        [MenuItem("Assets/Create/Sequine/C# Property Command Script", false, 151)]
        private static void CreatePropertyCommandScript() {
            CreateScript(propertyCommandTemplate, "151-C# Script-NewSequinePropertyCommand.cs.txt", "NewPropertyCommand.cs");
        }

        [MenuItem("Assets/Create/Sequine/C# Text Behaviour Component", false, 152)]
        private static void CreateTextBehaviourComponentScript() {
            CreateScript(textBehaviourComponentTemplate, "152-C# Script-NewSequineTextBehaviourComponent.cs.txt", "NewTextBehaviourComponent.cs");
        }
    }

}