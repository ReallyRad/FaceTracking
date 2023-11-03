using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    public class MonoBehaviourCommandExecutor : MonoBehaviour, ICommandExecutor {

        private CommandExecutor m_executor;
        public CommandExecutor executor {
            get {
                if (m_executor == null) m_executor = new CommandExecutor(this);
                return m_executor;
            }
        }

    }

}
