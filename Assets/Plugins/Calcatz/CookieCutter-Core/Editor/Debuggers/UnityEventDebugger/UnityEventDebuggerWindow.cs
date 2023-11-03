using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Linq;
using static Calcatz.CookieCutter.UnityEventReferenceFinder;

namespace Calcatz.CookieCutter {
    internal class UnityEventDebuggerWindow : EditorWindow {

        private static bool opened = false;

        public enum FindMethod {
            FindByMethodName,
            FindByAssignedObject
        }


        private int mainSpacing = 20;
        private int tabulation = 30;
        private float leftColoumnRelativeWidth = 0.6f;

        [SerializeField] private List<EventReferenceInfo> dependencies;
        [SerializeField] private string searchString = "";
        [SerializeField] private UnityEngine.Object assignedObject = null;

        private Rect drawableRect;

        Vector2 scroll = Vector2.zero;

        [SerializeField] private FindMethod findMethod;



        [MenuItem("Window/Calcatz/UnityEvent Finder")]
        public static void OpenWindow() {
            UnityEventDebuggerWindow window = GetWindow<UnityEventDebuggerWindow>();
            window.titleContent = new GUIContent("UnityEvent Debugger");
            window.minSize = new Vector2(250, 100);
        }

        [DidReloadScripts]
        private static void RefreshDependencies() {
            if (opened) {
                var inst = GetWindow<UnityEventDebuggerWindow>();
                if (inst.findMethod == FindMethod.FindByMethodName) {
                    inst.FindDependenciesByMethodName();
                }
                else {
                    inst.FindDependenciesByAssignedObject();
                }
            }
        }

        private void OnFocus() {
            RefreshDependencies();
        }

        private void OnEnable() {
            opened = true;
            FindDependencies();
            Undo.undoRedoPerformed += RefreshDependencies;
        }

        private void OnDisable() {
            opened = false;
            dependencies = null;
            Undo.undoRedoPerformed -= RefreshDependencies;
        }

        private void OnGUI() {
            DrawWindow();
        }

        private void DrawWindow() {
            var drawnVertically = 0;
            drawableRect = GetDrawableRect();

            EditorGUI.BeginChangeCheck();
            var newFindMethod = (FindMethod)EditorGUI.EnumPopup(new Rect(drawableRect.position, new Vector2(drawableRect.width - 150 - 5, EditorGUIUtility.singleLineHeight)), findMethod);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(this, "change find by method");
                findMethod = newFindMethod;
                RefreshDependencies();
            }

            if (GUI.Button(new Rect(drawableRect.position + Vector2.right * (drawableRect.width - 150), new Vector2(150, EditorGUIUtility.singleLineHeight)), "Refresh")) {
                RefreshDependencies();
            }

            if (findMethod == FindMethod.FindByMethodName) {
                HandleFindByMethodName();
            }
            else {
                HandleFindByAssignedObject();
            }

            if (dependencies != null) {
                GUILayout.Space(60);

                scroll = GUILayout.BeginScrollView(scroll, false, false);

                if (dependencies.Count == 0)
                    GUILayout.Label("Reference not found.", EditorStyles.centeredGreyMiniLabel);

                int i = 0;
                foreach (var d in dependencies) {
                    if (d.Listeners.Count == 0 && d.MethodNames.Count == 0) continue;
                    var position = new Vector2(drawableRect.position.x, 0f) + Vector2.up * drawnVertically;
                    var verticalSpace = (d.collapsed? (d.Listeners.Count + 1) : 1) * (int)EditorGUIUtility.singleLineHeight + mainSpacing;

                    DrawDependencies(d, position);

                    drawnVertically += verticalSpace;
                    i++;
                }

                GUILayout.Space(drawnVertically + 20);

                GUILayout.EndScrollView();
            }
        }

        private void HandleFindByMethodName() {
            var searchRect = new Rect(drawableRect.position + Vector2.up * (EditorGUIUtility.singleLineHeight + 5), new Vector2(drawableRect.width, EditorGUIUtility.singleLineHeight));
            EditorGUI.LabelField(new Rect(searchRect.position, new Vector2(75f, searchRect.height)), "Search:");

            EditorGUI.BeginChangeCheck();
            var newString = EditorGUI.TextField(new Rect(searchRect.position + Vector2.right * 60, new Vector2(searchRect.width - 60, searchRect.height)), searchString);

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(this, "change search string");
                searchString = newString;
                FindDependenciesByMethodName();
            }
        }

        private void FindDependenciesByMethodName() {
            if (searchString != "") {
                FindDependencies(searchString);
                foreach (var d in dependencies) {
                    d.collapsed = true;
                }
            }
            else {
                FindDependencies();
            }
        }

        private void HandleFindByAssignedObject() {
            var searchRect = new Rect(drawableRect.position + Vector2.up * (EditorGUIUtility.singleLineHeight + 5), new Vector2(drawableRect.width, EditorGUIUtility.singleLineHeight));
            EditorGUI.LabelField(new Rect(searchRect.position, new Vector2(75f, searchRect.height)), "Object:");

            EditorGUI.BeginChangeCheck();
            var newAssignedObject = EditorGUI.ObjectField(new Rect(searchRect.position + Vector2.right * 60, new Vector2(searchRect.width - 60, searchRect.height)), assignedObject, typeof(UnityEngine.Object), true);

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(this, "change search assigned object");
                assignedObject = newAssignedObject;
                FindDependenciesByAssignedObject();
            }
        }

        private void FindDependenciesByAssignedObject() {
            if (assignedObject != null) {
                FindDependencies(assignedObject);
            }
            else {
                FindDependencies();
            }
        }

        private void DrawDependencies(EventReferenceInfo dependency, Vector2 position) {
            float width = drawableRect.width * leftColoumnRelativeWidth;

            EditorGUI.BeginChangeCheck();
            var content = EditorGUIUtility.ObjectContent(dependency.Owner, dependency.Owner.GetType());
            content.text += " -> " + ObjectNames.NicifyVariableName(dependency.fieldName);
            bool newCollapsed = EditorGUI.BeginFoldoutHeaderGroup(new Rect(position, new Vector2(width, mainSpacing)), dependency.collapsed, content);
            if (EditorGUI.EndChangeCheck()) {
                dependency.collapsed = newCollapsed;
            }

            if (GUI.Button(new Rect(position + Vector2.right * (width + tabulation), new Vector2(150, mainSpacing)), "Select Game Object")) {
                EditorGUIUtility.PingObject(dependency.Owner);
                Selection.activeObject = dependency.Owner;
            }

            //EditorGUI.ObjectField(new Rect(position, new Vector2(width - tabulation, 16f)), dependency.Owner, typeof(MonoBehaviour), true);

            if (dependency.collapsed) {
                for (int i = 0; i < dependency.Listeners.Count; i++) {
                    Vector2 positionRoot = position + Vector2.up * mainSpacing + Vector2.up * EditorGUIUtility.singleLineHeight * i;
                    bool guiEnabled = GUI.enabled;
                    GUI.enabled = false;
                    EditorGUI.ObjectField(new Rect(positionRoot + Vector2.right * tabulation, new Vector2(width - tabulation, EditorGUIUtility.singleLineHeight)), dependency.Listeners[i], typeof(UnityEngine.Object), true);
                    GUI.enabled = guiEnabled;

                    Vector2 labelPosition = new Vector2(drawableRect.width * leftColoumnRelativeWidth + tabulation * 1.5f, positionRoot.y);
                    EditorGUI.SelectableLabel(new Rect(labelPosition, new Vector2(drawableRect.width * (1 - leftColoumnRelativeWidth) - tabulation / 1.5f, EditorGUIUtility.singleLineHeight)), dependency.MethodNames[i]);
                }
            }

            EditorGUI.EndFoldoutHeaderGroup();
        }

        private static void FindDependencies(string methodName) {
            var depens = UnityEventReferenceFinder.FindAllUnityEventsReferences();
            var foundInfo = new List<EventReferenceInfo>();

            EventReferenceInfo info;

            string[] searchSplits = ObjectNames.NicifyVariableName(methodName).ToLower().Split(' ');
            foreach (var d in depens) {

                info = null;

                for (int i = 0; i < d.MethodNames.Count; i++) {
                    var m = d.MethodNames[i];
                    bool found = m.ToLower().Contains(methodName.ToLower());
                    if (!found) {
                        found = true;
                        string nicifyLowerName = ObjectNames.NicifyVariableName(m).ToLower();
                        for (int j = 0; j < searchSplits.Length; j++) {
                            if (searchSplits[j] == "") continue;
                            if (!nicifyLowerName.Contains(searchSplits[j])) {
                                found = false;
                                break;
                            }
                        }
                    }
                    if (found) {
                        if (info == null) {
                            info = new EventReferenceInfo();
                            info.Owner = d.Owner;
                        }
                        info.Listeners.Add(d.Listeners[i]);
                        info.MethodNames.Add(d.MethodNames[i]);
                    }
                }

                if (info != null) {
                    foundInfo.Add(info);
                }

            }

            GetWindow<UnityEventDebuggerWindow>().dependencies = foundInfo;
        }

        private static void FindDependencies(UnityEngine.Object _assignedObject) {
            var depens = UnityEventReferenceFinder.FindAllUnityEventsReferences();
            var foundInfo = new List<EventReferenceInfo>();

            EventReferenceInfo info;

            foreach (var d in depens) {
                info = null;
                for(int i = 0; i < d.Listeners.Count; i++) {

                    bool listenerFound;
                    if (_assignedObject is GameObject) {
                        if (d.Listeners[i] != null && d.Listeners[i] is MonoBehaviour comp) {
                            listenerFound = comp.gameObject == _assignedObject;
                        }
                        else {
                            listenerFound = false;
                        }
                    }
                    else {
                        listenerFound = d.Listeners[i] == _assignedObject;
                    }

                    if (listenerFound) {
                        if (info == null) {
                            info = new EventReferenceInfo();
                            info.Owner = d.Owner;
                        }

                        info.Listeners.Add(d.Listeners[i]);
                        info.MethodNames.Add(d.MethodNames[i]);

                    }
                }
                if (info != null) {
                    foundInfo.Add(info);
                }
            }

            GetWindow<UnityEventDebuggerWindow>().dependencies = foundInfo;
        }

        private static void FindDependencies() {
            GetWindow<UnityEventDebuggerWindow>().dependencies = UnityEventReferenceFinder.FindAllUnityEventsReferences();
        }

        private Rect GetDrawableRect() {
            return new Rect(Vector2.one * 10f, position.size - Vector2.one * 20);
        }
    }
}