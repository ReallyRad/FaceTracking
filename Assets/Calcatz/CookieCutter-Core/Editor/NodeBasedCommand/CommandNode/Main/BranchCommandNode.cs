using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {

    [CustomCommandNodeDrawer(typeof(BranchCommand))]
    public class BranchCommandNode : CommandNode {

        private Config config;
        private System.Action<ConnectionPoint, ConnectionPoint> onRemoveConnection;
        protected System.Action<ConnectionPoint, ConnectionPoint, int> onSwapInConnections;

        public BranchCommandNode(CommandData _commandData, Command _command, Vector2 _position, Config _nodeConfig) 
            : base(_commandData, _command, _position, 220, 50, _nodeConfig) {
            nodeName = "Branch";
            tooltip = "Evaluates each condition in order, from top to bottom. The first condition met will be executed, while the rest will be skipped.";
            CommandNodeConfig commandNodeConfig = (CommandNodeConfig)_nodeConfig;
            config = commandNodeConfig;
            onRemoveConnection = commandNodeConfig.onRemovePropertyConnection;
            onSwapInConnections = commandNodeConfig.onSwapInConnections;

            AddPropertyInPoint<bool>(config);

            for (int i=1; i< _command.nextIds.Count; i++) {
                AddPropertyInPoint<bool>(config);
                AddMainOutPoint(config);
            }
        }

        protected void AddBranch() {
            OnBranchAdded();
            AddPropertyInPoint<bool>(config);
            AddMainOutPoint(config);

            EditorUtility.SetDirty(commandData.targetObject);
            if (onRefreshNeeded != null) {
                onRefreshNeeded.Invoke();
            }
        }
        protected virtual void OnBranchAdded() {
            Undo.RecordObject(commandData.targetObject, "Added a branch");
            BranchCommand branchCommand = (BranchCommand)GetCommand();
            branchCommand.AddBranch();
        }

        protected void RemoveBranch(int _index) {
            OnBranchRemoved(_index);
            onRemoveConnection.Invoke(inPoints[_index + 1], outPoints[_index]);
            inPoints.RemoveAt(_index + 1);
            outPoints.RemoveAt(_index);

            EditorUtility.SetDirty(commandData.targetObject);
            if (onRefreshNeeded != null) {
                onRefreshNeeded.Invoke();
            }
        }
        protected virtual void OnBranchRemoved(int _index) {
            Undo.RecordObject(commandData.targetObject, "Removed a branch");
            BranchCommand branchCommand = (BranchCommand)GetCommand();
            branchCommand.RemoveBranch(_index);
        }

        protected virtual void SwapToUpper(int _index) {
            if (_index == 0) return;

            OnBeforeSwapToUpper(_index);

            BranchCommand branchCommand = (BranchCommand)GetCommand();

            List<Command.ConnectionTarget> lowerOutConnection = branchCommand.nextIds[_index];
            List<Command.ConnectionTarget> upperOutConnection = branchCommand.nextIds[_index - 1];
            branchCommand.nextIds[_index] = upperOutConnection;
            branchCommand.nextIds[_index - 1] = lowerOutConnection;

            Command.ConnectionTarget lowerInConnection = branchCommand.inputIds[_index]; //inputId 0 is main connection
            Command.ConnectionTarget upperInConnection = branchCommand.inputIds[_index - 1];
            branchCommand.inputIds[_index] = upperInConnection;
            branchCommand.inputIds[_index - 1] = lowerInConnection;

            bool lowerDefaultValue = branchCommand.defaultValues[_index];
            bool upperDefaultValue = branchCommand.defaultValues[_index - 1];
            branchCommand.defaultValues[_index] = upperDefaultValue;
            branchCommand.defaultValues[_index - 1] = lowerDefaultValue;

            onSwapInConnections.Invoke(inPoints[_index + 1], inPoints[_index], _index + 1);

            EditorUtility.SetDirty(commandData.targetObject);
            if (onRefreshNeeded != null) {
                onRefreshNeeded.Invoke();
            }
        }
        protected virtual void OnBeforeSwapToUpper(int _index) {
            Undo.RecordObject(commandData.targetObject, "Swapped a branch");
        }

        protected override void OnDrawTitle(Vector2 _offset) {
            if (inPoints.Count > 0) DrawInPoint(_offset, 0, currentY);
        }

        protected override void OnDrawContents(Vector2 _absolutePosition) {
            BranchCommand branchCommand = (BranchCommand)GetCommand();
            float halfWidth = (rect.width - 20) / 2;
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float height = singleLineHeight + verticalSpacing;

            EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, 40, singleLineHeight), new GUIContent("Notes:", "Optional, only contains description and has no effect in runtime."), styles.label);

            EditorGUI.BeginChangeCheck();
            string newNotes = EditorGUI.TextArea(new Rect(_absolutePosition.x + 10 + 40, currentY, rect.width - 20 - 40, singleLineHeight * 2), branchCommand.notes);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(commandData.targetObject, "Modified branch notes");
                branchCommand.notes = newNotes;
                EditorUtility.SetDirty(commandData.targetObject);
            }
            AddRectHeight(singleLineHeight * 2);

            for (int i = 0; i < branchCommand.nextIds.Count; i++) {
                DrawInPoint(_absolutePosition, i + 1, currentY);
                DrawOutPoint(_absolutePosition, i, currentY);

                if (branchCommand.inputIds[i].targetId == 0) {
                    bool newDefaultValue = EditorGUI.ToggleLeft(new Rect(_absolutePosition.x + 10, currentY, 80, height), "Default " + i, branchCommand.defaultValues[i], styles.label);
                    if (newDefaultValue != branchCommand.defaultValues[i]) {
                        Undo.RecordObject(commandData.targetObject, "Modified branch default value");
                        branchCommand.defaultValues[i] = newDefaultValue;
                        EditorUtility.SetDirty(commandData.targetObject);
                    }
                }
                else {
                    EditorGUI.LabelField(new Rect(_absolutePosition.x + 10, currentY, 80, height), "Boolean " + i, styles.label);
                }
                if (GUI.Button(new Rect(_absolutePosition.x + 10 + 80, currentY + verticalSpacing, height, singleLineHeight), "X")) {
                    if (EditorUtility.DisplayDialog("Remove a Branch", "Are you sure you want remove a branch of condition #" + i + "?", "Yes", "No")) {
                        RemoveBranch(i);
                    }
                }
                bool guiEnabled = GUI.enabled;
                GUI.enabled = guiEnabled && i > 0;
                if (GUI.Button(new Rect(_absolutePosition.x + 10 + 80 + height, currentY + verticalSpacing, height, singleLineHeight), "↑")) {
                    SwapToUpper(i);
                }
                GUI.enabled = guiEnabled;
                EditorGUI.LabelField(new Rect(_absolutePosition.x + 10 + halfWidth, currentY, halfWidth, height), "Then " + i, styles.labelRight);
                AddRectHeight(height);
            }

            if (GUI.Button(new Rect(_absolutePosition.x + 10, currentY + verticalSpacing, rect.width - 20, singleLineHeight), "Add Branch")) {
                AddBranch();
            }
            AddRectHeight(height);
        }

        public override bool FilterByContent(string _filter, out string _displayContent) {
            if (base.FilterByContent(_filter, out _displayContent)) {
                return true;
            }
            BranchCommand branchCommand = (BranchCommand)command;
            if (_filter != null && _filter != "") {
                if (branchCommand.notes == null || !branchCommand.notes.ToLower().Contains(_filter.ToLower())) {
                    _displayContent = null;
                    return false;
                }
            }
            if (branchCommand.notes == null || branchCommand.notes == "") {
                _displayContent = nodeName + " (" + branchCommand.id + ")";
            }
            else {
                _displayContent = nodeName + ": " + branchCommand.notes.Substring(0, branchCommand.notes.Length > 21 ? 21 : branchCommand.notes.Length).Replace('\n', ' ');
            }
            return true;
        }
    }
}
