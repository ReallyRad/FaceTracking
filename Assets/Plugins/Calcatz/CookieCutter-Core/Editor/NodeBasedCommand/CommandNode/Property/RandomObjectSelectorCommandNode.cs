using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Calcatz.CookieCutter.Command;

namespace Calcatz.CookieCutter {

    [CustomCommandNodeDrawer(typeof(RandomObjectSelectorCommand))]
    public class RandomObjectSelectorCommandNode : CommandNode {

        private static string typeSearchTerm = "";

        private DirectObjectReorderableList<UnityEngine.Object> objectList;

        public RandomObjectSelectorCommandNode(CommandData _commandData, Command _command, Vector2 _position, Config _nodeConfig)
            : base(_commandData, _command, _position, 250, 50, _nodeConfig) {
            nodeName = "Random Object Selector";
            tooltip = "Gets 1 random object from the specified list of objects.";
            var randomObjectCommand = (RandomObjectSelectorCommand)_command;

            objectList = new DirectObjectReorderableList<Object>(_commandData.targetObject, ref randomObjectCommand.objectList, "", 20);
            objectList.customOnDrawElement = OnDrawListElement;
        }

        private void OnDrawListElement(Rect _rect, List<UnityEngine.Object> _list, int _index, bool _active, bool _focused) {
            var randomObjectCommand = (RandomObjectSelectorCommand)command;
            EditorGUI.BeginChangeCheck();
            _rect.y+=2;
            _rect.height -= 4;
            var newObj = EditorGUI.ObjectField(_rect, _list[_index], randomObjectCommand.typeToSelect == null ? typeof(UnityEngine.Object) : randomObjectCommand.typeToSelect, true);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(commandData.targetObject, "modify object list");
                _list[_index] = newObj;
                EditorUtility.SetDirty(commandData.targetObject);
            }
        }

        protected override void HandleFirstInPointCreation(Config _config) { }

        protected override void HandleFirstOutPointCreation(Config _config) {
            if (command.nextIds.Count < 1) command.nextIds.Add(new List<ConnectionTarget>());
            AddPropertyOutPoint<UnityEngine.Object>(_config);
        }

        protected override void OnDrawContents(Vector2 _absolutePosition) {
            var randomObjectCommand = (RandomObjectSelectorCommand)command;
            var labelWidth = EditorGUIUtility.labelWidth;
            UnityEditor.EditorGUIUtility.labelWidth = 80;
            
            string displayType = randomObjectCommand.typeToSelect == null ? 
                "(Please Select a Type)" : ObjectNames.NicifyVariableName(randomObjectCommand.typeToSelect.Name);
            GenerateSingleLineLabeledRect(_absolutePosition, out var typeLabelRect, out var typeValueRect);
            GUI.Label(typeLabelRect, "Object Type");
            if (GUI.Button(typeValueRect, displayType, EditorStyles.popup)) {
                var index = -1;
                var allTypes = UnityEditor.TypeCache.GetTypesDerivedFrom<UnityEngine.Object>();

                var filteredTypes = allTypes.Where(type => {
                    return !type.Assembly.FullName.Contains("Editor");
                });

                var contextMenu = new GenericMenu();
                int i = 0;
                foreach (var type in filteredTypes) {
                    if (type == randomObjectCommand.typeToSelect) {
                        index = i;
                    }
                    var newType = type;
                    contextMenu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(newType.Name)), index == i, () => {
                        Undo.RecordObject(commandData.targetObject, "change object type");
                        randomObjectCommand.typeToSelect = newType;
                        EditorUtility.SetDirty(commandData.targetObject);
                    });
                    i++;
                }

                GenericMenuPopup.Show(contextMenu, displayType, typeValueRect.position, null, typeSearchTerm, _searchTerm => {
                    typeSearchTerm = _searchTerm;
                });
            }

            EditorGUIUtility.labelWidth = labelWidth;
            AddRectHeight(typeLabelRect.height);

            var listRect = GenerateLineRect(_absolutePosition);
            listRect.height = objectList.GetHeight();
            objectList.DoList(listRect);
            AddRectHeight(listRect.height);
        }

    }

}