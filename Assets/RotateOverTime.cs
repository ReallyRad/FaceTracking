using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverTime : MonoBehaviour
{

    public float rate; 
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Rotation = new Vector3(0, rate * Time.timeSinceLevelLoad, 0);
        transform.Rotate(Rotation);    
    }
}
