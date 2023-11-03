using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.Sequine {
    internal static class SequineSettingsProvider {

        public const string version = "1.3.0";

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

            var provider = new SettingsProvider("Project/CookieCutter/Sequine", SettingsScope.Project) {
                label = "Sequine",
                guiHandler = (searchContext) => {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("sequine-v" + version, greyStyle);
                        if (GUILayout.Button("sequine.calcatz.com", EditorStyles.miniButton)) {
                            Application.OpenURL("https://sequine.calcatz.com/");
                        }
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.LabelField("Developed and published by Calcatz Dilaura", greyStyle);
                    EditorGUILayout.LabelField("All rights reserved", greyStyle);
                    if (GUILayout.Button("Get Started")) {
                        Application.OpenURL("https://sequine.calcatz.com/docs/intro");
                    }

                    EditorGUILayout.Space();

                },

                keywords = new HashSet<string>(new[] { "sequine", "Sequine", "calcatz" })
            };

            return provider;
        }
    }

}