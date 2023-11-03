
#if !UNITY_2021_3_OR_NEWER
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Compilation;

namespace Calcatz {
    [InitializeOnLoad]
    internal class AsmdefDebug : EditorWindow {

        [MenuItem("Window/Analysis/Calcatz/Asmdef Debug")]
        private static void OpenWindow() {
            var window = GetWindow<AsmdefDebug>();
            window.Show();
        }

        const string RELOAD_TIME_KEY = "AssemblyReloadEventsTime";
        const string RESULT_KEY = "AssemblyReloadResultString";

        static Dictionary<string, DateTime> compilationStartTimes = new Dictionary<string, DateTime>();
        static double compilationTotalTime;

        private static string result = "";

        static AsmdefDebug() {
            CompilationPipeline.assemblyCompilationStarted += OnCompilationStarted;
            CompilationPipeline.assemblyCompilationFinished += OnCompilationFinished;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterReload;
        }

        static void OnCompilationStarted(string obj) {
            compilationStartTimes[obj] = DateTime.UtcNow;
        }

        static void OnCompilationFinished(string assembly, CompilerMessage[] msgs) {
            int pathStrLen = "Library/ScriptAssemblies/".Length;
            var timeSpan = DateTime.UtcNow - compilationStartTimes[assembly];
            compilationTotalTime += timeSpan.TotalMilliseconds;
            result += string.Format("{0:0.00}s {1} compile", timeSpan.TotalMilliseconds / 1000f, assembly.Substring(pathStrLen, assembly.Length - pathStrLen));
            result += "\n\n\n";
        }

        static void OnBeforeReload() {
            result += string.Format("compilation total: {0:0.00}s", compilationTotalTime / 1000f);
            result += "\n\n\n";
            EditorPrefs.SetString(RELOAD_TIME_KEY, DateTime.UtcNow.ToBinary().ToString());
            EditorPrefs.SetString(RESULT_KEY, result);
        }

        static void OnAfterReload() {
            var dateBinStr = EditorPrefs.GetString(RELOAD_TIME_KEY);
            result = EditorPrefs.GetString(RESULT_KEY);
            long dateBin = 0;
            if (long.TryParse(dateBinStr, out dateBin)) {
                var date = DateTime.FromBinary(dateBin);
                var timeSpan = DateTime.UtcNow - date;
                result += string.Format("reload assemblies: {0:0.00}s", (float)timeSpan.TotalMilliseconds / 1000f);
                result += "\n\n\n";
            }
            EditorPrefs.DeleteKey(RELOAD_TIME_KEY);
            EditorPrefs.DeleteKey(RESULT_KEY);
        }

        private void OnGUI() {
            EditorGUILayout.TextArea(result, UnityEngine.GUILayout.ExpandWidth(true), UnityEngine.GUILayout.ExpandHeight(true));
        }
    }
}
#endif