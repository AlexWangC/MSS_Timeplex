using System;
using System.Collections.Generic;
using Fries;
using Fries.Inspector;
# if UNITY_EDITOR
using UnityEditor;
# endif
using UnityEngine;

namespace DialogueSystem {
    public class ComplexStaticDialogue : MonoBehaviour {
        
        [FieldAnchor]
        public List<ComplexStaticDialogueData> dialogues;

        private void Start() {
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
    
    # if UNITY_EDITOR
    [CustomEditor(typeof(ComplexStaticDialogue))]
    public class ComplexStaticDialogueInspector : AnInspector {}
    # endif
}