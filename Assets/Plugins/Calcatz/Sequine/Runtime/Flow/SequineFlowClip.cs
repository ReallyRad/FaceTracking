#if TIMELINE_AVAILABLE
using Calcatz.CookieCutter;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
#else
using Calcatz.OdinSerializer;
#endif

namespace Calcatz.Sequine {

    /// <summary>
    /// Sequine Flow Clip is a timeline clip that carries Sequine Flow data.
    /// </summary>
    public class SequineFlowClip : TimelineCommandDataClip {

        public override void ValidateObject() {
            base.ValidateObject();
            if (commandData == null) {
                commandData = new SequineFlowCommandData(this);
            }
        }

        [OdinSerialize][ShowCommandNodes(450, true)]
#if ODIN_INSPECTOR
        [HideReferenceObjectPicker]
        private SequineFlowCommandData m_commandData;
#else
        public SequineFlowCommandData m_commandData = new SequineFlowCommandData(null);
#endif
        public override CommandData commandData { get => m_commandData; set => m_commandData = (SequineFlowCommandData)value; }

    }
}
#endif