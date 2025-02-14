using System.Collections.Generic;
# if UNITY_EDITOR
using UnityEditor;
# endif
using UnityEngine;
using UnityEngine.Localization;

namespace DialogueSystem {
    public class LocalizedDialogue : MonoBehaviour {
        public List<LocalizedDialogueData> dialogues;

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