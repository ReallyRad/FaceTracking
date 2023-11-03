using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// A data container for action clips.
    /// Also used as a binder asset of Sequine Player component.
    /// </summary>
    [CreateAssetMenu(fileName = "AnimationData_New", menuName = "Sequine/Animation Data", order = 1)]
    public class SequineAnimationData : ScriptableObject {

        public ActionClip[] actionClips;

        [System.Serializable]
        public class ActionClip {
            [Min(1)]
            public int id = 1;
            public AnimationClip clip;
        }


#if UNITY_EDITOR
        private void OnValidate() {
            if (actionClips.Length == 1) {
                if (actionClips[0].id <= 0) actionClips[0].id = 1;
            }
            else {
                bool retry;
                do {
                    retry = false;
                    for (int i = 0; i < actionClips.Length; i++) {
                        for (int j = 0; j < actionClips.Length; j++) {
                            if (i == j) continue;
                            if (actionClips[i].id == actionClips[j].id) {
                                actionClips[j].id++;
                                retry = true;
                            }
                        }
                    }
                } while (retry);
            }
        }
#endif

    }

    /// <summary>
    /// Configuration on an animation would be played.
    /// </summary>
    public struct AnimationConfig {
        public float speed;
        public float transitionDuration;
        public bool normalizedTransition;
        public float lengthToPlay;

        public AnimationConfig(float _speed = 1f, float _transitionDuration = 0.25f, bool _normalizedTransition = false, float _lengthToPlay = 1.0f) {
            speed = _speed;
            transitionDuration = _transitionDuration;
            normalizedTransition = _normalizedTransition;
            lengthToPlay = _lengthToPlay;
        }

    }

}
