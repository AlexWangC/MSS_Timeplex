using System;
using System.Collections.Generic;
using System.Linq;
using Fries.Inspector;
using UnityEngine;

namespace DialogueSystem {
    public static class DialogueSystem {
        // Dialogue Id => Dialogue Data Info
        public static Dictionary<string, DialogueDataInfo> dialogueData = new();
        // Dialogue Id => Get Start Func
        public static Dictionary<string, Func<string>> getStartFuncs = new();
        // Func Id => Get Option Target Func
        public static Dictionary<string, Func<string>> getOptionTargetFuncs = new();
        // Dialogue Id + Line Id "{DialogueId}.{LineId}" => Filter Option Func
        public static Dictionary<string, Func<List<string>>> filterOptionFuncs = new();
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
    }

    public class DialogueDataInfo {
        public DialogueData data;
        public MonoBehaviour registerer;
    }
}