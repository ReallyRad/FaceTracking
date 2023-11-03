using UnityEngine;
using UnityEditor;

namespace Calcatz.CookieCutter {
    public static class ComponentUtility {

        private static SerializedObject source;
        private static TransformData sourceTransformData;

        [MenuItem("CONTEXT/Component/Copy Serialization")]
        public static void CopySerializedFromBase(MenuCommand command) { 
            source = new SerializedObject(command.context); 
        }

        [MenuItem("CONTEXT/Component/Paste Serialization")]
        public static void PasteSerializedFromBase(MenuCommand command) {
            SerializedObject dest = new SerializedObject(command.context);
            SerializedProperty prop_iterator = source.GetIterator();
            //jump into serialized object, this will skip script type so that we dont override the destination component's type
            if (prop_iterator.NextVisible(true)) {
                while (prop_iterator.NextVisible(true)) //itterate through all serializedProperties
                {
                    //try obtaining the property in destination component
                    SerializedProperty prop_element = dest.FindProperty(prop_iterator.name);

                    //validate that the properties are present in both components, and that they're the same type
                    if (prop_element != null && prop_element.propertyType == prop_iterator.propertyType) {
                        //copy value from source to destination component
                        dest.CopyFromSerializedProperty(prop_iterator);
                    }
                }
            }
            dest.ApplyModifiedProperties();
        }

        [MenuItem("CONTEXT/Transform/Deep Copy")]
        public static void DeepCopyTransform(MenuCommand command) {
            var transform = command.context as Transform;
            if (ReferenceEquals(transform, null)) return;
            var transformData = new TransformData();
            HandleDeepTransforms(transform, transformData, (_transform, _transformData) => {
                _transformData.localPosition = _transform.localPosition;
                _transformData.localRotation = _transform.localRotation.eulerAngles;
                _transformData.localScale = _transform.localScale;
                _transformData.children = new TransformData[_transform.childCount];
                for (int i = 0; i < _transform.childCount; i++) {
                    _transformData.children[i] = new TransformData();
                }
            });
            sourceTransformData = transformData;
        }

        [MenuItem("CONTEXT/Transform/Deep Paste", validate = true)]
        public static bool ValidateDeepPasteTransform(MenuCommand command) {
            return sourceTransformData != null;
        }

        [MenuItem("CONTEXT/Transform/Deep Paste")]
        public static void DeepPasteTransform(MenuCommand command) {
            var transform = command.context as Transform;
            if (ReferenceEquals(transform, null)) return;
            var transformData = sourceTransformData;
            HandleDeepTransforms(transform, transformData, (_transform, _transformData) => {
                _transform.localPosition = _transformData.localPosition;
                _transform.localRotation = Quaternion.Euler(_transformData.localRotation);
                _transform.localScale = _transformData.localScale;
                //_transformData.children = new TransformData[_transform.childCount];
                //for (int i = 0; i < _transform.childCount; i++) {
                //    _transformData.children[i] = new TransformData();
                //}
            });
        }

        private static void HandleDeepTransforms(Transform _transform, TransformData _transformData, System.Action<Transform, TransformData> _handler) {
            _handler.Invoke(_transform, _transformData);
            for (int i = 0; i < _transform.childCount; i++) {
                var child = _transform.GetChild(i);
                var childData = _transformData.children[i];
                HandleDeepTransforms(child, childData, _handler);
            }
        }

        [System.Serializable]
        public class TransformData {
            public Vector3 localPosition;
            public Vector3 localRotation;
            public Vector3 localScale;
            public TransformData[] children;
        }

    }

}