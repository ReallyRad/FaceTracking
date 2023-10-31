using UnityEditor;

namespace Calcatz.Sequine {

    [CustomTextBehaviourEditor(typeof(TransformTextBehaviourComponent))]
    public class TransformTextBehaviourEditor {

        private SerializedProperty animatePositionProperty;
        private SerializedProperty animateRotationProperty;
        private SerializedProperty animateScaleProperty;
        private SerializedProperty applyOnXProperty;
        private SerializedProperty applyOnYProperty;
        private SerializedProperty applyOnZProperty;
        private SerializedProperty positionCurveProperty;
        private SerializedProperty positionMultiplierProperty;
        private SerializedProperty rotationCurveProperty;
        private SerializedProperty rotationMultiplierProperty;
        private SerializedProperty scaleCurveProperty;

        /// <summary>
        /// Magic method that is called when drawing Text Behaviour Profile in inspector.
        /// </summary>
        /// <param name="_serializedObject"></param>
        protected virtual void OnTextBehaviourComponentGUI(SerializedObject _serializedObject) {
            animateScaleProperty = _serializedObject.FindProperty("animateScale");
            scaleCurveProperty = _serializedObject.FindProperty("scaleCurve");
            applyOnXProperty = _serializedObject.FindProperty("applyOnX");
            applyOnYProperty = _serializedObject.FindProperty("applyOnY");
            applyOnZProperty = _serializedObject.FindProperty("applyOnZ");

            animatePositionProperty = _serializedObject.FindProperty("animatePosition");
            positionMultiplierProperty = _serializedObject.FindProperty("positionMultiplier");
            positionCurveProperty = _serializedObject.FindProperty("positionCurve");

            animateRotationProperty = _serializedObject.FindProperty("animateRotation");
            rotationCurveProperty = _serializedObject.FindProperty("rotationCurve");
            rotationMultiplierProperty = _serializedObject.FindProperty("rotationMultiplier");

            EditorGUILayout.PropertyField(animateScaleProperty);
            if (animateScaleProperty.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(scaleCurveProperty);

                EditorGUILayout.PropertyField(applyOnXProperty);
                EditorGUILayout.PropertyField(applyOnYProperty);
                EditorGUILayout.PropertyField(applyOnZProperty);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(animatePositionProperty);
            if (animatePositionProperty.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(positionMultiplierProperty);

                EditorGUILayout.PropertyField(positionCurveProperty);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(animateRotationProperty);
            if (animateRotationProperty.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(rotationCurveProperty);

                EditorGUILayout.PropertyField(rotationMultiplierProperty);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }

    }

}