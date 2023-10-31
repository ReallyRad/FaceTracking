using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Calcatz.Sequine {

    [CustomEditor(typeof(SequineFlowExecutor))]
#if ODIN_INSPECTOR
    internal class SequineFlowExecutorEditor : Sirenix.OdinInspector.Editor.OdinEditor {
#else
    internal class SequineFlowExecutorEditor : Editor {
#endif

        [MenuItem("GameObject/Create Other/Sequine/Sequine Flow Executor", false, 0)]
        private static void CreateGameObject() {
            ComponentEditorUtility.CreateGameObject<SequineFlowExecutor>("SequineFlowExecutor");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            var executeOnStart = serializedObject.FindProperty("m_executeOnStart");
            var flowToExecute = serializedObject.FindProperty("m_flowToExecute");
            var m_dontDestroyOnLoad = serializedObject.FindProperty("m_dontDestroyOnLoad");

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(executeOnStart);
            if (executeOnStart.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(flowToExecute);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(m_dontDestroyOnLoad, new GUIContent("Don't Destroy on Load"));

            EditorGUILayout.Space();
            bool guiEnabled = GUI.enabled;
            var flowAsset = flowToExecute.objectReferenceValue as SequineFlowAsset;
            GUI.enabled = guiEnabled && Application.isPlaying && flowAsset != null;
            if (GUILayout.Button(new GUIContent("Execute", "Can only execute during Play Mode."))) {
                if (flowToExecute.objectReferenceValue)
                ((SequineFlowExecutor)target).Execute(flowAsset);
            }

            serializedObject.ApplyModifiedProperties();
        }


    }
}