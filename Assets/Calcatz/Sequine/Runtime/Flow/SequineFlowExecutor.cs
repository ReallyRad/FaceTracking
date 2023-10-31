using Calcatz.CookieCutter;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// The executor of a Sequine Flow Asset.
    /// </summary>
    public class SequineFlowExecutor : MonoBehaviourCommandExecutor {

        [SerializeField, HideInInspector] protected bool m_executeOnStart = true;
        [SerializeField, HideInInspector] protected SequineFlowAsset m_flowToExecute;
        [SerializeField, HideInInspector] protected bool m_dontDestroyOnLoad = false;

        public bool executeOnStart { get { return m_executeOnStart; } set { m_executeOnStart = value; } }
        /// <summary>
        /// Flow to execute if Execute on Start is enabled.
        /// </summary>
        public SequineFlowAsset flowToExecute { get { return m_flowToExecute; } set { m_flowToExecute = value; } }
        public bool dontDestroyOnLoad { get { return m_dontDestroyOnLoad; } set { m_dontDestroyOnLoad = value; } }


        private void Awake() {
            if (m_dontDestroyOnLoad) {
                DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void Start() {
            if (m_executeOnStart && m_flowToExecute != null) {
                Execute(m_flowToExecute);
            }
        }

        public void Execute(SequineFlowAsset _asset) {
            Execute(_asset, null);
        }

        public void Execute(SequineFlowAsset _asset, System.Action _onComplete) {
            executor.mainCommandData = _asset.commandData;
            executor.RunFlow(_asset.commandData.GetStartingCommand(), _onComplete);
        }

    }

}