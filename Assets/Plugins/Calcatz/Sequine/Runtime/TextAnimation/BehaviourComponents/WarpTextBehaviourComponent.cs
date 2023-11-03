using TMPro;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Using Warp component, we can warp the text mesh based on the Warp Curve.
    /// The warp amplitude can be increased or decreased using the Curve Multiplier.
    /// We can then define how it applies to the animation using the Intensity Curve.
    /// </summary>
    public class WarpTextBehaviourComponent : TextBehaviourComponent {

        [SerializeField] private float curveMultipler = 10;
        [SerializeField] private AnimationCurve intensityCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        [SerializeField] private AnimationCurve warpCurve = new AnimationCurve(new Keyframe(0, 0),
                                                                               new Keyframe(0.25f, 2.0f),
                                                                               new Keyframe(0.5f, 0), new Keyframe(0.75f, 2.0f),
                                                                               new Keyframe(1, 0f));

        private Matrix4x4 matrix;
        private Vector3[] vertices;


        public override bool overrideGeometry => true;
        public override bool overrideVertexData => true;

        public override void HandleCharacterBehaviour(CharacterData _characterData, TMP_Text _textComponent, TMP_TextInfo _textInfo, TMP_MeshInfo[] _meshInfo, Bounds _meshBounds, float _segmentNormalizedTime) {

            float boundsMinX = _meshBounds.min.x;
            float boundsMaxX = _meshBounds.max.x;

            vertices = _textInfo.meshInfo[_characterData.materialIndex].vertices;

            int characterDataVertexIndex = _characterData.vertexIndex;
            Vector3 offsetToMidBaseline = new Vector2(
                (vertices[characterDataVertexIndex + 0].x
                 + vertices[characterDataVertexIndex + 2].x) / 2,
                _textInfo.characterInfo[_characterData.index].baseLine);

            vertices[characterDataVertexIndex + 0] += -offsetToMidBaseline;
            vertices[characterDataVertexIndex + 1] += -offsetToMidBaseline;
            vertices[characterDataVertexIndex + 2] += -offsetToMidBaseline;
            vertices[characterDataVertexIndex + 3] += -offsetToMidBaseline;

            float x0 = (offsetToMidBaseline.x - boundsMinX) / (boundsMaxX - boundsMinX);
            float x1 = x0 + 0.0001f;
            float y0 = warpCurve.Evaluate(x0) * curveMultipler;
            float y1 = warpCurve.Evaluate(x1) * curveMultipler;

            y0 *= intensityCurve.Evaluate(_characterData.progress);
            y1 *= intensityCurve.Evaluate(_characterData.progress);

            Vector3 horizontal = new Vector3(1, 0, 0);
            Vector3 tangent = new Vector3(x1 * (boundsMaxX - boundsMinX) + boundsMinX, y1) - new Vector3(offsetToMidBaseline.x, y0);

            float dot = Mathf.Acos(Vector3.Dot(horizontal, tangent.normalized)) * 57.2957795f;
            Vector3 cross = Vector3.Cross(horizontal, tangent);
            float angle = cross.z > 0 ? dot : 360 - dot;

            matrix = Matrix4x4.TRS(new Vector3(0, y0, 0), Quaternion.Euler(0, 0, angle), Vector3.one);

            vertices[characterDataVertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[characterDataVertexIndex + 0]);
            vertices[characterDataVertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[characterDataVertexIndex + 1]);
            vertices[characterDataVertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[characterDataVertexIndex + 2]);
            vertices[characterDataVertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[characterDataVertexIndex + 3]);

            vertices[characterDataVertexIndex + 0] += offsetToMidBaseline;
            vertices[characterDataVertexIndex + 1] += offsetToMidBaseline;
            vertices[characterDataVertexIndex + 2] += offsetToMidBaseline;
            vertices[characterDataVertexIndex + 3] += offsetToMidBaseline;
        }
    }
}