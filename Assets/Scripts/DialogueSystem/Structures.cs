using System;
using System.Collections.Generic;
using System.Linq;
using Fries.Inspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DialogueSystem {
    public static class DialogueSystem {
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
        
        // TODO 没有setDialogueId的时候给出明确的报错提示
        // 按钮存在连点现象，点了一个之后立马吧钢弹出的新选项也点了
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