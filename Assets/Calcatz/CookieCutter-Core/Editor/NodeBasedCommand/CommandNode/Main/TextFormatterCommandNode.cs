using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {

    public class TextFormatterCommandNode : CommandNode {

        private Config config;
        private System.Action<ConnectionPoint, ConnectionPoint> onRemoveConnection;
        protected System.Action<ConnectionPoint, ConnectionPoint, int> onSwapInConnections;

        public TextFormatterCommandNode(CommandData _commandData, Command _command, Vector2 _position, Config _nodeConfig) 
            : base(_commandData, _command, _position, 220, 50, _nodeConfig) {
            tooltip = "To put arguments into the Text, put {<argument-index>} inside the Text, for example {0}.";
            CommandNodeConfig commandNodeConfig = (CommandNodeConfig)_nodeConfig;
            config = commandNodeConfig;
            onRemoveConnection = commandNodeConfig.onRemovePropertyConnection;
            onSwapInConnections = commandNodeConfig.onSwapInConnections;

            for (int i = 0; i < _command.inputIds.Count; i++) {
                AddPropertyInPoint<PropertyConnectionPoint>(_nodeConfig);
            }
        }

        protected void AddArgument() {
            Undo.RecordObject(commandData.targetObject, "add argument");
            ITextFormatterCommand textFormatterCommand = (ITextFormatterCommand)GetCommand();
            textFormatterCommand.AddArgument();
            AddPropertyInPoint<PropertyConnectionPoint>(config);

            EditorUtility.SetDirty(commandData.targetObject);
            if (onRefreshNeeded != null) {
                onRefreshNeeded.Invoke();
            }
        }
        protected void RemoveArgument(int _index) {
            Undo.RecordObject(commandData.targetObject, "remove argument");
            ITextFormatterCommand textFormatterCommand = (ITextFormatterCommand)GetCommand();
            textFormatterCommand.RemoveArgument(_index);
            onRemoveConnection.Invoke(inPoints[_index + 1], null);
            inPoints.RemoveAt(_index + 1);

            EditorUtility.SetDirty(commandData.targetObject);
            if (onRefreshNeeded != null) {
                onRefreshNeeded.Invoke();
            }
        }

        protected virtual void SwapToUpper(int _index) {
            if (_index == 0) return;

            OnBeforeSwapToUpper(_index);

            Command textFormatterCommand = GetCommand();

            Command.ConnectionTarget lowerInConnection = textFormatterCommand.inputIds[_index]; //inputId 0 is main connection
            Command.ConnectionTarget upperInConnection = textFormatterCommand.inputIds[_index - 1];
            textFormatterCommand.inputIds[_index] = upperInConnection;
            textFormatterCommand.inputIds[_index - 1] = lowerInConnection;

            onSwapInConnections.Invoke(inPoints[_index + 1], inPoints[_index], _index + 1);
            EditorUtility.SetDirty(commandData.targetObject);
            if (onRefreshNeeded != null) {
                onRefreshNeeded.Invoke();
            }
        }
        protected virtual void OnBeforeSwapToUpper(int _index) {
            Undo.RecordObject(commandData.targetObject, "swap argument");
        }

        protected override void HandleFirstInPointCreation(Config _config) {
            base.HandleFirstInPointCreation(_config);
        }

        protected override void OnDrawContents(Vector2 _absolutePosition) {
            ITextFormatterCommand textFormatterCommand = (ITextFormatterCommand)GetCommand();

            DrawTextFieldLabel(_absolutePosition);

            EditorGUI.BeginChangeCheck();
            string newTemplateText = DrawTextFieldArea(_absolutePosition, textFormatterCommand);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(commandData.targetObject, "Modified text");
                textFormatterCommand.TemplateText = newTemplateText;
                EditorUtility.SetDirty(commandData.targetObject);
            }

            DrawArgumentsField(_absolutePosition, (Command)textFormatterCommand);
        }

        protected void DrawArgumentsField(Vector2 _absolutePosition, Command _textFormatterCommand) {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float height = singleLineHeight + verticalSpacing;
            for (int i = 0; i < _textFormatterCommand.inputIds.Count; i++) {
                DrawInPoint(_absolutePosition, _textFormatterCommand is PropertyCommand? i : i + 1, currentY);

                EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, 80, height), "Arg {" + i + "}", styles.label);

                if (GUI.Button(new Rect(_absolutePosition.x + 10 + 80, currentY + verticalSpacing, height, singleLineHeight), "X")) {
                    if (EditorUtility.DisplayDialog("Remove an Argument", "Are you sure you want remove an argument at index " + i + "?", "Yes", "No")) {
                        RemoveArgument(i);
                    }
                }
                bool guiEnabled = GUI.enabled;
                GUI.enabled = guiEnabled && i > 0;
                if (GUI.Button(new Rect(_absolutePosition.x + 10 + 80 + height, currentY + verticalSpacing, height, singleLineHeight), "↑")) {
                    SwapToUpper(i);
                }
                GUI.enabled = guiEnabled;
                AddRectHeight(height);
            }

            if (GUI.Button(new Rect(_absolutePosition.x + 10, currentY + verticalSpacing, rect.width - 20, singleLineHeight), "Add Argument")) {
                AddArgument();
            }
            AddRectHeight(height);
        }

        protected virtual void DrawTextFieldLabel(Vector2 _absolutePosition) {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, singleLineHeight), "Text:", styles.label);
            AddRectHeight(singleLineHeight);
        }

        protected virtual string DrawTextFieldArea(Vector2 _absolutePosition, ITextFormatterCommand textFormatterCommand) {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            string result = EditorGUI.TextArea(new Rect(_absolutePosition.x + 10, currentY, rect.width - 20, singleLineHeight * 2), textFormatterCommand.TemplateText, styles.wrappedTextArea);
            AddRectHeight(singleLineHeight * 2);
            return result;
        }

        public override bool FilterByContent(string _filter, out string _displayContent) {
            ITextFormatterCommand textFormatterCommand = (ITextFormatterCommand)command;
            if (_filter != null && _filter != "") {
                if (textFormatterCommand.TemplateText == null || !textFormatterCommand.TemplateText.ToLower().Contains(_filter.ToLower())) {
                    _displayContent = null;
                    return false;
                }
            }
            if (textFormatterCommand.TemplateText == null || textFormatterCommand.TemplateText == "") {
                _displayContent = nodeName + " (" + ((Command)textFormatterCommand).id + ")";
            }
            else {
                _displayContent = nodeName + ": " + textFormatterCommand.TemplateText.Substring(0, textFormatterCommand.TemplateText.Length > 21 ? 21 : textFormatterCommand.TemplateText.Length).Replace('\n', ' ');
            }
            return true;
        }
    }
}
