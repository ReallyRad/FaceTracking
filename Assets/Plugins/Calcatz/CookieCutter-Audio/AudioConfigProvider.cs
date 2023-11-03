#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter.Audio {
    static class AudioConfigProvider {

        [SettingsProvider]
        static SettingsProvider CreateAudioConfigSettingsProvider() {
            return CreateSettingsProvider(AudioConfig.GetOrCreateSettings(), "Project/CookieCutter/Audio Config", "audio", "Audio");
        }

        static SettingsProvider CreateSettingsProvider<T>(T _settingsAsset, string _path, params string[] _keywords) where T : UnityEngine.Object {
            if (_settingsAsset == null) return null;

            var settingsSO = new SerializedObject(_settingsAsset);
            var pathSplits = _path.Split('/');

            var provider = new SettingsProvider(_path, SettingsScope.Project) {
                label = pathSplits[pathSplits.Length - 1],
                guiHandler = (searchContext) => {

                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 250;

                    bool guiEnabled = GUI.enabled;
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField("Asset", _settingsAsset, typeof(T), false);
                    GUI.enabled = guiEnabled;
                    EditorGUILayout.Space();

                    settingsSO.Update();

                    SerializedProperty prop = settingsSO.GetIterator();
                    prop.NextVisible(true);
                    if (prop.NextVisible(true)) {
                        do {
                            EditorGUILayout.PropertyField(settingsSO.FindProperty(prop.name), true);
                        }
                        while (prop.NextVisible(false));
                    }

                    EditorGUIUtility.labelWidth = labelWidth;
                    settingsSO.ApplyModifiedProperties();
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(_keywords)
            };

            return provider;
        }
    }

}
#endif