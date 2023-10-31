using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Compares 2 booleans with OR logical operator.
    /// </summary>
    public class OrCommand : BoolComparatorCommand {

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            return (T)(object)(GetInput(_flow, 0, a) || GetInput(_flow, 1, b));
        }

    }
}