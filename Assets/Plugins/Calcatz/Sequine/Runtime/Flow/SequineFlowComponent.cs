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
    /// Sequine Flow Component is a component that carries Sequine Flow data.
    /// </summary>
    public class SequineFlowComponent : MonoBehaviourCommandData {

        [SerializeField, HideInInspector] protected bool m_executeOnStart = true;

        [SerializeField, HideInInspector] protected bool m_dontDestroyOnLoad = false;

        private CommandExecutor m_executor;
        public CommandExecutor executor {
            get {
                if (m_executor == null) m_executor = new CommandExecutor(this);
                return m_executor;
            }
        }

        public bool executeOnStart { get { return m_executeOnStart; } set { m_executeOnStart = value; } }
        public bool dontDestroyOnLoad { get { return m_dontDestroyOnLoad; } set { m_dontDestroyOnLoad = value; } }


        private void Awake() {
            if (m_dontDestroyOnLoad) {
                DontDestroyOnLoad(gameObject);
            }
        }

        public override void ValidateObject() {
            base.ValidateObject();
            if (commandData == null) {
                commandData = new SequineFlowCommandData(this);
            }
        }

        private void Start() {
            if (m_executeOnStart) {
                Execute();
            }
        }

        [OdinSerialize, HideInInspector]
        private SequineFlowCommandData m_commandData;
        public override CommandData commandData { get => m_commandData; set => m_commandData = (SequineFlowCommandData)value; }

        public void Execute() {
            Execute(null);
        }

        public void Execute(System.Action _onComplete) {
            executor.mainCommandData = commandData;
            executor.RunFlow(commandData.GetStartingCommand(), _onComplete);
        }

    }
}