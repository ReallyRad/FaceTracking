using UnityEditor;
using UnityEngine;

namespace Calcatz.Sequine {
    [CustomEditor(typeof(SequineText), true)]
#if ODIN_INSPECTOR
    internal sealed class SequineTextEditor : Sirenix.OdinInspector.Editor.OdinEditor {
#else
    internal sealed class SequineTextEditor : Editor {
#endif
        private SequineText sequineText;

        public override bool RequiresConstantRepaint() {
            if (Application.isPlaying) return true;
            return base.RequiresConstantRepaint();
        }

#if ODIN_INSPECTOR
        protected override void OnEnable() {
            base.OnEnable();
#else
        private void OnEnable() {
#endif

            sequineText = (SequineText)target;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (!float.IsNaN(sequineText.normalizedTime)) {
                EditorGUI.BeginChangeCheck();
                var normalizedTime = EditorGUILayout.Slider(sequineText.normalizedTime, 0, 1);
                if (EditorGUI.EndChangeCheck()) {
                    sequineText.normalizedTime = normalizedTime;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

    }
}