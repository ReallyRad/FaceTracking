using System.Linq;
using UnityEngine;

public class VFXSequence : MonoBehaviour //takes care of enabling and disabling the different sequenceable vfxs so they can respond to progress one after the other
{
    [SerializeField] private Sequenceable[] _sequenceableItems;
    
    private int sequenceIndex;

    private void Start()
    {
        _sequenceableItems = transform.GetComponentsInChildren<Sequenceable>(); //get sequenceable items
        _sequenceableItems.First().Initialize(); //initialize it. the first one will now start receiving progress values
        _sequenceableItems.First().Completed += SequenceItemCompleted; //it will report back once it's completed 
    }

    private void SequenceItemCompleted(Sequenceable item) //called when current sequence item completed
    {
        item.Completed -= SequenceItemCompleted; //disable handler for current element
        sequenceIndex++;
        _sequenceableItems[sequenceIndex].Initialize();
        _sequenceableItems[sequenceIndex].Completed += SequenceItemCompleted; //assign handler to new sequence item
    }
}
