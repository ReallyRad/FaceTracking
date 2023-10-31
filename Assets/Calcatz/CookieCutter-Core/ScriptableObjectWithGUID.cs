using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
#else
using Calcatz.OdinSerializer;
#endif

namespace Calcatz.CookieCutter {
    public abstract class ScriptableObjectWithGUID : SerializedScriptableObject {

#if ODIN_INSPECTOR
        [ReadOnly]
#else
        [HideInInspector]
#endif
        public string guid;

        protected virtual void OnValidate() {
            ValidateObject();
        }

        public virtual void ValidateObject() {
            if (guid == null || guid == "" || guid == Guid.Empty.ToString()) {
                guid = Guid.NewGuid().ToString();
            }
        }

    }
}
 