using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem {
    public class DialogueDisplayer : MonoBehaviour {
        public string dialogueId;
        public CharSequenceDisplayer csd;
        private DialogueDataInfo ddi;

        private void Start() {
            if (DialogueSystem.dialogueData.ContainsKey(dialogueId)) {
                ddi = DialogueSystem.dialogueData[dialogueId];
                Open();
                return;
            }
            
            Debug.LogError($"Dialogue with id {dialogueId} could not be found!");
            Destroy(this);
        }

        public void Open(string lineId = null) {
            if (lineId == null) {
                lineId = "Start";
                if (DialogueSystem.getStartFuncs.ContainsKey(dialogueId))
                    lineId = DialogueSystem.getStartFuncs[dialogueId]();
            }
            var mainTextBox = csd.display(ddi.data.getLine(lineId));
            
            List<string> options = ddi.data.getOptionContents(lineId);
            string fullLineId = $"{dialogueId}.{lineId}";
            if (DialogueSystem.filterOptionFuncs.ContainsKey(fullLineId))
                options = DialogueSystem.filterOptionFuncs[fullLineId]();
            csd.listOptions(this, lineId, options, mainTextBox.transform.position);
        }

        public string GetOptionTarget(string lineId, string optionContent) {
            string optionTarget = DialogueSystem.dialogueData[dialogueId].data.getOptionTarget(lineId, optionContent);
            if (!optionTarget.StartsWith("//")) return optionTarget;
            
            optionTarget = optionTarget.Replace("//", "");
            optionTarget = DialogueSystem.getOptionTargetFuncs[optionTarget]();
            return optionTarget;
        }

        public void Select(string optionTarget) {
            if (optionTarget.Trim() == "") return;
            if (optionTarget.StartsWith("//")) {
                string funcId = optionTarget.Replace("//", "");
                if (!DialogueSystem.getOptionTargetFuncs.ContainsKey(funcId)) {
                    Debug.LogError($"No GetOptionTarget Func named {funcId} could be found!");
                    return;
                }
                optionTarget = DialogueSystem.getOptionTargetFuncs[funcId]();
            }
            var mainTextBox = csd.display(ddi.data.getLine(optionTarget));
            
            List<string> options = ddi.data.getOptionContents(optionTarget);
            string fullLineId = $"{dialogueId}.{optionTarget}";
            if (DialogueSystem.filterOptionFuncs.ContainsKey(fullLineId))
                options = DialogueSystem.filterOptionFuncs[fullLineId]();
            csd.listOptions(this, optionTarget, options, mainTextBox.transform.position);
        }
    }
}