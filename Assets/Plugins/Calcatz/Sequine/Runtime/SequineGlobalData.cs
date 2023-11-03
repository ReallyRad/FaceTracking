using System.Collections.Generic;
using UnityEngine;
using Calcatz.CookieCutter;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;
#else
using Calcatz.OdinSerializer;
using SerializationUtility = Calcatz.OdinSerializer.SerializationUtility;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Calcatz.Sequine {

    /// <summary>
    /// The container of Global Variables, which are basically Variables,
    /// except they can be accessed and shared globally, not limited to the scope of the Sequine Flow object.
    /// </summary>
    public class SequineGlobalData : SerializedScriptableObject, IVariableUser {

        #region STATIC
        public static SequineGlobalData m_persistent;
        public static SequineGlobalData GetPersistent() {
            if (m_persistent == null) {
                m_persistent = Resources.Load<SequineGlobalData>("SequineGlobalData");
#if UNITY_EDITOR
                if (m_persistent == null) {
                    m_persistent = ScriptableObject.CreateInstance<SequineGlobalData>();
                    if (!System.IO.Directory.Exists(Application.dataPath + "/Resources")) {
                        System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources");
                    }
                    AssetDatabase.CreateAsset(m_persistent, "Assets/Resources/SequineGlobalData.asset");
                    AssetDatabase.SaveAssets();
                }
#endif
            }
            return m_persistent;
        }

        private static SequineGlobalData m_transient;
        public static SequineGlobalData GetTransient() {
            if (!Application.isPlaying) {
                Debug.LogError("GetTransient() can only be called during runtime.");
                return null;
            }
            if (m_transient == null) {
                m_transient = Instantiate(GetPersistent());
            }
            return m_transient;
        }
        #endregion

        #region PROPERTIES
        public Object targetObject => this;

        [OdinSerialize] private DataContainer m_data = new DataContainer();
        public Dictionary<int, CommandData.Variable> variables { get => m_data.variables; set => m_data.variables = value; }

        public static T GetVariable<T>(int _id) {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                Debug.Log("Transient variable can only be called during runtime.");
                return default(T);
            }
#endif
            if (GetTransient().variables.TryGetValue(_id, out var variable)) {
                return (T)variable.value;
            }
            Debug.LogError("Can't get variable: There is no global variable with id " + _id);
            return default(T);
        }

        public static bool TryGetVariable<T>(int _id, out T _value) {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                Debug.Log("Transient variable can only be called during runtime.");
                _value = default(T);
                return false;
            }
#endif
            if (GetTransient().variables.TryGetValue(_id, out var variable)) {
                _value = (T)variable.value;
                return true;
            }
            _value = default(T);
            return false;
        }

        public static void SetVariable<T>(int _id, T _value) {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                Debug.Log("Transient variable can only be called during runtime.");
                return;
            }
#endif
            if (GetTransient().variables.TryGetValue(_id, out var variable)) {
                variable.value = _value;
            }
            else {
                Debug.LogError("Can't set variable: There is no global variable with id " + _id);
            }
        }

        public static bool TrySetVariable<T>(int _id, T _value) {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                Debug.Log("Transient variable can only be called during runtime.");
                return false;
            }
#endif
            if (GetTransient().variables.TryGetValue(_id, out var variable)) {
                variable.value = _value;
                return true;
            }
            else {
                return false;
            }
        }
        #endregion

        private void OnDestroy() {
            if (m_transient == this) m_transient = null;
        }

        /// <summary>
        /// Use this to create save data.
        /// </summary>
        /// <returns></returns>
        public static byte[] SerializeToBinary() {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                Debug.Log("Serialize to Binary can only be called during runtime.");
                return null;
            }
#endif
            return SerializationUtility.SerializeValue(GetTransient().m_data, DataFormat.Binary);
        }

        /// <summary>
        /// Use this to load data.
        /// </summary>
        /// <param name="_bytes"></param>
        public static void DeserializeFromBinary(byte[] _bytes) {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                Debug.Log("Deserialize from Binary can only be called during runtime.");
                return;
            }
#endif
            GetTransient().m_data = SerializationUtility.DeserializeValue<DataContainer>(_bytes, DataFormat.Binary);
        }

        [System.Serializable]
        public class DataContainer {
            [OdinSerialize] private Dictionary<int, CommandData.Variable> m_variables = new Dictionary<int, CommandData.Variable>();
            public Dictionary<int, CommandData.Variable> variables { get => m_variables; set => m_variables = value; }
        }

    }

}