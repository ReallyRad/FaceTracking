using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Calcatz.CookieCutter {

    internal class UnityEventReferenceFinder {

        public static List<EventReferenceInfo> FindAllUnityEventsReferences() {
#if UNITY_2020_OR_NEWER
            var components = MonoBehaviour.FindObjectsOfType<MonoBehaviour>(true);
#else
            var components = MonoBehaviour.FindObjectsOfType<MonoBehaviour>();
#endif
            var eventObjects = new List<EventObject>();

            foreach (var comp in components) {
                ExtractUnityEventObjects(eventObjects, comp, comp);
            }

            var infos = new List<EventReferenceInfo>();

            foreach (var e in eventObjects) {
                if (e.unityEvent == null) continue;
                int count = e.unityEvent.GetPersistentEventCount();
                var info = new EventReferenceInfo();
                info.Owner = e.component;
                info.fieldName = e.fieldName;

                for (int i = 0; i < count; i++) {
                    var obj = e.unityEvent.GetPersistentTarget(i);
                    var method = e.unityEvent.GetPersistentMethodName(i);

                    info.Listeners.Add(obj);
                    info.MethodNames.Add(obj.GetType().Name.ToString() + "." + method);
                }

                infos.Add(info);
            }

            return infos;
        }

        private static void ExtractUnityEventObjects(List<EventObject> _eventsObjects, MonoBehaviour _component, object _object, int _depth = 0) {
            if (_depth > 2) return;
            if (_object == null) return;
            var info = _object.GetType().GetTypeInfo();
            foreach (var field in info.DeclaredFields) {
                if (field.FieldType.IsSubclassOf(typeof(UnityEventBase))) {
                    _eventsObjects.Add(new EventObject() {
                        fieldName = field.Name,
                        component = _component,
                        obj = _object,
                        unityEvent = field.GetValue(_object) as UnityEventBase
                    }); ;
                }
                else if (field.FieldType.IsClass && field.FieldType.IsSerializable) {
                    ExtractUnityEventObjects(_eventsObjects, _component, field.GetValue(_object), _depth + 1);
                }
            }
        }


        public class EventReferenceInfo {
            public bool collapsed = false;
            public string fieldName;
            public MonoBehaviour Owner { get; set; }
            public List<UnityEngine.Object> Listeners { get; set; } = new List<UnityEngine.Object>();
            public List<string> MethodNames { get; set; } = new List<string>();
        }


        public class EventObject {
            public string fieldName;
            public MonoBehaviour component;
            public object obj;
            public UnityEventBase unityEvent;
        }

    }
}