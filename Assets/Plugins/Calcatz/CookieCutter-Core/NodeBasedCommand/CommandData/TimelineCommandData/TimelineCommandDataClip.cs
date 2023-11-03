#if TIMELINE_AVAILABLE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
#else
using Calcatz.OdinSerializer;
#endif

namespace Calcatz.CookieCutter {
    [Serializable]
#if ODIN_INSPECTOR
    [ShowOdinSerializedPropertiesInInspector]
#endif
    public abstract class TimelineCommandDataClip : PlayableAsset, ITimelineClipAsset, ICommandData, ISerializationCallbackReceiver, ISupportsPrefabSerialization {

        public virtual void ValidateObject() {
            if (commandData != null && commandData.targetObject == null) {
                commandData.SetTargetObject(this);
            }
        }

#region SERIALIZATION
        [SerializeField, HideInInspector]
        private SerializationData serializationData;

        SerializationData ISupportsPrefabSerialization.SerializationData { get { return this.serializationData; } set { this.serializationData = value; } }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);
        }
#endregion

        public virtual CommandData commandData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ClipCaps clipCaps {
            get {
                return ClipCaps.None;
            }
        }

        public string Name => name;

        public UnityEngine.Object targetObject => this;

        [HideInInspector][SerializeField] private TimelineCommandExecutorBehaviour commandExecutor = new TimelineCommandExecutorBehaviour();

        private void OnValidate() {
            ValidateObject();
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
            ValidateObject();
            //name = owner.name;
            commandExecutor.SetCommandData(commandData);
            return ScriptPlayable<TimelineCommandExecutorBehaviour>.Create(graph, commandExecutor);
        }

    }
}
#endif