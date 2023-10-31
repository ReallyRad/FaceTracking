using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Calcatz.CookieCutter.Audio {
    public class AudioManager : MonoBehaviour {

        private static AudioManager instance;
        public static AudioManager Instance {
            get {
                if (instance == null) {
                    GameObject go = new GameObject("AudioManager");
                    instance = go.AddComponent<AudioManager>();
                    instance.Init();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        public static bool IsDestroyed() {
            return instance == null;
        }

        private void OnDestroy() {
            if (instance == this) {
                instance = null;
            }
        }

        #region PROPERTIES
        public enum Channel { Master, BGM, Ambience, ME, SFX };

        private const string masterPlayerPrefs = "master vol";
        private const string bgmPlayerPrefs = "bgm vol";
        private const string ambiencePlayerPrefs = "ambience vol";
        private const string mePlayerPrefs = "me vol";
        private const string sfxPlayerPrefs = "sfx vol";

        //private AudioSource sfx2DSource;
        private AudioSource[] bgmSources;
        private int activeBGMSourceIndex;

        private AudioSource[] ambienceSources;
        private int activeAmbienceSourceIndex;

        private List<AudioSourceAutoAdjustVolume> audioSourceInstances = new List<AudioSourceAutoAdjustVolume>();
        private AudioSource[] sfxAudioSourcePool = new AudioSource[20];
        private int currentSfxAudioPool = 0;

        private Coroutine bgmFadeCoroutine;
        private Coroutine ambienceFadeCoroutine;
        #endregion PROPERTIES

        #region REGION INITIALIZATION
        private void Init() {
            CreateGameObjects();

            AudioConfig.masterVolume = PlayerPrefs.GetFloat(masterPlayerPrefs, AudioConfig.masterVolume);
            AudioConfig.bgmVolume = PlayerPrefs.GetFloat(bgmPlayerPrefs, AudioConfig.bgmVolume);
            AudioConfig.ambienceVolume = PlayerPrefs.GetFloat(ambiencePlayerPrefs, AudioConfig.ambienceVolume);
            AudioConfig.meVolume = PlayerPrefs.GetFloat(mePlayerPrefs, AudioConfig.meVolume);
            AudioConfig.sfxVolume = PlayerPrefs.GetFloat(sfxPlayerPrefs, AudioConfig.sfxVolume);

            PlayerPrefs.Save();
        }
        private void CreateGameObjects() {
            CreateAudioTransitionGameObjects(out bgmSources, "Music Source");
            CreateAudioTransitionGameObjects(out ambienceSources, "Ambience Source");

            for(int i=0; i<sfxAudioSourcePool.Length; i++) {
                GameObject oneShotAudioGO = new GameObject("[Audio Pool]");
                oneShotAudioGO.transform.parent = Instance.transform;
                AudioSource oneShotAudio = oneShotAudioGO.AddComponent<AudioSource>();
                oneShotAudio.playOnAwake = false;
                sfxAudioSourcePool[i] = oneShotAudio;
            }

            /*GameObject newSfx2DSource = new GameObject("SFX 2D Source");
            sfx2DSource = newSfx2DSource.AddComponent<AudioSource>();
            newSfx2DSource.transform.parent = transform;*/
        }

        private void CreateAudioTransitionGameObjects(out AudioSource[] _audioTransitionGameObjects, string _gameObjectName) {
            _audioTransitionGameObjects = new AudioSource[2];
            for (int i = 0; i < 2; i++) {
                GameObject go = new GameObject(_gameObjectName + " " + (i + 1));
                _audioTransitionGameObjects[i] = go.AddComponent<AudioSource>();
                _audioTransitionGameObjects[i].playOnAwake = false;
                _audioTransitionGameObjects[i].loop = true;
                go.transform.parent = transform;
            }
        }
        #endregion INITIALIZATION

        #region PRIVATE-ORDER
        private IEnumerator AnimateCrossfade(AudioSource[] _audioSources, int _activeSourceIndex, float _targetVolume, float _duration) {
            if (_duration < 0) {
                _duration = AudioConfig.defaultAudioFadeDuration;
            }
            if (_duration > 0) {
                float percent = 0;
                while (percent < 1) {
                    percent += Time.unscaledDeltaTime * (1 / _duration);
                    _audioSources[_activeSourceIndex].volume = Mathf.Lerp(0, _targetVolume, percent);
                    _audioSources[1 - _activeSourceIndex].volume = Mathf.Lerp(_targetVolume, 0, percent);
                    yield return null;
                }
            }
            _audioSources[_activeSourceIndex].volume = _targetVolume;
            _audioSources[1 - _activeSourceIndex].volume = 0;
            _audioSources[1 - _activeSourceIndex].Pause();
        }

        private static void RefreshTransitionAudioSourceVolumes(AudioSource[] _audioSources, int _activeSourceIndex, float _masteredVolume) {
            _audioSources[_activeSourceIndex].volume = _masteredVolume;
            _audioSources[1 - _activeSourceIndex].volume = 0;
        }

        #endregion PRIVATE-ORDER

        #region REGION PUBLIC-ORDER

        /// <summary>
        /// Set volume of the selected channel.
        /// </summary>
        /// <param name="_volumePercent"></param>
        /// <param name="_channel"></param>
        public static void SetVolume(float _volumePercent, Channel _channel) {
            if (_channel == Channel.Master) {
                AudioConfig.masterVolume = _volumePercent;

                RefreshTransitionAudioSourceVolumes(Instance.bgmSources, Instance.activeBGMSourceIndex, AudioConfig.bgmMasteredVolume);
                RefreshTransitionAudioSourceVolumes(Instance.ambienceSources, Instance.activeAmbienceSourceIndex, AudioConfig.ambienceMasteredVolume);
                foreach (AudioSourceAutoAdjustVolume audioSource in instance.audioSourceInstances) {
                    audioSource.SetVolume(AudioConfig.sfxMasteredVolume);
                }

                PlayerPrefs.SetFloat(masterPlayerPrefs, _volumePercent);
            }
            else if (_channel == Channel.BGM) {
                AudioConfig.bgmVolume = _volumePercent;
                RefreshTransitionAudioSourceVolumes(Instance.bgmSources, Instance.activeBGMSourceIndex, AudioConfig.bgmMasteredVolume);
                PlayerPrefs.SetFloat(bgmPlayerPrefs, _volumePercent);
            }
            else if (_channel == Channel.Ambience) {
                AudioConfig.ambienceVolume = _volumePercent;
                RefreshTransitionAudioSourceVolumes(Instance.ambienceSources, Instance.activeAmbienceSourceIndex, AudioConfig.ambienceMasteredVolume);
                PlayerPrefs.SetFloat(ambiencePlayerPrefs, _volumePercent);
            }
            else if (_channel == Channel.ME) {
                AudioConfig.meVolume = _volumePercent;
                PlayerPrefs.SetFloat(mePlayerPrefs, _volumePercent);
            }
            else if (_channel == Channel.SFX) {
                AudioConfig.sfxVolume = _volumePercent;
                foreach (AudioSourceAutoAdjustVolume audioSource in instance.audioSourceInstances) {
                    audioSource.SetVolume(AudioConfig.sfxMasteredVolume);
                }
                PlayerPrefs.SetFloat(sfxPlayerPrefs, _volumePercent);
            }
            else {
                return;
            }

            PlayerPrefs.Save();
        }

        #region SUB-REGION AUDIO-SOURCE
        public static void AddAudioSourceInstance(AudioSourceAutoAdjustVolume _audioSource) {
            if (!instance.audioSourceInstances.Contains(_audioSource)) {
                instance.audioSourceInstances.Add(_audioSource);
                _audioSource.SetVolume(AudioConfig.sfxMasteredVolume);
            }
        }
        public static void RemoveAudioSourceInstance(AudioSourceAutoAdjustVolume _audioSource) {
            instance.audioSourceInstances.Remove(_audioSource);
        }
        #endregion AUDIO-SOURCE

        #region  SUB-REGION BGM
        /// <summary>
        /// Play music with given crossfade duration.
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_restart">Restart if the clip has already been played.</param>
        /// <param name="_fadeDuration">Set to 0 to disable crossfade, and set to -1 to use default fade duration.</param>
        public static void PlayBGM(AudioClip _clip, bool _restart = true , float _fadeDuration = -1, float _volumeMultiplier = 1f) {
            if (!_restart) {
                if (Instance.bgmSources[Instance.activeBGMSourceIndex].clip == _clip) {
                    return;
                }
            }
            Instance.activeBGMSourceIndex = 1 - Instance.activeBGMSourceIndex;
            Instance.bgmSources[Instance.activeBGMSourceIndex].clip = _clip;
            Instance.bgmSources[Instance.activeBGMSourceIndex].Play();
            if (Instance.bgmFadeCoroutine != null) {
                Instance.StopCoroutine(Instance.bgmFadeCoroutine);
            }
            Instance.bgmFadeCoroutine =
                Instance.StartCoroutine(Instance.AnimateCrossfade(Instance.bgmSources, Instance.activeBGMSourceIndex, AudioConfig.bgmMasteredVolume * _volumeMultiplier, _fadeDuration));
        }
        /// <summary>
        /// Play music with given sound name/ID in SoundLibrary.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_restart">Restart if the clip has already been played.</param>
        /// <param name="_fadeDuration">Set to 0 to disable crossfade, and set to -1 to use default fade duration.</param>
        public static void PlayBGM(string _id, bool _restart = true, float _fadeDuration = -1, float _volumeMultiplier = 1f) {
            PlayBGM(AudioConfig.bgmLibrary.GetClipByID(_id), _restart, _fadeDuration, AudioConfig.bgmLibrary.GetClipVolume(_id) * _volumeMultiplier);
        }
        /// <summary>
        /// Return AudioClip of the currently/lastly played music.
        /// </summary>
        /// <returns></returns>
        public static AudioClip GetActiveBGM() {
            return Instance.bgmSources[Instance.activeBGMSourceIndex].clip;
        }
        /// <summary>
        /// Stop current music with given crossfade duration.
        /// </summary>
        /// <param name="_fadeDuration"></param>
        public static void StopBGM(float _fadeDuration = -1) {
            Instance.activeBGMSourceIndex = 1 - Instance.activeBGMSourceIndex;
            Instance.bgmSources[Instance.activeBGMSourceIndex].clip = null;
            Instance.bgmSources[Instance.activeBGMSourceIndex].Play();
            if (Instance.bgmFadeCoroutine != null) {
                Instance.StopCoroutine(Instance.bgmFadeCoroutine);
            }
            Instance.bgmFadeCoroutine =
                Instance.StartCoroutine(Instance.AnimateCrossfade(Instance.bgmSources, Instance.activeBGMSourceIndex, AudioConfig.bgmMasteredVolume, _fadeDuration));
        }
        /// <summary>
        /// Resume the previously stopped BGM.
        /// </summary>
        /// <param name="_fadeDuration">Set to 0 to disable crossfade, and set to -1 to use default fade duration.</param>
        public static void ResumeBGM(float _fadeDuration = -1) {
            Instance.activeBGMSourceIndex = 1 - Instance.activeBGMSourceIndex;
            Instance.bgmSources[Instance.activeBGMSourceIndex].UnPause();
            if (Instance.bgmFadeCoroutine != null) {
                Instance.StopCoroutine(Instance.bgmFadeCoroutine);
            }
            Instance.bgmFadeCoroutine =
                Instance.StartCoroutine(Instance.AnimateCrossfade(Instance.bgmSources, Instance.activeBGMSourceIndex, AudioConfig.bgmMasteredVolume, _fadeDuration));
        }
        #endregion BGM

        #region  SUB-REGION AMBIENCE
        /// <summary>
        /// Play ambience with given crossfade duration.
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_restart">Restart if the clip has already been played.</param>
        /// <param name="_fadeDuration">Set to 0 to disable crossfade, and set to -1 to use default fade duration.</param>
        public static void PlayAmbience(AudioClip _clip, bool _restart = true, float _fadeDuration = -1, float _volumeMultiplier = 1f) {
            if (!_restart) {
                if (Instance.ambienceSources[Instance.activeAmbienceSourceIndex].clip == _clip) {
                    return;
                }
            }
            Instance.activeAmbienceSourceIndex = 1 - Instance.activeAmbienceSourceIndex;
            Instance.ambienceSources[Instance.activeAmbienceSourceIndex].clip = _clip;
            Instance.ambienceSources[Instance.activeAmbienceSourceIndex].Play();
            if (Instance.ambienceFadeCoroutine != null) {
                Instance.StopCoroutine(Instance.ambienceFadeCoroutine);
            }
            Instance.ambienceFadeCoroutine =
                Instance.StartCoroutine(Instance.AnimateCrossfade(Instance.ambienceSources, Instance.activeAmbienceSourceIndex, AudioConfig.ambienceMasteredVolume * _volumeMultiplier, _fadeDuration));
        }
        /// <summary>
        /// Play ambience with given sound name/ID in SoundLibrary.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_fadeDuration">Set to 0 to disable crossfade, and set to -1 to use default fade duration.</param>
        public static void PlayAmbience(string _id, bool _restart = true, float _fadeDuration = -1, float _volumeMultiplier = 1f) {
            PlayAmbience(AudioConfig.ambienceLibrary.GetClipByID(_id), _restart, _fadeDuration, AudioConfig.ambienceLibrary.GetClipVolume(_id) * _volumeMultiplier);
        }
        /// <summary>
        /// Return AudioClip of the currently/lastly played ambience.
        /// </summary>
        /// <returns></returns>
        public static AudioClip GetActiveAmbience() {
            return Instance.ambienceSources[Instance.activeAmbienceSourceIndex].clip;
        }
        /// <summary>
        /// Stop current ambience with given crossfade duration.
        /// </summary>
        /// <param name="_fadeDuration">Set to 0 to disable crossfade, and set to -1 to use default fade duration.</param>
        public static void StopAmbience(float _fadeDuration = -1) {
            Instance.activeAmbienceSourceIndex = 1 - Instance.activeAmbienceSourceIndex;
            Instance.ambienceSources[Instance.activeAmbienceSourceIndex].clip = null;
            Instance.ambienceSources[Instance.activeAmbienceSourceIndex].Play();
            if (Instance.ambienceFadeCoroutine != null) {
                Instance.StopCoroutine(Instance.ambienceFadeCoroutine);
            }
            Instance.ambienceFadeCoroutine =
                Instance.StartCoroutine(Instance.AnimateCrossfade(Instance.ambienceSources, Instance.activeAmbienceSourceIndex, AudioConfig.ambienceMasteredVolume, _fadeDuration));
        }
        /// <summary>
        /// Resume the previously stopped ambience with given crossfade duration.
        /// </summary>
        /// <param name="_fadeDuration">Set to 0 to disable crossfade, and set to -1 to use default fade duration.</param>
        public static void ResumeAmbience(float _fadeDuration = -1) {
            Instance.activeAmbienceSourceIndex = 1 - Instance.activeAmbienceSourceIndex;
            Instance.ambienceSources[Instance.activeAmbienceSourceIndex].UnPause();
            if (Instance.ambienceFadeCoroutine != null) {
                Instance.StopCoroutine(Instance.ambienceFadeCoroutine);
            }
            Instance.ambienceFadeCoroutine =
                Instance.StartCoroutine(Instance.AnimateCrossfade(Instance.ambienceSources, Instance.activeAmbienceSourceIndex, AudioConfig.ambienceMasteredVolume, _fadeDuration));
        }
        #endregion AMBIENCE

        #region SUB-REGION GENERAL-SOUND
        /// <summary>
        /// Play 3D sound on the given position.
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_position"></param>
        /// <param name="_volume">If volume is less than 0, then the value will set to master volume.</param>
        /// <param name="_onComplete"></param>
        /// <returns></returns>
        public static AudioSource PlaySoundAtPosition(AudioClip _clip, Vector3 _position, float _volume = -1, System.Action _onComplete = null) {
            if (Instance == null) {
                Debug.LogWarning("AudioManager instance doesn't exist");
                return null;
            }
            if (_clip != null) {
                if (_volume < 0) _volume = AudioConfig.masterVolume;
                //AudioSource.PlayClipAtPoint(clip, pos, Instance.sfxVolumePercent * Instance.masterVolumePercent);
                AudioSource oneShotAudio = Instance.sfxAudioSourcePool[Instance.currentSfxAudioPool];
                oneShotAudio.transform.position = _position;
                oneShotAudio.clip = _clip;
                oneShotAudio.volume = _volume;
                oneShotAudio.rolloffMode = AudioRolloffMode.Linear;
                oneShotAudio.spatialBlend = 0.9f;
                oneShotAudio.Play();
                if (_onComplete != null) {
                    Instance.StartCoroutine(PlaySoundCoroutine(oneShotAudio, _onComplete));
                }
                instance.currentSfxAudioPool++;
                if (instance.currentSfxAudioPool >= instance.sfxAudioSourcePool.Length) {
                    instance.currentSfxAudioPool = 0;
                }
                return oneShotAudio;
            }
            return null;
        }

        private static IEnumerator PlaySoundCoroutine(AudioSource _oneShotAudio, System.Action _onComplete = null) {
            while (_oneShotAudio.isPlaying) {
                yield return null;
            }
            if (_onComplete != null) {
                _onComplete.Invoke();
            }
            //Destroy(_oneShotAudio.gameObject);
        }

        /// <summary>
        /// Play 2D sound, ignoring the position of sound's source.
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_volume">If volume is less than 0, then the value will set to master volume.</param>
        /// <param name="_onComplete"></param>
        /// <returns></returns>
        public static AudioSource PlaySound2D(AudioClip _clip, float _volume = -1, System.Action _onComplete = null) {
            if (Instance == null) {
                Debug.LogWarning("AudioManager instance doesn't exist");
                return null;
            }
            if (_clip != null) {
                if (_volume < 0) _volume = AudioConfig.masterVolume;
                AudioSource oneShotAudio = PlaySoundAtPosition(_clip, Vector3.zero, _volume, _onComplete);
                if (oneShotAudio != null) {
                    oneShotAudio.spatialBlend = 0;
                }
                //Instance.sfx2DSource.PlayOneShot(clip, Instance.sfxVolumePercent * Instance.masterVolumePercent);
            }
            return null;
        }
        #endregion

        #region SUB-REGION SFX
        /// <summary>
        /// Play sound effect with given name/ID in SoundLibrary on the given position.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_position"></param>
        public static void PlaySFXAtPosition(string _id, Vector3 _position, System.Action _onComplete = null, float _volumeMultiplier = 1f) {
            if (Instance != null) {
                PlaySoundAtPosition(AudioConfig.sfxLibrary.GetClipByID(_id), _position, _volumeMultiplier * AudioConfig.sfxMasteredVolume * AudioConfig.sfxLibrary.GetClipVolume(_id), _onComplete);
            }
            else {
                Debug.LogWarning("AudioManager instance doesn't exist");
            }
        }

        /// <summary>
        /// Play 2D sound effect with given name/ID in SoundLibrary, ignoring the position of sound's source.
        /// </summary>
        /// <param name="_id"></param>
        public static void PlaySFX2D(string _id, System.Action _onComplete = null, float _volumeMultiplier = 1f) {
            if (Instance == null) {
                Debug.LogWarning("AudioManager instance doesn't exist");
                return;
            }
            AudioSource oneShotAudio = PlaySoundAtPosition(AudioConfig.sfxLibrary.GetClipByID(_id), Vector3.zero, _volumeMultiplier * AudioConfig.sfxMasteredVolume * AudioConfig.sfxLibrary.GetClipVolume(_id), _onComplete);
            if (oneShotAudio != null) {
                oneShotAudio.spatialBlend = 0;
            }
            //Instance.sfx2DSource.PlayOneShot(Instance.sfxLibrary.getClipByID(id), Instance.sfxVolumePercent * Instance.masterVolumePercent);
        }
        #endregion SFX
        
        #region SUB-REGION ME
        /// <summary>
        /// Play music effect with given name/ID in SoundLibrary on the given position.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_position"></param>
        public static void PlayMEAtPosition(string _id, Vector3 _position, System.Action _onComplete = null, float _volumeMultiplier = 1f) {
            if (Instance != null) {
                PlaySoundAtPosition(AudioConfig.meLibrary.GetClipByID(_id), _position, _volumeMultiplier * AudioConfig.meMasteredVolume * AudioConfig.meLibrary.GetClipVolume(_id), _onComplete);
            }
            else {
                Debug.LogWarning("AudioManager instance doesn't exist");
            }
        }

        /// <summary>
        /// Play 2D music effect with given name/ID in SoundLibrary, ignoring the position of sound's source.
        /// </summary>
        /// <param name="_id"></param>
        public static void PlayME2D(string _id, System.Action _onComplete = null, float _volumeMultiplier = 1f) {
            if (Instance == null) {
                Debug.LogWarning("AudioManager instance doesn't exist");
                return;
            }
            AudioSource oneShotAudio = PlaySoundAtPosition(AudioConfig.meLibrary.GetClipByID(_id), Vector3.zero, _volumeMultiplier * AudioConfig.meMasteredVolume * AudioConfig.meLibrary.GetClipVolume(_id), _onComplete);
            if (oneShotAudio != null) {
                oneShotAudio.spatialBlend = 0;
            }
            //Instance.sfx2DSource.PlayOneShot(Instance.sfxLibrary.getClipByID(id), Instance.sfxVolumePercent * Instance.masterVolumePercent);
        }
        #endregion ME

        #endregion PUBLIC-ORDER

    }
}