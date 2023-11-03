using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Calcatz.Sequine {

    /// <summary>
    /// Text Behaviour Profile is an asset that contains the concrete profile of the stacked text animation behaviour.
    /// </summary>
    [CreateAssetMenu(fileName = "TextBehaviourProfile_New", menuName = "Sequine/Text Behaviour Profile", order = 1)]
    public sealed class TextBehaviourProfile : ScriptableObject {

        public float durationPerCharacter = 0.1f;
        public float delayPerCharacter = 0.05f;

        /// <summary>
        /// A list of every setting that this Sequine Text Behaviour Profile stores.
        /// </summary>
        public List<TextBehaviourComponent> components = new List<TextBehaviourComponent>();

        // Editor only, doesn't have any use outside of it
        [NonSerialized]
        public bool isDirty = true;

        void OnEnable() {
            components.RemoveAll(x => x == null);
        }

        public void Reset() {
            isDirty = true;
        }

        /// <summary>
        /// Adds a <see cref="TextBehaviourComponent"/> to this Sequine Text Behaviour Profile.
        /// </summary>
        /// <remarks>
        /// You can only have a single component of the same type per Behaviour Profile.
        /// </remarks>
        /// <typeparam name="T">A type of <see cref="TextBehaviourComponent"/>.</typeparam>
        /// <returns>The instance for the given type that you added to the Behaviour Profile</returns>
        /// <seealso cref="Add"/>
        public T Add<T>()
            where T : TextBehaviourComponent {
            return (T)Add(typeof(T));
        }

        /// <summary>
        /// Adds a <see cref="TextBehaviourComponent"/> to this Behaviour Profile.
        /// </summary>
        /// <remarks>
        /// You can only have a single component of the same type per Behaviour Profile.
        /// </remarks>
        /// <param name="type">A type that inherits from <see cref="TextBehaviourComponent"/>.</param>
        /// <param name="overrides">Specifies whether Unity should automatically override all the settings when
        /// you add a <see cref="TextBehaviourComponent"/> to the Behaviour Profile.</param>
        /// <returns>The instance created for the given type that has been added to the profile</returns>
        /// <see cref="Add{T}"/>
        public TextBehaviourComponent Add(Type type) {
            if (Has(type))
                throw new InvalidOperationException("Component already exists in the volume");

            var component = (TextBehaviourComponent)CreateInstance(type);
            components.Add(component);
            isDirty = true;
            return component;
        }

        /// <summary>
        /// Removes a <see cref="TextBehaviourComponent"/> from this Behaviour Profile.
        /// </summary>
        /// <remarks>
        /// This method does nothing if the type does not exist in the Behaviour Profile.
        /// </remarks>
        /// <typeparam name="T">A type of <see cref="TextBehaviourComponent"/>.</typeparam>
        /// <seealso cref="Remove"/>
        public void Remove<T>()
            where T : TextBehaviourComponent {
            Remove(typeof(T));
        }

        /// <summary>
        /// Removes a <see cref="TextBehaviourComponent"/> from this Behaviour Profile.
        /// </summary>
        /// <remarks>
        /// This method does nothing if the type does not exist in the Behaviour Profile.
        /// </remarks>
        /// <param name="type">A type that inherits from <see cref="TextBehaviourComponent"/>.</param>
        /// <seealso cref="Remove{T}"/>
        public void Remove(Type type) {
            int toRemove = -1;

            for (int i = 0; i < components.Count; i++) {
                if (components[i].GetType() == type) {
                    toRemove = i;
                    break;
                }
            }

            if (toRemove >= 0) {
                components.RemoveAt(toRemove);
                isDirty = true;
            }
        }

        /// <summary>
        /// Checks if this Behaviour Profile contains the <see cref="TextBehaviourComponent"/> you pass in.
        /// </summary>
        /// <typeparam name="T">A type of <see cref="TextBehaviourComponent"/>.</typeparam>
        /// <returns><c>true</c> if the <see cref="TextBehaviourComponent"/> exists in the Behaviour Profile,
        /// <c>false</c> otherwise.</returns>
        /// <seealso cref="Has"/>
        /// <seealso cref="HasSubclassOf"/>
        public bool Has<T>()
            where T : TextBehaviourComponent {
            return Has(typeof(T));
        }

        /// <summary>
        /// Checks if this Behaviour Profile contains the <see cref="TextBehaviourComponent"/> you pass in.
        /// </summary>
        /// <param name="type">A type that inherits from <see cref="TextBehaviourComponent"/>.</param>
        /// <returns><c>true</c> if the <see cref="TextBehaviourComponent"/> exists in the Behaviour Profile,
        /// <c>false</c> otherwise.</returns>
        /// <seealso cref="Has{T}"/>
        /// <seealso cref="HasSubclassOf"/>
        public bool Has(Type type) {
            foreach (var component in components) {
                if (component.GetType() == type)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if this Behaviour Profile contains the <see cref="TextBehaviourComponent"/>, which is a subclass of <paramref name="type"/>,
        /// that you pass in.
        /// </summary>
        /// <param name="type">A type that inherits from <see cref="TextBehaviourComponent"/>.</param>
        /// <returns><c>true</c> if the <see cref="TextBehaviourComponent"/> exists in the Behaviour Profile,
        /// <c>false</c> otherwise.</returns>
        /// <seealso cref="Has"/>
        /// <seealso cref="Has{T}"/>
        public bool HasSubclassOf(Type type) {
            foreach (var component in components) {
                if (component.GetType().IsSubclassOf(type))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the <see cref="TextBehaviourComponent"/> of the specified type, if it exists.
        /// </summary>
        /// <typeparam name="T">A type of <see cref="TextBehaviourComponent"/>.</typeparam>
        /// <param name="component">The output argument that contains the <see cref="TextBehaviourComponent"/>
        /// or <c>null</c>.</param>
        /// <returns><c>true</c> if the <see cref="TextBehaviourComponent"/> is in the Behaviour Profile,
        /// <c>false</c> otherwise.</returns>
        /// <seealso cref="TryGet{T}(Type, out T)"/>
        /// <seealso cref="TryGetSubclassOf{T}"/>
        /// <seealso cref="TryGetAllSubclassOf{T}"/>
        public bool TryGet<T>(out T component)
            where T : TextBehaviourComponent {
            return TryGet(typeof(T), out component);
        }

        /// <summary>
        /// Gets the <see cref="TextBehaviourComponent"/> of the specified type, if it exists.
        /// </summary>
        /// <typeparam name="T">A type of <see cref="TextBehaviourComponent"/></typeparam>
        /// <param name="type">A type that inherits from <see cref="TextBehaviourComponent"/>.</param>
        /// <param name="component">The output argument that contains the <see cref="TextBehaviourComponent"/>
        /// or <c>null</c>.</param>
        /// <returns><c>true</c> if the <see cref="TextBehaviourComponent"/> is in the Behaviour Profile,
        /// <c>false</c> otherwise.</returns>
        /// <seealso cref="TryGet{T}(out T)"/>
        /// <seealso cref="TryGetSubclassOf{T}"/>
        /// <seealso cref="TryGetAllSubclassOf{T}"/>
        public bool TryGet<T>(Type type, out T component)
            where T : TextBehaviourComponent {
            component = null;

            foreach (var comp in components) {
                if (comp.GetType() == type) {
                    component = (T)comp;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the <seealso cref="TextBehaviourComponent"/>, which is a subclass of <paramref name="type"/>, if
        /// it exists.
        /// </summary>
        /// <typeparam name="T">A type of <see cref="TextBehaviourComponent"/>.</typeparam>
        /// <param name="type">A type that inherits from <see cref="TextBehaviourComponent"/>.</param>
        /// <param name="component">The output argument that contains the <see cref="TextBehaviourComponent"/>
        /// or <c>null</c>.</param>
        /// <returns><c>true</c> if the <see cref="TextBehaviourComponent"/> is in the Behaviour Profile,
        /// <c>false</c> otherwise.</returns>
        /// <seealso cref="TryGet{T}(Type, out T)"/>
        /// <seealso cref="TryGet{T}(out T)"/>
        /// <seealso cref="TryGetAllSubclassOf{T}"/>
        public bool TryGetSubclassOf<T>(Type type, out T component)
            where T : TextBehaviourComponent {
            component = null;

            foreach (var comp in components) {
                if (comp.GetType().IsSubclassOf(type)) {
                    component = (T)comp;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets all the <seealso cref="TextBehaviourComponent"/> that are subclasses of the specified type,
        /// if there are any.
        /// </summary>
        /// <typeparam name="T">A type of <see cref="TextBehaviourComponent"/>.</typeparam>
        /// <param name="type">A type that inherits from <see cref="TextBehaviourComponent"/>.</param>
        /// <param name="result">The output list that contains all the <seealso cref="TextBehaviourComponent"/>
        /// if any. Note that Unity does not clear this list.</param>
        /// <returns><c>true</c> if any <see cref="TextBehaviourComponent"/> have been found in the profile,
        /// <c>false</c> otherwise.</returns>
        /// <seealso cref="TryGet{T}(Type, out T)"/>
        /// <seealso cref="TryGet{T}(out T)"/>
        /// <seealso cref="TryGetSubclassOf{T}"/>
        public bool TryGetAllSubclassOf<T>(Type type, List<T> result)
            where T : TextBehaviourComponent {
            Assert.IsNotNull(components);
            int count = result.Count;

            foreach (var comp in components) {
                if (comp.GetType().IsSubclassOf(type))
                    result.Add((T)comp);
            }

            return count != result.Count;
        }
    }
}