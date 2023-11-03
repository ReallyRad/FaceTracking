#if TIMELINE_AVAILABLE
using UnityEngine.Playables;
using UnityEngine.Timeline;
#endif

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Modifies the wrap mode of a PlayableDirector.
    /// </summary>
    public class TimelineExtrapolationCommand : BuildableCommand {

        public override float nodeWidth => 275;

#if TIMELINE_AVAILABLE

        public TimelineExtrapolationCommand() {
            AddPropertyInput<TimelineAsset>("Timeline Asset");
            AddPropertyInput<DirectorWrapMode>("Wrap Mode");
        }

        public override void Execute(CommandExecutionFlow _flow) {
            var asset = GetInput<TimelineAsset>(_flow, 0);
            if (asset != null) {
                var playableDirector = CrossSceneBindingUtility.GetObject<PlayableDirector>(asset);
                if (playableDirector != null) {
                    playableDirector.extrapolationMode = GetInput<DirectorWrapMode>(_flow, 1);
                }
            }
            Exit();
        }
#endif

    }
}
