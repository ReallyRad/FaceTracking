using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Calculates a value by operating 2 floats.
    /// </summary>
    public class FloatOperatorCommand : OperatorCommand<float> {

#if UNITY_EDITOR
        protected override void OnDrawAField() {
            CommandGUI.DrawFloatField("A", ref a);
        }

        protected override void OnDrawBField() {
            CommandGUI.DrawFloatField("B", ref b);
        }
#endif

    }
}