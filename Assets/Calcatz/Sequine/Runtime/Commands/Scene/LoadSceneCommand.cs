
using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Calcatz.Sequine {

    /// <summary>
    /// Loads a scene synchronously.
    /// Make sure to add the scene to the build settings first.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Scene/Load Scene", typeof(SequineFlowCommandData))]
    public class LoadSceneCommand : Command {

        public override void Execute(CommandExecutionFlow _flow) { 
            var sceneToLoad_value = GetInput(_flow, IN_PORT_SCENE_TO_LOAD, sceneToLoad);
            SceneManager.LoadScene(sceneToLoad_value, loadSceneMode);
            Exit();
        }

        #region PROPERTIES
        public override float nodeWidth => 275f;

        private const int IN_PORT_SCENE_TO_LOAD = 0;

        public string sceneToLoad;
        public LoadSceneMode loadSceneMode;
        #endregion PROPERTIES

        #region NODE-DRAWER
#if UNITY_EDITOR
        public UnityEditor.SceneAsset sceneAssetToLoad;

        public override void Editor_InitInPoints() {
            CommandGUI.AddMainInPoint();
            if (inputIds.Count <= IN_PORT_SCENE_TO_LOAD) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint<string>();
        }

        public override void Editor_InitOutPoints() {
            CommandGUI.AddMainOutPoint();
        }

        public override void Editor_OnDrawTitle(out string _tooltip) {
            _tooltip = @"Loads a scene synchronously.
Make sure to add the scene to the build settings first.";
            CommandGUI.DrawInPoint(0);
            CommandGUI.DrawOutPoint(0);
        }
                
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            CommandGUI.DrawInPoint(IN_PORT_SCENE_TO_LOAD + 1);
            if (inputIds[IN_PORT_SCENE_TO_LOAD].targetId == 0) {    
                CommandGUI.DrawObjectField("Scene to Load", "The scene to load. This will be stored as string in build (the scene's name).", sceneAssetToLoad, _sceneToLoad => {
                    sceneAssetToLoad = _sceneToLoad;
                    if (_sceneToLoad != null) {
                        sceneToLoad = _sceneToLoad.name;
                    }
                    else {
                        sceneToLoad = "";
                    }
                }, true);
            }
            else {
                CommandGUI.DrawLabel("Scene to Load");
            }
            CommandGUI.DrawEnumField("Load Scene Mode", "", ref loadSceneMode);
            if (loadSceneMode == LoadSceneMode.Single) {
                var warningRect = CommandGUI.GetRect(3);
                UnityEditor.EditorGUI.HelpBox(warningRect,
                    "Single Mode requires the executor to be Dont Destroy On Load, or else, there might be unexpected problem due to the destroyed executor.",
                    UnityEditor.MessageType.Warning);
                CommandGUI.AddRectHeight(warningRect.height);
            }
        }
                
#endif
        #endregion NODE-DRAWER

    }

}