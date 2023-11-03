using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.CookieCutter {

    public class ScrollWheelSearchPopup : PopupWindowContent {
        
        public static void Show(GenericMenu _genericMenu, Vector2 _mousePosition, KeyCode _keyCode, List<string> _starredPaths = null, List<string> _excludedNames = null) {
            ScrollWheelSearchPopup popup = new ScrollWheelSearchPopup(EditorWindow.focusedWindow);
            popup.GenerateMenuItem(_genericMenu, _keyCode.ToString().ToCharArray()[0], _starredPaths, _excludedNames);
            if (popup.content.items.Count == 0) return;
            if (selectionIndex >= popup.content.items.Count) selectionIndex = 0;
            popup.UpdateIndexRange();
            UnityEditor.PopupWindow.Show(new Rect(_mousePosition.x, _mousePosition.y, 0, 0), popup);
        }
        public void GenerateMenuItem(GenericMenu p_menu, char keyCode, List<string> _starredPaths, List<string> _excludedNames) {
            content = new Content();
            if (p_menu == null)
                return;

            //var menuItemsField = p_menu.GetType().GetField("menuItems", BindingFlags.Instance | BindingFlags.NonPublic);
            //var menuItems = menuItemsField.GetValue(p_menu) as ArrayList;

            var menuItemsField = p_menu.GetType().GetField("menuItems", BindingFlags.Instance | BindingFlags.NonPublic);
            IList menuItems;

            if (menuItemsField == null) {   //Unity 2021.2
                menuItemsField = p_menu.GetType().GetField("m_MenuItems", BindingFlags.Instance | BindingFlags.NonPublic);
                menuItems = menuItemsField.GetValue(p_menu) as IList;
            }

            else { //Older Unity Versions 
                menuItems = menuItemsField.GetValue(p_menu) as ArrayList;
            }

            foreach (var menuItem in menuItems) {
                var menuItemType = menuItem.GetType();
                GUIContent guiContent = (GUIContent)menuItemType.GetField("content").GetValue(menuItem);

                bool separator = (bool)menuItemType.GetField("separator").GetValue(menuItem);
                string path = guiContent.text;
                string[] splitPath = path.Split('/');
                string name = splitPath[splitPath.Length - 1];

                if (!separator && name.Length > 0 && name[0] == keyCode) {
                    if (_excludedNames != null && _excludedNames.Contains(name)) {
                        continue;
                    }
                    Content.ContentItem item = new Content.ContentItem();
                    item.text = splitPath[splitPath.Length - 1];
                    item.func1 = (GenericMenu.MenuFunction)menuItemType.GetField("func").GetValue(menuItem);
                    item.func2 = (GenericMenu.MenuFunction2)menuItemType.GetField("func2").GetValue(menuItem);
                    item.userData = menuItemType.GetField("userData").GetValue(menuItem);
                    content.items.Add(item);
                }
            }

            content.items.Sort((_a, _b) => _a.text.CompareTo(_b.text));
            if (_starredPaths != null) {
                List<string> starredNames = new List<string>();
                foreach (string path in _starredPaths) {
                    string[] splits = path.Split('/');
                    starredNames.Add(splits[splits.Length - 1]);
                }
                for (int i = 0; i < content.items.Count; i++) {
                    if (starredNames.Contains(content.items[i].text)) {
                        selectionIndex = i;
                        break;
                    }
                }
            }

        }

        private Content content;
        private Vector2 offset;
        private Vector2 targetPosition;
        private static int selectionIndex;
        private int selectedRowIndex;
        private float elementHeight = 30;
        private EditorWindow caller;

        private int minRange;
        private int maxRange;

        public ScrollWheelSearchPopup(EditorWindow _caller) {
            caller = _caller;
        }

        public override Vector2 GetWindowSize() {
            float maxHeight = 21 * elementHeight + elementHeight * 2;
            float height = elementHeight * content.items.Count + elementHeight * 2;
            return new Vector2(200, Mathf.Min(height, maxHeight));
        }

        public override void OnOpen() {
            base.OnOpen();
            //UpdateTargetPosition(Event.current.mousePosition);
            //UpdatePosition();
        }

        private bool skipFirstFrame = false;
        public override void OnGUI(Rect _rect) {
            if (!skipFirstFrame) {
                skipFirstFrame = true;
                return;
            }
            editorWindow.wantsMouseMove = true;
            editorWindow.wantsMouseEnterLeaveWindow = true;
            editorWindow.maxSize = GetWindowSize();
            Event e = Event.current;
            if (e.type == EventType.KeyDown) {
                if (e.keyCode == KeyCode.DownArrow) {
                    selectionIndex++;
                    if (selectionIndex >= content.items.Count) selectionIndex = content.items.Count - 1;
                }
                else if (e.keyCode == KeyCode.UpArrow) {
                    selectionIndex--;
                    if (selectionIndex < 0) selectionIndex = 0;
                }
                UpdateIndexRange();
                UpdateTargetPosition(e.mousePosition);
                if (selectedRowIndex != selectionIndex) {
                    editorWindow.Repaint();
                }
            }
            if (e.type == EventType.ScrollWheel) {
                if (e.delta.y > 0) {
                    selectionIndex++;
                    if (selectionIndex >= content.items.Count) selectionIndex = content.items.Count - 1;
                }
                else {
                    selectionIndex--;
                    if (selectionIndex < 0) selectionIndex = 0;
                }
                UpdateIndexRange();
                UpdateTargetPosition(e.mousePosition);
                if (selectedRowIndex != selectionIndex) {
                    editorWindow.Repaint();
                }
            }
            if (e.type == EventType.MouseMove || e.type == EventType.Repaint) {
                UpdateTargetPosition(e.mousePosition);
            }
            if (e.type == EventType.KeyUp) {
                if (e.keyCode != KeyCode.DownArrow && e.keyCode != KeyCode.UpArrow &&
                    e.keyCode != KeyCode.LeftArrow && e.keyCode != KeyCode.RightArrow) {
                    editorWindow.Close();
                    if (EditorWindow.focusedWindow != caller) {
                        caller.Focus();
                    }
                    //caller.Show();
                }
            }
            UpdatePosition();

            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            switch (e.GetTypeForControl(controlId)) {

                case EventType.MouseMove:
                    UpdatePosition();
                    GUIUtility.hotControl = controlId;
                    break;

            }

            DrawItems(e);

        }

        private void DrawItems(Event _e) {
            int count = UpdateIndexRange();
            for (int i = 0; i <= count; i++) {
                Color prevColor = GUI.color;
                GUI.color = new Color(prevColor.r, prevColor.g, prevColor.b, i == selectedRowIndex? 1f : 0.5f);
                {
                    Content.ContentItem item = content.items[i + minRange];
                    Rect rect = new Rect(4, elementHeight * (i + 1), editorWindow.position.width - 8, elementHeight);
                    Rect textRect = new Rect(8, elementHeight * (i + 1) + 5, editorWindow.position.width - 16, elementHeight - 10);
                    GUI.Box(rect, GUIContent.none, (GUIStyle)"TE NodeBox");
                    GUI.Label(textRect, new GUIContent(item.text));
                }
                GUI.color = prevColor;
            }
            if (_e.type == EventType.MouseDown) {
                if (_e.button == 0 /*&& rect.Contains(_e.mousePosition)*/) {
                    Submit();
                    return;
                }
            }
            if (_e.type == EventType.KeyDown) {
                if (_e.keyCode == KeyCode.Return) {
                    Submit();
                }
            }
        }

        private void Submit() {
            Content.ContentItem item = content.items[selectionIndex];
            if (item.func1 != null) {
                item.func1.Invoke();
            }
            else {
                item.func2?.Invoke(item.userData);
            }
            editorWindow.Close();
        }

        private int UpdateIndexRange() {
            minRange = selectionIndex - 10;
            if (minRange < 0) minRange = 0;
            maxRange = selectionIndex + 10;
            if (selectionIndex - minRange < 10) {
                maxRange += 10 - selectionIndex;
            }
            if (maxRange >= content.items.Count) {
                maxRange = content.items.Count - 1;
            }
            int count = maxRange - minRange;
            selectedRowIndex = selectionIndex - minRange;
            return count;
        }

        private void UpdateTargetPosition(Vector2 _mousePosition) {
            Vector2 mousePosition = GUIUtility.GUIToScreenPoint(_mousePosition);
            mousePosition += offset;
            targetPosition.x = mousePosition.x - editorWindow.position.width / 2;
            targetPosition.y = mousePosition.y - ((selectedRowIndex+1) * elementHeight + elementHeight / 2f); //Mathf.Lerp(targetPosition.y, mousePosition.y - (selectionIndex * elementHeight + elementHeight/2f), 0.01f);
        }

        private void UpdatePosition() {
            Rect pos = editorWindow.position;
            pos.x = targetPosition.x;
            pos.y = targetPosition.y;
            editorWindow.position = pos;
        }

        public class Content {

            public class ContentItem {
                public string text;
                public Texture image;
                public GenericMenu.MenuFunction func1;
                public GenericMenu.MenuFunction2 func2;
                public object userData;
            }

            public List<ContentItem> items = new List<ContentItem>();
        }
    }
}
