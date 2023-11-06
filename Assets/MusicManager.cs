using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Metaface.Debug;

public class MusicManager : MonoBehaviour
{
    private void OnEnable()
    {
        if (ModeControl.Discrete)
        {
            if (ModeControl.auraApproachingRP == "Progressive")
            {
                if (ModeControl.DiscreteFB_In_Ex)
                {
                    BreathingControl.New_In_Ex_FullBreath += DiscreteProgressiveWhole;
                }
                if (ModeControl.DiscreteFB_PeInPi_Ex)
                {
                    BreathingControl.New_PeInPi_Ex_FullBreath += DiscreteProgressiveWhole;
                }
            }
            if (ModeControl.auraApproachingRP == "Interactive")
            {
                if (ModeControl.DiscreteFB_In_Ex)
                {
                    BreathingControl.New_In_Ex_FullBreath += DiscreteInteractiveWhole;
                }
                if (ModeControl.DiscreteFB_PeInPi_Ex)
                {
                    BreathingControl.New_PeInPi_Ex_FullBreath += DiscreteInteractiveWhole;
                }
            }
        }
        if (ModeControl.Continuous)
        {
            if (ModeControl.auraApproachingRP == "Progressive")
            {
                if (ModeControl.ContinuousACT_Ex)
                {
                    BreathingControl.New_Ex_LongerThanMinimum += ContinuousProgressiveWhole;
                }
                if (ModeControl.ContinuousACT_In)
                {
                    BreathingControl.New_In_LongerThanMinimum += ContinuousProgressiveWhole;
                }
                if (ModeControl.ContinuousACT_PeInPi)
                {
                    BreathingControl.New_PeInPi_LongerThanMinimum = ContinuousProgressiveWhole;
                }
            }
            if (ModeControl.auraApproachingRP == "Interactive")
            {
                if (ModeControl.ContinuousACT_Ex)
                {
                    BreathingControl.New_Ex_LongerThanMinimum += ContinuousInteractiveWhole;
                }
                if (ModeControl.ContinuousACT_In)
                {
                    BreathingControl.New_In_LongerThanMinimum += ContinuousInteractiveWhole;
                }
                if (ModeControl.ContinuousACT_PeInPi)
                {
                    BreathingControl.New_PeInPi_LongerThanMinimum += ContinuousInteractiveWhole;
                }
            }
        }
    }
    private void OnDisable()
    {
        if (ModeControl.Discrete)
        {
            if (ModeControl.auraApproachingRP == "Progressive")
            {
                if (ModeControl.DiscreteFB_In_Ex)
                {
                    BreathingControl.New_In_Ex_FullBreath -= DiscreteProgressiveWhole;
                }
                if (ModeControl.DiscreteFB_PeInPi_Ex)
                {
                    BreathingControl.New_PeInPi_Ex_FullBreath -= DiscreteProgressiveWhole;
                }
            }
            if (ModeControl.auraApproachingRP == "Interactive")
            {
                if (ModeControl.DiscreteFB_In_Ex)
                {
                    BreathingControl.New_In_Ex_FullBreath -= DiscreteInteractiveWhole;
                }
                if (ModeControl.DiscreteFB_PeInPi_Ex)
                {
                    BreathingControl.New_PeInPi_Ex_FullBreath -= DiscreteInteractiveWhole;
                }
            }
        }
        if (ModeControl.Continuous)
        {
            if (ModeControl.auraApproachingRP == "Progressive")
            {
                if (ModeControl.ContinuousACT_Ex)
                {
                    BreathingControl.New_Ex_LongerThanMinimum -= ContinuousProgressiveWhole;
                }
                if (ModeControl.ContinuousACT_In)
                {
                    BreathingControl.New_In_LongerThanMinimum -= ContinuousProgressiveWhole;
                }
                if (ModeControl.ContinuousACT_PeInPi)
                {
                    BreathingControl.New_PeInPi_LongerThanMinimum -= ContinuousProgressiveWhole;
                }
            }
            if (ModeControl.auraApproachingRP == "Interactive")
            {
                if (ModeControl.ContinuousACT_Ex)
                {
                    BreathingControl.New_Ex_LongerThanMinimum -= ContinuousInteractiveWhole;
                }
                if (ModeControl.ContinuousACT_In)
                {
                    BreathingControl.New_In_LongerThanMinimum -= ContinuousInteractiveWhole;
                }
                if (ModeControl.ContinuousACT_PeInPi)
                {
                    BreathingControl.New_PeInPi_LongerThanMinimum -= ContinuousInteractiveWhole;
                }
            }
        }
    }

    private float maxVolume = 1f;
    private float currentVolumeOfCurrentSL = 0;
    private float currentVolumeOfCurrentSLIncrease = 0;
    [SerializeField] private SeamlessLoop[] _seamlessLoops;
    [SerializeField] private int _progress;
    [SerializeField] private int _currentLoopIndex;

    private void DiscreteProgressiveWhole()
    {
        Vector3 movement = AllVfxsControl.auraApproachingFinalPosition - AllVfxsControl.auraApproachingInitialPosition;
        float movementSteps = AllVfxsControl.auraApproachingEndBN - AllVfxsControl.auraApproachingStartBN;
        Vector3 movementSpeed = movement / movementSteps;

        if (gameObject.activeSelf)
        {
            StartCoroutine(DiscreteProgressiveIncrement(AllVfxsControl.progressIncrementTimeForDiscrete, movementSpeed));
        }
    }
    private IEnumerator DiscreteProgressiveIncrement(float increment, Vector3 speed)
    {
        float startTime = Time.time;
        float endTime = startTime + increment;

        while (Time.time < endTime)
        {
            //ps.transform.position += (speed / increment) * Time.deltaTime;
            yield return null;
        }
    }

    private void DiscreteInteractiveWhole()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(DiscreteInteractiveIncrement(AllVfxsControl.interactiveLoopTimeForDiscrete,
                                                        AllVfxsControl.auraApproachingInitialPosition,
                                                        AllVfxsControl.auraApproachingFinalPosition));
        }
    }
    private IEnumerator DiscreteInteractiveIncrement(float increment, Vector3 initialPosition, Vector3 finalPosition)
    {
        float startTime = Time.time;
        float middleTime = startTime + (increment / 2);
        float endTime = startTime + increment;

        while (Time.time < middleTime)
        {
            float progress = (middleTime - Time.time) / (increment / 2);
            Vector3 currentPosition = Vector3.Lerp(initialPosition, finalPosition, progress);
            //ps.transform.position = currentPosition;
            yield return null;
        }
        while (Time.time > middleTime && Time.time < endTime)
        {
            float progress = (endTime - Time.time) / (increment / 2);
            Vector3 currentPosition = Vector3.Lerp(finalPosition, initialPosition, progress);
            //ps.transform.position = currentPosition;
            yield return null;
        }
    }

    private void ContinuousProgressiveWhole(float fraction)
    {
        if (_currentLoopIndex < _seamlessLoops.Length)
        {
            currentVolumeOfCurrentSL = _seamlessLoops[_currentLoopIndex].GetVolume();
            if (currentVolumeOfCurrentSL < maxVolume)
            {
                currentVolumeOfCurrentSLIncrease = (maxVolume * fraction * Time.deltaTime) / 2;
                _seamlessLoops[_currentLoopIndex].SetVolume(currentVolumeOfCurrentSL + currentVolumeOfCurrentSLIncrease);
            }
            else if (_currentLoopIndex < (_seamlessLoops.Length - 1))
            {
                _currentLoopIndex++;
                currentVolumeOfCurrentSL = _seamlessLoops[_currentLoopIndex].GetVolume();
                currentVolumeOfCurrentSLIncrease = maxVolume * fraction * Time.deltaTime;
                _seamlessLoops[_currentLoopIndex].SetVolume(currentVolumeOfCurrentSL + currentVolumeOfCurrentSLIncrease);
            }
        }
    }

    private void ContinuousInteractiveWhole(float fraction)
    {
        Vector3 currentPosition = Vector3.Lerp(AllVfxsControl.auraApproachingInitialPosition,
                                               AllVfxsControl.auraApproachingFinalPosition,
                                               fraction);

        //ps.transform.position = currentPosition;
    }



    private void LevelUp()
    {
        //Debug.Log("level up");
        _progress++;
        //Debug.Log("progress = " + _progress);
        if (_progress % 10 == 0)
        {
            _currentLoopIndex++;
            //Debug.Log("currentLoopIndex " + _currentLoopIndex);
        }

        _seamlessLoops[_currentLoopIndex].SetVolume(_progress % 10f / 10f);
        //Debug.Log("set volume " + _progress % 10f/ 10f);
    }
}
