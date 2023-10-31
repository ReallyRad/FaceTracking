using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private void OnEnable()
    {
        if (ModeControl.Continuous)
        {
            if (ModeControl.auraApproachingRP == "Progressive")
            {
                if (ModeControl.ContinuousACT_Ex)
                {
                    BreathingControl.New_Ex_LongerThanMinimum += ContinuousProgressiveWhole;
                }
                if (ModeControl.ContinuousACT_In)
                {
                    BreathingControl.New_In_LongerThanMinimum += ContinuousProgressiveWhole;
                }
                if (ModeControl.ContinuousACT_PeInPi)
                {
                    BreathingControl.New_PeInPi_LongerThanMinimum = ContinuousProgressiveWhole;
                }
            }
        }
    }
    private void OnDisable()
    {
        if (ModeControl.Continuous)
        {
            if (ModeControl.auraApproachingRP == "Progressive")
            {
                if (ModeControl.ContinuousACT_Ex)
                {
                    BreathingControl.New_Ex_LongerThanMinimum -= ContinuousProgressiveWhole;
                }
                if (ModeControl.ContinuousACT_In)
                {
                    BreathingControl.New_In_LongerThanMinimum -= ContinuousProgressiveWhole;
                }
                if (ModeControl.ContinuousACT_PeInPi)
                {
                    BreathingControl.New_PeInPi_LongerThanMinimum -= ContinuousProgressiveWhole;
                }
            }
        }
    }

    private float _maxVolume = 1f;
    private float _currentVolumeOfCurrentSL;
    private float _currentVolumeOfCurrentSLIncrease;
    
    [SerializeField] private SeamlessLoop[] _seamlessLoops;
    [SerializeField] private int _progress;
    [SerializeField] private int _currentLoopIndex;

    private void ContinuousProgressiveWhole(float fraction)
    {
        if (_currentLoopIndex < _seamlessLoops.Length)
        {
            _currentVolumeOfCurrentSL = _seamlessLoops[_currentLoopIndex].GetVolume();
            if (_currentVolumeOfCurrentSL < _maxVolume)
            {
                _currentVolumeOfCurrentSLIncrease = _maxVolume * fraction * Time.deltaTime;
                _seamlessLoops[_currentLoopIndex].SetVolume(_currentVolumeOfCurrentSL + _currentVolumeOfCurrentSLIncrease);
            }
            else if (_currentLoopIndex < (_seamlessLoops.Length - 1))
            {
                _currentLoopIndex++;
                _currentVolumeOfCurrentSL = _seamlessLoops[_currentLoopIndex].GetVolume();
                _currentVolumeOfCurrentSLIncrease = _maxVolume * fraction * Time.deltaTime;
                _seamlessLoops[_currentLoopIndex].SetVolume(_currentVolumeOfCurrentSL + _currentVolumeOfCurrentSLIncrease);
            }
        }
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
