using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    public enum UpdateMode {
        NormalUpdate, LateUpdate, FixedUpdate, UnscaledTime, Manual
    }

    public static class UpdateModeUtility {

        public static Dictionary<UpdateMode, Func<float>> GetDeltaTime = new Dictionary<UpdateMode, Func<float>>() {
            {UpdateMode.NormalUpdate, ()=>Time.deltaTime },
            {UpdateMode.LateUpdate, ()=>Time.deltaTime },
            {UpdateMode.FixedUpdate, ()=>Time.fixedDeltaTime },
            {UpdateMode.UnscaledTime, ()=>Time.unscaledDeltaTime },
            {UpdateMode.Manual, ()=> 0f }
        };

        public static Dictionary<UpdateMode, Func<YieldInstruction>> GetYieldReturn = new Dictionary<UpdateMode, Func<YieldInstruction>>() {
            {UpdateMode.NormalUpdate, ()=> null },
            {UpdateMode.LateUpdate, ()=> new WaitForEndOfFrame() },
            {UpdateMode.FixedUpdate, ()=> new WaitForFixedUpdate() },
            {UpdateMode.UnscaledTime, ()=> null },
            {UpdateMode.Manual, ()=> null }
        };
    }

}