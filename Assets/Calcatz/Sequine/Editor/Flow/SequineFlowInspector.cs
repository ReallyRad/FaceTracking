using Calcatz.CookieCutter;
using UnityEditor;
using UnityEngine;

namespace Calcatz.Sequine {

    [CustomEditor(typeof(SequineFlowComponent), true)]
    internal class SequineFlowInspector : Editor {

        private SequineFlowComponent flow;
        private Rect lastRect;

        private void OnEnable() {
            flow = (SequineFlowComponent)target;
        }

        [MenuItem("GameObject/Create Other/Sequine/Sequine Flow Component", false, 0)]
        private static void CreateGameObject() {
            ComponentEditorUtility.CreateGameObject<SequineFlowComponent>("SequineFlow");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            if (lastRect.size == Vector2.zero) {
                lastRect = new Rect(18, 4, EditorGUIUtility.currentViewWidth - 22.8f, EditorGUIUtility.singleLineHeight);
            }

            EditorGUILayout.Space();
            var executeOnStart = serializedObject.FindProperty("m_executeOnStart");
            var m_dontDestroyOnLoad = serializedObject.FindProperty("m_dontDestroyOnLoad");
            EditorGUILayout.PropertyField(executeOnStart);
            EditorGUILayout.PropertyField(m_dontDestroyOnLoad, new GUIContent("Don't Destroy on Load"));

            bool guiEnabled = GUI.enabled;
            GUILayout.Label("");
            Rect rect = GUILayoutUtility.GetLastRect();
            if (rect.width != 1) lastRect = rect;

            GUILayout.BeginHorizontal();
            int buttonWidth = 200;
            GUILayout.Space(lastRect.width / 2 - buttonWidth / 2);
            if (GUILayout.Button("Open in Command Inspector", EditorStyles.miniButtonMid, GUILayout.Width(buttonWidth))) {
                CommandInspectorWindow.OpenWindow();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUI.enabled = guiEnabled && Application.isPlaying;
            GUILayout.BeginHorizontal();
            buttonWidth = 100;
            GUILayout.Space(lastRect.width / 2 - buttonWidth / 2);
            if (GUILayout.Button(new GUIContent("Execute", "Sequine flow can only be executed during runtime (play mode)."), GUILayout.Width(buttonWidth), GUILayout.Height(30))) {
                flow.Execute();
            }
            GUILayout.EndHorizontal();

            GUI.enabled = guiEnabled;
            GUILayout.Label("");

            serializedObject.ApplyModifiedProperties();
        }

    }

}

