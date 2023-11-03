using TMPro;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Base class for Text Behaviour Components, which defines the actual behaviour of a Text Behaviour Profile.
    /// </summary>
    [System.Serializable]
    public abstract class TextBehaviourComponent : ScriptableObject {

#if UNITY_EDITOR
        [HideInInspector] public bool editor_foldout = true;
#endif

        [HideInInspector] public bool active = true;

        public abstract bool overrideGeometry { get; }
        public abstract bool overrideVertexData { get; }

        public abstract void HandleCharacterBehaviour(CharacterData _characterData, TMP_Text _textComponent, TMP_TextInfo _textInfo, TMP_MeshInfo[] _meshInfo, Bounds _meshBounds, float _segmentNormalizedTime);

    }
}