using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Sticky note which contains only notes/information without actually affecting the process.
    /// </summary>
    public class StickyNoteCommand : Command {

#if UNITY_EDITOR
        public StickyNoteCommand() {
            nextIds.Clear();
            inputIds.Clear();
        }

        public Color color = new Color(0.2569865f, 0.4540747f, 0.7264151f, 0.815f);
        public string notes;
        public Vector2 size = new Vector2(200, 50);
#endif

    }
}
