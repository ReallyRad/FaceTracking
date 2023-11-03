using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Prints a text to the Console using Debug.Log.
    /// </summary>
    public class DebugLogCommand : TextFormatterCommand {

        protected override void HandleFormattedText(CommandExecutionFlow _flow, string _formattedText) {
            Debug.Log(_formattedText, _flow.executor.targetObject is Object? _flow.executor.targetObject as Object : null);
        }

    }
}
