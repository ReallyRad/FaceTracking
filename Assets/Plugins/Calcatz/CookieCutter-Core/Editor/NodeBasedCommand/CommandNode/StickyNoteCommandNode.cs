using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {
    [CustomCommandNodeDrawer(typeof(StickyNoteCommand))]
    public class StickyNoteCommandNode : CommandNode {

        private bool isResizerDragged = false;
        private Vector2 prevSize;
        private Vector2 deltaResize;

        public StickyNoteCommandNode(CommandData _commandData, Command _command, Vector2 position, Config config)
            : base(_commandData, _command, position, 200, 100, config) {

            nodeName = "Sticky Note";
            StickyNoteCommand stickyNote = (StickyNoteCommand)GetCommand();
            rect.size = stickyNote.size;
        }

        public override void Draw(Vector2 _offset) {
            Vector2 pos = rect.position + _offset;
            StickyNoteCommand stickyNote = (StickyNoteCommand)GetCommand();
            Color col = GUI.color;
            GUI.color = stickyNote.color;
            GUI.DrawTexture(new Rect(pos, rect.size), EditorGUIUtility.whiteTexture);
            GUI.color = col;

            currentY = pos.y + verticalSpacing;
            float height = EditorGUIUtility.singleLineHeight;
            rect.height = height;

            Rect labelRect = new Rect(pos.x + 5, currentY, rect.width - 10 - height*2, height);
            //GUI.Label(labelRect, nodeName, styles.name);
            Color newColor = EditorGUI.ColorField(new Rect(labelRect.x + labelRect.width, currentY, height*2, height), stickyNote.color);
            if (newColor != stickyNote.color) {
                Undo.RecordObject(commandData.targetObject, "Modified sticky note color");
                stickyNote.color = newColor;
                EditorUtility.SetDirty(commandData.targetObject);
            }
            AddRectHeight(height);

            float notesHeight = stickyNote.size.y - (currentY - pos.y);
            col = GUI.color;
            GUI.color = new Color(col.r, col.g, col.b, 0.85f);
            EditorGUI.BeginChangeCheck();
            string newNotes = EditorGUI.TextArea(new Rect(pos.x + 5, currentY, rect.width - 10, notesHeight), stickyNote.notes, styles.wrappedTextArea);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(commandData.targetObject, "Modified sticky notes");
                stickyNote.notes = newNotes;
                EditorUtility.SetDirty(commandData.targetObject);
            }
            GUI.color = col;
            AddRectHeight(notesHeight-verticalSpacing, false);

            ProcessEvents(Event.current, pos);
        }

        private void ProcessEvents(Event e, Vector3 _pos) {
            StickyNoteCommand stickyNote = (StickyNoteCommand)GetCommand();
            Rect cursorRect = new Rect(new Vector2(_pos.x + rect.width - 10, _pos.y + rect.height - 10), new Vector2(10, 10));

            Color col = GUI.color;
            GUI.color = new Color(col.r, col.g, col.b, 0.3f);
            GUI.Box(cursorRect, GUIContent.none, EditorStyles.helpBox);
            GUI.color = col;

            EditorGUIUtility.AddCursorRect(cursorRect, MouseCursor.ResizeUpLeft);

            switch (e.type) {
                case EventType.MouseDown:
                    if (e.button == 0) {
                        if (cursorRect.Contains(e.mousePosition)) {
                            prevSize = stickyNote.size;
                            deltaResize.Set(0, 0);
                            isResizerDragged = true;
                            GUI.changed = true;
                            e.Use();
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (isResizerDragged) {
                        stickyNote.size = prevSize;
                        Undo.RecordObject(commandData.targetObject, "Resized sticky note");
                        stickyNote.size += deltaResize;
                        EditorUtility.SetDirty(commandData.targetObject);
                    }
                    isResizerDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isResizerDragged) {
                        rect.size += e.delta;
                        stickyNote.size += e.delta;
                        if (rect.width < 50) { rect.width = 50; stickyNote.size.x = 50; }
                        if (rect.height < 50) { rect.height = 50; stickyNote.size.y = 50; }
                        deltaResize = stickyNote.size - prevSize;
                        e.Use();
                    }
                    break;

            }
        }
    }
}