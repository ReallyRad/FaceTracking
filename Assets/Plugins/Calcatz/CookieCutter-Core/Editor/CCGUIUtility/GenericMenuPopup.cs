﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {
    internal class MenuItemNode {
        public GUIContent content;
        public GenericMenu.MenuFunction func;
        public GenericMenu.MenuFunction2 func2;
        public object userData;
        public bool separator;
        public bool on;

        public string name { get; }
        public MenuItemNode parent { get; }

        public List<MenuItemNode> Nodes { get; private set; }

        public MenuItemNode(string p_name = "", MenuItemNode p_parent = null) {
            name = p_name;
            parent = p_parent;
            Nodes = new List<MenuItemNode>();
        }

        public MenuItemNode CreateNode(string p_name) {
            var node = new MenuItemNode(p_name, this);
            Nodes.Add(node);
            return node;
        }

        // TODO Optimize
        public MenuItemNode GetOrCreateNode(string p_name) {
            var node = Nodes.Find(n => n.name == p_name);
            if (node == null) {
                node = CreateNode(p_name);
            }

            return node;
        }

        public List<MenuItemNode> Search(string p_search) {
            var lowerSearch = p_search.ToLower();
            List<MenuItemNode> result = new List<MenuItemNode>();

            string[] searchSplits = ObjectNames.NicifyVariableName(p_search).ToLower().Split(' ');

            foreach (var node in Nodes) {

                if (node.Nodes.Count == 0) {
                    bool found = node.name.ToLower().Contains(lowerSearch);
                    if (!found) {
                        found = node.name.Replace(" ", "").ToLower().Contains(lowerSearch);
                    }
                    if (!found) {
                        string nicifyLowerName = ObjectNames.NicifyVariableName(node.name).ToLower();
                        found = true;
                        for (int i = 0; i < searchSplits.Length; i++) {
                            if (searchSplits[i] == "") continue;
                            if (!nicifyLowerName.Contains(searchSplits[i])) {
                                found = false;
                                break;
                            }
                        }
                    }
                    if (found) {
                        result.Add(node);
                    }
                }

                result.AddRange(node.Search(p_search));
            }

            return result;
        }

        public string GetPath() {
            return parent == null ? "" : parent.GetPath() + "/" + name;
        }

        public void Execute() {
            if (func != null) {
                func?.Invoke();
            }
            else {
                func2?.Invoke(userData);
            }
        }
    }

    public class GenericMenuPopup : PopupWindowContent {

        public System.Action<string> onSearchTermChanged;

        public static GenericMenuPopup Get(GenericMenu p_menu, string p_title) {
            var popup = new GenericMenuPopup(p_menu, p_title, null);
            return popup;
        }

        public static GenericMenuPopup Show(GenericMenu p_menu, string p_title, Vector2 p_position, List<string> starredPaths, string _searchTerm = "", System.Action<string> _onSearchTermChanged = null) {
            var popup = new GenericMenuPopup(p_menu, p_title, starredPaths);
            popup.onSearchTermChanged = _onSearchTermChanged;
            popup._search = _searchTerm;
            popup.resizeToContent = true;
            PopupWindow.Show(new Rect(p_position.x, p_position.y, 0, 0), popup);
            return popup;
        }

        private static GUIStyle _labelWhite;
        private static GUIStyle LabelWhite {
            get {
                if (_labelWhite == null) {
                    _labelWhite = new GUIStyle("label");
                    _labelWhite.normal.textColor = Color.white;
                }
                return _labelWhite;
            }
        }

        private static GUIStyle _backStyle;
        public static GUIStyle BackStyle {
            get {
                if (_backStyle == null) {
                    _backStyle = new GUIStyle(GUI.skin.button);
                    _backStyle.alignment = TextAnchor.MiddleLeft;
                    _backStyle.hover.background = Texture2D.grayTexture;
                    _backStyle.normal.textColor = Color.black;
                }

                return _backStyle;
            }
        }

        private static GUIStyle _plusStyle;
        public static GUIStyle PlusStyle {
            get {
                if (_plusStyle == null) {
                    _plusStyle = new GUIStyle();
                    _plusStyle.fontStyle = FontStyle.Bold;
                    _plusStyle.normal.textColor = Color.white;
                    _plusStyle.fontSize = 16;
                }

                return _plusStyle;
            }
        }

        private string _title;
        private Vector2 _scrollPosition;
        private MenuItemNode _rootNode;
        private MenuItemNode _currentNode;
        private MenuItemNode _hoverNode;
        private int hoveredIndex;
        private string _search;
        private bool _repaint = false;
        private int _contentHeight;
        private bool _useScroll;
        private List<MenuItemNode> _starredNodes;

        public int width = 215;
        public int height = 250;
        public int maxHeight = 360;
        public bool resizeToContent = false;
        public bool showOnStatus = true;
        public bool showSearch = true;
        public bool showTooltip = false;
        public bool showTitle = false;
        private Texture backgroundTexture;

        public GenericMenuPopup(GenericMenu p_menu, string p_title, List<string> p_starredPaths) {
            _title = p_title;
            showTitle = !string.IsNullOrWhiteSpace(_title);
            _currentNode = _rootNode = GenerateMenuItemNodeTree(p_menu);
            if (p_starredPaths != null) {
                _starredNodes = new List<MenuItemNode>();
                for (int i=0; i<p_starredPaths.Count; i++) {
                    string[] split = p_starredPaths[i].Split('/');
                    List<MenuItemNode> items = _currentNode.Search(split[split.Length - 1]);
                    foreach(MenuItemNode item in items) {
                        if (item.name == split[split.Length - 1]) {
                            _starredNodes.Add(item);
                        }
                    }
                }
            }
        }

        public override Vector2 GetWindowSize() {
            return new Vector2(width, height);
        }

        public void Show(float p_x, float p_y) {
            PopupWindow.Show(new Rect(p_x, p_y, 0, 0), this);
        }

        public void Show(Vector2 p_position) {
            PopupWindow.Show(new Rect(p_position.x, p_position.y, 0, 0), this);
        }

        public override void OnGUI(Rect p_rect) {
            if (backgroundTexture == null) {
                Texture swatch = ScreenSwatch.GrabScreenSwatch(editorWindow.position);
                SimpleSeparableBlur blur = new SimpleSeparableBlur(width, height);
                backgroundTexture = blur.BlurTexture(swatch);
            }
            GUI.DrawTexture(new Rect(0, 0, width, height), backgroundTexture);

            if (Event.current.type == EventType.Layout)
                _useScroll = _contentHeight > maxHeight || (!resizeToContent && _contentHeight > height);

            _contentHeight = 0;
            GUIStyle style = new GUIStyle();
            style.normal.background = Texture2D.whiteTexture;
            GUI.color = new Color(0.1f, 0.1f, 0.1f, 1);
            //GUI.Box(p_rect, string.Empty, style);
            GUI.color = Color.white;

            if (showTitle) {
                DrawTitle(new Rect(p_rect.x, p_rect.y, p_rect.width, 24));
            }

            if (showSearch) {
                DrawSearch(new Rect(p_rect.x, p_rect.y + (showTitle ? 24 : 0), p_rect.width, 20));
            }

            DrawMenuItems(new Rect(p_rect.x, p_rect.y + (showTitle ? 24 : 0) + (showSearch ? 22 : 0), p_rect.width, p_rect.height - (showTooltip ? 60 : 0) - (showTitle ? 24 : 0) - (showSearch ? 22 : 0)));

            if (showTooltip) {
                DrawTooltip(new Rect(p_rect.x + 5, p_rect.y + p_rect.height - 58, p_rect.width - 10, 56));
            }

            if (resizeToContent) {
                _contentHeight += 10;
                height = Mathf.Min(_contentHeight, maxHeight);
            }
            EditorGUI.FocusTextInControl("Search");
        }

        private void DrawTitle(Rect p_rect) {
            _contentHeight += 24;
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 12;
            style.alignment = TextAnchor.LowerCenter;
            p_rect.y -= 5;
            GUI.Label(p_rect, _title, style);
        }

        private void DrawSearch(Rect p_rect) {
            _contentHeight += 22;

            List<MenuItemNode> nodes;
            List<MenuItemNode> sortedNodes;
            if (_search != null && _search != "") {
                nodes = _rootNode.Search(_search);
                sortedNodes = new List<MenuItemNode>(nodes);
                sortedNodes.Sort((n1, n2) => {
                    string p1 = n1.parent.GetPath();
                    string p2 = n2.parent.GetPath();
                    if (p1 == p2)
                        return n1.name.CompareTo(n2.name);

                    return p1.CompareTo(p2);
                });
            }
            else {
                nodes = _currentNode.Nodes;
                sortedNodes = nodes;
            }

            if (Event.current.type == EventType.KeyDown && GUI.GetNameOfFocusedControl() == "Search") {
                int currentSearchIndex = sortedNodes.IndexOf(_hoverNode);
                int searchIndex = currentSearchIndex;

                switch (Event.current.keyCode) {
                    case KeyCode.DownArrow:
                        do {
                            searchIndex++;
                            if (searchIndex >= nodes.Count) {
                                searchIndex = 0;
                            }
                            _hoverNode = sortedNodes[searchIndex];
                            if (searchIndex == currentSearchIndex) {
                                break;
                            }
                        } while (_hoverNode.separator);
                        hoveredIndex = nodes.IndexOf(_hoverNode);
                        break;

                    case KeyCode.UpArrow:
                        do {
                            searchIndex--;
                            if (searchIndex < 0) {
                                searchIndex = nodes.Count - 1;
                            }
                            _hoverNode = sortedNodes[searchIndex];
                            if (searchIndex == currentSearchIndex) {
                                break;
                            }
                        } while (_hoverNode.separator);
                        hoveredIndex = nodes.IndexOf(_hoverNode);
                        break;

                    case KeyCode.Return:
                        if (_hoverNode != null) {
                            if (_hoverNode.Nodes.Count == 0) {
                                _hoverNode.Execute();
                                base.editorWindow.Close();
                            }
                            else {
                                hoveredIndex = 0;
                                _currentNode = _hoverNode;
                                if (_currentNode.Nodes.Count > 0) {
                                    _hoverNode = _currentNode.Nodes[hoveredIndex];
                                }
                                _repaint = true;
                            }
                        }
                        break;

                    case KeyCode.Escape:
                        if (_currentNode.parent != null) {
                            _currentNode = _currentNode.parent;
                            hoveredIndex = 0;
                            if (_currentNode.Nodes.Count > 0) {
                                _hoverNode = _currentNode.Nodes[hoveredIndex];
                            }
                            _repaint = true;
                        }
                        break;
                }
            }

            GUI.SetNextControlName("Search");
            string newSearch = GUI.TextField(p_rect, _search);
            if (newSearch != _search) {
                if (newSearch != null & newSearch != "") {
                    nodes = _rootNode.Search(newSearch);
                }
                else {
                    nodes = _currentNode.Nodes;
                }

                if (nodes.Count > 0) {
                    _hoverNode = nodes[0];
                    hoveredIndex = 0;
                }
                else {
                    _hoverNode = null;
                }
                _search = newSearch;
            }
            onSearchTermChanged.Invoke(_search);
        }

        private void DrawTooltip(Rect p_rect) {
            _contentHeight += 60;
            if (_hoverNode == null || _hoverNode.content == null || string.IsNullOrWhiteSpace(_hoverNode.content.tooltip))
                return;

            GUIStyle style = new GUIStyle();
            style.fontSize = 9;
            style.wordWrap = true;
            style.normal.textColor = Color.white;
            GUI.Label(p_rect, _hoverNode.content.tooltip, style);
        }

        private void DrawMenuItems(Rect p_rect) {
            GUILayout.BeginArea(p_rect);
            if (_useScroll) {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);
            }

            GUILayout.BeginVertical();

            if (string.IsNullOrEmpty(_search) && _starredNodes != null && _starredNodes.Count > 0) {
                DrawStaredNodes(p_rect);
            }
            if (string.IsNullOrWhiteSpace(_search) || _search.Length < 2) {
                DrawNodeTree(p_rect);
            }
            else {
                DrawNodeSearch(p_rect);
            }

            GUILayout.EndVertical();
            if (_useScroll) {
                EditorGUILayout.EndScrollView();
            }

            GUILayout.EndArea();
        }

        private void DrawStaredNodes(Rect p_rect) {

            string lastPath = "";
            for (int i = 0; i < _starredNodes.Count; i++) {
                string nodePath = _starredNodes[i].parent.GetPath();
                if (nodePath != lastPath) {
                    _contentHeight += 21;
                    GUILayout.Label(nodePath, LabelWhite);
                    lastPath = nodePath;
                }

                _contentHeight += 21;
                GUI.color = _hoverNode == _starredNodes[i] ? Color.white : Color.gray;
                GUIStyle style = new GUIStyle();
                style.normal.background = Texture2D.grayTexture;
                GUILayout.BeginHorizontal(style);

                if (showOnStatus) {
                    style = new GUIStyle("box");
                    style.normal.background = Texture2D.whiteTexture;
                    GUI.color = _starredNodes[i].on ? new Color(0, .6f, .8f) : new Color(.2f, .2f, .2f);
                    //GUILayout.Box("", style, GUILayout.Width(14), GUILayout.Height(14));
                }

                GUI.color = _hoverNode == _starredNodes[i] ? Color.white : Color.white;
                GUILayout.Label("⋆ " + _starredNodes[i].name, LabelWhite);

                GUILayout.EndHorizontal();

                var nodeRect = GUILayoutUtility.GetLastRect();
                if (Event.current.isMouse) {
                    if (nodeRect.Contains(Event.current.mousePosition)) {
                        hoveredIndex = i;
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                            if (_starredNodes[i].Nodes.Count > 0) {
                                _currentNode = _starredNodes[i];
                                _repaint = true;
                            }
                            else {
                                if (onSearchTermChanged != null) {
                                    onSearchTermChanged.Invoke(_search);
                                }
                                _starredNodes[i].Execute();
                                base.editorWindow.Close();
                            }

                            break;
                        }

                        if (_hoverNode != _starredNodes[i]) {
                            _hoverNode = _starredNodes[i];
                            _repaint = true;
                        }
                    }
                    else if (_hoverNode == _starredNodes[i]) {
                        _hoverNode = null;
                        _repaint = true;
                    }
                }
            }

            if (_starredNodes.Count == 0) {
                GUILayout.Label("No result found for specified search.");
            }
        }

        private void DrawNodeSearch(Rect p_rect) {
            List<MenuItemNode> search = _rootNode.Search(_search);
            search.Sort((n1, n2) => {
                string p1 = n1.parent.GetPath();
                string p2 = n2.parent.GetPath();
                if (p1 == p2)
                    return n1.name.CompareTo(n2.name);

                return p1.CompareTo(p2);
            });

            string lastPath = "";
            for (int i=0; i<search.Count; i++) {
                string nodePath = search[i].parent.GetPath();
                if (nodePath != lastPath) {
                    _contentHeight += 21;
                    GUILayout.Label(nodePath, LabelWhite);
                    lastPath = nodePath;
                }

                _contentHeight += 21;
                GUI.color = _hoverNode == search[i] ? Color.white : Color.gray;
                GUIStyle style = new GUIStyle();
                style.normal.background = Texture2D.grayTexture;
                GUILayout.BeginHorizontal(style);

                if (showOnStatus) {
                    style = new GUIStyle("box");
                    style.normal.background = Texture2D.whiteTexture;
                    GUI.color = search[i].on ? new Color(0, .6f, .8f) : new Color(.2f, .2f, .2f);
                    //GUILayout.Box("", style, GUILayout.Width(14), GUILayout.Height(14));
                }

                GUI.color = _hoverNode == search[i] ? Color.white : Color.white;
                GUILayout.Label(search[i].name, LabelWhite);

                GUILayout.EndHorizontal();

                var nodeRect = GUILayoutUtility.GetLastRect();
                if (Event.current.isMouse) {
                    if (nodeRect.Contains(Event.current.mousePosition)) {
                        hoveredIndex = i;
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                            if (search[i].Nodes.Count > 0) {
                                _currentNode = search[i];
                                _repaint = true;
                            }
                            else {
                                if (onSearchTermChanged != null) {
                                    onSearchTermChanged.Invoke(_search);
                                }
                                search[i].Execute();
                                base.editorWindow.Close();
                            }

                            break;
                        }

                        if (_hoverNode != search[i]) {
                            _hoverNode = search[i];
                            _repaint = true;
                        }
                    }
                    else if (_hoverNode == search[i]) {
                        _hoverNode = null;
                        _repaint = true;
                    }
                }
            }

            if (search.Count == 0) {
                GUILayout.Label("No result found for specified search.");
            }
        }

        private void DrawNodeTree(Rect p_rect) {
            if (_currentNode != _rootNode) {
                _contentHeight += 21;
                if (GUILayout.Button(_currentNode.GetPath(), BackStyle)) {
                    _currentNode = _currentNode.parent;
                }
            }

            foreach (var node in _currentNode.Nodes) {
                if (node.separator) {
                    GUILayout.Space(4);
                    _contentHeight += 4;
                    continue;
                }

                _contentHeight += 21;
                GUI.color = _hoverNode == node ? Color.white : Color.gray;
                GUIStyle style = new GUIStyle();
                style.normal.background = Texture2D.grayTexture;
                GUILayout.BeginHorizontal(style);

                if (showOnStatus) {
                    style = new GUIStyle("box");
                    style.normal.background = Texture2D.whiteTexture;
                    GUI.color = node.on ? new Color(0, .6f, .8f, .5f) : new Color(.2f, .2f, .2f, .2f);
                    //GUILayout.Box("", style, GUILayout.Width(14), GUILayout.Height(14));
                }

                GUI.color = _hoverNode == node ? Color.white : Color.white;
                style = LabelWhite;
                style.fontStyle = node.Nodes.Count > 0 ? FontStyle.Bold : FontStyle.Normal;
                GUILayout.Label(node.name, style);

                GUILayout.EndHorizontal();

                var nodeRect = GUILayoutUtility.GetLastRect();
                if (Event.current.isMouse) {
                    if (nodeRect.Contains(Event.current.mousePosition)) {
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                            if (node.Nodes.Count > 0) {
                                _currentNode = node;
                                _repaint = true;
                            }
                            else {
                                if (onSearchTermChanged != null) {
                                    onSearchTermChanged.Invoke(_search);
                                }
                                node.Execute();
                                base.editorWindow.Close();
                            }

                            break;
                        }

                        if (_hoverNode != node) {
                            _hoverNode = node;
                            _repaint = true;
                        }
                    }
                    else if (_hoverNode == node) {
                        _hoverNode = null;
                        _repaint = true;
                    }
                }

                if (node.Nodes.Count > 0) {
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    GUI.Label(new Rect(lastRect.x + lastRect.width - 16, lastRect.y - 2, 20, 20), "+", PlusStyle);
                }
            }
        }

        void OnEditorUpdate() {
            if (_repaint) {
                _repaint = false;
                base.editorWindow.Repaint();
            }
        }

        // TODO Possible type caching? 
        internal static MenuItemNode GenerateMenuItemNodeTree(GenericMenu p_menu) {
            MenuItemNode rootNode = new MenuItemNode();
            if (p_menu == null)
                return rootNode;

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
                GUIContent content = (GUIContent)menuItemType.GetField("content").GetValue(menuItem);

                bool separator = (bool)menuItemType.GetField("separator").GetValue(menuItem);
                string path = content.text;
                string[] splitPath = path.Split('/');
                MenuItemNode currentNode = rootNode;
                for (int i = 0; i < splitPath.Length; i++) {
                    currentNode = (i < splitPath.Length - 1)
                        ? currentNode.GetOrCreateNode(splitPath[i])
                        : currentNode.CreateNode(splitPath[i]);
                }

                if (separator) {
                    currentNode.separator = true;
                } else {
                    currentNode.content = content;
                    currentNode.func = (GenericMenu.MenuFunction)menuItemType.GetField("func").GetValue(menuItem);
                    currentNode.func2 = (GenericMenu.MenuFunction2)menuItemType.GetField("func2").GetValue(menuItem);
                    currentNode.userData = menuItemType.GetField("userData").GetValue(menuItem);
                    currentNode.on = (bool)menuItemType.GetField("on").GetValue(menuItem);
                }
            }

            DeepSortNodes(rootNode);

            return rootNode;
        }

        private static void DeepSortNodes(MenuItemNode _currentNode) {
            List<List<MenuItemNode>> separatedNodes = new List<List<MenuItemNode>>();

            int i = 0;
            separatedNodes.Add(new List<MenuItemNode>());
            foreach (var node in _currentNode.Nodes) {
                separatedNodes[i].Add(node);
                if (node.separator) {
                    separatedNodes.Add(new List<MenuItemNode>());
                    i++;
                }
            }

            _currentNode.Nodes.Clear();
            for (i = 0; i < separatedNodes.Count; i++) {
                separatedNodes[i].Sort((n1, n2) => {
                    if (n1.separator) return 1;
                    if (n1.Nodes.Count == 0 && n2.Nodes.Count > 0) return -1;
                    if (n1.Nodes.Count > 0 && n2.Nodes.Count == 0) return 1;
                    return n1.name.CompareTo(n2.name);
                });
                _currentNode.Nodes.AddRange(separatedNodes[i]);
            }
            foreach (var childNode in _currentNode.Nodes) {
                DeepSortNodes(childNode);
            }
        }

        public override void OnOpen() {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }

        public override void OnClose() {
            EditorApplication.update -= OnEditorUpdate;
        }
    }
}