using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Calcatz.CookieCutter {
    public class CommandNodesContainerLeftPane {

        private VisualElement m_leftPaneArea;
        private Button showLeftPaneButton;
        private float m_leftPaneWidth = 0;
        private bool showLeftPane = false;
        private Button saveButton;
        private VariableTable m_variableTable = new VariableTable();
        private TextField filter;
        private ListView commandList;

        private Type filteredType = null;
        private Action<bool> updateNodesAreaHandler;
        private Func<CommandData> commandData;
        private Func<Type> commandDataType;

        public bool saveAvailable {
            set {
                if (m_leftPaneArea != null) {
                    saveButton?.SetEnabled(value);
                }
            }
        }

        public VisualElement leftPaneArea => m_leftPaneArea;
        public float leftPaneWidth { get => m_leftPaneWidth; set => m_leftPaneWidth = value; }
        public VariableTable variableTable => m_variableTable;

        public void CreateGUI(VisualElement _root, Action<VisualElement> _onCreateLeftPane, Action<bool> _updateNodesAreaHandler, Func<CommandData> _commandData, Func<Type> _commandDataType) {
            updateNodesAreaHandler = _updateNodesAreaHandler;
            commandData = _commandData;
            commandDataType = _commandDataType;
            m_leftPaneArea = CreateLeftPaneArea();
            _root.Add(m_leftPaneArea);
            _onCreateLeftPane(m_leftPaneArea);
            _root.Add(CreateShowLeftPaneButton());
        }

        private Button CreateShowLeftPaneButton() {
            showLeftPaneButton = new Button(() => {
                ToggleLeftPane();
            }) {
                text = showLeftPane ? "-" : "+",
                style = {
                    position = Position.Absolute,
                    left = new StyleLength(new Length(showLeftPane? leftPaneWidth : 0, LengthUnit.Pixel)),
                    bottom = new StyleLength(new Length(0, LengthUnit.Pixel))
                }
            };
            return showLeftPaneButton;
        }

        private void ToggleLeftPane() {
            showLeftPane = !showLeftPane;
            if (showLeftPane) {
                showLeftPaneButton.text = "-";
                m_leftPaneArea.experimental.animation.Start(m_leftPaneArea.style.width.value.value, leftPaneWidth, 150, (_e, _value) => {
                    m_leftPaneArea.style.opacity = new StyleFloat(_value / leftPaneWidth);
                    //leftPaneArea.style.left = new StyleLength(new Length(_value - leftPaneWidth, LengthUnit.Pixel));
                    m_leftPaneArea.style.width = new StyleLength(new Length(_value, LengthUnit.Pixel));
                    showLeftPaneButton.style.left = new StyleLength(new Length(_value, LengthUnit.Pixel));
                    updateNodesAreaHandler(true);
                }).Ease(Easing.OutCirc);
            }
            else {
                showLeftPaneButton.text = "+";
                m_leftPaneArea.experimental.animation.Start(m_leftPaneArea.style.width.value.value, 0, 150, (_e, _value) => {
                    m_leftPaneArea.style.opacity = new StyleFloat(_value / leftPaneWidth);
                    //leftPaneArea.style.left = new StyleLength(new Length(_value - leftPaneWidth, LengthUnit.Pixel));
                    m_leftPaneArea.style.width = new StyleLength(new Length(_value, LengthUnit.Pixel));
                    showLeftPaneButton.style.left = new StyleLength(new Length(_value, LengthUnit.Pixel));
                    updateNodesAreaHandler(true);
                }).Ease(Easing.InCubic);
            }
        }

        private VisualElement CreateLeftPaneArea() {
            return new VisualElement() {
                name = "left-pane",
                style = {
                    opacity = new StyleFloat(showLeftPane? 1f : 0f),
                    backgroundColor = new StyleColor(NodesContainer.backgroundColor),
                    position = new StyleEnum<Position>(Position.Absolute),
                    left = new StyleLength(new Length(0, LengthUnit.Pixel)),
                    width =  new StyleLength(new Length(showLeftPane? leftPaneWidth : 0, LengthUnit.Pixel)),
                    top = new StyleLength(new Length(0, LengthUnit.Pixel)),
                    bottom = new StyleLength(new Length(0, LengthUnit.Pixel)),
                    paddingBottom = new StyleLength(new Length(CommandNode.verticalSpacing, LengthUnit.Pixel)),
                    paddingLeft = new StyleLength(new Length(CommandNode.verticalSpacing, LengthUnit.Pixel)),
                    paddingRight = new StyleLength(new Length(CommandNode.verticalSpacing, LengthUnit.Pixel)),
                    paddingTop = new StyleLength(new Length(CommandNode.verticalSpacing, LengthUnit.Pixel)),
                    borderRightWidth = new StyleFloat(2),
                    borderRightColor = new StyleColor(NodesContainer.backgroundColor * 0.5f)
                }
            };
        }

        public VisualElement CreateSaveSettingsArea(Action<bool> _reloadNodesHandler, Action _saveHandler) {
            VisualElement saveSettings = new VisualElement() {
                style = {
                    flexDirection = FlexDirection.Row
                }
            };
            Button loadButton = new Button(()=>_reloadNodesHandler(true)) {
                text = "Load",
                style = {
                    flexGrow = 1
                }
            };
            saveButton = new Button(_saveHandler) {
                text = "Save",
                style = {
                    flexGrow = 1
                }
            };
            saveButton.SetEnabled(false);
            saveSettings.Add(loadButton);
            saveSettings.Add(saveButton);
            return saveSettings;
        }

        #region VARIABLE
        public void CreateVariableMenu(VisualElement _commandDataProperties, Func<Vector2> _realPositionOffsetGetter, Action<Command> _addNewCommandHandler, bool _showCreateCommandButton) {
            m_variableTable.SetShowCreateCommandButton(_showCreateCommandButton);
            m_variableTable.CreateElement(_commandDataProperties, ()=>(IVariableUser)commandData());
            m_variableTable.SetCreateCommandHandler((_variableId, _isSetter) => {
                Command variableCommand;
                if (_isSetter)
                    variableCommand = new SetVariableCommand() { variableId = _variableId };
                else
                    variableCommand = new VariableCommand() { variableId = _variableId };
                variableCommand.nodePosition = -_realPositionOffsetGetter();
                _addNewCommandHandler.Invoke(variableCommand);
            });
            m_variableTable.visualElement.style.maxHeight = new StyleLength(new Length(300, LengthUnit.Pixel));
            m_variableTable.visualElement.style.marginTop = new StyleLength(new Length(10, LengthUnit.Pixel));
        }
        #endregion

        #region COMMAND-LIST
        public void CreateCommandListMenu(VisualElement commandDataProperties, List<Node> _nodes, Action<CommandNode> _goToNodeHandler) {
            VisualElement commandListArea = new VisualElement() {
                style = {
                    flexGrow = 1f
                }
            };
            commandDataProperties.Add(commandListArea);
            commandListArea.Add(new Label("Command List Filter:") {
                style = {
                    whiteSpace = WhiteSpace.Normal
                }
            });
            filter = new TextField();
            filter.RegisterValueChangedCallback(_event => {
                RefreshCommandList(_nodes, _goToNodeHandler);
            });
            commandListArea.Add(filter);

            CreateTypeFilter(commandListArea, _nodes, _goToNodeHandler);

            commandList = CreateCommandList(_nodes, _goToNodeHandler);
            commandListArea.Add(commandList);
        }

        private void CreateTypeFilter(VisualElement _leftPane, List<Node> _nodes, Action<CommandNode> _goToNodeHandler) {
            List<Type> availableTypes = new List<Type>() { null };
            TraverseAvailableCommandTypes((_type, _registry) => {
                if (_registry.allowCreateNode) {
                    availableTypes.Add(_type);
                }
            });
            int currentIndex = availableTypes.IndexOf(filteredType);

            Func<Type, string> listItemCallback = _type => {
                if (_type == null) return "-All-";
                CommandRegistry.Registry reg = CommandRegistry.GetRegistry(commandDataType(), _type);
                string[] split = reg.pathName.Split('/');
                return split[split.Length - 1];
            };
            Func<Type, string> selectedValueCallback = _type => {
                filteredType = _type;
                RefreshCommandList(_nodes, _goToNodeHandler);
                return listItemCallback(_type);
            };

            PopupField<Type> typeFilter = new PopupField<Type>(availableTypes, currentIndex, selectedValueCallback, listItemCallback);
            _leftPane.Add(typeFilter);
        }

        private ListView CreateCommandList(List<Node> _nodes, Action<CommandNode> _goToNodeHandler) {
            List<CommandNode> filteredNodes = new List<CommandNode>();
            List<string> nodeContents = new List<string>();
            foreach (CommandNode node in _nodes) {
                if (filteredType != null && node.GetCommand().GetType() != filteredType) continue;
                string nodeContent;
                if (node.FilterByContent(filter.text, out nodeContent)) {
                    filteredNodes.Add(node);
                    nodeContents.Add(nodeContent);
                }
            }

            Action<VisualElement, int> bindItem = (e, i) => {
                (e.ElementAt(0) as Label).text = nodeContents[i];
            };

            commandList = new ListView(filteredNodes, 16, MakeCommandItem, bindItem) {
                name = "command-list",
                style = {
                    marginTop = new StyleLength(new Length(CommandNode.verticalSpacing, LengthUnit.Pixel)),
                    flexGrow = 1f,
                    flexShrink = 0,
                    flexBasis = 0,
                    backgroundColor = new StyleColor(NodesContainer.backgroundColor * 0.5f)
                }
            };

            commandList.selectionType = SelectionType.Single;
            //commandList.onItemChosen += obj => GoToNode((CommandNode)obj);

#if UNITY_2022_2_OR_NEWER
            commandList.selectionChanged += objects => {
#elif UNITY_2021_2_OR_NEWER
            commandList.onSelectionChange += objects => {
#else
            commandList.onSelectionChanged += objects => {
#endif
                foreach (object obj in objects) {
                    _goToNodeHandler((CommandNode)obj);
                    break;
                }
            };

            return commandList;
        }

        private static VisualElement MakeCommandItem() {
            var box = new VisualElement() {
                style = {
                        flexDirection = FlexDirection.Row,
                        flexGrow = 1f,
                        flexShrink = 0,
                        flexBasis = 0
                    }
            };
            box.Add(new Label() {
                style = {
                        left = new StyleLength(new Length(5, LengthUnit.Pixel))
                    }
            });
            return box;
        }

        public void RefreshCommandList(List<Node> _nodes, Action<CommandNode> _goToNodeHandler) {
            if (commandList != null && m_leftPaneArea != null) {
                VisualElement parent = commandList.parent;
                if (parent != null) {
                    parent.Remove(commandList);
                    parent.Add(CreateCommandList(_nodes, _goToNodeHandler));
                }
            }
        }

        private void TraverseAvailableCommandTypes(Action<Type, CommandRegistry.Registry> _commandTypeHandler) {
            if (commandDataType == null || commandDataType() == null) return;
            HashSet<KeyValuePair<Type, CommandRegistry.Registry>> mainCommands;
            HashSet<KeyValuePair<Type, CommandRegistry.Registry>> propertyCommands;
            CommandRegistry.GetCommands(commandDataType(), out mainCommands, out propertyCommands);
            foreach (KeyValuePair<Type, CommandRegistry.Registry> kvp in mainCommands) {
                _commandTypeHandler.Invoke(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<Type, CommandRegistry.Registry> kvp in propertyCommands) {
                _commandTypeHandler.Invoke(kvp.Key, kvp.Value);
            }
        }
        #endregion
    }
}
