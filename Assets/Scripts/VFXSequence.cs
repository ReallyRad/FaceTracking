using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class
    VFXSequence : MonoBehaviour //takes care of enabling and disabling the different sequenceable vfxs so they can respond to progress one after the other
{
    [SerializeField] private Sequenceable[] _sequenceableItems;
    [SerializeField] private AudioMixer _mixer;

    private int sequenceIndex;

    private void OnEnable()
    {
        Sequenceable.StartNextPhase += SequenceItemCompleted;
        PostProcessingControl.StartNextPhase += FirstPhaseCompleted;
    }

    private void OnDisable()
    {
        Sequenceable.StartNextPhase -= SequenceItemCompleted; 
        PostProcessingControl.StartNextPhase -= FirstPhaseCompleted;

    }

    private void Start()
    {
        _sequenceableItems = transform.GetComponentsInChildren<Sequenceable>(); //get sequenceable items
        _sequenceableItems.First().Initialize(); //initialize it. the first one will now start receiving progress values
        MuteMixer(_mixer);
    }

    private void MuteMixer(AudioMixer mixer)
    {
        foreach (AudioMixerGroup group in mixer.FindMatchingGroups("")) mixer.SetFloat(group.name, -80f);
    }

    private void SequenceItemCompleted(Sequenceable item) //called when current sequence item completed
    {
        sequenceIndex++;
        _sequenceableItems[sequenceIndex].Initialize();
    }

    private void FirstPhaseCompleted(Sequenceable item)
    {
        MuteMixer(_mixer);
    }
}
