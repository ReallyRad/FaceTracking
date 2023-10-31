using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {

    [CustomCommandNodeDrawer(typeof(CrossSceneComponentCommand))]
    public class CrossSceneComponentCommandNode : CommandNode {

        public CrossSceneComponentCommandNode(CommandData _commandData, Command _command, Vector2 position, Config config)
            : base(_commandData, _command, position, 275, 50, config) {
            nodeName = "Get Cross Scene Component";

            //asset
            AddPropertyInPoint<UnityEngine.Object>(config);

            //component
            AddPropertyOutPoint<UnityEngine.Object>(config);
            //position
            AddPropertyOutPoint<Vector3>(config);
            //rotation
            AddPropertyOutPoint<Vector3>(config);
            //scale
            AddPropertyOutPoint<Vector3>(config);
        }

        protected override void HandleFirstInPointCreation(Config _config) { }

        protected override void HandleFirstOutPointCreation(Config _config) { }

        protected override void OnDrawTitle(Vector2 _absolutePosition) { }

        protected override void OnDrawContents(Vector2 _absolutePosition) {
            float fieldHeight = EditorGUIUtility.singleLineHeight;
            var crossSceneCompCommand = (CrossSceneComponentCommand)GetCommand();

            DrawInPoint(_absolutePosition, 0, currentY);

            EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, 100, fieldHeight), new GUIContent("Binder Asset", CommandGUI.binderIconTexture), styles.label);
            if (crossSceneCompCommand.inputIds[0].targetId == 0) {
                var assetRect = new Rect(_absolutePosition.x + 10 + 100, currentY, rect.width - 100 - 20, fieldHeight);
                DropBinderAreaGUI<UnityEngine.Object>(assetRect, _newAsset => {
                    Undo.RecordObject(commandData.targetObject, "modify cross scene binder asset");
                    crossSceneCompCommand.defaultAsset = _newAsset;
                    EditorUtility.SetDirty(commandData.targetObject);
                });
                var newAsset = EditorGUI.ObjectField(assetRect, crossSceneCompCommand.defaultAsset, typeof(UnityEngine.Object), false);
                if (newAsset != crossSceneCompCommand.defaultAsset) {
                    Undo.RecordObject(commandData.targetObject, "modify cross scene binder asset");
                    crossSceneCompCommand.defaultAsset = newAsset;
                    EditorUtility.SetDirty(commandData.targetObject);
                }
            }
            AddRectHeight(fieldHeight + verticalSpacing);


            DrawOutPoint(_absolutePosition, 0, currentY);
            EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, fieldHeight), "Component", styles.labelRight);
            AddRectHeight(fieldHeight + verticalSpacing);

            bool newShowTransform = EditorGUI.BeginFoldoutHeaderGroup(new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, fieldHeight), crossSceneCompCommand.showTransform, "Transform");
            if (crossSceneCompCommand.showTransform != newShowTransform) {
                Undo.RecordObject(commandData.targetObject, "modify show transform");
                crossSceneCompCommand.showTransform = newShowTransform;
                EditorUtility.SetDirty(commandData.targetObject);
            }
            AddRectHeight(fieldHeight + verticalSpacing);

            if (crossSceneCompCommand.showTransform || crossSceneCompCommand.nextIds[1].Count > 0) {
                DrawOutPoint(_absolutePosition, 1, currentY);
                EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, fieldHeight), "Position", styles.labelRight);
                AddRectHeight(fieldHeight);
            }

            if (crossSceneCompCommand.showTransform || crossSceneCompCommand.nextIds[2].Count > 0) {
                DrawOutPoint(_absolutePosition, 2, currentY);
                EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, fieldHeight), "Rotation", styles.labelRight);
                AddRectHeight(fieldHeight);
            }

            if (crossSceneCompCommand.showTransform || crossSceneCompCommand.nextIds[3].Count > 0) {
                DrawOutPoint(_absolutePosition, 3, currentY);
                EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, fieldHeight), "Local Scale", styles.labelRight);
                AddRectHeight(fieldHeight);
            }

            EditorGUI.EndFoldoutHeaderGroup();
        }

    }

}