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

    public abstract class ScriptableObjectCommandData : ScriptableObjectWithGUID, ICommandData {

        public virtual CommandData commandData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Name { get { return name; } }
        public UnityEngine.Object targetObject { get { return this; } }


        public override void ValidateObject() {
            base.ValidateObject();
            if (commandData != null && ReferenceEquals(commandData.targetObject, null)) {
                commandData.SetTargetObject(this);
            }
        }

    }

}
