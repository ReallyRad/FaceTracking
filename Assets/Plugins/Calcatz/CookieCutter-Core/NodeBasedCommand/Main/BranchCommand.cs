using System.Collections.Generic;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Evaluates each condition in order, from top to bottom. The first condition met will be executed, while the rest will be skipped.
    /// </summary>
    [System.Serializable]
    public class BranchCommand : Command {

#if UNITY_EDITOR
        /// <summary>
        /// Prevent including editor data for build
        /// </summary>
        public string notes;
#endif

        public BranchCommand() {
            inputIds.Add(new ConnectionTarget());
            defaultValues.Add(true);
        }

        public List<bool> defaultValues = new List<bool>();

        protected int nextOutputIndex = -1;

        public virtual void AddBranch() {
            nextIds.Add(new List<ConnectionTarget>() { new ConnectionTarget() });
            inputIds.Add(new ConnectionTarget());
            defaultValues.Add(true);
        }

        public virtual void RemoveBranch(int _index) {
            if (_index < nextIds.Count) {
                nextIds.RemoveAt(_index);
                inputIds.RemoveAt(_index);
                defaultValues.RemoveAt(_index);
            }
        }

        public override void Execute(CommandExecutionFlow _flow) {
            nextOutputIndex = -1;
            for(int i=0; i < inputIds.Count; i++) {
                if (inputIds[i].targetId == 0) {
                    if (defaultValues[i] == true) {
                        nextOutputIndex = i;
                        break;
                    }
                }
                else if (_flow.GetCommandOutput<bool>(inputIds[i].targetId, inputIds[i].pointIndex) == true) {
                    nextOutputIndex = i;
                    break;
                }
            }
            base.Execute(_flow);
        }

        public override int GetNextOutputIndex() {
            return nextOutputIndex;
        }

    }
}
