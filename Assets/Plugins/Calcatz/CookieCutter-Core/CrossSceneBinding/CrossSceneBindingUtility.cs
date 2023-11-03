using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Calcatz.CookieCutter {

    /// <summary>
    /// A utility class to manage cross-scene binded components.
    /// A cross-scene component can be obtained by specifying the binder asset.
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoadAttribute]
#endif
    public static class CrossSceneBindingUtility {

#if UNITY_EDITOR
        static CrossSceneBindingUtility() {
            EditorApplication.playModeStateChanged += HandlePlayModeState;
        }
        private static void HandlePlayModeState(PlayModeStateChange state) {
            if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.ExitingPlayMode) {
                bindings.Clear();
            }
        }

        //For Editor Window
        internal static bool changed = false;
#endif

        //key: asset, value: scene object
        internal static Dictionary<UnityEngine.Object, List<UnityEngine.Object>> bindings = new Dictionary<Object, List<Object>>();

        public static void Bind(UnityEngine.Object _binderAsset, UnityEngine.Object _sceneObject) {
            if (bindings.ContainsKey(_binderAsset)) {
                //Debug.LogError(_binderAsset + " is already binded to an object: " + bindings[_binderAsset]);
                if (!bindings[_binderAsset].Contains(_sceneObject)) {
                    AddBinding(_binderAsset, _sceneObject);
                }
            }
            else {
                bindings.Add(_binderAsset, new List<Object>());
                AddBinding(_binderAsset, _sceneObject);
            }
        }

        private static void AddBinding(Object _binderAsset, Object _sceneObject) {
            bindings[_binderAsset].Add(_sceneObject);
#if UNITY_EDITOR
            changed = true;
#endif
        }

        public static void RemoveBinding(UnityEngine.Object _binderAsset, UnityEngine.Object _sceneObject) {
            if (bindings.ContainsKey(_binderAsset)) {
                bindings[_binderAsset].Remove(_sceneObject);
                if (bindings[_binderAsset].Count == 0) bindings.Remove(_binderAsset);
#if UNITY_EDITOR
                changed = true;
#endif
            }
        }

        public static UnityEngine.Object GetObject(UnityEngine.Object _binderAsset) {
            if (_binderAsset == null) return null;
            List<UnityEngine.Object> results;
            if (bindings.TryGetValue(_binderAsset, out results)) {
                if (results.Count > 0) {
                    return results[0];
                }
            }
            return null;
        }

        public static T GetObject<T>(UnityEngine.Object _binderAsset) {
            if (_binderAsset == null) return default(T);
            List<UnityEngine.Object> results;
            if (bindings.TryGetValue(_binderAsset, out results)) {
                if (results.Count > 0) {
                    return (T)(object)(results[0]);
                }
            }
            return default(T);
        }

        public static bool TryGetObject<T>(UnityEngine.Object _binderAsset, out T _object) {
            if (_binderAsset == null) {
                _object = default(T);
                return false;
            }
            List<UnityEngine.Object> results;
            if (bindings.TryGetValue(_binderAsset, out results)) {
                if (results.Count > 0) {
                    _object = (T)(object)(results[0]);
                    return true;
                }
            }
            _object = default(T);
            return false;
        }

        public static UnityEngine.Object[] GetObjects(UnityEngine.Object _binderAsset) {
            if (_binderAsset == null) return null;
            List<UnityEngine.Object> results;
            if (bindings.TryGetValue(_binderAsset, out results)) {
                return results.ToArray();
            }
            return null;
        }

        public static T[] GetObjects<T>(UnityEngine.Object _binderAsset) {
            if (_binderAsset == null) return new T[0];
            List<UnityEngine.Object> results;
            if (bindings.TryGetValue(_binderAsset, out results)) {
                T[] arrayOfT = new T[results.Count];
                for (int i = 0; i < arrayOfT.Length; i++) {
                    arrayOfT[i] = (T)(object)(results[i]);
                }
            }
            return new T[0];
        }

        /*public static bool IsBinded(UnityEngine.Object _binderAsset) {
            return bindings.ContainsKey(_binderAsset);
        }*/

    }

}
