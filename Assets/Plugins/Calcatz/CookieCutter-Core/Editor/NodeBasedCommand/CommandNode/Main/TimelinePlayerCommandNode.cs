using UnityEditor;
using UnityEngine;
#if TIMELINE_AVAILABLE
using UnityEngine.Timeline;
using UnityEngine.Playables;
#endif

namespace Calcatz.CookieCutter {

    [CustomCommandNodeDrawer(typeof(TimelinePlayerCommand))]
    public class TimelinePlayerCommandNode : CommandNode {

        public TimelinePlayerCommandNode(CommandData _commandData, Command _command, Vector2 _position, Config _nodeConfig)
            : base(_commandData, _command, _position, 275, 50, _nodeConfig) {
            nodeName = "Timeline Player";
        }
#if TIMELINE_AVAILABLE
        protected override void OnDrawContents(Vector2 _absolutePosition) {
            float fieldHeight = EditorGUIUtility.singleLineHeight;
            TimelinePlayerCommand timelinePlayerCommand = (TimelinePlayerCommand)GetCommand();

            EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, 64, fieldHeight), "Control", styles.label);
            var newControl = (TimelinePlayerCommand.Control)EditorGUI.EnumPopup(new Rect(_absolutePosition.x + 10 + 64, currentY, rect.width - 64 - 20, fieldHeight), timelinePlayerCommand.control);
            if (newControl != timelinePlayerCommand.control) {
                Undo.RecordObject(commandData.targetObject, "modify timeline control");
                timelinePlayerCommand.control = newControl;
                EditorUtility.SetDirty(commandData.targetObject);
            }
            AddRectHeight(fieldHeight);

            EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, 64, fieldHeight), new GUIContent("Asset", CommandGUI.binderIconTexture), styles.label);
            var assetRect = new Rect(_absolutePosition.x + 10 + 64, currentY, rect.width - 64 - 20, fieldHeight);
            DropBinderAreaGUI<TimelineAsset>(assetRect, _newAsset => {
                Undo.RecordObject(commandData.targetObject, "modify timeline asset");
                timelinePlayerCommand.asset = _newAsset;
                EditorUtility.SetDirty(commandData.targetObject);
            });
            TimelineAsset newAsset = (TimelineAsset)EditorGUI.ObjectField(assetRect, timelinePlayerCommand.asset, typeof(TimelineAsset), false);
            if (newAsset != timelinePlayerCommand.asset) {
                Undo.RecordObject(commandData.targetObject, "modify timeline asset");
                timelinePlayerCommand.asset = newAsset;
                EditorUtility.SetDirty(commandData.targetObject);
            }
            AddRectHeight(fieldHeight);

            if (timelinePlayerCommand.control == TimelinePlayerCommand.Control.Stop) {
                EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, 150, fieldHeight), "Deactivate GameObject", styles.label);
                bool newDeactivate = EditorGUI.Toggle(new Rect(_absolutePosition.x + 10 + 150, currentY, rect.width - 150 - 20, fieldHeight), timelinePlayerCommand.deactivateGameObject);
                if (newDeactivate != timelinePlayerCommand.deactivateGameObject) {
                    Undo.RecordObject(commandData.targetObject, "modify timeline deactivate");
                    timelinePlayerCommand.deactivateGameObject = newDeactivate;
                    EditorUtility.SetDirty(commandData.targetObject);
                }
            }
            else {
                EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, 64, fieldHeight), "Async", styles.label);
                bool newAsync = EditorGUI.Toggle(new Rect(_absolutePosition.x + 10 + 64, currentY, rect.width - 64 - 20, fieldHeight), timelinePlayerCommand.asynchronous);
                if (newAsync != timelinePlayerCommand.asynchronous) {
                    Undo.RecordObject(commandData.targetObject, "modify timeline async");
                    timelinePlayerCommand.asynchronous = newAsync;
                    EditorUtility.SetDirty(commandData.targetObject);
                }
            }
            AddRectHeight(fieldHeight);
        }
#endif

    }
}
