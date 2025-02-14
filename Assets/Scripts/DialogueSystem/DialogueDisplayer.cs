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
        
        // Start Line Id
        public Action<string> onOpen;
        // Selected Option Content, Old Line Id, New Line Id
        public Action<string, string, string> onOptionClicked;
        // Old Line Id, New Line Id
        public Action<string, string> onLineChanged;

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
            onOpen?.Invoke(lineId);
            csd.display(ddi.data.getLine(lineId));
            onLineChanged?.Invoke(currentLineId, lineId);
            currentLineId = lineId;

            List<string> options = ddi.data.getOptionContents(lineId);
            string fullLineId = $"{dialogueId}.{lineId}";
            if (DialogueSystem.filterOptionFuncs.ContainsKey(fullLineId))
                options = DialogueSystem.filterOptionFuncs[fullLineId]();
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
            
            onOptionClicked?.Invoke(optionContent, currentLineId, optionTarget);
            csd.display(ddi.data.getLine(optionTarget));
            onLineChanged?.Invoke(currentLineId, optionTarget);
            currentLineId = optionTarget;

            List<string> options = ddi.data.getOptionContents(optionTarget);
            string fullLineId = $"{dialogueId}.{optionTarget}";
            if (DialogueSystem.filterOptionFuncs.ContainsKey(fullLineId))
                options = DialogueSystem.filterOptionFuncs[fullLineId]();
            csd.listOptions(this, optionTarget, options);
        }
    }
}