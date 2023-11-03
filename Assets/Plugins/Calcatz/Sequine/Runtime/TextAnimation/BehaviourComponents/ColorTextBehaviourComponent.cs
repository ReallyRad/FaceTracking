using TMPro;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Using Colors component, we can change the text colors by specifying the color list from the Colors field.
    /// </summary>
    public class ColorTextBehaviourComponent : TextBehaviourComponent {

        [SerializeField] private Color[] colors = new Color[] { Color.white };

        private Color32[] newVertexColors;
        private Color32 targetColor;

        public override bool overrideGeometry => false;
        public override bool overrideVertexData => true;

        public override void HandleCharacterBehaviour(CharacterData _characterData, TMP_Text _textComponent, TMP_TextInfo _textInfo, TMP_MeshInfo[] _meshInfo, Bounds _meshBounds, float _segmentNormalizedTime) {

            if (colors == null || colors.Length == 0)
                return;

            int materialIndex = _characterData.materialIndex;

            newVertexColors = _textInfo.meshInfo[materialIndex].colors32;

            int vertexIndex = _characterData.vertexIndex;

            targetColor = colors[Mathf.CeilToInt(_characterData.progress * (colors.Length - 1))];

            newVertexColors[vertexIndex + 0] = targetColor;
            newVertexColors[vertexIndex + 1] = targetColor;
            newVertexColors[vertexIndex + 2] = targetColor;
            newVertexColors[vertexIndex + 3] = targetColor;
        }

    }

}
