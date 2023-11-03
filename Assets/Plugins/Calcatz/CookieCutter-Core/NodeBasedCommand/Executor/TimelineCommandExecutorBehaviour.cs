using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
#else
using Calcatz.OdinSerializer;
#endif

namespace Calcatz.CookieCutter {
    [Serializable]
    public class TimelineCommandExecutorBehaviour : PlayableBehaviour, ICommandExecutor {

        private CommandExecutor m_executor;
        public CommandExecutor executor {
            get {
                if (m_executor == null) m_executor = new CommandExecutor(this);
                return m_executor;
            }
        }

        private bool executed = false;
        private bool forceStopped = false;

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [HideInInspector]
        public bool firstFrameHappened = false;

        private CommandData rootCommandData;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            base.ProcessFrame(playable, info, playerData);

            if (forceStopped && playable.GetTime() <= info.deltaTime) {
                forceStopped = false;
                firstFrameHappened = false;
                executed = false;
            }

            if (executed) {
                return;
            }

            if (!firstFrameHappened) {
                firstFrameHappened = true;
            } else {
                return;
            }
            
            if (Application.isPlaying) {
                executor.RunFlow(rootCommandData, rootCommandData.GetStartingCommand());
                executed = true;
            }

        }

        public override void OnBehaviourPause(Playable playable, FrameData info) {
            if (playable.GetTime() >= (playable.GetDuration() - info.deltaTime) || playable.GetTime() <= 0) {
                firstFrameHappened = false;
                executed = false;
            }
            else if (Application.isPlaying) {
                forceStopped = true;
            }
            base.OnBehaviourPause(playable, info);
        }

        public void SetCommandData(CommandData _commandData) {
            rootCommandData = _commandData;
        }

    }
}