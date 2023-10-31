using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Calcatz.CookieCutter {

    [CustomEditor(typeof(CrossSceneBinder))]
    internal class CrossSceneBinderInspector : Editor {

        private CrossSceneBinder binder;

        private void OnEnable() {
            binder = (CrossSceneBinder)target;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            var binderAsset = serializedObject.FindProperty("m_binderAsset");
            var componentToBind = serializedObject.FindProperty("m_componentToBind");
            var deactivateGameObjectOnAwake = serializedObject.FindProperty("deactivateGameObjectOnAwake");

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            DrawBinderAssetProperty(binderAsset);
            var col = GUI.color;
            if (binderAsset.objectReferenceValue == null || componentToBind.objectReferenceValue == null) {
                GUI.color = new Color(1, 1, 1, 0.3f);
            }
            DrawChainIcon();
            GUI.color = col;
            DrawComponentProperty(componentToBind);
            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            deactivateGameObjectOnAwake.boolValue =
                EditorGUILayout.ToggleLeft(new GUIContent("Deactivate GameObject on Awake", deactivateGameObjectOnAwake.tooltip), deactivateGameObjectOnAwake.boolValue);

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawChainIcon() {
            float size = EditorGUIUtility.singleLineHeight;
            var chainRect = GUILayoutUtility.GetRect(size, size*2, size, size*2, GUILayout.ExpandWidth(false));
            chainRect.height /= 2f;
            chainRect.y += chainRect.height;
            GUI.Label(chainRect, EditorGUIUtility.IconContent("d_FixedJoint Icon"), EditorStyles.centeredGreyMiniLabel);
        }

        private void DrawBinderAssetProperty(SerializedProperty binderAsset) {
            var binderAssetRect = GUILayoutUtility.GetRect(1, (int)(EditorGUIUtility.singleLineHeight * 2), GUILayout.ExpandWidth(true));
            var labelRect = binderAssetRect;
            labelRect.height = EditorGUIUtility.singleLineHeight;
            var fieldRect = labelRect;
            fieldRect.y += EditorGUIUtility.singleLineHeight;
            if (binderAsset.objectReferenceValue == null) {
                fieldRect.width -= 65;
                var createRect = fieldRect;
                createRect.x = fieldRect.x + fieldRect.width + 5;
                createRect.width = 60;
                EditorGUI.BeginProperty(binderAssetRect, GUIContent.none, binderAsset);
                EditorGUI.LabelField(labelRect, new GUIContent("Binder Asset:", binderAsset.tooltip));
                EditorGUI.PropertyField(fieldRect, binderAsset, GUIContent.none);
                if (GUI.Button(createRect, "New")) {
                    binderAsset.objectReferenceValue = CCAssetUtility.CreateAssetAtSceneFolder<CrossSceneBinderAsset>(binder.gameObject.scene, "Binders", "Binder_" + binder.name);
                }
                EditorGUI.EndProperty();
            }
            else {
                EditorGUI.BeginProperty(binderAssetRect, GUIContent.none, binderAsset);
                EditorGUI.LabelField(labelRect, new GUIContent("Binder Asset:", binderAsset.tooltip));
                EditorGUI.PropertyField(fieldRect, binderAsset, GUIContent.none);
                EditorGUI.EndProperty();
            }
        }

        private void DrawComponentProperty(SerializedProperty componentToBind) {
            var componentRect = GUILayoutUtility.GetRect(1, (int)(EditorGUIUtility.singleLineHeight * 3), GUILayout.ExpandWidth(true));
            var labelRect = componentRect;
            labelRect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginProperty(componentRect, GUIContent.none, componentToBind);
            componentRect.height = EditorGUIUtility.singleLineHeight;
            componentRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(labelRect, new GUIContent("Component to Bind:", componentToBind.tooltip));
            EditorGUI.PropertyField(componentRect, componentToBind, GUIContent.none);
            componentRect.y += componentRect.height + 2;
            if (GUI.Button(componentRect, "Select Component", EditorStyles.popup)) {
                var contextMenu = new GenericMenu();
                var components = binder.GetComponents<Component>();
                foreach (var component in components) {
                    if (component == binder) continue;
                    var c = component;
                    string nicifiedTypeName = ObjectNames.NicifyVariableName(c.GetType().Name);
                    contextMenu.AddItem(new GUIContent(nicifiedTypeName), false, () => {
                        serializedObject.Update();
                        serializedObject.FindProperty("m_componentToBind").objectReferenceValue = c;
                        serializedObject.ApplyModifiedProperties();
                    });
                }
                contextMenu.ShowAsContext();
            }
            EditorGUI.EndProperty();
        }

    }

    [InitializeOnLoad]
    static class CrossSceneBinderComponentCreator {
        static CrossSceneBinderComponentCreator() {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;
        }

        static void HierarchyWindowItemCallback(int instanceID, Rect pRect) {
            if (Event.current.type == EventType.DragPerform && pRect.Contains(Event.current.mousePosition)) {
                DragAndDrop.AcceptDrag();
                GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

                if (gameObject == null) return;

                bool used = false;

                if (gameObject != null) {
                    foreach (var objectRef in DragAndDrop.objectReferences) {
                        if (objectRef is CrossSceneBinderAsset binderAsset) {
                            var binderComponent = Undo.AddComponent<CrossSceneBinder>(gameObject);
                            typeof(CrossSceneBinder).GetField("m_binderAsset", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                .SetValue(binderComponent, binderAsset);
                            used = true;
                        }
                    }
                }

                if (used) {
                    Event.current.Use();
                }
            }
        }
    }

}