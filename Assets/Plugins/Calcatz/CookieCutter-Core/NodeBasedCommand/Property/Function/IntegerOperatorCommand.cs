using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Calculates a value by operating 2 integers.
    /// </summary>
    public class IntegerOperatorCommand : OperatorCommand<int> {

#if UNITY_EDITOR
        protected override void OnDrawAField() {
            CommandGUI.DrawIntField("A", ref a);
        }

        protected override void OnDrawBField() {
            CommandGUI.DrawIntField("B", ref b);
        }
#endif

    }
}