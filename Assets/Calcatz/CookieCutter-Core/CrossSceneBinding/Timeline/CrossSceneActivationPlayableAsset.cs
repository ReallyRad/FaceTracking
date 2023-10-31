#if TIMELINE_AVAILABLE
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine;
#if UNITY_EDITOR
using System.ComponentModel;
#endif

namespace Calcatz.CookieCutter {
    /// <summary>
    /// Playable Asset class for Global Directional Light Activation Tracks
    /// </summary>
#if UNITY_EDITOR
    [DisplayName("Cross Scene Activation Clip")]
#endif
    class CrossSceneActivationPlayableAsset : PlayableAsset, ITimelineClipAsset {
        /// <summary>
        /// Returns a description of the features supported by activation clips
        /// </summary>
        public ClipCaps clipCaps { get { return ClipCaps.None; } }

        /// <summary>
        /// Overrides PlayableAsset.CreatePlayable() to inject needed Playables for an activation asset
        /// </summary>
	    public override Playable CreatePlayable(PlayableGraph graph, GameObject go) {
            return Playable.Create(graph);
        }
    }
}
#endif