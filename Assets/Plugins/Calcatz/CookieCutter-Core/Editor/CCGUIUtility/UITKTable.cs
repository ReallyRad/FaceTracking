using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using Object = UnityEngine.Object;

namespace Calcatz.CookieCutter {
    [System.Serializable]
    public class UITKTable {

#region PROPERTIES
        private static UITKTable focusedTable = null;
        public static UITKTable FocusedTable {
            get { return focusedTable; }
        }

        private const float borderRadius = 3f;
        private const float outlineWidth = 1.3f;

        private VisualElement m_visualElement;
        private VisualElement sliderSpace;
        private ScrollView scrollView;
        private VisualElement m_emptyRow;
        private List<VisualElement> rowElements;
        private int row = 1;
        private int column = 1;
        private int elementHeight = 20;
        private float padding = 3;
        private float headerFontSize = 12;
        private Action<VisualElement, int>[] onCreateCell;
        private Action<int> onDoubleClick;
        private Action<int> onSelect;
        private string[] headerTexts;
        private bool selectable = true;
        private bool reorderable = true;
        private int minSelection = 0;
        private int maxSelection = int.MaxValue;
        private bool multiSelect = true;
        private bool foldable = false;
        private List<int> selectedRows = new List<int>();
        private float[] columnFlexes;

        private UnityEngine.Object targetObject = null;
        private Func<IList> listGetter;
        private Func<IDictionary> dictionaryGetter;
        private Action<DropdownMenu> contextMenuHandler;
        private TableEvent tableEvent;
        private Action onClickAddButton;

        private int[] frozenRows;

        public delegate bool RowSearchMethod(string _searchText, VisualElement _visualElement);

        private RowSearchMethod rowSearchMethod = null;

        [SerializeField] private string searchText = "";

        private bool TryUndo(string _name) {
#if UNITY_EDITOR
            if (targetObject != null) {
                Undo.RecordObject(targetObject, _name);
                return true;
            }
            return false;
#else
            return false;
#endif
        }

        private void SetDirty(bool _undo) {
#if UNITY_EDITOR
            if (_undo) {
                EditorUtility.SetDirty(targetObject);
            }
#endif
        }

#if UNITY_EDITOR
        private static Color baseColor => EditorGUIUtility.isProSkin ? new Color32(56, 56, 56, 255) : new Color32(194, 194, 194, 255);
        private static Color outlineColor => (EditorGUIUtility.isProSkin ? Color.white : Color.black) * 0.5f;
#else
        private static Color baseColor => new Color32(56, 56, 56, 255);
        private static Color outlineColor => Color.white * 0.5f;
#endif

        private static Color backgroundColor => baseColor;
        private static Color headerColor {
            get {
                Color col = baseColor * 0.65f;
                col.a = 1;
                return col;
            }
        }
        private static Color oddRowColor => baseColor * 0.85f;
        private static Color evenRowColor => baseColor;
        private static Color highlightColor => new Color32(72, 109, 132, 255);

        public class TableEvent {
            public Action<int[]> onCut;
            public Action<int[]> onCopy;
            public Action onPaste;
            public Action<int[]> onSelectAll;
            public Action<int[]> onDelete;
            public Action onCreateNew;
            public Action<int> onEdit;
            public Action onCleared;
            public Action<UITKTable, Event> onMouseArea;
        }

        public int[] SelectedRows {
            get {
                return selectedRows.ToArray();
            }
        }

        public int SelectedRow {
            get {
                return selectedRows.Count > 0 ? selectedRows[0] : -1;
            }
        }

        public int[] SortedSelectedRows {
            get {
                int[] result = selectedRows.ToArray();
                Array.Sort(result);
                return result;
            }
        }

#endregion

#region BUILDER
        public UITKTable SetElementHeight(int _height) {
            elementHeight = _height;
            return this;
        }

        public UITKTable SetSize(int _column, int _row) {
            row = _row;
            column = _column;
            onCreateCell = new Action<VisualElement, int>[_column];
            headerTexts = new string[_column];
            columnFlexes = new float[_column];
            for (int i=0; i<_column; i++) {
                columnFlexes[i] = 1f;
            }
            return this;
        }

        public UITKTable SetColumnFlexes(params float[] _params) {
            columnFlexes = _params;
            return this;
        }

        public UITKTable SetPadding(float _padding) {
            padding = _padding;
            return this;
        }

        public UITKTable SetHeaderTexts(params string[] _headerTexts) {
            headerTexts = _headerTexts;
            return this;
        }

        public UITKTable SetHeaderFontSize(float _fontSize) {
            headerFontSize = _fontSize;
            return this;
        }

        public UITKTable SetOnCreateCell(int _column, Action<VisualElement, int> _cellRowHandler) {
            onCreateCell[_column] = _cellRowHandler;
            return this;
        }

        public UITKTable SetOnDoubleClick(Action<int> _onDoubleClick) {
            onDoubleClick = _onDoubleClick;
            return this;
        }

        public UITKTable SetContextMenuHandler(Action<DropdownMenu> _contextMenuHandler) {
            contextMenuHandler = _contextMenuHandler;
            return this;
        }

        public UITKTable SetEventHandler(TableEvent _tableEvent) {
            tableEvent = _tableEvent;
            return this;
        }

        public UITKTable SetSelectable(bool _selectable, bool _multiSelect = true) {
            selectable = _selectable;
            multiSelect = _multiSelect;
            return this;
        }

        public UITKTable SetMinSelection(int _minSelection = 0) {
            minSelection = _minSelection;
            return this;
        }

        public UITKTable SetMaxSelection(int _maxSelection) {
            maxSelection = _maxSelection;
            return this;
        }

        public UITKTable SetOnSelect(Action<int> _onSelectHandler) {
            onSelect = _onSelectHandler;
            return this;
        }

        public UITKTable BindList(UnityEngine.Object _targetObject, Func<IList> _listGetter) {
            targetObject = _targetObject;
            listGetter = _listGetter;
            dictionaryGetter = null;
            return this;
        }

        public UITKTable BindDictionary(UnityEngine.Object _targetObject, Func<IDictionary> _dictionaryGetter) {
            targetObject = _targetObject;
            dictionaryGetter = _dictionaryGetter;
            listGetter = null;
            return this;
        }

        public UITKTable SetReorderable(bool _reorderable) {
            reorderable = _reorderable;
            return this;
        }

        public UITKTable SetFoldable(bool _foldable) {
            foldable = _foldable;
            return this;
        }

        public UITKTable SetOnClickAddButton(Action _onClickAdd) {
            onClickAddButton = _onClickAdd;
            return this;
        }

        public UITKTable SetFrozenRows(int[] _rows) {
            frozenRows = _rows;
            return this;
        }

        public UITKTable SetRowSearchMethod(RowSearchMethod _rowSearchMethod) {
            rowSearchMethod = _rowSearchMethod;
            return this;
        }

        #endregion

        #region SELECTION
        public void SelectAll() {
            selectedRows = new List<int>();
            for (int i = 0; i < row; i++) {
                AddSelection(i);
            }
        }

        public void ClearSelection(bool _invokeEvent = true) {
            while (selectedRows.Count > 0 && selectedRows.Count >= minSelection) {
                int removedRow = selectedRows[0];
                EaseRowColor(removedRow, removedRow % 2 == 0 ? evenRowColor : oddRowColor);
                selectedRows.RemoveAt(0);
            }
            if (_invokeEvent && tableEvent != null && tableEvent.onCleared != null) {
                tableEvent.onCleared.Invoke();
            }
        }

        private void AddSelection(int _row) {
            selectedRows.Add(_row);
            if (selectedRows.Count > maxSelection) {
                int removedRow = selectedRows[0];
                EaseRowColor(removedRow, removedRow % 2 == 0 ? evenRowColor : oddRowColor);
                selectedRows.RemoveAt(0);
            }
            if (onSelect != null) {
                onSelect.Invoke(_row);
            }
            EaseRowColor(_row, highlightColor);
        }

        private Dictionary<int, ValueAnimation<Color>> easingCache = new Dictionary<int, ValueAnimation<Color>>();
        private void EaseRowColor(int _row, Color _targetColor) {
            if (easingCache.ContainsKey(_row)) {
                easingCache[_row].Stop();
            }
            else {
                easingCache.Add(_row, null);
            }
            easingCache[_row] = visualElement.experimental.animation.Start(rowElements[_row].style.backgroundColor.value, _targetColor, 175, (_e, _color) => {
                if (_row < rowElements.Count) {
                    rowElements[_row].style.backgroundColor = new StyleColor(_color);
                }
            }).Ease(Easing.InOutCubic).KeepAlive();
        }

        public void AddSelections(int[] _rows) {
            foreach (int _row in _rows) {
                if (_row < row) {
                    rowElements[_row].style.backgroundColor = highlightColor;
                    selectedRows.Add(_row);
                }
            }
        }

#endregion

#region EVENT-HANDLER

        private void HandleKeyDownEvent(KeyDownEvent _e) {
            if (_e.ctrlKey) {
                if (_e.keyCode == KeyCode.V) {
                    HandlePasteRows();
                }
                else if (_e.keyCode == KeyCode.A) {
                    HandleSelectAll();
                }
                else if (_e.keyCode == KeyCode.X) {
                    HandleCutRows();
                }
                else if (_e.keyCode == KeyCode.C) {
                    HandleCopyRows();
                }
            }

            if (_e.shiftKey) {
                if (_e.keyCode == KeyCode.N) {
                    HandleCreateNewRow();
                }
            }

            if (_e.keyCode == KeyCode.Space) {
                if (tableEvent != null && selectedRows.Count == 1 && tableEvent.onEdit != null) {
                    tableEvent.onEdit.Invoke(SelectedRow);
                }
            }
            else if (_e.keyCode == KeyCode.Delete) {
                if (selectedRows.Count > 0) {
                    HandleDeleteRows();
                }
            }
            else if (_e.keyCode == KeyCode.PageUp) {
                HandleMoveUp();
            }
            else if (_e.keyCode == KeyCode.PageDown) {
                HandleMoveDown();
            }

            HandleShiftArrowSelection(_e);
        }

        private void HandleShiftArrowSelection(KeyDownEvent _e) {
            if (!selectable || focusedTable != this) return;
            if (!_e.shiftKey) {
                if (selectedRows.Count > 0) {
                    if (_e.keyCode == KeyCode.UpArrow) {
                        int newSelectedRow = selectedRows[selectedRows.Count - 1] - 1;
                        if (newSelectedRow < 0) newSelectedRow = 0;
                        ClearSelection();
                        AddSelection(newSelectedRow);
                    }
                    else if (_e.keyCode == KeyCode.DownArrow) {
                        int newSelectedRow = selectedRows[selectedRows.Count - 1] + 1;
                        if (newSelectedRow >= row) newSelectedRow = row - 1;
                        ClearSelection();
                        AddSelection(newSelectedRow);
                    }
                }
                else {
                    if (_e.keyCode == KeyCode.UpArrow) {
                        AddSelection(row - 1);
                    }
                    else if (_e.keyCode == KeyCode.DownArrow) {
                        AddSelection(0);
                    }
                }
            }
            else {
                if (selectedRows.Count > 0) {
                    if (_e.keyCode == KeyCode.UpArrow) {
                        int maxRow = selectedRows.Max();
                        int minRow = selectedRows.Min();
                        int lastSelectedRow = selectedRows[selectedRows.Count - 1];
                        if (lastSelectedRow == minRow) {
                            minRow--;
                            if (minRow < 0) minRow = 0;
                            ClearSelection();
                            for (int i = maxRow; i >= minRow; i--) {
                                AddSelection(i);
                            }
                        }
                        else if (lastSelectedRow == maxRow) {
                            maxRow--;
                            if (maxRow < 0) maxRow = 0;
                            ClearSelection();
                            for (int i = minRow; i <= maxRow; i++) {
                                AddSelection(i);
                            }
                        }
                    }
                    else if (_e.keyCode == KeyCode.DownArrow) {
                        int maxRow = selectedRows.Max();
                        int minRow = selectedRows.Min();
                        int lastSelectedRow = selectedRows[selectedRows.Count - 1];
                        if (lastSelectedRow == maxRow) {
                            maxRow++;
                            if (maxRow >= row) maxRow = row - 1;
                            ClearSelection();
                            for (int i = minRow; i <= maxRow; i++) {
                                AddSelection(i);
                            }
                        }
                        else if (lastSelectedRow == minRow) {
                            minRow++;
                            if (minRow >= row) minRow = row - 1;
                            ClearSelection();
                            for (int i = maxRow; i >= minRow; i--) {
                                AddSelection(i);
                            }
                        }
                    }
                }
                else {
                    if (_e.keyCode == KeyCode.UpArrow) {
                        AddSelection(row - 1);
                    }
                    else if (_e.keyCode == KeyCode.DownArrow) {
                        AddSelection(0);
                    }
                }
            }
        }

        private void HandleContextMenu(DropdownMenu _dropdownMenu) {
            //GenericMenu contextMenu = null;

            if (tableEvent?.onEdit != null) {
                if (SelectedRows.Length > 0 && SelectedRows.Length == 1) {
                    _dropdownMenu.AppendAction("Edit... _space", _action => {
                        tableEvent.onEdit.Invoke(SelectedRow);
                    });
                }
            }

            if (listGetter != null || tableEvent?.onCreateNew != null) {
                _dropdownMenu.AppendAction("New... #n", _action => {
                    HandleCreateNewRow();
                });
            }

            _dropdownMenu.AppendSeparator("");

            if ((listGetter != null && dictionaryGetter == null) ||  tableEvent?.onCut != null) {
                _dropdownMenu.AppendAction("Cut %x", _action => {
                    HandleCutRows();
                }, selectedRows.Count > 0 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }

            if ((listGetter != null && dictionaryGetter != null) || tableEvent?.onCopy != null) {
                _dropdownMenu.AppendAction("Copy %c", _action => {
                    HandleCopyRows();
                }, selectedRows.Count > 0 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

                bool listPasteAvailable = listGetter != null && ClipboardUtility.IsPasteAvailable<List<object>>();
                _dropdownMenu.AppendAction("Paste %v", _action => {
                    HandlePasteRows();
                }, tableEvent?.onPaste != null || listPasteAvailable ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }

            if (listGetter != null || dictionaryGetter != null || (tableEvent?.onDelete != null)) {
                _dropdownMenu.AppendAction("Delete _del", _action => {
                    HandleDeleteRows();
                }, selectedRows.Count > 0 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }

            if (listGetter != null || dictionaryGetter != null || tableEvent.onCopy != null || tableEvent.onCut != null || tableEvent.onDelete != null) {
                _dropdownMenu.AppendAction("Select All %a", delegate {
                    HandleSelectAll();
                });
            }

            if (listGetter != null && reorderable && dictionaryGetter == null) {
                _dropdownMenu.AppendAction("Move Up _pgup", delegate {
                    HandleMoveUp();
                }, selectedRows.Count > 0 && !selectedRows.Contains(0) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

                _dropdownMenu.AppendAction("Move Down _pgdn", _action => {
                    HandleMoveDown();
                }, selectedRows.Count > 0 && !selectedRows.Contains(row - 1) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }

            if (contextMenuHandler != null) {
                contextMenuHandler.Invoke(_dropdownMenu);
            }

        }

        private void HandleSelectAll() {
            SelectAll();
            if (tableEvent?.onSelectAll != null) {
                tableEvent.onSelectAll.Invoke(SortedSelectedRows);
            }
        }

        private void HandleCreateNewRow() {
            if (tableEvent?.onCreateNew != null) {
                tableEvent.onCreateNew.Invoke();
            }
            else if (listGetter != null) {
                bool undo = TryUndo("Added a new list object");
                listGetter().Add(default);
                SetDirty(undo);
            }
            /*else if (dictionaryGetter != null) {
                TryUndo("Added a new key value pair object");
                dictionaryGetter().Add(default, default);
            }*/
            RefreshRows();
        }

        private void HandleCutRows() {
            if (tableEvent?.onCut != null) {
                tableEvent.onCut.Invoke(SortedSelectedRows);
            }
            else if (listGetter != null && dictionaryGetter == null) {
                HandleCopyRows();
                HandleDeleteRows(false);
            }
        }

        private void HandleCopyRows() {
            if (tableEvent?.onCopy != null) {
                tableEvent.onCopy.Invoke(SortedSelectedRows);
            }
            else if (listGetter != null) {
                List<object> copiedObjects = new List<object>();
                int[] sortedRows = SortedSelectedRows;
                foreach (int row in sortedRows) {
                    copiedObjects.Add(listGetter()[row]);
                }
                ClipboardUtility.Copy(copiedObjects);
            }
        }

        private void HandlePasteRows() {
            if (tableEvent?.onPaste != null) {
                tableEvent.onPaste();
                RefreshRows();
            }
            else if (listGetter != null && ClipboardUtility.IsPasteAvailable<List<object>>()) {
                try {
                    bool undo = TryUndo("Pasted list objects");
                    List<object> copiedObjects = ClipboardUtility.Paste<List<object>>();
                    ClearSelection();
                    List<int> pastedRows = new List<int>();
                    IList list = listGetter();
                    foreach (object copiedObject in copiedObjects) {
                        list.Add(copiedObject);
                        pastedRows.Add(list.Count - 1);
                    }
                    RefreshRows();
                    foreach(int row in pastedRows) {
                        AddSelection(row);
                    }
                    SetDirty(undo);
                }
                catch {

                }
            }
        }

        private void HandleDeleteRows(bool _prompt = true) {
#if UNITY_EDITOR
            if (!_prompt || EditorUtility.DisplayDialog("Delete Row(s)", "Are you sure you want to delete this row(s)?", "Yes", "No")) {
#endif
                bool deleted = false;
                if (tableEvent?.onDelete != null) {
                    tableEvent.onDelete.Invoke(SortedSelectedRows);
                    deleted = true;
                }
                else if (listGetter != null) {
                    bool undo = TryUndo("Deleted list objects");
                    List<int> sortedRows = SortedSelectedRows.Reverse().ToList();
                    if (frozenRows != null) {
                        foreach (int row in frozenRows) {
                            sortedRows.Remove(row);
                        }
                    }
                    foreach (int removedRow in sortedRows) {
                        listGetter().RemoveAt(removedRow);
                    }
                    deleted = true;
                    SetDirty(undo);
                }
                else if (dictionaryGetter != null) {
                    bool undo = TryUndo("Deleted list objects");
                    List<int> sortedRows = SortedSelectedRows.Reverse().ToList();
                    if (frozenRows != null) {
                        foreach(int row in frozenRows) {
                            sortedRows.Remove(row);
                        }
                    }
                    object[] keys = dictionaryGetter().Keys.Cast<object>().ToArray();
                    foreach (int removedRow in sortedRows) {
                        dictionaryGetter().Remove(keys[removedRow]);
                    }
                    deleted = true;
                    SetDirty(undo);
                }
                if (deleted) {
                    ClearSelection();
                    RefreshRows();
                }
#if UNITY_EDITOR
            }
#endif
        }

        private void HandleDoubleClick(int _selectedIndex) {
            if (onDoubleClick != null) {
                onDoubleClick.Invoke(_selectedIndex);
                RefreshRows();
            }
        }

        private void HandleMoveUp() {
            if (listGetter == null || !reorderable) return;
            if (selectedRows.Count > 0) {
                if (selectedRows.Contains(0)) return;
                int[] sortedRows = SortedSelectedRows;
                ClearSelection();
                foreach (int i in sortedRows) {
                    IList list = listGetter();
                    var item = list[i];
                    list.RemoveAt(i);
                    list.Insert(i - 1, item);
                    AddSelection(i - 1);
                }
            }
            RefreshRows();
        }

        private void HandleMoveDown() {
            if (listGetter == null || !reorderable) return;
            if (selectedRows.Count > 0) {
                if (selectedRows.Contains(row - 1)) return;
                int[] sortedRows = SortedSelectedRows.Reverse().ToArray();
                ClearSelection();
                foreach (int i in sortedRows) {
                    IList list = listGetter();
                    var item = list[i];
                    list.RemoveAt(i);
                    list.Insert(i + 1, item);
                    AddSelection(i + 1);
                }
            }
            RefreshRows();
        }

        private void HandleClickOutsideRows(MouseDownEvent _e) {
            if (_e.button == 0) {
                if (!_e.ctrlKey && !_e.shiftKey) {
                    ClearSelection();
                }
                if (_e.clickCount == 2) {
                    HandleDoubleClick(-1);
                }
            }
        }

        private void HandleMouseSelection(MouseDownEvent _e, int j) {
            if (!selectable /*|| focusedTable != this*/) return;
            if (_e.button == 0) {
                if (_e.ctrlKey && multiSelect) {
                    if (selectedRows.Contains(j)) {
                        selectedRows.Remove(j);
                    }
                    else {
                        AddSelection(j);
                    }
                }
                else if (_e.shiftKey && multiSelect) {
                    int minRow = selectedRows.Count > 0? selectedRows.Min() : 0;
                    int maxRow = selectedRows.Count > 0 ? selectedRows.Max() : 0;
                    int lastSelectedRow = selectedRows[selectedRows.Count - 1];
                    ClearSelection();
                    if (j > maxRow) {
                        for (int i = minRow; i <= j; i++) {
                            AddSelection(i);
                        }
                    }
                    else if (j < minRow) {
                        for (int i = j; i <= maxRow; i++) {
                            AddSelection(i);
                        }
                        selectedRows.Reverse();
                    }
                    else {
                        if (j < lastSelectedRow) {
                            for (int i = minRow; i <= j; i++) {
                                AddSelection(i);
                            }
                        }
                        else if (j > lastSelectedRow) {
                            for (int i = j; i <= maxRow; i++) {
                                AddSelection(i);
                            }
                        }
                    }
                }
                else {
                    ClearSelection(selectedRows.Count != 1 || SelectedRow != j);
                    AddSelection(j);
                }
            }
        }
#endregion

#region UI-ELEMENTS
        public VisualElement visualElement {
            get {
                if (m_visualElement == null) {
                    CreateElement();
                }
                return m_visualElement;
            }
        }

        public VisualElement CreateElement() {
            m_visualElement = new VisualElement();
            if (rowSearchMethod != null) {
                CreateSearchField();
            }
            m_visualElement.Add(CreateHeader());
            m_visualElement.Add(CreateScrollView());
            m_visualElement.focusable = true;
            m_visualElement.RegisterCallback<KeyDownEvent>(_event => {
                HandleKeyDownEvent(_event);
            });
            m_visualElement.RegisterCallback<FocusInEvent>(_event => {
                focusedTable = this;
            });
            m_visualElement.RegisterCallback<FocusOutEvent>(_event => {
                ClearSelection();
                if (focusedTable == this) {
                    focusedTable = null;
                }
            });
            m_visualElement.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent _event) => {
                HandleContextMenu(_event.menu);
            }));
            return m_visualElement;
        }

        private void CreateSearchField() {
            var searchField = new TextField("Search:") {
                value = searchText,
                style = {
                    marginBottom = 5
                }
            };
            searchField.RegisterValueChangedCallback(_e => {
                searchText = _e.newValue;
                RefreshRows();
            });
            foreach (var ve in searchField.Children())
                if (ve is Label) {
                    ve.style.minWidth = 50;
                    ve.style.maxWidth = 50;
                    break;
                }
            m_visualElement.Add(searchField);
        }

        private VisualElement CreateHeader() {
            VisualElement header = new VisualElement() {
                style = {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    backgroundColor = new StyleColor(headerColor),
                    borderTopLeftRadius = new StyleLength(new Length(borderRadius, LengthUnit.Pixel)),
                    borderTopRightRadius = new StyleLength(new Length(borderRadius, LengthUnit.Pixel))
                }
            };
            for (int i = 0; i < column; i++) {
                VisualElement cell = new VisualElement() {
                    style = {
                        paddingBottom = new StyleLength(new Length(padding, LengthUnit.Pixel)),
                        paddingTop = new StyleLength(new Length(padding, LengthUnit.Pixel)),
                        paddingLeft = new StyleLength(new Length(padding, LengthUnit.Pixel)),
                        paddingRight = new StyleLength(new Length(padding, LengthUnit.Pixel)),
                        borderTopWidth = new StyleFloat(outlineWidth),
                        borderTopColor = new StyleColor(outlineColor),
                        borderBottomWidth = new StyleFloat(outlineWidth),
                        borderBottomColor = new StyleColor(outlineColor),
                        borderRightWidth = new StyleFloat(outlineWidth),
                        borderRightColor = new StyleColor(outlineColor),
                        height = new StyleLength(new Length(elementHeight, LengthUnit.Pixel)),
                        flexGrow = columnFlexes[i],
                        flexBasis = 0,
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.Center
                    }
                };
                if (i == 0) {
                    cell.style.borderTopLeftRadius = new StyleLength(new Length(borderRadius, LengthUnit.Pixel));
                    cell.style.borderLeftWidth = new StyleFloat(outlineWidth);
                    cell.style.borderLeftColor = new StyleColor(outlineColor);
                    if (foldable) {
                        CreateFoldout(cell);
                    }
                }
                else if (i == column-1) {
                    cell.style.borderTopRightRadius = new StyleLength(new Length(borderRadius, LengthUnit.Pixel));
                }
                header.Add(cell);
                cell.Add(new Label(headerTexts[i]) {
                    style = {
                        unityFontStyleAndWeight = FontStyle.Bold,
                        fontSize = new StyleLength(new Length(headerFontSize, LengthUnit.Pixel))
                    }
                });
            }

            sliderSpace = new VisualElement() {
                style = {
                    width = new StyleLength(new Length(0, LengthUnit.Pixel))
                }
            };
            header.Add(sliderSpace);

            return header;
        }

        private void CreateFoldout(VisualElement cell) {
            Foldout foldout = new Foldout() { value = true };
            float previousFlexGrow = visualElement.style.flexGrow != null ? visualElement.style.flexGrow.value : 0;
            Length previousHeight = visualElement.style.height != null ? visualElement.style.height.value : 0;
            foldout.RegisterValueChangedCallback(_event => {
                if (_event.previousValue) {
                    previousFlexGrow = visualElement.style.flexGrow != null ? visualElement.style.flexGrow.value : 0;
                    previousHeight = visualElement.style.height != null ? visualElement.style.height.value : 0;
                }
                float targetValue = _event.newValue ? previousFlexGrow : 0;

                visualElement.experimental.animation.Start(visualElement.style.flexGrow.value, targetValue, 175, (_e, _value) => {
                    visualElement.style.flexGrow = new StyleFloat(_value);
                }).Ease(Easing.InOutCubic).KeepAlive();

                if (_event.newValue) {
                    targetValue = previousHeight.value;
                }
                else {
                    targetValue = elementHeight;
                }
                visualElement.experimental.animation.Start(visualElement.style.height.value.value, targetValue, 175, (_e, _value) => {
                    visualElement.style.height = new StyleLength(new Length(_value, previousHeight.unit));
                }).Ease(Easing.InOutCubic).KeepAlive();
            });
            cell.Add(foldout);
        }

        private ScrollView CreateScrollView() {
            scrollView = new ScrollView() {
                style = {
                    flexDirection = FlexDirection.Column,
                    flexGrow = 1,
                    backgroundColor = new StyleColor(backgroundColor),
                    borderBottomWidth = new StyleFloat(outlineWidth),
                    borderLeftWidth = new StyleFloat(outlineWidth),
                    borderRightWidth = new StyleFloat(outlineWidth),
                    borderBottomColor = new StyleColor(outlineColor),
                    borderLeftColor = new StyleColor(outlineColor),
                    borderRightColor = new StyleColor(outlineColor),
                    borderBottomLeftRadius = new StyleLength(new Length(borderRadius, LengthUnit.Pixel)),
                    borderBottomRightRadius = new StyleLength(new Length(borderRadius, LengthUnit.Pixel))
                }
            };
            m_emptyRow = FillRows();

            VisualElement viewport = scrollView.Q("unity-content-viewport");

            scrollView.RegisterCallback<MouseDownEvent>(_event => {
                focusedTable = this;
                if (_event.target == viewport || _event.target == m_emptyRow) {
                    HandleClickOutsideRows(_event);
                }
            });
            return scrollView;
        }

        private VisualElement FillRows() {
            if (rowElements == null) {
                rowElements = new List<VisualElement>();
            }
            else {
                rowElements.Clear();
            }
            if (rowSearchMethod == null || string.IsNullOrEmpty(searchText)) {
                for (int i = 0; i < row; i++) {
                    rowElements.Add(CreateRow(i));
                    scrollView.Add(rowElements[i]);
                }
            }
            else {
                for (int i = 0; i < row; i++) {
                    rowElements.Add(CreateRow(i));
                    scrollView.Add(rowElements[i]);
                    if (!rowSearchMethod(searchText, rowElements[i])) {
                        rowElements[i].style.display = DisplayStyle.None;
                    }
                }
            }
            VisualElement emptyElement = new VisualElement {
                style = {
                    height = new StyleLength(new Length(elementHeight, LengthUnit.Pixel))
                }
            };
            scrollView.Add(emptyElement);

            //Determine empty slider space for header
            scrollView.schedule.Execute(() => {
                Task.Yield();
                if (scrollView.verticalScroller.visible) {
                    sliderSpace.style.width = new StyleLength(new Length(scrollView.verticalScroller.contentRect.width, LengthUnit.Pixel));
                }
                else {
                    sliderSpace.style.width = new StyleLength(new Length(0, LengthUnit.Pixel));
                }
            });

            if (onClickAddButton != null) {
                emptyElement.Add(new Button(() => {
                    onClickAddButton.Invoke();
                    RefreshRows();
                }) {
                    text = "+",
                    style = {
                        flexGrow = 1
                    }
                });
            }
            return emptyElement;
        }

        public void RefreshRows() {
            scrollView.Clear();
            if (listGetter != null) {
                row = listGetter().Count;
            }
            else if (dictionaryGetter != null) {
                row = dictionaryGetter().Count;
            }
            m_emptyRow = FillRows();
        }

        private VisualElement CreateRow(int _rowIndex) {
            var rowElement = new VisualElement() {
                style = {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center
                }
            };

            rowElement.style.backgroundColor = _rowIndex % 2 == 0 ? evenRowColor : oddRowColor;

            RegisterEvents(_rowIndex, rowElement);

            for (int i = 0; i < column; i++) {
                VisualElement cell = new VisualElement() {
                    style = {
                        paddingBottom = new StyleLength(new Length(padding, LengthUnit.Pixel)),
                        paddingTop = new StyleLength(new Length(padding, LengthUnit.Pixel)),
                        paddingLeft = new StyleLength(new Length(padding, LengthUnit.Pixel)),
                        paddingRight = new StyleLength(new Length(padding, LengthUnit.Pixel)),
                        borderBottomWidth = new StyleFloat(outlineWidth),
                        borderBottomColor = new StyleColor(outlineColor),
                        height = new StyleLength(new Length(100, LengthUnit.Percent)),
                        flexGrow = columnFlexes[i],
                        flexBasis = 0,
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.Center
                    }
                };
                if (i != column - 1) {
                    cell.style.borderRightWidth = new StyleFloat(outlineWidth);
                    cell.style.borderRightColor = new StyleColor(outlineColor);
                }
                rowElement.Add(cell);
                if (i < onCreateCell.Length && onCreateCell[i] != null) {
                    onCreateCell[i].Invoke(cell, _rowIndex);
                }
            }
            return rowElement;
        }

        private void RegisterEvents(int _rowIndex, VisualElement rowElement) {
            rowElement.RegisterCallback<MouseDownEvent>(_event => {
                if (_event.button == 0) {
                    if (_event.clickCount == 2) {
                        HandleDoubleClick(_rowIndex);
                    }
                }
                HandleMouseSelection(_event, _rowIndex);
            });
        }
#endregion
    }
}