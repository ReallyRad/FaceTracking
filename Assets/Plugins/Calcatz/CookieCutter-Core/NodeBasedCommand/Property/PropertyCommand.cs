
namespace Calcatz.CookieCutter {

    /// <summary>
    /// A command that passes through property connection, and has no main (white) connection.
    /// </summary>
    [System.Serializable]
    public abstract class PropertyCommand : Command {

#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            
        }
        public override void Editor_InitOutPoints() {
            
        }
        public override void Editor_OnDrawTitle(out string _tooltip) {
            _tooltip = null;
        }
#endif

    }
}
