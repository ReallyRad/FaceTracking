
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter.Audio {

    /// <summary>
    /// Library of sound groups, each identified with a customizable ID.
    /// </summary>
    [System.Serializable]
    public class SoundLibrary {

        [SerializeField] private SoundData[] soundData;

        private Dictionary<string, SoundData> soundDictionary = new Dictionary<string, SoundData>();

        /// <summary>
        /// Initialize the sound dictionary based on sound data.
        /// </summary>
        public void Init() {
            foreach (SoundData sound in soundData) {
                soundDictionary.Add(sound.soundID, sound);
            }
        }

        public float GetClipVolume(string id) {
            if (soundDictionary.TryGetValue(id, out SoundData soundData)) {
                return soundData.volume;
            }
            return 0f;
        }

        /// <summary>
        /// Get an audio clip by ID. If there are more than one clip, the get at random index.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AudioClip GetClipByID(string id) {
            if (string.IsNullOrEmpty(id)) {
                Debug.LogError("Can't get a clip using an empty ID.");
                return null;
            }
            if (soundDictionary.ContainsKey(id)) {
                AudioClip[] sounds = soundDictionary[id].group;
                return sounds[Random.Range(0, sounds.Length)];
            }
            else {
                Debug.LogError("Clip with id " + id + " not found.");
            }
            return null;
        }

        /// <summary>
        /// Get an audio clip at index of a clip group with the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public AudioClip GetClipByID(string id, int index) {
            if (soundDictionary.ContainsKey(id)) {
                AudioClip[] sounds = soundDictionary[id].group;
                if (index < sounds.Length) {
                    return sounds[index];
                }
            }
            return null;
        }

        public string[] GetAllIDs() {
            var allIds = new string[soundData.Length];
            for (int i=0; i<allIds.Length; i++) {
                allIds[i] = soundData[i].soundID;
            }
            return allIds;
        }

        [System.Serializable]
        public class SoundData {
            public string soundID;
            [Range(0, 1)]
            public float volume = 1;
            [Tooltip("A clip will be choosen randomly from this group.\nAdd more than one sound to make it sounds variative such as Footsteps.")]
            public AudioClip[] group;
        }

    }
}