using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Calcatz.Sequine {

    /// <summary>
    /// Plays an animation by specifying the animation clip asset, and only play within the percentaged range.
    /// 
    /// Note that in script, you can't get nor modify the SequineState of a ranged clip.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Animation/Clip Control/Play Animation Clip At Range", typeof(SequineFlowCommandData))]
    public class PlayAnimationClipAtRangeCommand : PlayAnimationClipCommand {

        public float minTimePercentage = 0.25f;
        public float maxTimePercentage = 0.75f;

        public bool useRandomVariation = false;
        public float minRandomTimePercentage = 0.25f;
        public float maxRandomTimePercentage = 0.75f;

        protected override void OnPlayClip(CommandExecutionFlow _flow, SequinePlayer _sequine, AnimationClip _clip) {
            float min, max;
            if (useRandomVariation) {
                min = Random.Range(minTimePercentage, minRandomTimePercentage);
                max = Random.Range(maxRandomTimePercentage, maxTimePercentage);
            }
            else {
                min = minTimePercentage;
                max = maxTimePercentage;
            }
            calculatedDuration = (max-min) * _clip.length * lengthToPlay;
            _sequine.PlayRangedAnimationClip(layer, _clip, GetAnimationConfig(), min, max, restart, () => ExecuteOnComplete(_flow));
        }

#if UNITY_EDITOR
        public override void Editor_OnDrawTitle(out string _tooltip) {
            base.Editor_OnDrawTitle(out _tooltip);
            _tooltip = @"Plays an animation by specifying the animation clip asset, and only play within the percentaged range.

Note that in script, you can't get nor modify the SequineState of a ranged clip.";
        }

        protected override void Editor_OnDrawAnimationContent(Vector2 _absPosition) {
            base.Editor_OnDrawAnimationContent(_absPosition);
            var mainSliderRect = CommandGUI.GetRect();

            var boxRect = new Rect(mainSliderRect.x-2, mainSliderRect.y, mainSliderRect.width+4, mainSliderRect.height * 3 + 8);
            EditorGUI.HelpBox(boxRect, "", MessageType.None);

            float minTimeRef = minTimePercentage;
            float maxTimeRef = maxTimePercentage;

            EditorGUI.BeginChangeCheck();
            EditorGUI.MinMaxSlider(mainSliderRect, "Time Range (%)", ref minTimeRef, ref maxTimeRef, 0f, 1f);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(CommandGUI.currentTargetObject, "modify time range");
                minTimePercentage = minTimeRef;
                maxTimePercentage = maxTimeRef;
                if (minRandomTimePercentage < minTimePercentage) minRandomTimePercentage = minTimePercentage;
                if (maxRandomTimePercentage > maxTimePercentage) maxRandomTimePercentage = maxTimePercentage;
                if (minRandomTimePercentage > maxRandomTimePercentage) maxRandomTimePercentage = minRandomTimePercentage;
                if (maxRandomTimePercentage < minRandomTimePercentage) minRandomTimePercentage = maxRandomTimePercentage;
            }
            CommandGUI.AddRectHeight(mainSliderRect.height);

            CommandGUI.GetSingleLineLabeledRect(out var labelRect, out var sliderDetailsRect);
            sliderDetailsRect.height += EditorGUIUtility.singleLineHeight;
            GUILayout.BeginArea(sliderDetailsRect);
            {
                GUILayout.BeginHorizontal();
                {
                    var labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 30;
                    minTimeRef = EditorGUILayout.FloatField("Min", minTimePercentage);
                    if (minTimeRef != minTimePercentage) {
                        Undo.RecordObject(CommandGUI.currentTargetObject, "modify min time range");
                        if (minTimeRef > maxTimePercentage) minTimeRef = maxTimePercentage;
                        else if (minTimeRef < 0) minTimeRef = 0;
                        minTimePercentage = minTimeRef;
                        if (minRandomTimePercentage < minTimePercentage) minRandomTimePercentage = minTimePercentage;
                        if (minRandomTimePercentage > maxRandomTimePercentage) maxRandomTimePercentage = minRandomTimePercentage;
                    }
                    GUILayout.FlexibleSpace();
                    maxTimeRef = EditorGUILayout.FloatField("Max", maxTimePercentage);
                    if (maxTimeRef != maxTimePercentage) {
                        Undo.RecordObject(CommandGUI.currentTargetObject, "modify max time range");
                        if (maxTimeRef < minTimePercentage) maxTimeRef = minTimePercentage;
                        else if (maxTimeRef > 1) maxTimeRef = 1;
                        maxTimePercentage = maxTimeRef;
                        if (maxRandomTimePercentage > maxTimePercentage) maxRandomTimePercentage = maxTimePercentage;
                        if (maxRandomTimePercentage < minRandomTimePercentage) minRandomTimePercentage = maxRandomTimePercentage;
                    }
                    EditorGUIUtility.labelWidth = labelWidth;
                }
                GUILayout.EndHorizontal();

                if (inputIds[IN_PORT_CLIP].targetId == 0) {
                    if (clip != null) {
                        GUILayout.Label((clip.length*minTimePercentage) + " s  ~  " + (clip.length * maxTimePercentage) + " s", EditorStyles.centeredGreyMiniLabel);
                    }
                    else {
                        GUILayout.Label("Set a clip to preview range in seconds.", EditorStyles.centeredGreyMiniLabel);
                    }
                }
                else {
                    GUILayout.Label("Clip is dynamically set using input.", EditorStyles.centeredGreyMiniLabel);
                }
            }
            GUILayout.EndArea();

            CommandGUI.AddRectHeight(sliderDetailsRect.height);

            CommandGUI.DrawToggleLeftField("Use Random Variation", ref useRandomVariation);
            if (useRandomVariation) {
                var randomSliderRect = CommandGUI.GetRect();
                minTimeRef = minRandomTimePercentage;
                maxTimeRef = maxRandomTimePercentage;

                EditorGUI.BeginChangeCheck();
                EditorGUI.MinMaxSlider(randomSliderRect, new GUIContent("Smallest Range", "Min time will be randomized between the original Min value and the smallest range's Min value.\nMax time will be randomized between the smallest range's Max value and the original Max value."), ref minTimeRef, ref maxTimeRef, 0f, 1f);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(CommandGUI.currentTargetObject, "modify random time range");
                    minRandomTimePercentage = minTimeRef;
                    maxRandomTimePercentage = maxTimeRef;
                    if (minRandomTimePercentage < minTimePercentage) minRandomTimePercentage = minTimePercentage;
                    if (maxRandomTimePercentage < minRandomTimePercentage) maxRandomTimePercentage = minRandomTimePercentage;
                    if (maxRandomTimePercentage > maxTimePercentage) maxRandomTimePercentage = maxTimePercentage;
                    if (minRandomTimePercentage > maxRandomTimePercentage) minRandomTimePercentage = maxRandomTimePercentage;
                }
                CommandGUI.AddRectHeight(randomSliderRect.height);
            }

            CommandGUI.AddRectHeight(4);
        }
#endif

    }

}