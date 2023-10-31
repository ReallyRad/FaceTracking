#if TIMELINE_AVAILABLE
using UnityEngine;
using UnityEngine.Playables;

namespace Calcatz.CookieCutter {
    class CrossSceneActivationMixerPlayable : PlayableBehaviour {
	    private CrossSceneActivationTrack.PostPlaybackState m_PostPlaybackState;
        private bool m_BoundGameObjectInitialStateIsActive;

	    private UnityEngine.Object m_binder;
        private GameObject m_BoundGameObject;


        public static ScriptPlayable<CrossSceneActivationMixerPlayable> Create(PlayableGraph graph, int inputCount) {
            return ScriptPlayable<CrossSceneActivationMixerPlayable>.Create(graph, inputCount);
        }

        public CrossSceneActivationTrack.PostPlaybackState postPlaybackState {
            get { return m_PostPlaybackState; }
            set { m_PostPlaybackState = value; }
        }
        
	    public UnityEngine.Object binder {
		    get { return m_binder; }
		    set {
		    	m_binder = value;
		    }
	    }
        
        public override void OnPlayableDestroy(Playable playable) {
	        //Debug.Log("Post " + m_BoundGameObject.name + ": " + m_BoundGameObjectInitialStateIsActive);
            if (m_BoundGameObject == null)
                return;

            switch (m_PostPlaybackState) {
                case CrossSceneActivationTrack.PostPlaybackState.Active:
                    m_BoundGameObject.SetActive(true);
                    break;
                case CrossSceneActivationTrack.PostPlaybackState.Inactive:
                    m_BoundGameObject.SetActive(false);
                    break;
                case CrossSceneActivationTrack.PostPlaybackState.Revert:
                    m_BoundGameObject.SetActive(m_BoundGameObjectInitialStateIsActive);
                    break;
                case CrossSceneActivationTrack.PostPlaybackState.LeaveAsIs:
                default:
                    break;
            }
        }

	    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
		    if (m_BoundGameObject == null) {
			    m_BoundGameObject = (CrossSceneBindingUtility.GetObject(m_binder) as Component).gameObject;
			    m_BoundGameObjectInitialStateIsActive = m_BoundGameObject != null && m_BoundGameObject.activeSelf;
            }

            if (m_BoundGameObject == null)
                return;

            int inputCount = playable.GetInputCount();
            bool hasInput = false;
            for (int i = 0; i < inputCount; i++) {
                if (playable.GetInputWeight(i) > 0) {
                    hasInput = true;
                    break;
                }
            }

            m_BoundGameObject.SetActive(hasInput);
        }
    }
}
#endif