using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Face Data")]
public class FaceData : ScriptableObject
{
    public bool smiling;
    public bool slightSmile;
    public bool slightPucker;
    public bool pucker;

    [HideInInspector] public bool previouslySmiling;
    [HideInInspector] public bool previouslySlightSmile;
    [HideInInspector] public bool previouslySlightPucker;
    [HideInInspector] public bool previouslyPucker;

    public void SetData(bool newSmiling, bool newSlightSmile, bool newSlightPucker, bool newPucker)
    {
        previouslySmiling = smiling;
        previouslySlightSmile = slightSmile;
        previouslySlightPucker = slightPucker;
        previouslyPucker = pucker;     
        
        smiling = newSmiling;
        slightSmile = newSlightSmile;
        slightPucker = newSlightPucker;
        pucker = newPucker;
    }
}
