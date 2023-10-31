using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class CrossSceneBinderDebugger : EditorWindow {

        [MenuItem("Window/Calcatz/CookieCutter/Cross-Scene Binder Debugger")]
        private static void OpenWindow() {
            var window = GetWindow<CrossSceneBinderDebugger>();
            window.titleContent.text = "Cross-Scene Binder Debugger";
            window.autoRepaintOnSceneChange = true;
            window.Show();
        }

        [SerializeField] private string searchText = "";

        private Vector2 scrollPos;

        private Dictionary<UnityEngine.Object, List<UnityEngine.Object>> bindings;
        private Dictionary<UnityEngine.Object, List<UnityEngine.Object>> searchDict = new Dictionary<Object, List<Object>>();

        private void OnEnable() {
            var field = typeof(CrossSceneBindingUtility).GetField("bindings", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            bindings = field.GetValue(null) as Dictionary<UnityEngine.Object, List<UnityEngine.Object>>;
        }

        private void OnGUI() {
            if (bindings == null) return;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Search:", GUILayout.Width(70));
                EditorGUI.BeginChangeCheck();
                string newSearchText = EditorGUILayout.TextField(searchText);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(this, "changed search");
                    searchText = newSearchText;
                }
            }
            GUILayout.EndHorizontal();

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            {
                bool guiEnabled = GUI.enabled;
                GUI.enabled = false;
                if (searchText != "") {
                    RefreshSearchDict();
                    DrawFromDictionary(searchDict);
                }
                else {
                    DrawFromDictionary(bindings);
                }
                GUI.enabled = guiEnabled;
            }
            GUILayout.EndScrollView();

        }

        private void DrawFromDictionary(Dictionary<UnityEngine.Object, List<UnityEngine.Object>> _dictionary) {
            foreach (var kvp in _dictionary) {
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.ObjectField(kvp.Key, kvp.Key.GetType(), false);
                    GUILayout.Label(EditorGUIUtility.IconContent("d_FixedJoint Icon"), EditorStyles.centeredGreyMiniLabel, 
                        GUILayout.Width(EditorGUIUtility.singleLineHeight),
                        GUILayout.Height(EditorGUIUtility.singleLineHeight),
                        GUILayout.ExpandWidth(false),
                        GUILayout.ExpandHeight(false));
                    GUILayout.BeginVertical();
                    foreach (var comp in kvp.Value) {
                        EditorGUILayout.ObjectField(comp, comp.GetType(), true);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
        }

        private void RefreshSearchDict() {
            searchDict.Clear();
            string[] searchSplits = ObjectNames.NicifyVariableName(searchText).ToLower().Split(' ');
            foreach (var kvp in bindings) {
                bool foundInKey = kvp.Key.name.ToLower().Contains(searchText.ToLower());
                if (!foundInKey) {
                    foundInKey = true;
                    {
                        string nicifyLowerName = ObjectNames.NicifyVariableName(kvp.Key.name).ToLower();
                        for (int i = 0; i < searchSplits.Length; i++) {
                            if (searchSplits[i] == "") continue;
                            if (!nicifyLowerName.Contains(searchSplits[i])) {
                                foundInKey = false;
                                break;
                            }
                        }
                    }
                }

                if (foundInKey) {
                    searchDict.Add(kvp.Key, kvp.Value);
                }
                else {
                    List<UnityEngine.Object> comps = new List<Object>();
                    foreach (var comp in kvp.Value) {
                        bool foundInComp = comp.name.ToLower().Contains(searchText.ToLower());
                        if (!foundInComp) {
                            foundInComp = true;
                            string nicifyLowerName = ObjectNames.NicifyVariableName(comp.name).ToLower();
                            for (int i = 0; i < searchSplits.Length; i++) {
                                if (searchSplits[i] == "") continue;
                                if (!nicifyLowerName.Contains(searchSplits[i])) {
                                    foundInComp = false;
                                    break;
                                }
                            }
                        }
                        if (foundInComp) {
                            comps.Add(comp);
                        }
                    }
                    if (comps.Count > 0) {
                        searchDict.Add(kvp.Key, comps);
                    }
                }
            }

        }

    }
}
