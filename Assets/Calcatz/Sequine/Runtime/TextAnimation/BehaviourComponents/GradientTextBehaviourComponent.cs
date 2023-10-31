using TMPro;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Using Gradient component, we can change the color of the text overtime by specifying the gradient color from the Gradient field.
    /// </summary>
    public class GradientTextBehaviourComponent : TextBehaviourComponent {

        [SerializeField] private Gradient gradient;

        private Color32[] newVertexColors;
        private Color targetColor;

        public override bool overrideGeometry => false;
        public override bool overrideVertexData => true;

        public override void HandleCharacterBehaviour(CharacterData _characterData, TMP_Text _textComponent, TMP_TextInfo _textInfo, TMP_MeshInfo[] _meshInfo, Bounds _meshBounds, float _segmentNormalizedTime) {

            if (gradient == null)
                return;

            int materialIndex = _characterData.materialIndex;

            newVertexColors = _textInfo.meshInfo[materialIndex].colors32;

            int vertexIndex = _characterData.vertexIndex;

            targetColor = gradient.Evaluate(_characterData.progress);

            newVertexColors[vertexIndex + 0] = targetColor;
            newVertexColors[vertexIndex + 1] = targetColor;
            newVertexColors[vertexIndex + 2] = targetColor;
            newVertexColors[vertexIndex + 3] = targetColor;
        }
    }
}
