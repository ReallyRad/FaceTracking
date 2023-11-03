using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.CookieCutter {

    [CustomPropertyDrawer(typeof(ShowCommandNodesAttribute), true)]
    public class CommandDataPropertyDrawer : PropertyDrawer {

        private const float resizeHandleHeight = 5;

        private CommandData commandData;
        private CommandNodesContainer nodesContainer;
        private bool showCommandInspectorButton;
        private float height = 0;
        private bool isDraggingResizeHandle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            SerializedProperty targetObjectProperty = property.FindPropertyRelative("m_targetObject");
            if (targetObjectProperty == null || ReferenceEquals(targetObjectProperty.objectReferenceValue, null)) {
                return;
            }

            if (targetObjectProperty.objectReferenceValue is ICommandData commandDataContainer) {

                FieldInfo commandDataField = commandDataContainer.GetType().GetField(property.propertyPath, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                CommandData newCommandData = commandDataField.GetValue(commandDataContainer) as CommandData;

                if (commandData != newCommandData) {
                    commandData = newCommandData;
                    if (commandData == null) {
                        nodesContainer = null;
                    }
                    else {
                        nodesContainer = CommandNodesContainer.CreateFromCommandData(commandData);
                        if (height == 0) {
                            var attribute = commandDataField.GetCustomAttribute<ShowCommandNodesAttribute>();
                            showCommandInspectorButton = attribute.showCommandInspectorButton;
                            height = attribute.height;
                            if (height == 0) height = 450;
                        }
                    }
                }
            }

            if (nodesContainer != null) {
                Rect fullscreenButton = new Rect(position.x + position.width - 175, position.y, 175, 20);
                if (showCommandInspectorButton && GUI.Button(fullscreenButton, "Open in Command Inspector", EditorStyles.miniButton)) {
                    CommandInspectorWindow.OpenWindow();
                }

                EditorGUI.BeginProperty(position, label, property);
                GUI.Box(position, GUIContent.none, EditorStyles.helpBox);
                position.height = height - resizeHandleHeight;
                nodesContainer.containerAreaGetter = () => {
                    return position;
                };
                nodesContainer.OnDrawNodesArea();
                EditorGUI.EndProperty();

                DrawResizeHandle(position);
            }
        }

        private void DrawResizeHandle(Rect position) {
            Rect resizeHandleRect = new Rect(position.x, position.y + position.height, position.width, resizeHandleHeight);
            GUI.Box(resizeHandleRect, GUIContent.none, EditorStyles.helpBox);

            EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeVertical);

            Event evnt = Event.current;

            if (isDraggingResizeHandle) {
                height = evnt.mousePosition.y - position.y;
            }

            if (evnt.type == EventType.MouseDown) {
                if (resizeHandleRect.Contains(evnt.mousePosition)) {
                    isDraggingResizeHandle = true;
                    evnt.Use();
                }
            }
            else if (evnt.type == EventType.MouseUp || evnt.type == EventType.Used) {
                isDraggingResizeHandle = false;
                evnt.Use();
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return height;
        }

    }

}
