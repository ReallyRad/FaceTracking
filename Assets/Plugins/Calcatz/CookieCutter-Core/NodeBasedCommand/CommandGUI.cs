#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.CookieCutter {
    /// <summary>
    /// Shortcuts for drawing custom node through Command (without using Command Node).
    /// Can only be used if the Command has no Command Node.
    /// Otherwise, an error will be occured.
    /// </summary>
    public static class CommandGUI {

        public static class internal_Actions {
            public static Action addMainInPoint;
            public static Action addMainOutPoint;
            public static Action<Type> addPropertyInPoint;
            public static Action<Type> addPropertyOutPoint;
            public static Action<int> drawInPoint;
            public static Action<int> drawOutPoint;
            public static Action<float> addRectHeight;
            public static GUIStyle labelStyle;
            public static GUIStyle labelRightStyle;
            public static Vector2 absolutePosition;
            public static Func<float> currentY;
            public static Func<float> nodeWidth;
            public static UnityEngine.Object targetObject;
            public static bool selected;
        }

        public static UnityEngine.Object currentTargetObject => internal_Actions.targetObject;

        public static bool IsSelected() => internal_Actions.selected;

        private static GUIStyle m_wrappedTextAreaStyle;
        private static GUIStyle wrappedTextAreaStyle {
            get {
                if (m_wrappedTextAreaStyle == null) {
                    m_wrappedTextAreaStyle = new GUIStyle(EditorStyles.textArea);
                    m_wrappedTextAreaStyle.wordWrap = true;
                }
                return m_wrappedTextAreaStyle;
            }
        }

        private static void RecordUndo(string _label) {
            if (internal_Actions.targetObject != null) {
                Undo.RecordObject(internal_Actions.targetObject, _label);
            }
        }

        private static void SetDirty() {
            if (internal_Actions.targetObject != null) {
                EditorUtility.SetDirty(internal_Actions.targetObject);
            }
        }

        public static void AddMainInPoint() {
            internal_Actions.addMainInPoint();
        }

        public static void AddMainOutPoint() {
            internal_Actions.addMainOutPoint();
        }

        public static void AddPropertyInPoint<T>() {
            internal_Actions.addPropertyInPoint(typeof(T));
        }

        public static void AddPropertyInPoint(System.Type _type) {
            internal_Actions.addPropertyInPoint(_type);
        }

        public static void AddPropertyOutPoint<T>() {
            internal_Actions.addPropertyOutPoint(typeof(T));
        }

        public static void AddPropertyOutPoint(System.Type _type) {
            internal_Actions.addPropertyOutPoint(_type);
        }

        public static void DrawInPoint(int _inputPointIndex) {
            internal_Actions.drawInPoint(_inputPointIndex);
        }

        public static void DrawOutPoint(int _outputPointIndex) {
            internal_Actions.drawOutPoint(_outputPointIndex);
        }

        public static void AddRectHeight(float _height) {
            internal_Actions.addRectHeight(_height);
        }

        public static Rect GetRect(int _lineCount = 1, int _indent = 0) {
            return new Rect(internal_Actions.absolutePosition.x + 10 + _indent * 10, internal_Actions.currentY(), internal_Actions.nodeWidth() - 20 - _indent*10, _lineCount * EditorGUIUtility.singleLineHeight);
        }

        public static void GetSingleLineLabeledRect(out Rect labelRect, out Rect valueRect) {
            valueRect = GetRect();
            labelRect = valueRect;
            labelRect.width = EditorGUIUtility.labelWidth;
            valueRect.x += labelRect.width;
            valueRect.width -= labelRect.width;
        }

        public static void PropertySpace() {
            AddRectHeight(EditorGUIUtility.standardVerticalSpacing);
        }

        public static void DrawFoldoutGroup(string _label, ref bool _targetBool, Action _onDrawContent) {
            DrawFoldoutGroup(_label, null, ref _targetBool, _onDrawContent);
        }

        public static void DrawFoldoutGroup(string _label, string _tooltip, ref bool _targetBool, Action _onDrawContent) {
            BeginFoldoutGroup(_label, _tooltip, ref _targetBool);
            if (_targetBool) {
                _onDrawContent?.Invoke();
            }
            EndFoldoutGroup();
        }

        public static void BeginFoldoutGroup(string _label, ref bool _targetBool) {
            BeginFoldoutGroup(_label, null, ref _targetBool);
        }

        public static void BeginFoldoutGroup(string _label, string _tooltip, ref bool _targetBool) {
            _targetBool = EditorGUI.BeginFoldoutHeaderGroup(GetRect(), _targetBool, new GUIContent(_label, _tooltip));
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void EndFoldoutGroup() {
            EditorGUI.EndFoldoutHeaderGroup();
        }

        public static void DrawLabel(Rect _rect, string _label, bool _alignRight = false) {
            DrawLabel(_rect, _label, null, _alignRight);
        }

        public static void DrawLabel(Rect _rect, string _label, string _tooltip, bool _alignRight = false) {
            EditorGUI.LabelField(_rect, new GUIContent(_label, _tooltip), _alignRight ? internal_Actions.labelRightStyle : internal_Actions.labelStyle);
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void DrawLabel(string _label, bool _alignRight = false) {
            DrawLabel(_label, null, _alignRight);
        }

        public static void DrawLabel(string _label, string _tooltip, bool _alignRight = false) {
            EditorGUI.LabelField(GetRect(), new GUIContent(_label, _tooltip), _alignRight ? internal_Actions.labelRightStyle : internal_Actions.labelStyle);
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }


        public static void DrawTextField(string _label, ref string _targetText) {
            DrawTextField(_label, null, ref _targetText);
        }
        public static void DrawTextField(string _label, string _tooltip, ref string _targetText) {
            Rect labelRect, valueRect;
            GetSingleLineLabeledRect(out labelRect, out valueRect);

            EditorGUI.LabelField(labelRect, new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            EditorGUI.BeginChangeCheck();
            string newText = EditorGUI.TextField(valueRect, _targetText);
            if (EditorGUI.EndChangeCheck()) {
                RecordUndo("Modified text value");
                _targetText = newText;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void DrawTextAreaField(string _label, ref string _targetText, int _lineCount = 2, bool _wordWrap = true) {
            DrawTextAreaField(_label, null, ref _targetText, _lineCount, _wordWrap);
        }
        public static void DrawTextAreaField(string _label, string _tooltip, ref string _targetText, int _lineCount = 2, bool _wordWrap = true) {
            EditorGUI.BeginChangeCheck();
            EditorGUI.LabelField(GetRect(), new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            AddRectHeight(EditorGUIUtility.singleLineHeight);
            Rect rect = GetRect(_lineCount, 1);
            string newText;
            if (_wordWrap) {
                newText = EditorGUI.TextArea(rect, _targetText, wrappedTextAreaStyle);
            }
            else {
                newText = EditorGUI.TextArea(rect, _targetText);
            }
            if (EditorGUI.EndChangeCheck()) {
                RecordUndo("Modified text area value");
                _targetText = newText;
                SetDirty();
            }
            AddRectHeight(rect.height);
        }


        public static void DrawRoundedFloatField(string _label, ref float _targetFloat, float _min = float.MinValue, float _max = float.MaxValue) {
            DrawRoundedFloatField(_label, null, ref _targetFloat, _min, _max);
        }
        public static void DrawRoundedFloatField(string _label, string _tooltip, ref float _targetFloat, float _min = float.MinValue, float _max = float.MaxValue) {
            Rect labelRect, valueRect;
            GetSingleLineLabeledRect(out labelRect, out valueRect);

            EditorGUI.LabelField(labelRect, new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            float newFloat = EditorGUI.FloatField(valueRect, _targetFloat);
            if (newFloat < _min) newFloat = _min;
            else if (newFloat > _max) newFloat = _max;
            int roundedValue = (int)newFloat;
            if (newFloat != roundedValue) {
                RecordUndo("Modified float value");
                _targetFloat = roundedValue;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }


        public static void DrawFloatField(string _label, ref float _targetFloat, float _min = float.MinValue, float _max = float.MaxValue) {
            DrawFloatField(_label, null, ref _targetFloat, _min, _max);
        }
        public static void DrawFloatField(string _label, string _tooltip, ref float _targetFloat, float _min = float.MinValue, float _max = float.MaxValue) {
            Rect labelRect, valueRect;
            GetSingleLineLabeledRect(out labelRect, out valueRect);

            EditorGUI.LabelField(labelRect, new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            float newFloat = EditorGUI.FloatField(valueRect, _targetFloat);
            if (newFloat < _min) newFloat = _min;
            else if (newFloat > _max) newFloat = _max;
            if (newFloat != _targetFloat) {
                RecordUndo("Modified float value");
                _targetFloat = newFloat;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        /// <summary>
        /// Will be shown as ordinary Float Field if node width is not wide enough. Use nodeWidth 240 at minimum.
        /// </summary>
        /// <param name="_label"></param>
        /// <param name="_targetFloat"></param>
        /// <param name="_min"></param>
        /// <param name="_max"></param>
        public static void DrawFloatSliderField(string _label, ref float _targetFloat, float _min, float _max) {
            DrawFloatSliderField(_label, null, ref _targetFloat, _min, _max);
        }
        /// <summary>
        /// Will be shown as ordinary Float Field if node width is not wide enough. Use nodeWidth 240 at minimum.
        /// </summary>
        /// <param name="_label"></param>
        /// <param name="_tooltip"></param>
        /// <param name="_targetFloat"></param>
        /// <param name="_min"></param>
        /// <param name="_max"></param>
        public static void DrawFloatSliderField(string _label, string _tooltip, ref float _targetFloat, float _min, float _max) {
            Rect labelRect, valueRect;
            GetSingleLineLabeledRect(out labelRect, out valueRect);

            EditorGUI.LabelField(labelRect, new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            float newFloat = EditorGUI.Slider(valueRect, _targetFloat, _min, _max);
            if (newFloat != _targetFloat) {
                RecordUndo("Modified float value");
                _targetFloat = newFloat;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void DrawIntField(string _label, ref int _targetInt, int _min = int.MinValue, int _max = int.MaxValue) {
            DrawIntField(_label, null, ref _targetInt, _min, _max);
        }
        public static void DrawIntField(string _label, string _tooltip, ref int _targetInt, int _min = int.MinValue, int _max = int.MaxValue) {
            Rect labelRect, valueRect;
            GetSingleLineLabeledRect(out labelRect, out valueRect);

            EditorGUI.LabelField(labelRect, new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            int newInt = EditorGUI.IntField(valueRect, _targetInt);
            if (newInt < _min) newInt = _min;
            else if (newInt > _max) newInt = _max;
            if (newInt != _targetInt) {
                RecordUndo("Modified int value");
                _targetInt = newInt;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void IntSliderField(string _label, ref int _targetInt, int _min, int _max) {
            IntSliderField(_label, null, ref _targetInt, _min, _max);
        }
        public static void IntSliderField(string _label, string _tooltip, ref int _targetInt, int _min, int _max) {
            Rect labelRect, valueRect;
            GetSingleLineLabeledRect(out labelRect, out valueRect);
            
            EditorGUI.LabelField(labelRect, new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            int newInt = EditorGUI.IntSlider(valueRect, _targetInt, _min, _max);
            if (newInt != _targetInt) {
                RecordUndo("Modified int value");
                _targetInt = newInt;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void DrawToggleField(string _label, ref bool _targetBool) {
            DrawToggleField(_label, null, ref _targetBool);
        }
        public static void DrawToggleField(string _label, string _tooltip, ref bool _targetBool) {
            Rect labelRect, valueRect;
            GetSingleLineLabeledRect(out labelRect, out valueRect);

            EditorGUI.LabelField(labelRect, new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            bool newBool = EditorGUI.ToggleLeft(valueRect, "", _targetBool);
            if (newBool != _targetBool) {
                RecordUndo("Modified bool value");
                _targetBool = newBool;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void DrawToggleLeftField(string _label, ref bool _targetBool) {
            DrawToggleLeftField(_label, null, ref _targetBool);
        }
        public static void DrawToggleLeftField(string _label, string _tooltip, ref bool _targetBool) {
            bool newBool = EditorGUI.ToggleLeft(GetRect(), new GUIContent(_label, _tooltip), _targetBool, internal_Actions.labelStyle);
            if (newBool != _targetBool) {
                RecordUndo("Modified bool value");
                _targetBool = newBool;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void DrawObjectField<T>(string _label, T _currentObject, Action<T> _onObjectChanged, bool _allowSceneObjects = false) {
            DrawObjectField<T>(_label, null, _currentObject, _onObjectChanged, _allowSceneObjects);
        }
        public static void DrawObjectField<T>(string _label, string _tooltip, T _currentObject, Action<T> _onObjectChanged, bool _allowSceneObjects = false) {
            Rect labelRect, valueRect;
            GetSingleLineLabeledRect(out labelRect, out valueRect);

            EditorGUI.LabelField(labelRect, new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            UnityEngine.Object currentObject = (UnityEngine.Object)(object)_currentObject;
            UnityEngine.Object newObject = EditorGUI.ObjectField(valueRect, currentObject, typeof(T), _allowSceneObjects);
            if (newObject != currentObject) {
                RecordUndo("Modified object value");
                _onObjectChanged.Invoke((T)(object)newObject);
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        private static Texture m_binderIconTexture;
        internal static Texture binderIconTexture {
            get {
                if (m_binderIconTexture == null) {
                    m_binderIconTexture = EditorGUIUtility.IconContent("d_FixedJoint Icon").image;
                }
                return m_binderIconTexture;
            }
        }

        public static void DrawBinderField<T>(string _label, T _currentObject, Action<T> _onObjectChanged, Func<UnityEngine.Object, UnityEngine.Object> _customValidateDraggedObject = null) {
            DrawBinderField<T>(_label, null, _currentObject, _onObjectChanged, _customValidateDraggedObject);
        }
        public static void DrawBinderField<T>(string _label, string _tooltip, T _currentObject, Action<T> _onObjectChanged, Func<UnityEngine.Object, UnityEngine.Object> _customValidateDraggedObject = null) {
            if (!EditorUtility.IsPersistent(_currentObject as UnityEngine.Object)) {
                _currentObject = default(T);
                _onObjectChanged.Invoke(_currentObject);
            }
            Rect labelRect, valueRect;
            GetSingleLineLabeledRect(out labelRect, out valueRect);

            EditorGUI.LabelField(labelRect, new GUIContent(_label, binderIconTexture, _tooltip), internal_Actions.labelStyle);
            UnityEngine.Object currentObject = (UnityEngine.Object)(object)_currentObject;
            DropBinderAreaGUI(valueRect, _onObjectChanged, _customValidateDraggedObject);
            UnityEngine.Object newObject = EditorGUI.ObjectField(valueRect, currentObject, typeof(T), false);
            if (newObject != currentObject) {
                RecordUndo("Modified binder object value");
                _onObjectChanged.Invoke((T)(object)newObject);
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        private static void DropBinderAreaGUI<T>(Rect _dropArea, Action<T> _onObjectChanged, Func<UnityEngine.Object, UnityEngine.Object> _customValidateDraggedObject) {
            Event evt = Event.current;
            switch (evt.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!_dropArea.Contains(evt.mousePosition) || DragAndDrop.objectReferences.Length > 1)
                        return;

                    UnityEngine.Object draggedObject = DragAndDrop.objectReferences[0];

                    UnityEngine.Object customDraggedObject = null;
                    if (_customValidateDraggedObject != null) {
                        customDraggedObject = _customValidateDraggedObject.Invoke(draggedObject);
                    }

                    if (customDraggedObject != null) {
                        draggedObject = customDraggedObject;
                    }
                    else if (!EditorUtility.IsPersistent(draggedObject)) {
                        if (draggedObject is CrossSceneBinder binderComponent) {
                            draggedObject = binderComponent.binderAsset;
                        }
                        else if (draggedObject is GameObject go) {
                            if (go.TryGetComponent<CrossSceneBinder>(out var binderComp)) {
                                draggedObject = binderComp.binderAsset;
                            }
                            else {
                                draggedObject = null;
                            }
                        }
                        else if (draggedObject is Component comp) {
                            if (comp.TryGetComponent<CrossSceneBinder>(out var binderComp)) {
                                draggedObject = binderComp.binderAsset;
                            }
                            else {
                                draggedObject = null;
                            }
                        }
                    }
                    else if (!(draggedObject is T)) {
                        draggedObject = null;
                    }

                    if (draggedObject == null) {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        return;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    if (evt.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();
                        RecordUndo("Modified binder object value");
                        _onObjectChanged((T)(object)draggedObject);
                        SetDirty();
                    }
                    evt.Use();
                    break;
            }
        }

        public static void DrawColorField(string _label, ref Color _targetColor) {
            DrawColorField(_label, null, ref _targetColor);
        }
        public static void DrawColorField(string _label, string _tooltip, ref Color _targetColor) {
            EditorGUI.LabelField(GetRect(), new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            AddRectHeight(EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            Color newColor = EditorGUI.ColorField(GetRect(1, 1), _targetColor);
            if (EditorGUI.EndChangeCheck()) {
                RecordUndo("Modified color value");
                _targetColor = newColor;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void DrawCurveField(string _label, AnimationCurve _currentCurve, Action<AnimationCurve> _onChangeCurve) {
            DrawCurveField(_label, null, _currentCurve, _onChangeCurve);
        }
        public static void DrawCurveField(string _label, string _tooltip, AnimationCurve _currentCurve, Action<AnimationCurve> _onChangeCurve) {
            Rect labelRect, valueRect;
            GetSingleLineLabeledRect(out labelRect, out valueRect);

            EditorGUI.LabelField(labelRect, new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            if (_currentCurve == null) _currentCurve = new AnimationCurve();
            EditorGUI.BeginChangeCheck();
            AnimationCurve newCurve = EditorGUI.CurveField(valueRect, _currentCurve);
            if (EditorGUI.EndChangeCheck()) {
                RecordUndo("Modified curve value");
                _onChangeCurve.Invoke(newCurve);
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void DrawEnumField<T>(string _label, ref T _targetEnum) {
            DrawEnumField<T>(_label, null, ref _targetEnum);
        }
        public static void DrawEnumField<T>(string _label, string _tooltip, ref T _targetEnum) {
            Rect labelRect, valueRect;
            GetSingleLineLabeledRect(out labelRect, out valueRect);

            EditorGUI.LabelField(labelRect, new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            System.Enum newEnum = EditorGUI.EnumPopup(valueRect, (System.Enum)(object)_targetEnum);
            if (newEnum != (System.Enum)(object)_targetEnum) {
                RecordUndo("Modified enum value");
                _targetEnum = (T)(object)newEnum;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void DrawVector3Field(string _label, ref Vector3 _targetVector) {
            DrawVector3Field(_label, null, ref _targetVector);
        }
        public static void DrawVector3Field(string _label, string _tooltip, ref Vector3 _targetVector) {
            EditorGUI.LabelField(GetRect(), new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            AddRectHeight(EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            Vector3 newVector = EditorGUI.Vector3Field(GetRect(1, 1), "", _targetVector);
            if (EditorGUI.EndChangeCheck()) {
                RecordUndo("Modified Vector3 value");
                _targetVector = newVector;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void DrawVector3IntField(string _label, ref Vector3Int _targetVector) {
            DrawVector3IntField(_label, null, ref _targetVector);
        }
        public static void DrawVector3IntField(string _label, string _tooltip, ref Vector3Int _targetVector) {
            EditorGUI.LabelField(GetRect(), new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            AddRectHeight(EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            Vector3Int newVector = EditorGUI.Vector3IntField(GetRect(1, 1), "", _targetVector);
            if (EditorGUI.EndChangeCheck()) {
                RecordUndo("Modified Vector3Int value");
                _targetVector = newVector;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void DrawVector2Field(string _label, ref Vector2 _targetVector) {
            DrawVector2Field(_label, null, ref _targetVector);
        }
        public static void DrawVector2Field(string _label, string _tooltip, ref Vector2 _targetVector) {
            EditorGUI.LabelField(GetRect(), new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            AddRectHeight(EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            Vector2 newVector = EditorGUI.Vector2Field(GetRect(1, 1), "", _targetVector);
            if (EditorGUI.EndChangeCheck()) {
                RecordUndo("Modified Vector2 value");
                _targetVector = newVector;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void DrawVector2IntField(string _label, ref Vector2Int _targetVector) {
            DrawVector2IntField(_label, null, ref _targetVector);
        }
        public static void DrawVector2IntField(string _label, string _tooltip, ref Vector2Int _targetVector) {
            EditorGUI.LabelField(GetRect(), new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            AddRectHeight(EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            Vector2Int newVector = EditorGUI.Vector2IntField(GetRect(1, 1), "", _targetVector);
            if (EditorGUI.EndChangeCheck()) {
                RecordUndo("Modified Vector2Int value");
                _targetVector = newVector;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void DrawVector4Field(string _label, ref Vector4 _targetVector) {
            DrawVector4Field(_label, null, ref _targetVector);
        }
        public static void DrawVector4Field(string _label, string _tooltip, ref Vector4 _targetVector) {
            EditorGUI.LabelField(GetRect(), new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            AddRectHeight(EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            Vector4 newVector = EditorGUI.Vector4Field(GetRect(1, 1), "", _targetVector);
            if (EditorGUI.EndChangeCheck()) {
                RecordUndo("Modified Vector4 value");
                _targetVector = newVector;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight);
        }

        public static void DrawRectField(string _label, ref Rect _targetRect) {
            DrawRectField(_label, null, ref _targetRect);
        }
        public static void DrawRectField(string _label, string _tooltip, ref Rect _targetRect) {
            EditorGUI.LabelField(GetRect(), new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            AddRectHeight(EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            Rect newRect = EditorGUI.RectField(GetRect(2, 1), "", _targetRect);
            if (EditorGUI.EndChangeCheck()) {
                RecordUndo("Modified Rect value");
                _targetRect = newRect;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight*2);
        }

        public static void DrawRectIntField(string _label, ref RectInt _targetRect) {
            DrawRectIntField(_label, null, ref _targetRect);
        }
        public static void DrawRectIntField(string _label, string _tooltip, ref RectInt _targetRect) {
            EditorGUI.LabelField(GetRect(), new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
            AddRectHeight(EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            RectInt newRect = EditorGUI.RectIntField(GetRect(2, 1), "", _targetRect);
            if (EditorGUI.EndChangeCheck()) {
                RecordUndo("Modified RectInt value");
                _targetRect = newRect;
                SetDirty();
            }
            AddRectHeight(EditorGUIUtility.singleLineHeight*2);
        }


	    public static void DrawPopupField(string _label, int _currentIndex, string[] _options, Action<int> _onValueChanged) {
	    	DrawPopupField(_label, null, _currentIndex, _options, _onValueChanged);
	    }
	    public static void DrawPopupField(string _label, string _tooltip, int _currentIndex, string[] _options, Action<int> _onValueChanged) {
		    Rect labelRect, valueRect;
		    GetSingleLineLabeledRect(out labelRect, out valueRect);

		    EditorGUI.LabelField(labelRect, new GUIContent(_label, _tooltip), internal_Actions.labelStyle);
		    int newIndex = EditorGUI.Popup(valueRect, _currentIndex, _options);
		    if (newIndex != _currentIndex) {
			    RecordUndo("Modified popup value");
			    _onValueChanged?.Invoke(newIndex);
                SetDirty();
            }
		    AddRectHeight(EditorGUIUtility.singleLineHeight);
	    }

    }
}
#endif