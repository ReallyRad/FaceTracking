using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Binds a component with an asset, so that the component is accessible from outside the scene by getting the component using the binder asset.
    /// </summary>
#if SEQUINE
    [AddComponentMenu("Sequine/Cross Scene Binder")]
#else
    [AddComponentMenu("CookieCutter/Cross Scene Binder")]
#endif
    [ExecuteAlways]
    public class CrossSceneBinder : MonoBehaviour {

        [SerializeField] private UnityEngine.Object m_binderAsset;
        public UnityEngine.Object binderAsset => m_binderAsset;

        [SerializeField] private Component m_componentToBind;
        public Component componentToBind => m_componentToBind;

        [Tooltip("In order to register a binded component, it needs to be activated during play mode.\nHence, if you need the GameObject to be initially deactivated during play mode, set this to true.")]
        [SerializeField] private bool deactivateGameObjectOnAwake = false;

        private void Awake()
        {
            if (m_binderAsset != null) {
                if (m_componentToBind != null) {
                    CrossSceneBindingUtility.Bind(m_binderAsset, m_componentToBind);
                }
                else {
#if UNITY_EDITOR
                    if (Application.isPlaying)
#endif
                    Debug.Log(name + " has no binded component.", gameObject);
                }
            }
            else {
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                Debug.Log(name + " has no binder asset.", gameObject);
            }

            if (deactivateGameObjectOnAwake && Application.isPlaying)
            {
                gameObject.SetActive(false);
                if (Application.isPlaying) deactivateGameObjectOnAwake = false;
            }
        }

        private void OnEnable() {
            if (m_binderAsset != null) {
                if (m_componentToBind != null) {
                    CrossSceneBindingUtility.Bind(m_binderAsset, m_componentToBind);
                }
                else {
#if UNITY_EDITOR
                    if (Application.isPlaying)
#endif
                    Debug.Log(name + " has no binded component.", gameObject);
                }
            }
            else {
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                Debug.Log(name + " has no binder asset.", gameObject);
            }

            if (deactivateGameObjectOnAwake && Application.isPlaying)
            {
                gameObject.SetActive(false);
                if (Application.isPlaying) deactivateGameObjectOnAwake = false;
            }
        }

        private void OnValidate()
        {
            if (m_binderAsset == null && m_componentToBind == null) {
                m_componentToBind = GetComponent<PlayableDirector>();
            }
            if (m_binderAsset == null && m_componentToBind != null)
            {
                if (m_componentToBind is PlayableDirector playableDirector)
                {
                    m_binderAsset = playableDirector.playableAsset;
                }
            }
        }

        private void OnDestroy() {
            if (m_binderAsset != null && m_componentToBind != null) {
                CrossSceneBindingUtility.RemoveBinding(m_binderAsset, m_componentToBind);
            }
        }

    }
}