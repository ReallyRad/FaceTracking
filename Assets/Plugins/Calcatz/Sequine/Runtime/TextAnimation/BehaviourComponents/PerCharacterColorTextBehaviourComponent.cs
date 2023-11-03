using TMPro;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Using Per Character Color component, we can change the text color per character by turns, according to the color order specified in the Colors list.
    /// </summary>
    public class PerCharacterColorTextBehaviourComponent : TextBehaviourComponent {

        [SerializeField] private AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        [SerializeField] private Color[] colors = new Color[] { Color.white };

        private Color32[] newVertexColors;

        public override bool overrideGeometry => false;
        public override bool overrideVertexData => true;

        public override void HandleCharacterBehaviour(CharacterData _characterData, TMP_Text _textComponent, TMP_TextInfo _textInfo, TMP_MeshInfo[] _meshInfo, Bounds _meshBounds, float _segmentNormalizedTime) {

            if (colors == null || colors.Length == 0)
                return;

            int materialIndex = _characterData.materialIndex;

            newVertexColors = _textInfo.meshInfo[materialIndex].colors32;

            int vertexIndex = _characterData.vertexIndex;


            Color targetColor = colors[_characterData.index % colors.Length];
            Color currentColor = newVertexColors[0];


            targetColor = targetColor * curve.Evaluate(_characterData.progress);
            currentColor = currentColor * (1f - curve.Evaluate(_characterData.progress));

            newVertexColors[vertexIndex + 0] = currentColor + targetColor;
            newVertexColors[vertexIndex + 1] = currentColor + targetColor;
            newVertexColors[vertexIndex + 2] = currentColor + targetColor;
            newVertexColors[vertexIndex + 3] = currentColor + targetColor;
        }
    }
}