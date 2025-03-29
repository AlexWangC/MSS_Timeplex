using System;
using System.Collections.Generic;
using System.Linq;
using Fries;
using Fries.Inspector;
using Fries.Inspector.GameObjectBoxField;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DialogueSystem {
    public static class DialogueSystem {
        public static KeyCode interactKey = KeyCode.F;
        
        // Dialogue Id => Dialogue Data Info
        public static Dictionary<string, DialogueDataInfo> dialogueData = new();
        // Dialogue Id => Get Start Func
        public static Dictionary<string, Func<string>> getStartFuncs = new();
        // Func Id => Get Option Target Func
        public static Dictionary<string, Func<string>> getOptionTargetFuncs = new();
        // Dialogue Id + Line Id "{DialogueId}.{LineId}" => Filter Option Func
        public static Dictionary<string, Func<List<string>, List<string>>> filterOptionFuncs = new();
        
        // Dialogue Id, Dialogue Displayer Id, Start Line Id
        public static Action<string, string, string> onOpen;
        // Dialogue Id, Dialogue Displayer Id, Selected Option Content, Old Line Id, New Line Id
        public static Action<string, string, string, string, string> onOptionClicked;
        // Dialogue Id, Dialogue Displayer Id, Old Line Id, New Line Id
        public static Action<string, string, string, string> onLineChanged;

        public static void processCmds(GameObjectBox<StringSso> ssso, string name, string lineId) {
            if (ssso == null) return;
            if (ssso.sysObj == null) return;
            string str = ssso.sysObj.get<string>().Nullable().Trim();
            if (!string.IsNullOrEmpty(str)) {
                DialogueSystem.onLineChanged += (dialogueId, dialogueDisplayerId, oldLineId, newLineId) => {
                    if (dialogueId == name && oldLineId == lineId) {
                        string[] cmdLines = str.Split("\n");
                        foreach (var cmd in cmdLines) {
                            List<string> cmdComp = cmd.Split(" ").ToList();
                            string name = cmdComp[0];
                            cmdComp.RemoveAt(0);
                            DialogueCommandData data = new DialogueCommandData {
                                commandArgs = cmdComp.ToArray(),
                                dialogueDisplayer = DialogueDisplayer.getDialogueDisplayer(dialogueDisplayerId),
                                dialogueId = dialogueId,
                                gameObject = DialogueDisplayer.getDialogueDisplayer(dialogueDisplayerId).gameObject,
                                newLineId = newLineId,
                                oldLineId = oldLineId
                            };
                            DialogueCommands.runCmd(name, data);
                        }
                    }
                };
            }
        }
    }
    
    [Serializable]
    public class DialogueData {
        public string name;
        
        public string getDialogueName() {
            return name;
        }
        
        public virtual void init() {}

        public virtual string getLine(string lineId) {
            return null;
        }

        public virtual List<string> getOptionContents(string lineId) {
            return null;
        }

        public virtual string getOptionTarget(string lineId, string optionContent) {
            return null;
        }
        
        public virtual string toString() {
            return null;
        }
    }

    public class DialogueDataInfo {
        public DialogueData data;
        public MonoBehaviour registerer;
    }
}