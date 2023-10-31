using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Calcatz.Sequine {
    [InitializeOnLoad]
    public static class ComponentEditorUtility {

        static ComponentEditorUtility() {
            if (GetAnnotations == null) return;

            var annotations = (ICollection)GetAnnotations.Invoke(null, new object[0]);
            var annotationNames = new List<string>();
            foreach (var annotation in annotations) {
                var scriptClass = annotation.GetType()
                    .GetField("scriptClass", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .GetValue(annotation) as string;
                if (string.IsNullOrEmpty(scriptClass)) continue;
                annotationNames.Add(scriptClass);
            }

            if (IsAnnotationAvailable(annotationNames, typeof(SequineFlowComponent))) {
                SetGizmoIconEnabled(typeof(SequineFlowComponent), false);
            }
            if (IsAnnotationAvailable(annotationNames, typeof(SequineText))) {
                SetGizmoIconEnabled(typeof(SequineText), false);
            }
            if (IsAnnotationAvailable(annotationNames, typeof(SequinePlayer))) {
                SetGizmoIconEnabled(typeof(SequinePlayer), false);
            }
        }

        static MethodInfo getAnnotation;
        //Doesn't work, it returns non-null Annotation but with null scriptClass.
        static MethodInfo GetAnnotation => getAnnotation = getAnnotation ??
            Assembly.GetAssembly(typeof(Editor))
            ?.GetType("UnityEditor.AnnotationUtility")
            ?.GetMethod("GetAnnotation", BindingFlags.Static | BindingFlags.NonPublic);

        static MethodInfo getAnnotations;
        static MethodInfo GetAnnotations => getAnnotations = getAnnotations ??
            Assembly.GetAssembly(typeof(Editor))
            ?.GetType("UnityEditor.AnnotationUtility")
            ?.GetMethod("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic);

        static MethodInfo setIconEnabled;
        static MethodInfo SetIconEnabled => setIconEnabled = setIconEnabled ??
            Assembly.GetAssembly(typeof(Editor))
            ?.GetType("UnityEditor.AnnotationUtility")
            ?.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);

        public static bool IsAnnotationAvailable(List<string> _annotationNames, Type _type) {
            foreach (var annotationName in _annotationNames) {
                if (annotationName == _type.Name) return true;
            }
            return false;
        }

        public static void SetGizmoIconEnabled(Type _type, bool _on) {
            if (SetIconEnabled == null) return;
            const int MONO_BEHAVIOR_CLASS_ID = 114; // https://docs.unity3d.com/Manual/ClassIDReference.html
            SetIconEnabled.Invoke(null, new object[] { MONO_BEHAVIOR_CLASS_ID, _type.Name, _on ? 1 : 0 });
        }


        public static void CreateGameObject<T>(string _gameObjectName) where T : MonoBehaviour {
            GameObject parent = Selection.activeGameObject;
            if (parent != null && !IsObjectInCurrentScene(parent)) {
                parent = null;
            }

            GameObject newObject = new GameObject(_gameObjectName);
            if (parent != null) {
                newObject.transform.parent = parent.transform;
            }
            newObject.AddComponent<T>();

            Selection.activeGameObject = newObject;
            Undo.RegisterCreatedObjectUndo(newObject, "create a gameobject with component " + typeof(T));
        }

        private static bool IsObjectInCurrentScene(GameObject _obj) {
            Scene currentScene = SceneManager.GetActiveScene();
            return _obj != null && _obj.scene == currentScene;
        }

    }

}