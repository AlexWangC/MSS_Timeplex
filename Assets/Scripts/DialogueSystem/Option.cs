using UnityEngine;

namespace DialogueSystem {
    public class Option : MonoBehaviour {
        public DialogueDisplayer dd;
        public string optionTargetId;

        public void JumpToTarget() {
            dd.Select(optionTargetId);
        }
    }
}