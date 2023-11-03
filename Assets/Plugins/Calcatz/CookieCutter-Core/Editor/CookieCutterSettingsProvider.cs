using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {
    internal static class CookieCutterProvider {

        public const string version = "1.1.0";

        private static GUIStyle m_greyStyle;
        private static GUIStyle greyStyle {
            get {
                if (m_greyStyle == null) {
                    m_greyStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                    m_greyStyle.alignment = TextAnchor.MiddleLeft;
                }
                return m_greyStyle;
            }
        }

        [SettingsProvider]
        static SettingsProvider CreateSettingsProvider() {

            var provider = new SettingsProvider("Project/CookieCutter", SettingsScope.Project) {
                // By default the last token of the path is used as display name if no label is provided.
                label = "CookieCutter",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) => {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("cookiecutter-core-v" + version, greyStyle);
                        if (GUILayout.Button("www.calcatz.com", EditorStyles.miniButton)) {
                            Application.OpenURL("www.calcatz.com");
                        }
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.LabelField("Developed and published by Calcatz", greyStyle);
                    EditorGUILayout.LabelField("All rights reserved", greyStyle);
                    if (GUILayout.Button("Get Started")) {
                        Application.OpenURL("https://calcatz.com/doc/cookiecutter/");
                    }

                    EditorGUILayout.Space();
                    EditorGUI.BeginChangeCheck();
                    var resetOnPlay = EditorGUILayout.ToggleLeft(new GUIContent("Reset Command Execution Flow History on Play", "Command Execution Flow History is the yellow-colored lines that shows whether or not a command node was executed."), EditorPrefs.GetBool("resetCommandNodeExecutionFlowOnPlay", true));
                    if (EditorGUI.EndChangeCheck()) {
                        EditorPrefs.SetBool("resetCommandNodeExecutionFlowOnPlay", resetOnPlay);
                    }

                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] {"cookiecutter", "CookieCutter", "calcatz"})
            };

            return provider;
        }
    }

}