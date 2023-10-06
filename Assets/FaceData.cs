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

    public void SetData(bool smiling, bool slightSmile, bool slightPucker, bool pucker)
    {
        previouslySmiling = this.smiling;
        previouslySlightSmile = this.slightSmile;
        previouslySlightPucker = this.slightPucker;
        previouslyPucker = this.pucker;     
        
        this.smiling = smiling;
        this.slightSmile = slightSmile;
        this.slightPucker = slightPucker;
        this.pucker = pucker;
    }
}
