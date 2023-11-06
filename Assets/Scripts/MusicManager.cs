using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private float _maxVolume = 1f;
    
    [SerializeField] private SeamlessLoop[] _seamlessLoops;
    [SerializeField] private int _progress;
    [SerializeField] private int _currentLoopIndex;

    private void OnEnable()
    {
        ProgressManager.Progress += Progress;
    }

    private void OnDisable()
    {
        ProgressManager.Progress -= Progress;
    }

    private void Progress(float progress)
    {
        
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
