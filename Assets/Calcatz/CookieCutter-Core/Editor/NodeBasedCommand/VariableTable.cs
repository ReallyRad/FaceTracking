using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.CookieCutter {
    [System.Serializable]
    public class VariableTable {

        [SerializeField] private UITKTable table;
        private Func<IVariableUser> variableUserGetter;
        private Action<int, bool> createCommandHandler;
        private Func<IDictionary> dictionaryGetter;
        private bool showCreateCommandButton;
        private bool showVariableId;

        private UITKTable.RowSearchMethod rowSearchMethod = null;

        public VisualElement visualElement => table.visualElement;

        public static string GetProperPrimitiveTypeName(Type _type) {
            if (_type == typeof(int)) return "Integer";
            if (_type == typeof(float)) return "Float";
            return _type.Name;
        }

        private static readonly Dictionary<Type, Func<VisualElement, UnityEngine.Object, CommandData.Variable, VisualElement>> variableValueHandlers = new Dictionary<Type, Func<VisualElement, UnityEngine.Object, CommandData.Variable, VisualElement>>() {
            {typeof(bool), (_element, _targetObject, _variable) => {
                Toggle field = new Toggle() {
                    value = (bool)_variable.value,
                    style = {
                        flexGrow = 1,
                        flexBasis = 0
                    }
                };
                field.RegisterValueChangedCallback(_event => {
                    Undo.RecordObject(_targetObject, "Changed variable value");
                    _variable.value = _event.newValue;
                    EditorUtility.SetDirty(_targetObject);
                });
                return field;
            }},

            {typeof(int), (_element, _targetObject, _variable) => {
                IntegerField field = new IntegerField() {
                    value = (int)_variable.value,
                    style = {
                        flexGrow = 1,
                        flexBasis = 0
                    }
                };
                field.RegisterValueChangedCallback(_event => {
                    Undo.RecordObject(_targetObject, "Changed variable value");
                    _variable.value = _event.newValue;
                    EditorUtility.SetDirty(_targetObject);
                });
                return field;
            }},

            {typeof(float), (_element, _targetObject, _variable) => {
                FloatField field = new FloatField() {
                    value = (float)_variable.value,
                    style = {
                        flexGrow = 1,
                        flexBasis = 0
                    }
                };
                field.RegisterValueChangedCallback(_event => {
                    Undo.RecordObject(_targetObject, "Changed variable value");
                    _variable.value = _event.newValue;
                    EditorUtility.SetDirty(_targetObject);
                });
                return field;
            }},

            {typeof(string), (_element, _targetObject, _variable) => {
                TextField field = new TextField() {
                    value = (string)_variable.value,
                    style = {
                        flexGrow = 1,
                        flexBasis = 0
                    }
                };
                field.RegisterValueChangedCallback(_event => {
                    Undo.RecordObject(_targetObject, "Changed variable value");
                    _variable.value = _event.newValue;
                    EditorUtility.SetDirty(_targetObject);
                });
                return field;
            }},

            {typeof(Vector3), (_element, _targetObject, _variable) => {
                Vector3Field field = new Vector3Field() {
                    value = (Vector3)_variable.value,
                    style = {
                        flexGrow = 1,
                        flexBasis = 0
                    }
                };
                foreach(var axisParent in field.Children()) {
                    axisParent.style.flexShrink = 1f;
                    axisParent.style.flexDirection = FlexDirection.Column;
                    foreach (var row in axisParent.Children()) {
                        foreach (var elm in row.Children()) {
                            elm.style.marginLeft = 3;
                            break;
                        }
                        break;
                    }
                    break;
                }
                field.RegisterValueChangedCallback(_event => {
                    Undo.RecordObject(_targetObject, "Changed variable value");
                    _variable.value = _event.newValue;
                    EditorUtility.SetDirty(_targetObject);
                });
                return field;
            }}
        };

        public void CreateElement(VisualElement _parent, Func<IVariableUser> _variableUserGetter) {
            variableUserGetter = _variableUserGetter;
            table = new UITKTable();
            table
                .SetRowSearchMethod(rowSearchMethod)
                .SetElementHeight(25)
                .SetReorderable(false)
                .SetSize(3, 0)
                .SetColumnFlexes(0.4f, 0.9f, 0.7f)
                .SetHeaderTexts("Type", "Variable Name", "Initial Value")
                .SetHeaderFontSize(11)
                .SetOnCreateCell(0, (_element, _row) => {
                    int[] keys = variableUserGetter().variables.Keys.ToArray();
                    CommandData.Variable variable = variableUserGetter().variables[keys[_row]];
                    Type type = variable.value == null ? typeof(string) : variable.value.GetType();
                    Label typeLabel = new Label() {
                        text = GetProperPrimitiveTypeName(type),
                        style = {
                            flexGrow = 0.9f,
                            flexShrink = 0.1f
                        }
                    };
                    _element.Add(typeLabel);
                })
                .SetOnCreateCell(1, (_element, _row) => {
                    int[] keys = variableUserGetter().variables.Keys.ToArray();
                    CommandData.Variable variable = variableUserGetter().variables[keys[_row]];

                    if (showVariableId) {
                        _element.Add(new Label(keys[_row] + ": ") {
                            tooltip = "Variable ID"
                        });
                    }

                    TextField variableName = new TextField() {
                        value = variable.variableName,
                        isDelayed = true,
                        style = {
                            flexGrow = 0.9f,
                            flexShrink = 0.1f
                        }
                    };
                    variableName.RegisterValueChangedCallback(_event => {
                        Undo.RecordObject(variableUserGetter().targetObject, "Changed variable name");
                        variable.variableName = _event.newValue;
                        EditorUtility.SetDirty(variableUserGetter().targetObject);
                    });
                    _element.Add(variableName);
                })
                .SetOnCreateCell(2, (_element, _row) => {
                    int[] keys = variableUserGetter().variables.Keys.ToArray();
                    int id = keys[_row];
                    CommandData.Variable variable = variableUserGetter().variables[id];
                    Type type = variable.value == null ? typeof(string) : variable.value.GetType();
                    VisualElement field = variableValueHandlers[type](_element, variableUserGetter().targetObject, variable);
                    _element.Add(field);

                    Button addToCommandButton = new Button(() => {
                        if (createCommandHandler != null) {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Get"), false, ()=> createCommandHandler.Invoke(id, false));
                            menu.AddItem(new GUIContent("Set"), false, () => createCommandHandler.Invoke(id, true));
                            menu.ShowAsContext();
                        }
                    }) {
                        text = ">",
                        style = {
                            width = new StyleLength(new Length(20, LengthUnit.Pixel))
                        }
                    };
                    addToCommandButton.style.display = showCreateCommandButton ? DisplayStyle.Flex : DisplayStyle.None;
                    _element.Add(addToCommandButton);
                })
                .CreateElement();

            if (variableUserGetter() != null) {
                table.BindDictionary(variableUserGetter().targetObject, dictionaryGetter);
                table.RefreshRows();
            }

            _parent.Add(visualElement);
            CreateAddNewVariableMenu(_parent);
        }

        private void CreateAddNewVariableMenu(VisualElement _commandDataProperties) {
            Foldout foldout = new Foldout() {
                text = "Add New Variable",
                value = false,
                style = {
                    marginBottom = new StyleLength(new Length(10, LengthUnit.Pixel)),
                    height = new StyleLength(new Length(40, LengthUnit.Pixel)),
                    flexGrow = 1,
                    flexShrink = 0,
                    flexBasis = new StyleLength(new Length(40, LengthUnit.Pixel))
                }
            };
            _commandDataProperties.Add(foldout);
            VisualElement addVariable = new VisualElement() {
                style = {
                    flexDirection = FlexDirection.Row,
                    height = new StyleLength(new Length(20, LengthUnit.Pixel)),
                    marginTop = new StyleLength(new Length(2, LengthUnit.Pixel))
                }
            };
            foldout.Add(addVariable);

            Type selectedType = null;

            TextField nameField = new TextField() {
                value = "New Variable",
                style = {
                    flexGrow = 1
                }
            };

            Button addButton = new Button(() => {
                if (variableUserGetter() != null && selectedType != null) {
                    object value;
                    if (selectedType == typeof(string)) {
                        value = "";
                    }
                    else if (selectedType.IsValueType) {
                        value = Activator.CreateInstance(selectedType);
                    }
                    else {
                        value = null;
                    }
                    if (variableUserGetter().variables == null) {
                        variableUserGetter().variables = new Dictionary<int, CommandData.Variable>();
                        table.BindDictionary(variableUserGetter().targetObject, () => variableUserGetter().variables);
                    }
                    int newId = 1;
                    while (variableUserGetter().variables.ContainsKey(newId)) {
                        newId++;
                    }
                    Undo.RecordObject(variableUserGetter().targetObject, "Added a new variable");
                    variableUserGetter().variables.Add(newId, new CommandData.Variable() { variableName = nameField.value, value = value });
                    EditorUtility.SetDirty(variableUserGetter().targetObject);
                    table.RefreshRows();
                }
            }) {
                text = "Add"
            };
            addButton.SetEnabled(variableUserGetter != null);

            nameField.RegisterValueChangedCallback(_event => {
                if (variableUserGetter != null) {
                    addButton.SetEnabled(true);
                }
                else {
                    addButton.SetEnabled(false);
                }
            });

            List<Type> types = new List<Type>() {
                typeof(bool),
                typeof(int),
                typeof(float),
                typeof(string),
                typeof(Vector3)
            };

            Func<Type, string> listItemCallback = _type => {
                return VariableTable.GetProperPrimitiveTypeName(_type);
            };
            Func<Type, string> selectedValueCallback = _type => {
                selectedType = _type;
                return listItemCallback(_type);
            };

            PopupField<Type> typeField = new PopupField<Type>(types, 0, selectedValueCallback, listItemCallback);

            addVariable.Add(new Label("Name:"));
            addVariable.Add(nameField);
            addVariable.Add(typeField);
            addVariable.Add(addButton);
        }

        public VariableTable SetCreateCommandHandler(Action<int, bool> _createCommandHandler) {
            createCommandHandler = _createCommandHandler;
            return this;
        }

        public VariableTable BindDictionary(UnityEngine.Object _targetObject, Func<IDictionary> _dictionaryGetter) {
            table.BindDictionary(_targetObject, _dictionaryGetter);
            return this;
        }

        public VariableTable SetShowCreateCommandButton(bool _show) {
            showCreateCommandButton = _show;
            return this;
        }

        public VariableTable SetShowVariableId(bool _show) {
            showVariableId = _show;
            return this;
        }

        public VariableTable SetRowSearchMethod(UITKTable.RowSearchMethod _rowSearchMethod) {
            rowSearchMethod = _rowSearchMethod;
            return this;
        }

        public void RefreshRows() {
            table.RefreshRows();
        }

        public void BindVariables(Func<IDictionary> _dictionaryGetter) {
            dictionaryGetter = _dictionaryGetter;
            if (table != null) {
                if (variableUserGetter() != null) {
                    table.BindDictionary(variableUserGetter().targetObject, dictionaryGetter);
                    table.RefreshRows();
                }
            }
        }
    }
}
