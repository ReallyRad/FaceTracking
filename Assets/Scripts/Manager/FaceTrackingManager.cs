using UnityEngine;

public class FaceTrackingManager : MonoBehaviour
{
    public delegate void OnPuckerTrigger(bool pucker);
    public static OnPuckerTrigger PuckerTrigger;

    [SerializeField] private OVRFaceExpressions _faceExpressions;

    [Range(-0.065f, 0.0f)]
    [SerializeField] private float _mouthValue;
    [SerializeField] private float _puckerThreshold;
    [SerializeField] private bool _manualSmileControl;
    [SerializeField] private bool _debugPucker;
    private bool _wasDebugPucker;
    
    private float _previousMouthValue;
    
    private void Update()
    {
        Vector2 lipPucker = new Vector2();
       
        if (!_manualSmileControl) //using face tracking
        {
            lipPucker = GetExpressionValue(
                OVRFaceExpressions.FaceExpression.LipPuckerL,
                OVRFaceExpressions.FaceExpression.LipPuckerR);
            
            _mouthValue = - (lipPucker.x + lipPucker.y) / 2;
        }

        bool wasPucker = _previousMouthValue < _puckerThreshold;
        bool pucker = _mouthValue < _puckerThreshold;
        
        //normal pucker detection (with face tracking)
        if (pucker && !wasPucker) PuckerTrigger(true); //if we just started pucker
        else if (wasPucker && !pucker) PuckerTrigger(false); //if we just stopped pucker 
        
        //debug pucker (in editor or when automated)
        if (_debugPucker && !_wasDebugPucker) PuckerTrigger(true); //if we just started pucker
        else if (_wasDebugPucker && !_debugPucker) PuckerTrigger(false); //if we just stopped pucker 

        _wasDebugPucker = _debugPucker; 
        _previousMouthValue = _mouthValue;
    }

    private Vector2 GetExpressionValue(OVRFaceExpressions.FaceExpression key1, OVRFaceExpressions.FaceExpression key2)
    {
        float w;
        Vector2 expressionVector = new Vector2();
        _faceExpressions.TryGetFaceExpressionWeight(key1, out w);
        expressionVector.x = w;
        _faceExpressions.TryGetFaceExpressionWeight(key2, out w);
        expressionVector.y = w;

        return expressionVector;
    }
}
