#if TIMELINE_AVAILABLE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Calcatz.CookieCutter {
    [TrackColor(0.5f, 0.05f, 0.85f)]
    //[TrackBindingType(typeof(MonoBehaviourCommandExecutor))]  no need for external executor
    [TrackClipType(typeof(TimelineCommandDataClip))]
    public class TimelineCommandTrack : TrackAsset {

        private PlayableDirector playableDirector;
        public PlayableDirector GetPlayableDirector() {
            return playableDirector;
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver) {
            playableDirector = director;
            base.GatherProperties(director, driver);
        }

        protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip) {
            if (playableDirector != null) {
                Object binding = playableDirector.GetGenericBinding(this);
                if (binding != null) {
                    //ICommandExecutor commandExecutor = (ICommandExecutor)binding;
                    //initialization here if needed later
                }
            }
            return base.CreatePlayable(graph, gameObject, clip);
        }

        protected override void OnCreateClip(TimelineClip clip) {
            base.OnCreateClip(clip);
#if UNITY_EDITOR
            TimelineCommandDataClip commandDataClip = (TimelineCommandDataClip)clip.asset;
            commandDataClip.ValidateObject();
#endif
        }

    }
}
#endif