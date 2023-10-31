using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    public interface IVariableUser {

        Dictionary<int, CommandData.Variable> variables { get; set; }
        UnityEngine.Object targetObject { get; }

    }

}
