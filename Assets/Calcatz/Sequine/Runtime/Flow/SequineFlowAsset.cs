using UnityEngine;
using Calcatz.CookieCutter;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
#else
using Calcatz.OdinSerializer;
#endif

namespace Calcatz.Sequine {

    /// <summary>
    /// Sequine Flow Asset is an asset (a file in your Project window) that carries Sequine Flow data.
    /// </summary>
    [CreateAssetMenu(fileName = "SequineFlow_New", menuName = "Sequine/Sequine Flow Asset", order = 1)]
    public class SequineFlowAsset : ScriptableObjectCommandData {

        public override void ValidateObject() {
            base.ValidateObject();
            if (commandData == null) {
                commandData = new SequineFlowCommandData(this);
            }
        }

        [OdinSerialize, HideInInspector]
        private SequineFlowCommandData m_commandData;
        public override CommandData commandData { get => m_commandData; set => m_commandData = (SequineFlowCommandData)value; }

    }
}