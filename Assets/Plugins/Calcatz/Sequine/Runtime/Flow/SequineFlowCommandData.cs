using Calcatz.CookieCutter;
using System;

namespace Calcatz.Sequine {

    /// <summary>
    /// Command Data type for Sequine's scope.
    /// </summary>
    [Serializable]
    public class SequineFlowCommandData : CommandData {

        public SequineFlowCommandData(UnityEngine.Object _targetObject) : base(_targetObject) { }

    }
}