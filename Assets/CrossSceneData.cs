using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSceneData : MonoBehaviour
{
    public static ExperimentState experimentState;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

}
