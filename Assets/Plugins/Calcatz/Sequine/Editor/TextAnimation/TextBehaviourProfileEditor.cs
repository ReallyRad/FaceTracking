using Calcatz.CookieCutter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Calcatz.Sequine {

    [CustomEditor(typeof(TextBehaviourProfile))]
    internal sealed class TextBehaviourProfileEditor : Editor {

        private static bool editorsInitialized;
        private static string searchTerm = "";

        private TextBehaviourProfile profile;
        private List<SerializedObject> componentSOList = new List<SerializedObject>();
        private List<SerializedObject> componentsToDelete = new List<SerializedObject>();

        private void OnEnable() {
            if (!editorsInitialized) {
                foreach (var editorType in TypeCache.GetTypesWithAttribute<CustomTextBehaviourEditorAttribute>()) {
                    var attrib = editorType.GetCustomAttributes(typeof(CustomTextBehaviourEditorAttribute), false).OfType<CustomTextBehaviourEditorAttribute>().FirstOrDefault();
                    if (attrib != null) {
                        CustomTextBehaviourEditorAttribute.editorTypes.Add(attrib.textBehaviourComponentType, editorType);
                    }
                }
                editorsInitialized = true;
            }
            profile = (TextBehaviourProfile)target;
            RefreshComponentsSO();
        }

        private void RefreshComponentsSO() {
            profile.components.RemoveAll(_comp => _comp == null);
            componentSOList.Clear();
            foreach (var component in profile.components) {
                componentSOList.Add(new SerializedObject(component));
            }
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("durationPerCharacter"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("delayPerCharacter"));
            EditorGUILayout.Space();
            DrawBehaviourComponents();
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Behaviour")) {
                ShowAddBehaviourMenu();
            }
            serializedObject.ApplyModifiedProperties();

            if (Event.current.type == EventType.ValidateCommand) {
                if (Event.current.commandName == "UndoRedoPerformed") {
                    RefreshComponentsSO();
                    Repaint();
                }
            }
        }

        private void DrawBehaviourComponents() {
            componentsToDelete.Clear();
            foreach (var componentSO in componentSOList.ToArray()) {
                if (componentSO.targetObject == null) {
                    componentSOList.Remove(componentSO);
                    continue;
                }
                componentSO.Update();
                var foldoutProp = componentSO.FindProperty("editor_foldout");
                var activeProp = componentSO.FindProperty("active");
                GUILayout.BeginHorizontal();
                string displayName = GetDisplayName(componentSO);
                foldoutProp.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutProp.boolValue, new GUIContent(displayName));
                EditorGUILayout.EndFoldoutHeaderGroup();
                if (GUILayout.Button("Remove", GUILayout.Width(80))) {
                    componentsToDelete.Add(componentSO);
                }
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                if (foldoutProp.boolValue) {
                    activeProp.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Active"), activeProp.boolValue);
                    bool guiEnabled = GUI.enabled;
                    GUI.enabled = guiEnabled && activeProp.boolValue;
                    if (CustomTextBehaviourEditorAttribute.editorTypes.TryGetValue(componentSO.targetObject.GetType(), out var editorType)) {
                        var editorInstance = Activator.CreateInstance(editorType);
                        var drawerMethod = editorType.GetMethod("OnTextBehaviourComponentGUI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (drawerMethod == null) {
                            Debug.LogError("Can't find OnTextBehaviourComponentGUI(SerializedObject _serializedObject) method on " + editorType.Name);
                        }
                        else {
                            drawerMethod.Invoke(editorInstance, new object[] { componentSO });
                        }
                    }
                    else {
                        var serializedProperty = componentSO.GetIterator();
                        serializedProperty.NextVisible(true);
                        while (serializedProperty.NextVisible(false)) {
                            EditorGUILayout.PropertyField(serializedProperty, includeChildren: true);
                        }
                    }
                    GUI.enabled = guiEnabled;
                }
                EditorGUI.indentLevel--;
                componentSO.ApplyModifiedProperties();
                EditorGUILayout.Space();
                GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(2));
            }
            if (componentsToDelete.Count > 0) {
                RemoveSpecifiedComponents();
            }
        }

        private static string GetDisplayName(SerializedObject _componentSO) {
            return GetDisplayName(_componentSO.targetObject.name);
        }

        private static string GetDisplayName(string _name) {
            return ObjectNames.NicifyVariableName(_name).Replace("Text Behaviour Component", "");
        }

        private void RemoveSpecifiedComponents() {
            Undo.SetCurrentGroupName("remove text behaviour component");
            Undo.RecordObject(profile, "remove text behaviour component");
            foreach (var componentToDelete in componentsToDelete) {
                componentSOList.Remove(componentToDelete);
                profile.components.Remove(componentToDelete.targetObject as TextBehaviourComponent);
                Undo.DestroyObjectImmediate(componentToDelete.targetObject);

            }
            componentsToDelete.Clear();
            EditorUtility.SetDirty(profile);
            RefreshComponentsSO();
        }

        private void ShowAddBehaviourMenu() {
            GenericMenu genericMenu = new GenericMenu();
            var behaviourTypes = TypeCache.GetTypesDerivedFrom<TextBehaviourComponent>();
            foreach (var behaviourType in behaviourTypes) {
                var type = behaviourType;
                if (profile.components.Find(_comp => _comp.GetType() == type) == null) {
                    genericMenu.AddItem(new GUIContent(GetDisplayName(type.Name)), false, () => AddBehaviourComponent(type));
                }
            }
            GenericMenuPopup popup = GenericMenuPopup.Show(genericMenu, "Add Behaviour", Event.current.mousePosition, new List<string>(), searchTerm, _search => {
                searchTerm = _search;
            });
        }

        private void AddBehaviourComponent(Type _type) {
            Undo.SetCurrentGroupName("add text behaviour component");
            var component = CreateInstance(_type) as TextBehaviourComponent;
            component.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            component.name = _type.Name;
            Undo.RegisterCreatedObjectUndo(component, "create text behaviour component");
            if (EditorUtility.IsPersistent(profile))
                AssetDatabase.AddObjectToAsset(component, profile);

            Undo.RecordObject(profile, "add text behaviour component");
            profile.components.Add(component);
            EditorUtility.SetDirty(profile);
            RefreshComponentsSO();
        }

    }

}