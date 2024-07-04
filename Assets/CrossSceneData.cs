using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSceneData : MonoBehaviour
{
    public static ExperimentState experimentState;

    private void OnEnable()
    {
        WaitingRoomTransition.NotifyPrePostState += UpdatePrePostState;
    }

    private void OnDisable()
    {
        WaitingRoomTransition.NotifyPrePostState -= UpdatePrePostState;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    private void UpdatePrePostState(ExperimentState state)
    {
        experimentState = state;
    }
}
