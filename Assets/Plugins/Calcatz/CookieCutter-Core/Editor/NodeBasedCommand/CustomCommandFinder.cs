using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {
    /// <summary>
    /// This can add some more customized methods to search commands in Command Finder window.
    /// </summary>
    public class CustomCommandFinder {
        
        /// <summary>
        /// Called automatically by Command Finder Window.
        /// </summary>
        /// <param name="_findButtonText"></param>
        /// <param name="_commandDataListToSearch"></param>
        /// <param name="_onClickFoundItem">If null, Command Inspector will be used by default to try open the command if able.</param>
        public virtual void FindInSpecifiedCommandDataList(out string _findButtonText, out CommandDataListToSearch _commandDataListToSearch, out System.Action<FoundCommandItem> _onClickFoundItem) {
            _findButtonText = "Find By Custom Finder";
            _commandDataListToSearch = new CommandDataListToSearch();
            _onClickFoundItem = null;
        }

    }
}
