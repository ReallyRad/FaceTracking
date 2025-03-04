using UnityEngine;
using UnityEngine.UI;

public class SubjectIDInput : MonoBehaviour
{
    [SerializeField] private int _modifiedValues;
    [SerializeField] private int _minLength;
    [SerializeField] private Button _nextButton;

    public void IncrementModifiedValues(string value)
    {
        if (value.Length >= _minLength) _nextButton.interactable = true;
    }

}
