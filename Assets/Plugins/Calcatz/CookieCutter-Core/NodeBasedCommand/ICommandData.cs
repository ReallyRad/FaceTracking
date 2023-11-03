
using System.Collections.Generic;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Interface for command data so it can be used into various kind of unity object.
    /// </summary>
    public interface ICommandData {

        string Name { get; }
        CommandData commandData { get; set; }

        /// <summary>
        /// The actual Unity Object that contains this command data.
        /// </summary>
        UnityEngine.Object targetObject { get; }

        void ValidateObject();

    }
}
