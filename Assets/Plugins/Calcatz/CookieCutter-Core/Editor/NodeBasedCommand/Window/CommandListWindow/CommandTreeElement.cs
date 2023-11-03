using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.CookieCutter {

    public class CommandTreeElement {

        public string name;

        private Dictionary<string, CommandTreeElement> children = new Dictionary<string, CommandTreeElement>();
        private List<Type> commandTypes = new List<Type>();

        private Foldout m_visualElement;
        public Foldout visualElement => m_visualElement;

        private VisualElement commandsVisualElement;

        public CommandTreeElement(string _name, VisualElement _root) {
            name = _name;
            m_visualElement = new Foldout() {
                value = true
            };
            m_visualElement.text = _name;

            commandsVisualElement = new VisualElement();
            m_visualElement.Add(commandsVisualElement);

            _root.Add(m_visualElement);
        }

        public CommandTreeElement EnsureChild(string _name) {
            if (!children.ContainsKey(_name)) {
                children.Add(_name, new CommandTreeElement(_name, visualElement));
                visualElement.Remove(commandsVisualElement);
                visualElement.Add(commandsVisualElement);
            }
            return children[_name];
        }

        public void AddCommand(Type _commandType) {
            commandTypes.Add(_commandType);
            VisualElement commandVE = new VisualElement() {
                style = {
                    flexDirection = FlexDirection.Row,
                    borderBottomColor = new StyleColor(new Color(0.5f, 0.5f, 0.5f, 1)),
                    borderBottomWidth = new StyleFloat(1),
                    marginTop = new StyleLength(new Length(5, LengthUnit.Pixel))
                }
            };
            commandsVisualElement.Add(commandVE);

            commandVE.Add(new Label(ObjectNames.NicifyVariableName(_commandType.Name)) {
                style = {
                    flexGrow = 1.0f
                }
            });

            var monoScriptContainer = new VisualElement() {
                style = {
                    width = 200f,
                    marginRight = new StyleLength(new Length(15, LengthUnit.Pixel))
                }
            };
            var monoScript = new ObjectField() {
                value = CommandNode.GetMonoScript(_commandType),
                objectType = typeof(MonoScript)
            };
            monoScript.RegisterValueChangedCallback(_e => {
                monoScript.value = _e.previousValue;
            });
            monoScriptContainer.Add(monoScript);
            commandVE.Add(monoScriptContainer);

            Foldout usageFoldout = new Foldout() {
                text = "Usage",
                style = {
                    width = 300f
                }
            };
            usageFoldout.value = false;
            commandVE.Add(usageFoldout);

            AddCommandDataUsage(_commandType, usageFoldout);

            usageFoldout.Add(new Button(() => {
                MonoScript scriptAsset = CommandNode.GetMonoScript(_commandType);
                if (scriptAsset != null) {
                    CommandFinderWindow.FindAssetsByCommandType(scriptAsset);
                }
            }) {
                text = "Find In Assets"
            });
        }

        private void AddCommandDataUsage(Type _commandType, Foldout _usageFoldout) {
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<CommandData>();
            foreach(var commandDataType in types) {
                var registry = CommandRegistry.GetInstance(commandDataType);
                if (registry.ContainsCommand(_commandType)) {
                    VisualElement commandDataVE = new VisualElement() {
                        style = {
                            borderTopColor = new StyleColor(new Color(0.5f, 0.5f, 0.5f, 1)),
                            borderTopWidth = new StyleFloat(1),
                            paddingTop = new StyleLength(new Length(5, LengthUnit.Pixel)),
                            paddingBottom = new StyleLength(new Length(2, LengthUnit.Pixel))
                        }
                    };
                    _usageFoldout.Add(commandDataVE);
                    commandDataVE.Add(new Label(ObjectNames.NicifyVariableName(commandDataType.Name)));
                }
            }

            if (_usageFoldout.childCount == 0) {
                var parent = _usageFoldout.parent;
                parent.Remove(_usageFoldout);
                string label;
                if (_commandType.IsGenericType) {
                    label = "Generic";
                }
                else if (_commandType.IsAbstract) {
                    label = "Abstract";
                }
                else {
                    label = "Unused or Used as Base-Class";
                }
                parent.Add(new Label(label) {
                    style = {
                        width = 300f
                    }
                });
            }
        }

    }

}
