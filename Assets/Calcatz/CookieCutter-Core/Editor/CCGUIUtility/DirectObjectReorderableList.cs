using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Calcatz.CookieCutter {
    public class DirectObjectReorderableList<T> {

        public Action<Rect, List<T>, int, bool, bool> customOnDrawElement = null;
        protected UnityEngine.Object listRootObject;
        protected ReorderableList reorderableList;
        protected Texture2D backgroundImage;
        private string headerText;

        private float defaultElementHeight;
        private List<float> elementHeights;

        public float GetHeight() {
            return reorderableList.GetHeight();
        }

        public List<T> list {
            get {
                return (List<T>)reorderableList.list;
            }
        }

        public DirectObjectReorderableList(UnityEngine.Object _listRootObject, ref List<T> _list, string _headerText = "", float _elementHeight = 30) {
            headerText = _headerText;
            defaultElementHeight = _elementHeight;
            if (_list == null) _list = new List<T>();
            elementHeights = new List<float>(_list.Count);
            foreach(T t in _list) {
                elementHeights.Add(_elementHeight);
            }
            listRootObject = _listRootObject;
            reorderableList = new ReorderableList(_list, typeof(T));
            reorderableList.onAddCallback = OnAdd;
            reorderableList.onRemoveCallback = OnRemove;
            reorderableList.drawElementCallback = OnDrawElement;
            reorderableList.drawHeaderCallback = OnDrawHeader;
            reorderableList.elementHeightCallback = OnCalculateHeight;
            reorderableList.drawElementBackgroundCallback = OnDrawElementBackground;
            SetHightLightBackgroundImage();
        }

        ~DirectObjectReorderableList() {
            reorderableList.onAddCallback = null;
            reorderableList.onRemoveCallback = null;
            reorderableList.drawElementCallback = null;
            reorderableList.drawHeaderCallback = null;
            reorderableList.elementHeightCallback = null;
            reorderableList.drawElementBackgroundCallback = null;
            backgroundImage = null;
        }

        private void OnDrawElementBackground(Rect rect, int index, bool isActive, bool isFocused) {
            if (reorderableList.list == null || reorderableList.list.Count <= index || index < 0)
                return;

            T element = (T)reorderableList.list[index];
            float height = elementHeights[index];
            rect.height = height;
            rect.width -= 4;
            rect.x += 2;

            OnDrawElementBackground(rect, index, isActive, isFocused, element, height);
        }
        protected virtual void OnDrawElementBackground(Rect _rect, int _index, bool _isActive, bool _isFocused, T _element, float _height) {
            if (_isActive)
                EditorGUI.DrawTextureTransparent(_rect, backgroundImage, ScaleMode.ScaleAndCrop);
        }

        private float OnCalculateHeight(int _index) {
            return elementHeights[_index];
        }

        protected void RenewElementHeight(int _index, float _height) {
            elementHeights[_index] = _height;
        }

        private void OnDrawHeader(Rect rect) {
            EditorGUI.LabelField(rect, headerText);
        }

        private void OnAdd(ReorderableList list) {
            Undo.RecordObject(listRootObject, "Added an element.");
            T newElement;
            if (typeof(T).IsSubclassOf(typeof(UnityEngine.Object)) || typeof(T) == typeof(UnityEngine.Object)) {
                if (list.count > 0) {
                    newElement = (T)list.list[list.list.Count - 1];
                    elementHeights.Add(elementHeights[elementHeights.Count - 1]);
                } else {
                    newElement = default(T);
                    elementHeights.Add(defaultElementHeight);
                }
            } else {
                newElement = (T)Activator.CreateInstance(typeof(T));
                elementHeights.Add(defaultElementHeight);
            }
            list.list.Add(newElement);
            list.index = list.list.Count - 1;
            OnAdd(list, list.index, newElement);
            EditorUtility.SetDirty(listRootObject);
        }

        protected virtual void OnAdd(ReorderableList list, int index, T newElement) {

        }

        private void OnRemove(ReorderableList list) {
            Undo.RecordObject(listRootObject, "Removed an element.");
            OnRemove(list, list.index, (T)list.list[list.index]);
            elementHeights.RemoveAt(list.index);
            list.list.RemoveAt(list.index);
            list.index = list.count - 1;
            EditorUtility.SetDirty(listRootObject);
        }

        protected virtual void OnRemove(ReorderableList list, int index, T removedElement) {

        }

        protected virtual void OnDrawElement(Rect rect, int index, bool active, bool focused) {
            //base.OnDrawElement(rect, index, active, focused, element);\
            if (customOnDrawElement != null) {
                customOnDrawElement(rect, list, index, active, focused);
            }
        }

        public void DoLayoutList() {
            reorderableList.DoLayoutList();
        }

        public void DoList(Rect rect) {
            reorderableList.DoList(rect);
        }

        public virtual void SetHightLightBackgroundImage() {
            backgroundImage = new Texture2D(2, 1);
            //backgroundImage.SetPixel(0, 0, new Color(0.35f, .35f, .35f));
            //backgroundImage.SetPixel(1, 0, new Color(0.3f, .3f, .3f));
            backgroundImage.SetPixel(0, 0, new Color(81f / 255f, 98f / 255f, 120f / 255f, 1f));
            backgroundImage.SetPixel(1, 0, new Color(110f / 255f, 120f / 255f, 140 / 255f, 1f));
            backgroundImage.hideFlags = HideFlags.DontSave;
            backgroundImage.wrapMode = TextureWrapMode.Clamp;
            backgroundImage.Apply();
        }

    }
}
