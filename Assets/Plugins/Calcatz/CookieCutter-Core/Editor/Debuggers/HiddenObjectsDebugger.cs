using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Calcatz.CookieCutter {
    internal class HiddenObjectsDebugger : EditorWindow {

        private Vector2 scrollPos;

        [SerializeField] private List<UnityEngine.Object> hiddenObjects = new List<Object>();


        [MenuItem("Window/Analysis/Calcatz/Hidden Objects Debugger")]
        private static void OpenWindow() {
            var window = GetWindow<HiddenObjectsDebugger>();
            window.titleContent = new GUIContent("Hidden Objects Debugger");
            window.Show();
        }

        private void OnGUI() {
            if (GUILayout.Button("Refresh")) {
                Refresh();
            }
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);
            GUILayout.BeginVertical();
            foreach (var hiddenObject in hiddenObjects) {
                EditorGUILayout.ObjectField(hiddenObject, typeof(GameObject), true);
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private void Refresh() {
            hiddenObjects.Clear();
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

            foreach (var rootObject in rootObjects) {
                HandleDeepTransforms(rootObject.transform, _transform => {
                    if (_transform.gameObject.hideFlags.HasFlag(HideFlags.HideInHierarchy) ||
                        _transform.gameObject.hideFlags.HasFlag(HideFlags.HideInInspector)) {

                        hiddenObjects.Add(_transform.gameObject);
                    }
                });
            }

        }

        private void HandleDeepTransforms(Transform _transform, System.Action<Transform> _handler) {
            _handler.Invoke(_transform);
            for (int i=0; i<_transform.childCount; i++) {
                var child = _transform.GetChild(i);
                HandleDeepTransforms(child, _handler);
            }
        }

    }
}