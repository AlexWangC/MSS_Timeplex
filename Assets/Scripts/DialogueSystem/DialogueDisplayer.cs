using System;
using System.Collections.Generic;
using Fries;
using UnityEngine;

namespace DialogueSystem {
    [RequireComponent(typeof(GridObject))]
    public class DialogueDisplayer : MonoBehaviour {
        private static Dictionary<string, DialogueDisplayer> dialogueDisplayers = new();

        public static DialogueDisplayer getDialogueDisplayer(string id) {
            return dialogueDisplayers[id];
        }
        
        public string id;
        public string dialogueId;
        public string startLineId;

        public bool isInteractive = true;
        public bool drawGizmos = true;
        
        private CharSequenceDisplayer csd;
        private SpriteRenderer sr;
        private DialogueDataInfo ddi;
        private GridObject gridObject;

        private string currentLineId;

        private void Start() {
            if (id.Trim() == "") {
                Debug.LogWarning("Dialogues with empty id will not be able to use Dialogue System Events and ");
            }
            else {
                dialogueDisplayers[id] = this;
            }
            sr = GetComponent<SpriteRenderer>();
            csd = GetComponent<CharSequenceDisplayer>();
            gridObject = GetComponent<GridObject>();
            if (DialogueSystem.dialogueData.ContainsKey(dialogueId)) 
                ddi = DialogueSystem.dialogueData[dialogueId];
        }

        private void Update() {
            if (!Input.GetKeyDown(DialogueSystem.interactKey)) return;
            
            scrPlayer[] players = FindObjectsByType<scrPlayer>(FindObjectsSortMode.InstanceID);
            foreach (var pl in players) {
                if (getTimeIndex(pl.transform) != getTimeIndex()) continue;
                GridObject go = pl.getComponent<GridObject>();
                if (go.gridPosition != gridObject.gridPosition) continue;
                Open(startLineId);
            }
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
            if (lineId != null && lineId.Trim() == "") lineId = null;
            
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

            List<string> options = ddi?.data?.getOptionContents(optionTarget);
            string fullLineId = $"{dialogueId}.{optionTarget}";
            if (DialogueSystem.filterOptionFuncs.ContainsKey(fullLineId))
                options = DialogueSystem.filterOptionFuncs[fullLineId](options);
            csd?.listOptions(this, optionTarget, options);
        }

        private void OnDrawGizmos() {
            // 设置 Gizmos 的颜色（例如黄色）
            Gizmos.color = Color.yellow;
            // 在当前 gameobject 的世界位置绘制一个半径为 0.2 的小球
            Gizmos.DrawSphere(transform.position, 0.175f);
        }

        private int getTimeIndex(Transform t = null) {
            if (t == null) t = this.transform;
            scrPanel panel = t.parent.getComponent<scrPanel>();
            return panel.Time_index;
        }
    }
}