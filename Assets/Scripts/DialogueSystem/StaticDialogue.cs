using System.Collections.Generic;
# if UNITY_EDITOR
using UnityEditor;
# endif
using UnityEngine;

namespace DialogueSystem {
    public class StaticDialogue : MonoBehaviour {
        
        public List<StaticDialogueData> dialogues;

        private void Awake() {
            dialogues.ForEach(data => {
                if (DialogueSystem.dialogueData.TryGetValue(data.name, out var value)) {
                    Debug.LogWarning($"Global Dialogue Data already has dialogue with name {data.name}. The old one will be discarded.");
# if UNITY_EDITOR
                    Selection.activeObject = value.registerer;
# endif
                }
                data.init();
                
                DialogueSystem.dialogueData[data.name] = new DialogueDataInfo {
                    data = data,
                    registerer = this
                };
            });
        }
    }
}