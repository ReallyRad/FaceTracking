using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Metaface.Debug;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private SeamlessLoop[] _seamlessLoops;

    [SerializeField] private int _progress;
    [SerializeField] private int _currentLoopIndex;
    
    private void OnEnable()
    {
        FaceExpression.LevelUp += LevelUp;
    }

    private void OnDisable()
    {
        FaceExpression.LevelUp -= LevelUp;
    }
    
    private void LevelUp()
    {
        Debug.Log("level up");
        _progress++;
        Debug.Log("progress = " + _progress);
        if (_progress % 10 == 0)
        {
            _currentLoopIndex++;
            Debug.Log("currentLoopIndex " + _currentLoopIndex);
        }
    
        _seamlessLoops[_currentLoopIndex].SetVolume(_progress % 10f/ 10f);
        Debug.Log("set volume " + _progress % 10f/ 10f);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
