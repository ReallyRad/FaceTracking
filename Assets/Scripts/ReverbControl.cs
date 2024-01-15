using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class ReverbControl : InteractiveSequenceable
{
    [SerializeField] private AudioReverbZone reverbZone;
    [SerializeField] private int reverbZoneRoomInitialValue;
    [SerializeField] private int reverbZoneRoomFinalValue;

    private int _interactTween;
    private int _decayTween;

    public override void Initialize()
    {
        _active = true;
        reverbZone.room = reverbZoneRoomInitialValue;
    }

    public override void Interact()
    {
        if (_decayTween != 0)
        {
            LeanTween.pause(_decayTween);
            Debug.Log("paused decay tween  " + _decayTween);
        }

        _interactTween = LeanTween.value(gameObject, 
                                         reverbZone.room, 
                                         reverbZoneRoomFinalValue, 
                                         _riseTime)
            .setOnUpdate(val =>
            {
                TweenHandling(val);
            })
            .id;
    }

    public override void Decay()
    {
        if (_interactTween != 0)
        {
            LeanTween.pause(_interactTween);
            Debug.Log("paused interact tween  " + _interactTween);
        }

        _decayTween = LeanTween.value(gameObject, 
                                      reverbZone.room, 
                                      reverbZoneRoomInitialValue, 
                                      _decayTime)
            .setOnUpdate(val =>
            {
                TweenHandling(val);
            })
            .id;
    }

    private void TweenHandling(float val)
    {
        int intValue = Mathf.RoundToInt(val);
        reverbZone.room = intValue;
    }


}
