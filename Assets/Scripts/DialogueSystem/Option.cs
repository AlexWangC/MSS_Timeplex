using UnityEngine;

namespace DialogueSystem {
    public class Option : MonoBehaviour {
        public DialogueDisplayer dd;
        public string optionTargetId;
        public string optionContent;
        
        public void JumpToTarget() {
            dd.Select(optionContent, optionTargetId);
        }
    }
}