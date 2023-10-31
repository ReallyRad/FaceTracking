using System.Collections.Generic;
using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
#else
using Calcatz.OdinSerializer;
#endif

namespace Calcatz.CookieCutter {

    public abstract class MonoBehaviourCommandData : SerializedMonoBehaviour, ICommandData {

        public virtual void ValidateObject() {

        }

        public virtual CommandData commandData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Name { get { return name; } }
        public UnityEngine.Object targetObject { get { return this; } }

    }
}
