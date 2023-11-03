using System;
using UnityEditor;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;
#else
using Calcatz.OdinSerializer;
using SerializationUtility = Calcatz.OdinSerializer.SerializationUtility;
#endif

namespace Calcatz.CookieCutter {
    public class ClipboardUtility {

        private static object copiedObject = null;

        public static void Copy(object _objectToCopy) {
            copiedObject = _objectToCopy;
            //EditorGUIUtility.systemCopyBuffer = Convert.ToBase64String(SerializationUtility.SerializeValue(_objectToCopy, DataFormat.JSON));
        }

        public static T Paste<T>() {
            /*byte[] copied = null;
            try {
                copied = Convert.FromBase64String(EditorGUIUtility.systemCopyBuffer);
            }
            catch {

            }
            return SerializationUtility.DeserializeValue<T>(copied, DataFormat.JSON);*/
            if (IsPasteAvailable<T>()) {
                return (T)SerializationUtility.CreateCopy(copiedObject);
            }
            return default(T);
        }

        public static bool IsPasteAvailable<T>() {
            if (copiedObject != null) {
                if (copiedObject.GetType() == typeof(T)) {
                    return true;
                }
            }
            return false;
        }
    }
}