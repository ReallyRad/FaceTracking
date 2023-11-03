using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Calcatz.Sequine {

    [CustomEditor(typeof(SequinePlayer), true)]
#if ODIN_INSPECTOR
    internal class SequinePlayerEditor : Sirenix.OdinInspector.Editor.OdinEditor {
#else
    internal class SequinePlayerEditor : Editor {
#endif
        
        [SerializeField] private bool showDebugger;
        private SequinePlayer sequine;

#if ODIN_INSPECTOR
        protected override void OnEnable() {
            base.OnEnable();
#else
        private void OnEnable() {
#endif
            sequine = (SequinePlayer)target;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (Application.isPlaying) {
                if (sequine.animatorController != null) {
                    EditorGUILayout.Space();
                    bool guiEnabled = GUI.enabled;
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(new GUIContent("Controller", "The controller which is taken from the Animator"), sequine.animatorController, typeof(RuntimeAnimatorController), false);
                    GUI.enabled = guiEnabled;
                }
            }

            float newWeightValue = EditorGUILayout.Slider(new GUIContent("Weight", "Weight compared to the animator controller."), sequine.weight, 0f, 1f);
            if (newWeightValue != sequine.weight) {
                if (!Application.isPlaying) {
                    Undo.RecordObject(target, "modified weight");
                }
                sequine.weight = newWeightValue;
                if (!Application.isPlaying) {
                    EditorUtility.SetDirty(target);
                }
            }

            EditorGUILayout.Space();
            serializedObject.Update();

            var animationLayers = serializedObject.FindProperty("m_animationLayers");
            animationLayers.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(animationLayers.isExpanded, "Layers");
            if (animationLayers.isExpanded) {
                EditorGUI.indentLevel++;
                if (animationLayers.arraySize == 0) animationLayers.arraySize++;

                bool guiEnabled = GUI.enabled;

                var baseLayer = animationLayers.GetArrayElementAtIndex(0);
                baseLayer.isExpanded = EditorGUILayout.Foldout(baseLayer.isExpanded, "Base Layer", true);
                if (baseLayer.isExpanded) {
                    EditorGUI.indentLevel++;
                    GUI.enabled = false;
                    var baseWeight = baseLayer.FindPropertyRelative("weight");
                    if (baseWeight.floatValue != 1) baseWeight.floatValue = 1;
                    EditorGUILayout.PropertyField(baseWeight);
                    if (!Application.isPlaying)
                        GUI.enabled = guiEnabled;
                    var maskProp = baseLayer.FindPropertyRelative("mask");
                    EditorGUILayout.PropertyField(maskProp);
                    DrawBlending(baseLayer);
                    GUI.enabled = guiEnabled;
                    EditorGUI.indentLevel--;
                }

                for (int i = 1; i < animationLayers.arraySize; i++) {
                    var layerProp = animationLayers.GetArrayElementAtIndex(i);
                    GUILayout.BeginHorizontal();
                    var nameProp = layerProp.FindPropertyRelative("name");
                    string layerName = "Layer " + i;
                    if (nameProp.stringValue != "") {
                        layerName += " - " + nameProp.stringValue;
                    }
                    layerProp.isExpanded = EditorGUILayout.Foldout(layerProp.isExpanded, layerName, true);
                    if (GUILayout.Button("Remove", EditorStyles.miniButtonMid, GUILayout.Width(65))) {
                        animationLayers.DeleteArrayElementAtIndex(i);
                        i--;
                        continue;
                    }
                    GUILayout.EndHorizontal();
                    if (layerProp.isExpanded) {
                        EditorGUI.indentLevel++;
                        var weightProp = layerProp.FindPropertyRelative("weight");
                        var maskProp = layerProp.FindPropertyRelative("mask");
                        if (Application.isPlaying)
                            GUI.enabled = false;
                        EditorGUILayout.PropertyField(nameProp);
                        GUI.enabled = guiEnabled;
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(weightProp);
                        if (EditorGUI.EndChangeCheck()) {
                            if (i < sequine.layers.Count) {
                                sequine.layers[i].weight = weightProp.floatValue;
                            }
                        }
                        if (Application.isPlaying)
                            GUI.enabled = false;
                        EditorGUILayout.PropertyField(maskProp);
                        DrawBlending(layerProp);
                        GUI.enabled = guiEnabled;
                        EditorGUI.indentLevel--;
                    }
                }

                if (GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), "Add Layer", EditorStyles.miniButtonMid)) {
                    animationLayers.arraySize++;
                }

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            HandleDebugger(animationLayers);

            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying) {
                Repaint();
            }

        }

        private static void DrawBlending(SerializedProperty layerProp) {
            var rect = EditorGUILayout.GetControlRect();
            EditorGUI.BeginProperty(rect, new GUIContent("Blending"), layerProp);
            var blendingProp = layerProp.FindPropertyRelative("additive");
            var options = new string[] { "Override", "Additive" };
            int currentIndex = blendingProp.boolValue ? 1 : 0;
            int newIndex = EditorGUI.Popup(rect, "Blending", currentIndex, options);
            if (newIndex != currentIndex) {
                blendingProp.boolValue = newIndex == 0 ? false : true;
            }
            EditorGUI.EndProperty();
        }

        private void HandleDebugger(SerializedProperty _animationLayers) {
            EditorGUILayout.Space();
            if (sequine.layers.Count == 0) return;
            //if (sequine.states.Count() == 0) return;
            var newShowDebugger = EditorGUILayout.BeginFoldoutHeaderGroup(showDebugger, "Debugger");
            if (newShowDebugger != showDebugger) {
                Undo.RecordObject(this, "modify show debugger");
                showDebugger = newShowDebugger;
                EditorUtility.SetDirty(this);
            }

            if (showDebugger) {
                EditorGUI.indentLevel++;
                for (int i = 0; i < sequine.layers.Count; i++) {
                    var layer = sequine.layers[i];
                    var layerProp = _animationLayers.GetArrayElementAtIndex(i);
                    var layerExpanded =layerProp.FindPropertyRelative("editor_isDebuggerExpanded");
                    var nameProp = layerProp.FindPropertyRelative("name");
                    string layerName;
                    if (i == 0) layerName = "Base Layer";
                    else layerName = "Layer " + i;
                    if (i != 0 && nameProp.stringValue != "") {
                        layerName += " - " + nameProp.stringValue;
                    }
                    layerExpanded.boolValue = EditorGUILayout.Foldout(layerExpanded.boolValue, new GUIContent(layerName), true);
                    if (layerExpanded.boolValue) {
                        EditorGUI.indentLevel++;
                        DrawLayer(layer);
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();
        }

        private static void DrawLayer(SequineLayer layer) {
            SequineState stateToRelease = null;

            foreach (var state in layer.states) {
                stateToRelease = DrawStateDebugger(stateToRelease, state);
            }

            GUILayout.Space(5);
            GUILayout.Label("Temporary States", EditorStyles.boldLabel);

            foreach (var state in layer.temporaryStates) {
                stateToRelease = DrawStateDebugger(stateToRelease, state);
            }

            if (stateToRelease != null) {
                layer.RemoveState(stateToRelease.clip);
            }
        }

        private static SequineState DrawStateDebugger(SequineState stateToRelease, SequineState state) {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(state.clip.name, EditorStyles.boldLabel);
                if (GUILayout.Button("Release", GUILayout.Width(100))) {
                    stateToRelease = state;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("Elapsed Time: " + state.time);
            Rect timeRect = GUILayoutUtility.GetLastRect();
            EditorGUI.ProgressBar(timeRect, state.normalizedTime, "Elapsed Time: " + state.time);
            GUILayout.Label("Weight: " + state.weight);
            if (state.completed) {
                GUILayout.Label("Completed");
            }
            if (state.isExiting) {
                GUILayout.Label("Exiting...");
            }
            if (state.exited) {
                GUILayout.Label("Exited");
            }
            GUILayout.Box(GUIContent.none, GUILayout.Height(2), GUILayout.ExpandWidth(true));
            return stateToRelease;
        }
    }

}