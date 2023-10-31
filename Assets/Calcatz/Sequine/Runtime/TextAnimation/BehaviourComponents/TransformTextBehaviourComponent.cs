using TMPro;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Using Transform component, we can modify the transform of the characters.
    /// We can combine on which properties (Scale, Position, and Rotation) of the transform we want to apply.
    /// </summary>
    public class TransformTextBehaviourComponent : TextBehaviourComponent {

        [SerializeField] private bool animatePosition;
        [SerializeField] private bool animateRotation;
        [SerializeField] private bool animateScale;

        [SerializeField] private bool applyOnX;
        [SerializeField] private bool applyOnY;
        [SerializeField] private bool applyOnZ;

        [SerializeField] private AnimationCurve positionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        [SerializeField] private Vector3 positionMultiplier;

        [SerializeField] private AnimationCurve rotationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        [SerializeField] private Vector3 rotationMultiplier;

        [SerializeField] private AnimationCurve scaleCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        public override bool overrideGeometry => true;
        public override bool overrideVertexData => false;


        public override void HandleCharacterBehaviour(CharacterData _characterData, TMP_Text _textComponent, TMP_TextInfo _textInfo, TMP_MeshInfo[] _meshInfo, Bounds _meshBounds, float _segmentNormalizedTime) {

            int materialIndex = _characterData.materialIndex;

            int vertexIndex = _characterData.vertexIndex;

            Vector3[] sourceVertices = _meshInfo[materialIndex].vertices;

            Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

            Vector3 offset = charMidBasline;

            Vector3[] destinationVertices = _textInfo.meshInfo[materialIndex].vertices;

            destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
            destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
            destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
            destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

            Vector3 finalScale = Vector3.one;
            if (animateScale) {
                if (applyOnX)
                    finalScale.x = scaleCurve.Evaluate(_characterData.progress);

                if (applyOnY)
                    finalScale.y = scaleCurve.Evaluate(_characterData.progress);

                if (applyOnZ)
                    finalScale.z = scaleCurve.Evaluate(_characterData.progress);
            }

            Vector3 finalPosition = Vector3.zero;
            if (animatePosition)
                finalPosition = positionMultiplier * positionCurve.Evaluate(_characterData.progress);

            Quaternion finalQuaternion = Quaternion.identity;
            if (animateRotation)
                finalQuaternion = Quaternion.Euler(rotationMultiplier * rotationCurve.Evaluate(_characterData.progress));

            Matrix4x4 matrix = Matrix4x4.TRS(finalPosition, finalQuaternion, finalScale);

            destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
            destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
            destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
            destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

            destinationVertices[vertexIndex + 0] += offset;
            destinationVertices[vertexIndex + 1] += offset;
            destinationVertices[vertexIndex + 2] += offset;
            destinationVertices[vertexIndex + 3] += offset;
        }
    }
}