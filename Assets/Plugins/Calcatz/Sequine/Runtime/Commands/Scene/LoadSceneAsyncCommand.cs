
using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Calcatz.Sequine {

    /// <summary>
    /// Loads a scene asynchronously.
    /// Make sure to add the scene to the build settings first.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Scene/Load Scene Async", typeof(SequineFlowCommandData))]
    public class LoadSceneAsyncCommand : Command {

        private float returnedProgress;

        public override void Execute(CommandExecutionFlow _flow) { 
            var sceneToLoad_value = GetInput(_flow, IN_PORT_SCENE_TO_LOAD, sceneToLoad);
            LoadSceneAsync(_flow, sceneToLoad_value);
            Exit();
        }

        private async void LoadSceneAsync(CommandExecutionFlow _flow, string _sceneName) {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_sceneName, loadSceneMode);
            asyncOperation.allowSceneActivation = true;

            while (!asyncOperation.isDone) {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    return;
                }
#endif
                float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                returnedProgress = progress;
                RunSubFlow(_flow, OUT_PORT_ON_UPDATE);

                await Task.Yield();
            }
            RunSubFlow(_flow, OUT_PORT_ON_COMPLETE);
        }

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) { 
            switch(_pointIndex) { 

                case OUT_PORT_PROGRESS:
                    return (T)(object)returnedProgress;

                default:
                    Debug.LogError("Output port index does not exist in LoadSceneCommand: " + _pointIndex);
                    return default(T);
            }
        }

        #region PROPERTIES
        public override float nodeWidth => 275f;

        private const int IN_PORT_SCENE_TO_LOAD = 0;

        private const int OUT_PORT_ON_COMPLETE = 1;
        private const int OUT_PORT_ON_UPDATE = 2;
        private const int OUT_PORT_PROGRESS = 3;

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
            if (nextIds.Count <= OUT_PORT_ON_COMPLETE) {
                nextIds.Add(new List<ConnectionTarget>());
            }
            if (nextIds.Count <= OUT_PORT_ON_UPDATE) {
                nextIds.Add(new List<ConnectionTarget>());
            }
            if (nextIds.Count <= OUT_PORT_PROGRESS) {
                nextIds.Add(new List<ConnectionTarget>());
            }
            CommandGUI.AddMainOutPoint();
            CommandGUI.AddMainOutPoint();
            CommandGUI.AddPropertyOutPoint<System.Single>();
        }

        public override void Editor_OnDrawTitle(out string _tooltip) {
            _tooltip = @"Loads a scene asynchronously.
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
            CommandGUI.AddRectHeight(5);
            CommandGUI.DrawOutPoint(OUT_PORT_ON_COMPLETE);
            CommandGUI.DrawLabel("On Complete", "Invoked when the scene is loaded.", true);
            CommandGUI.DrawOutPoint(OUT_PORT_ON_UPDATE);
            CommandGUI.DrawLabel("On Update", "Invoked each frame during the loading process. Use this if you want to utilize the Progress output.", true);
            CommandGUI.DrawOutPoint(OUT_PORT_PROGRESS);
            CommandGUI.DrawLabel("Progress", "The loading progress (0-1).", true);
        }
                
#endif
        #endregion NODE-DRAWER

    }

}