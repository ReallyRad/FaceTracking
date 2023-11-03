using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter.Audio {
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "CookieCutter/Audio/Audio Config Asset", order = 1)]
    public class AudioConfig : ScriptableObject {
        internal const string k_AudioConfigResourcePath = "AudioConfig";
        internal const string k_AudioConfigPath = "Assets/Resources/" + k_AudioConfigResourcePath + ".asset";

        private static AudioConfig instance;
        public static AudioConfig Instance {
            get {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    Debug.LogError("Can't access AudioConfig when not in Play Mode");
                    return null;
                }
#endif
                if (instance == null) {
                    instance = Resources.Load(k_AudioConfigResourcePath) as AudioConfig;
#if UNITY_EDITOR
                    if (instance == null) {
                        instance = GetOrCreateSettings();
                        Debug.LogWarning("AudioConfig not found, and is now being newly created.");
                    }
#endif
                    instance.Init();

#if UNITY_EDITOR
                    UnityEditor.EditorApplication.playModeStateChanged += _playMode => {
                        if (_playMode == UnityEditor.PlayModeStateChange.ExitingEditMode) {
                            if (instance != null) instance.Init();
                        }
                    };
#endif
                }
                return instance;
            }
        }

        [Min(0)][SerializeField] internal float m_defaultAudioFadeDuration = 1f;

        [Header("Volumes")]
        [Range(0, 1)] [SerializeField] private float m_masterVolume = 1f;
        [Range(0, 1)] [SerializeField] private float m_bgmVolume = 1f;
        [Range(0, 1)] [SerializeField] private float m_ambienceVolume = 1f;
        [Range(0, 1)] [SerializeField] private float m_meVolume = 1f;
        [Range(0, 1)] [SerializeField] private float m_sfxVolume = 1f;

        [Header("Sound Libraries")]
        [SerializeField] internal SoundLibrary m_bgmLibrary;
        [SerializeField] internal SoundLibrary m_ambienceLibrary;
        [SerializeField] internal SoundLibrary m_meLibrary;
        [SerializeField] internal SoundLibrary m_sfxLibrary;

        public void Init() {
            m_bgmLibrary.Init();
            m_ambienceLibrary.Init();
            m_meLibrary.Init();
            m_sfxLibrary.Init();
        }

        public static float defaultAudioFadeDuration { get => Instance.m_defaultAudioFadeDuration; set => Instance.m_defaultAudioFadeDuration = value; }

        public static SoundLibrary bgmLibrary { get => Instance.m_bgmLibrary; set => Instance.m_bgmLibrary = value; }
        public static SoundLibrary ambienceLibrary { get => Instance.m_ambienceLibrary; set => Instance.m_ambienceLibrary = value; }
        public static SoundLibrary meLibrary { get => Instance.m_meLibrary; set => Instance.m_meLibrary = value; }
        public static SoundLibrary sfxLibrary { get => Instance.m_sfxLibrary; set => Instance.m_sfxLibrary = value; }

        public static float masterVolume { get => Instance.m_masterVolume; set => Instance.m_masterVolume = value; }
        public static float bgmVolume { get => Instance.m_bgmVolume; set => Instance.m_bgmVolume = value; }
        public static float bgmMasteredVolume => bgmVolume * masterVolume;
        public static float ambienceVolume { get => Instance.m_ambienceVolume; set => Instance.m_ambienceVolume = value; }
        public static float ambienceMasteredVolume => ambienceVolume * masterVolume;
        public static float meVolume { get => Instance.m_meVolume; set => Instance.m_meVolume = value; }
        public static float meMasteredVolume => meVolume * masterVolume;
        public static float sfxVolume { get => Instance.m_sfxVolume; set => Instance.m_sfxVolume = value; }
        public static float sfxMasteredVolume => sfxVolume * masterVolume;

#if UNITY_EDITOR
        internal static AudioConfig GetOrCreateSettings() {
            var settings = (AudioConfig)Resources.Load(k_AudioConfigResourcePath);
            if (settings == null) {
                settings = ScriptableObject.CreateInstance<AudioConfig>();
                if (!System.IO.Directory.Exists(Application.dataPath + "/Resources")) {
                    System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources");
                }
                UnityEditor.AssetDatabase.CreateAsset(settings, k_AudioConfigPath);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static UnityEditor.SerializedObject GetSerializedSettings() {
            return new UnityEditor.SerializedObject(GetOrCreateSettings());
        }

        internal static bool IsSettingsAvailable() {
            return Resources.Load(k_AudioConfigResourcePath) != null;
        }
#endif

    }
}