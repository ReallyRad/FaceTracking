using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.Sequine {
    internal class SequineGlobalDataWindow : EditorWindow {

        [SerializeField] private VariableTable variableTable;

        [MenuItem("Tools/Sequine/Global Data...")]
        public static void OpenWindow() {
            var window = GetWindow<SequineGlobalDataWindow>();
            window.titleContent = new GUIContent("Sequine Global Data");
            window.Show();
        }

        [MenuItem("Tools/Sequine/Command Creator...")]
        private static void OpenCommandCreatorWindow() {
            CommandCreatorWindow.OpenWindow();
        }

        private void OnEnable() {
            variableTable = new VariableTable()
                .SetShowVariableId(true)
                .SetRowSearchMethod(OnRowSearch);
        }

        private bool OnRowSearch(string _searchText, VisualElement _visualElement) {
            if (string.IsNullOrEmpty(_searchText)) return true;
            var textField = _visualElement.Q<TextField>();
            if (textField != null) {
                bool found = textField.value.ToLower().Contains(_searchText.ToLower());
                if (!found) {
                    found = true;
                    string nicifyLowerName = ObjectNames.NicifyVariableName(textField.value).ToLower();
                    string[] searchSplits = ObjectNames.NicifyVariableName(_searchText).ToLower().Split(' ');
                    for (int j = 0; j < searchSplits.Length; j++) {
                        if (searchSplits[j] == "") continue;
                        if (!nicifyLowerName.Contains(searchSplits[j])) {
                            found = false;
                            break;
                        }
                    }
                }
                return found;
            }
            return false;
        }

        private void CreateGUI() {
            var globalData = SequineGlobalData.GetPersistent();
            var variablesArea = new IMGUIContainer(HandleOnGUI) {
                style = {
                    paddingBottom = 5, paddingTop = 5, paddingLeft = 5, paddingRight = 5
                }
            };
            rootVisualElement.Add(variablesArea);

            variableTable.CreateElement(variablesArea, () => globalData);
            variableTable.BindVariables(() => globalData.variables);
            variableTable.SetRowSearchMethod(OnRowSearch);
        }

        private void RepaintWindow() {
            rootVisualElement.Clear();
            CreateGUI();
            Repaint();
        }

        private void HandleOnGUI() {
            Event e = Event.current;
            if (e.type == EventType.ValidateCommand) {
                switch (e.commandName) {
                    case "UndoRedoPerformed":
                        //case "SelectAll":
                        RepaintWindow();
                        break;
                }
            }
        }

    }

}