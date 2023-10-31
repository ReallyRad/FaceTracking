using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
#if TIMELINE_AVAILABLE
using UnityEngine.Timeline;
#endif

namespace Calcatz.CookieCutter {
    public class MissingScriptsFinder : EditorWindow {

        private Vector2 scrollPos;

        [SerializeField] private List<UnityEngine.Object> foundObjects = new List<Object>();


        [MenuItem("Window/Analysis/Calcatz/Find Missing Scripts")]
        private static void OpenWindow() {
            var window = GetWindow<MissingScriptsFinder>();
            window.titleContent = new GUIContent("Missing Components Finder");
            window.Show();
        }

        private void OnGUI() {
            if (GUILayout.Button("Find In Current Scenes")) {
                FindInCurrentScenes();
            }
            if (GUILayout.Button("Find In Assets")) {
                FindInAssets();
            }
#if TIMELINE_AVAILABLE
            if (GUILayout.Button("Find In Timelines")) {
                FindInTimelines();
            }
#endif
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);
            GUILayout.BeginVertical();
            foreach (var foundObject in foundObjects) {
                EditorGUILayout.ObjectField(foundObject, typeof(GameObject), true);
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private void FindInCurrentScenes() {
            foundObjects.Clear();
            List<GameObject> rootObjects = new List<GameObject>();

            if (Application.isPlaying) {
                for (int i = 0; i < SceneManager.sceneCount; i++) {
                    var scene = SceneManager.GetSceneAt(i);
                    var roots = new List<GameObject>();
                    scene.GetRootGameObjects(roots);
                    rootObjects.AddRange(roots);
                }
            }
            else {
                for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
                    var scene = EditorSceneManager.GetSceneAt(i);
                    var roots = new List<GameObject>();
                    scene.GetRootGameObjects(roots);
                    rootObjects.AddRange(roots);
                }
            }

            for (int i=0; i<rootObjects.Count; i++) {
                var rootObject = rootObjects[i];
                EditorUtility.DisplayProgressBar("Searching Missing Components", "Searching...", (i + 1) / (float)rootObjects.Count);
                HandleDeepTransforms(rootObject.transform, _transform => {
                    Component[] components = _transform.GetComponents<Component>();
                    for (int j = 0; j < components.Length; j++) {
                        Component currentComponent = components[j];
                        if (ReferenceEquals(currentComponent, null)) {
                            foundObjects.Add(_transform.gameObject);
                            break;
                        }
                    }
                });
            }
            EditorUtility.ClearProgressBar();

        }

        private void FindInAssets() {
            foundObjects.Clear();
            string[] assetsPaths = AssetDatabase.GetAllAssetPaths();

            if (assetsPaths.Length > 2000) {
                if (!EditorUtility.DisplayDialog("Warning", "There are more that " + assetsPaths.Length + " assets to check and it might take a while. Continue?", "Yes", "Cancel")) {
                    return;
                }
            }

            for (int i = 0; i < assetsPaths.Length; i++) {
                Object[] data = LoadAllAssetsAtPath(assetsPaths[i]);
                EditorUtility.DisplayProgressBar("Searching Missing Components", "Searching... (" + (i + 1) + "/" + assetsPaths.Length + ")", (i + 1) / (float)assetsPaths.Length);
                for (int j=0; j < data.Length; j++) {
                    if (data[j] is GameObject go) {
                        HandleDeepTransforms(go.transform, _transform => {
                            Component[] components = _transform.GetComponents<Component>();
                            for (int k = 0; k < components.Length; k++) {
                                Component currentComponent = components[k];
                                if (ReferenceEquals(currentComponent, null)) {
                                    foundObjects.Add(_transform.gameObject);
                                    break;
                                }
                            }
                        });
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }

#if TIMELINE_AVAILABLE
        private void FindInTimelines() {
            foundObjects.Clear();
            var timelineAssets = CCAssetUtility.FindAssetsByType<TimelineAsset>();

            for (int i = 0; i < timelineAssets.Count; i++) {
                var timeline = timelineAssets[i];
                EditorUtility.DisplayProgressBar("Searching Missing Components", "Searching... (" + (i + 1) + "/" + timelineAssets.Count + ")", (i + 1) / (float)timelineAssets.Count);

                var tracks = typeof(TimelineAsset).GetField("m_Tracks", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(timeline) as List<ScriptableObject>;
                foreach (var obj in tracks) {
                    if (obj.GetType() == typeof(ScriptableObject)) {
                        foundObjects.Add(timeline);
                    }
                    if (obj is TrackAsset track) {
                        HandleDeepTracks(track, _track => {
                            if (_track.GetType() == typeof(ScriptableObject)) {
                                foundObjects.Add(timeline);
                            }
                        });
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }
#endif

        private static Object[] LoadAllAssetsAtPath(string assetPath) {
            return typeof(SceneAsset).Equals(AssetDatabase.GetMainAssetTypeAtPath(assetPath))
                ?
                // prevent error "Do not use readobjectthreaded on scene objects!"
                new[] { AssetDatabase.LoadMainAssetAtPath(assetPath) }
                : AssetDatabase.LoadAllAssetsAtPath(assetPath);
        }

        private void HandleDeepTransforms(Transform _transform, System.Action<Transform> _handler) {
            _handler.Invoke(_transform);
            for (int i = 0; i < _transform.childCount; i++) {
                var child = _transform.GetChild(i);
                HandleDeepTransforms(child, _handler);
            }
        }

#if TIMELINE_AVAILABLE
        private void HandleDeepTracks(ScriptableObject _track, System.Action<ScriptableObject> _handler) {
            _handler.Invoke(_track);
            if (_track is TrackAsset) {
                var children = typeof(TrackAsset).GetField("m_Children", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(_track) as List<ScriptableObject>;
                foreach (var child in children) {
                    HandleDeepTracks(child, _handler);
                }
            }
        }
#endif

    }
}