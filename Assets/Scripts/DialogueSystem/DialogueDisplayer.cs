using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem {
    public class DialogueDisplayer : MonoBehaviour {
        public string id;
        
        public string dialogueId;
        private CharSequenceDisplayer csd;
        private SpriteRenderer sr;
        private DialogueDataInfo ddi;

        private string currentLineId;

        private void Start() {
            sr = GetComponent<SpriteRenderer>();
            csd = GetComponent<CharSequenceDisplayer>();
            if (DialogueSystem.dialogueData.ContainsKey(dialogueId)) 
                ddi = DialogueSystem.dialogueData[dialogueId];
        }
        
        public void SetDialogueId(string dialogueId) {
            this.dialogueId = dialogueId;
            if (DialogueSystem.dialogueData.ContainsKey(dialogueId)) {
                ddi = DialogueSystem.dialogueData[dialogueId];
                return;
            }
            
            Debug.LogError($"Dialogue with id {dialogueId} could not be found!");
            Destroy(this);
        }

        public void Open(string lineId = null) {
            sr.enabled = true;
            
            if (lineId == null) {
                lineId = "Start";
                if (DialogueSystem.getStartFuncs.ContainsKey(dialogueId))
                    lineId = DialogueSystem.getStartFuncs[dialogueId]();
            }
            DialogueSystem.onOpen?.Invoke(dialogueId, id, lineId);
            csd.display(ddi.data.getLine(lineId));
            DialogueSystem.onLineChanged?.Invoke(dialogueId, id, currentLineId, lineId);
            currentLineId = lineId;

            List<string> options = ddi.data.getOptionContents(lineId);
            string fullLineId = $"{dialogueId}.{lineId}";
            if (DialogueSystem.filterOptionFuncs.ContainsKey(fullLineId))
                options = DialogueSystem.filterOptionFuncs[fullLineId](options);
            csd.listOptions(this, lineId, options);
        }

        public void Close() {
            csd.clear();
            sr.enabled = false;
        }
        
        public string GetOptionTarget(string lineId, string optionContent) {
            string optionTarget = DialogueSystem.dialogueData[dialogueId].data.getOptionTarget(lineId, optionContent);
            if (!optionTarget.StartsWith("//")) return optionTarget;
            
            optionTarget = optionTarget.Replace("//", "");
            optionTarget = DialogueSystem.getOptionTargetFuncs[optionTarget]();
            return optionTarget;
        }

        public void Select(string optionContent, string optionTarget) {
            if (optionTarget == "End") {
                Close();
                return;
            }
            
            if (optionTarget.Trim() == "") return;
            if (optionTarget.StartsWith("//")) {
                string funcId = optionTarget.Replace("//", "");
                if (!DialogueSystem.getOptionTargetFuncs.ContainsKey(funcId)) {
                    Debug.LogError($"No GetOptionTarget Func named {funcId} could be found!");
                    return;
                }
                optionTarget = DialogueSystem.getOptionTargetFuncs[funcId]();
            }
            
            DialogueSystem.onOptionClicked?.Invoke(dialogueId, id, optionContent, currentLineId, optionTarget);
            csd.display(ddi.data.getLine(optionTarget));
            DialogueSystem.onLineChanged?.Invoke(dialogueId, id, currentLineId, optionTarget);
            currentLineId = optionTarget;

            List<string> options = ddi.data.getOptionContents(optionTarget);
            string fullLineId = $"{dialogueId}.{optionTarget}";
            if (DialogueSystem.filterOptionFuncs.ContainsKey(fullLineId))
                options = DialogueSystem.filterOptionFuncs[fullLineId](options);
            csd.listOptions(this, optionTarget, options);
        }
    }
}