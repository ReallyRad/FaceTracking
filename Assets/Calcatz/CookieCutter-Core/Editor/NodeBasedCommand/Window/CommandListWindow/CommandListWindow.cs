using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;
#if TIMELINE_AVAILABLE
using UnityEngine.Timeline;
using UnityEngine.Playables;
#endif
using System.Reflection;
using System.Linq;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
#else
using Calcatz.OdinSerializer;
#endif

namespace Calcatz.CookieCutter {
    public class CommandListWindow : EditorWindow {

        [MenuItem("Window/Calcatz/CookieCutter/Command List")]
        private static void OpenWindow() {
            var window = GetWindow<CommandListWindow>();
            window.titleContent.text = "Command List";
            window.Show();
        }

        [SerializeField] private string searchValue = "";
        private CommandTreeElement rootTreeElement;
        private ScrollView scrollView;

        private void CreateGUI() {
            VisualElement header = new VisualElement() {
                style = {
                    flexDirection = FlexDirection.Row
                }
            };

            Button refreshButton = new Button(Refresh) {
                text = "Refresh",
                style = {
                    width = 80f
                }
            };
            header.Add(refreshButton);
            CreateSearchArea(header);

            rootVisualElement.Add(header);

            Refresh();
        }

        private void Refresh() {
            if (scrollView != null) {
                scrollView.parent.Remove(scrollView);
            }

            scrollView = new ScrollView() {
                style = {
                    position = Position.Absolute,
                    top = 30,
                    left = 0, right = 0, bottom = 0
                }
            };
            rootVisualElement.Add(scrollView);
            CreateTreeElements(scrollView);

            rootTreeElement.visualElement.value = true;
        }

        private void CreateSearchArea(VisualElement _visualElement) {

            _visualElement.Add(new Label("Search:") {
                style = {
                    paddingTop = 3
                }
            });

            TextField searchField = new TextField() {
                value = searchValue,
                style = {
                    flexGrow = 1f
                }
            };
            searchField.RegisterValueChangedCallback(_e => {
                searchValue = _e.newValue;
                Refresh();
            });
            _visualElement.Add(searchField);
            _visualElement.Add(new Button(() => {
                searchField.value = "";
                Refresh();
            }) {
                text = "Clear",
                style = {
                    width = 80f
                }
            });
        }

        private void CreateTreeElements(VisualElement _root) {
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<Command>();

            List<Type> commandTypes = new List<Type>();

            if (searchValue == null || searchValue == "") {
                commandTypes.AddRange(types);
            }
            else {
                string[] searchSplits = ObjectNames.NicifyVariableName(searchValue).ToLower().Split(' ');
                foreach (var type in types) {
                    bool found = type.Name.ToLower().Contains(searchValue.ToLower());
                    if (!found) {
                        string nicifyLowerName = ObjectNames.NicifyVariableName(type.Name).ToLower();
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
                        commandTypes.Add(type);
                    }
                }
            }

            commandTypes.Remove(typeof(BuildableCommand));
            commandTypes.Remove(typeof(BuildablePropertyCommand));
            commandTypes.Remove(typeof(PropertyCommand));

            rootTreeElement = new CommandTreeElement("Commands (" + commandTypes.Count + ")", _root);

            foreach (Type commandType in commandTypes) {
                if (commandType.Namespace == null) {
                    rootTreeElement.AddCommand(commandType);
                }
                else {
                    string[] assemblySplits = commandType.Namespace.Split('.');
                    CommandTreeElement currentTreeElement = rootTreeElement;
                    for (int i = 0; i < assemblySplits.Length; i++) {
                        currentTreeElement = currentTreeElement.EnsureChild(assemblySplits[i]);
                    }
                    currentTreeElement.AddCommand(commandType);
                }
            }
        }

    }
}