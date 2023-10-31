using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ContentType = Calcatz.CookieCutter.CommandCreatorWindow.CommandLine.ContentType;
using PointLocation = Calcatz.CookieCutter.CommandCreatorWindow.CommandLine.PointLocation;
using PointType = Calcatz.CookieCutter.CommandCreatorWindow.CommandLine.PointType;
using FieldType = Calcatz.CookieCutter.CommandCreatorWindow.CommandLine.FieldType;
using System;

namespace Calcatz.CookieCutter {

    [CustomPropertyDrawer(typeof(CommandCreatorWindow.CommandLine), true)]
    public class CommandLinePropertyDrawer : PropertyDrawer {

        /// <summary>
        /// This will define the height of the element.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            var contentTypeProp = property.FindPropertyRelative("contentType");
            var pointLocationProp = property.FindPropertyRelative("pointLocation");
            var fieldTypeProp = property.FindPropertyRelative("fieldType");
            var useMin = property.FindPropertyRelative("useMin").boolValue;
            var useMax = property.FindPropertyRelative("useMax").boolValue;

            ContentType contentType = (ContentType)ContentType.GetValues(typeof(ContentType)).GetValue(contentTypeProp.enumValueIndex);
            FieldType fieldType = (FieldType)FieldType.GetValues(typeof(FieldType)).GetValue(fieldTypeProp.enumValueIndex);

            float height = 0;
            if (contentType == ContentType.None) {
                height += EditorGUIUtility.singleLineHeight * 4 + 10;
            }
            else if (contentType == ContentType.InputField) {
                height += EditorGUIUtility.singleLineHeight * 8 + 10;
            }
            else {
                height += EditorGUIUtility.singleLineHeight * 9 + 10;
            }

            PointLocation pointLocation = (PointLocation)PointLocation.GetValues(typeof(PointLocation)).GetValue(pointLocationProp.enumValueIndex);
            if (contentType != ContentType.InputField && pointLocation != PointLocation.None) {
                height += EditorGUIUtility.singleLineHeight;

                var pointTypeProp = property.FindPropertyRelative("pointType");
                PointType pointType = (PointType)PointType.GetValues(typeof(PointType)).GetValue(pointTypeProp.enumValueIndex);
                if (pointLocation == PointLocation.Right && pointType == PointType.Main) {
                    height += EditorGUIUtility.singleLineHeight;
                }
            }

            if (contentType == ContentType.InputField) {
                switch(fieldType) {
                    case FieldType.Text:
                        height += EditorGUIUtility.singleLineHeight;
                        break;
                    case FieldType.TextArea:
                        height += EditorGUIUtility.singleLineHeight*3;
                        break;
                    case FieldType.RoundedFloat:
                        height += EditorGUIUtility.singleLineHeight*3;
                        if (useMin) {
                            height += EditorGUIUtility.singleLineHeight;
                        }
                        if (useMax) {
                            height += EditorGUIUtility.singleLineHeight;
                        }
                        break;
                    case FieldType.Float:
                        height += EditorGUIUtility.singleLineHeight * 3;
                        if (useMin) {
                            height += EditorGUIUtility.singleLineHeight;
                        }
                        if (useMax) {
                            height += EditorGUIUtility.singleLineHeight;
                        }
                        break;
                    case FieldType.FloatSlider:
                        height += EditorGUIUtility.singleLineHeight * 3;
                        break;
                    case FieldType.Int:
                        height += EditorGUIUtility.singleLineHeight * 3;
                        if (useMin) {
                            height += EditorGUIUtility.singleLineHeight;
                        }
                        if (useMax) {
                            height += EditorGUIUtility.singleLineHeight;
                        }
                        break;
                    case FieldType.IntSlider:
                        height += EditorGUIUtility.singleLineHeight * 3;
                        break;
                    case FieldType.Toggle:
                        height += EditorGUIUtility.singleLineHeight;
                        break;
                    case FieldType.ToggleLeft:
                        height += EditorGUIUtility.singleLineHeight;
                        break;
                    case FieldType.Object:
                        height += EditorGUIUtility.singleLineHeight*2;
                        break;
                    case FieldType.Color:
                        height += EditorGUIUtility.singleLineHeight;
                        break;
                    case FieldType.Curve:
                        height += EditorGUIUtility.singleLineHeight;
                        break;
                    case FieldType.Enum:
                        height += EditorGUIUtility.singleLineHeight;
                        break;
                    case FieldType.Vector3:
                        height += EditorGUIUtility.singleLineHeight;
                        break;
                    case FieldType.Vector3Int:
                        height += EditorGUIUtility.singleLineHeight;
                        break;
                    case FieldType.Vector2:
                        height += EditorGUIUtility.singleLineHeight;
                        break;
                    case FieldType.Vector2Int:
                        height += EditorGUIUtility.singleLineHeight;
                        break;
                    case FieldType.Vector4:
                        height += EditorGUIUtility.singleLineHeight;
                        break;
                    case FieldType.Rect:
                        height += EditorGUIUtility.singleLineHeight;
                        break;
                    case FieldType.RectInt:
                        height += EditorGUIUtility.singleLineHeight;
                        break;
                    case FieldType.Popup:
                        height += EditorGUIUtility.singleLineHeight;
                        break;
                    default:
                        Debug.LogError("Field type not implemented: " + fieldType);
                        break;
                }
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;

            EditorGUI.BeginProperty(position, label, property);

            float singleLineHeight = EditorGUIUtility.singleLineHeight;

            var labelProp = property.FindPropertyRelative("label");
            var contentTypeProp = property.FindPropertyRelative("contentType");
            var fieldTypeProp = property.FindPropertyRelative("fieldType");

            ContentType contentType = (ContentType)ContentType.GetValues(typeof(ContentType)).GetValue(contentTypeProp.enumValueIndex);

            var currentRect = new Rect(position.x, position.y, position.width, singleLineHeight);

            string lineName = label.text.Replace("Element ", "") + ": ";
            if (contentType != ContentType.None) {
                if (labelProp.stringValue != null && labelProp.stringValue != "") {
                    lineName += labelProp.stringValue;
                }
                else lineName += "(Unlabeled)";
            }
            else lineName += "(None)";
            var evt = Event.current;
            if (evt.type == EventType.MouseDown && evt.button == 0) {
                if (currentRect.Contains(evt.mousePosition)) {
                    property.isExpanded = !property.isExpanded;
                    evt.Use();
                    GUI.changed = true;
                }
            }
            property.isExpanded = EditorGUI.Foldout(currentRect, property.isExpanded, lineName);
            if (property.isExpanded) {
                currentRect.y += singleLineHeight + 5;

                EditorGUI.PropertyField(currentRect, contentTypeProp);
                currentRect.y += singleLineHeight;

                if (contentType != ContentType.None) {
                    EditorGUI.PropertyField(currentRect, labelProp);
                    currentRect.y += singleLineHeight;

                    if (contentType == ContentType.Label) {
                        EditorGUI.indentLevel++;
                        EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("alignRight"));
                        currentRect.y += singleLineHeight;
                        EditorGUI.indentLevel--;
                    }

                    var tooltipRect = currentRect;
                    tooltipRect.height *= 3;
                    EditorGUI.PropertyField(tooltipRect, property.FindPropertyRelative("tooltip"));
                    currentRect.y += singleLineHeight * 3;
                }

                var pointLocationProp = property.FindPropertyRelative("pointLocation");
                PointLocation pointLocation = (PointLocation)PointLocation.GetValues(typeof(PointLocation)).GetValue(pointLocationProp.enumValueIndex);

                EditorGUI.PropertyField(currentRect, pointLocationProp);
                if (contentType == ContentType.InputField && pointLocation == PointLocation.Right) {
                    pointLocationProp.enumValueIndex = 0;
                }
                currentRect.y += singleLineHeight;

                if (contentType != ContentType.InputField && pointLocation != PointLocation.None) {
                    EditorGUI.indentLevel++;

                    var pointTypeProp = property.FindPropertyRelative("pointType");
                    EditorGUI.PropertyField(currentRect, pointTypeProp);
                    currentRect.y += singleLineHeight;

                    PointType pointType = (PointType)PointType.GetValues(typeof(PointType)).GetValue(pointTypeProp.enumValueIndex);
                    if (pointLocation == PointLocation.Right && pointType == PointType.Main) {
                        EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("outMainType"));
                        currentRect.y += singleLineHeight;
                    }

                    EditorGUI.indentLevel--;
                }

                if (contentType == ContentType.InputField) {
                    EditorGUI.PropertyField(currentRect, fieldTypeProp);
                    currentRect.y += singleLineHeight;
                    FieldType fieldType = (FieldType)FieldType.GetValues(typeof(FieldType)).GetValue(fieldTypeProp.enumValueIndex);
                    currentRect = HandleDrawInputField(property, singleLineHeight, fieldType, currentRect);
                }

                EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("spacing"));
                currentRect.y += singleLineHeight;
            }
            EditorGUI.EndProperty();

            EditorGUIUtility.labelWidth = labelWidth;
        }

        private static Rect HandleDrawInputField(SerializedProperty property, float singleLineHeight, FieldType fieldType, Rect currentRect) {
            var useMin = property.FindPropertyRelative("useMin").boolValue;
            var useMax = property.FindPropertyRelative("useMax").boolValue;

            EditorGUI.indentLevel++;
            if (fieldType == FieldType.TextArea) {
                EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("lineCount"));
                currentRect.y += singleLineHeight;
                EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("wordWrap"));
                currentRect.y += singleLineHeight;
            }
            else if (fieldType == FieldType.Float) {
                currentRect = DrawOptionalMinMax(property, singleLineHeight, currentRect, useMin, useMax);
            }
            else if (fieldType == FieldType.RoundedFloat) {
                currentRect = DrawOptionalMinMax(property, singleLineHeight, currentRect, useMin, useMax);
            }
            else if (fieldType == FieldType.FloatSlider) {
                currentRect = DrawMandatoryMinMax(property, singleLineHeight, currentRect);
            }
            else if (fieldType == FieldType.Int) {
                currentRect = DrawOptionalMinMax(property, singleLineHeight, currentRect, useMin, useMax);
            }
            else if (fieldType == FieldType.IntSlider) {
                currentRect = DrawMandatoryMinMax(property, singleLineHeight, currentRect);
            }
            else if (fieldType == FieldType.Object) {
                var allowSceneObjectsProp = property.FindPropertyRelative("allowSceneObjects");
                allowSceneObjectsProp.boolValue = EditorGUI.ToggleLeft(currentRect, "Allow Scene Objects", allowSceneObjectsProp.boolValue);
                currentRect.y += EditorGUIUtility.singleLineHeight;
            }
            EditorGUI.indentLevel--;

            return currentRect;
        }

        private static Rect DrawMandatoryMinMax(SerializedProperty property, float singleLineHeight, Rect currentRect) {
            EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("min"));
            currentRect.y += singleLineHeight;
            EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("max"));
            currentRect.y += singleLineHeight;
            return currentRect;
        }

        private static Rect DrawOptionalMinMax(SerializedProperty property, float singleLineHeight, Rect currentRect, bool useMin, bool useMax) {
            EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("useMin"));
            currentRect.y += singleLineHeight;
            if (useMin) {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("min"));
                currentRect.y += singleLineHeight;
                EditorGUI.indentLevel--;
            }
            EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("useMax"));
            currentRect.y += singleLineHeight;
            if (useMax) {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("max"));
                currentRect.y += singleLineHeight;
                EditorGUI.indentLevel--;
            }

            return currentRect;
        }

    }
}