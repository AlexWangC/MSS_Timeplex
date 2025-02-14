using System;
using System.Collections.Generic;
using System.Linq;
using Fries.Inspector;
using Fries.Inspector.GameObjectBoxField;
using UnityEngine;
using UnityEngine.Localization;

namespace DialogueSystem {
    [Serializable]
    public class LocalizedDialogueData : DialogueData {
        public List<LinePair2> lines;
        private Dictionary<string, LocalizedLine> data;

        public override void init() {
            data = new();
            lines.ForEach(pair => {
                string lineId = pair.key;
                GameObjectBox<LocalizedString> lineContent = pair.value.key;
                if (lineContent.sysObj == null || lineContent.sysObj.get<LocalizedString>() == null) {
                    Debug.LogError($"Unassigned localized string presents in line content of {lineId}!");
                    return;
                }

                GameObjectBoxes<LocalizedString> lineOptionsRaw = pair.value.value;
                List<LocalizedString> lineOptions = 
                    lineOptionsRaw.list.Select(optionRaw => {
                        if (optionRaw.sysObj == null || optionRaw.sysObj.get<LocalizedString>() == null) 
                            Debug.LogError($"Unassigned localized string presents in line options of {lineId}!");
                        return optionRaw.sysObj.get<LocalizedString>();
                    }).ToList();

                if (data.ContainsKey(lineId)) 
                    Debug.LogWarning($"{lineId} is already present in the line set! Please make sure there is no duplicate names!");
                data[lineId] = new LocalizedLine(lineContent.sysObj.get<LocalizedString>(), lineOptions);
            });
        }

        public override string getLine(string lineId) {
            return data[lineId].getLine();
        }

        public override List<string> getOptionContents(string lineId) {
            return data[lineId].getOptionContents();
        }

        public override string getOptionTarget(string lineId, string optionContent) {
            return data[lineId].getOptionTarget(optionContent);
        }
    }

    public class LocalizedLine {
        private readonly LocalizedString lineContent;
        private readonly List<LocalizedString> options = new();

        public LocalizedLine(LocalizedString lineContent, List<LocalizedString> lineOptions) {
            this.lineContent = lineContent;
            this.options = lineOptions;
        }

        public string getLine() {
            return lineContent.GetLocalizedString();
        }

        public List<string> getOptionContents() {
            if (options.Count == 0) return new List<string>();
            return options.Select(option => option.GetLocalizedString().Split("->")[0].Trim()).ToList();
        }

        public string getOptionTarget(string optionContent) {
            if (options.Count == 0) return null;
            foreach (var option in options.Where(option => !option.GetLocalizedString().Contains("->"))) 
                Debug.LogError($"Option missing -> sign, please check your option settings. Error Option: {option.GetLocalizedString()}");
            return (from option in options 
                select option.GetLocalizedString().Split("->") 
                into comps 
                where comps[0].Trim() == optionContent 
                select comps[1]
                ).FirstOrDefault();
        }
    }
    
    [Serializable]
    public class LinePair3 : KiiValuePair<GameObjectBox<LocalizedString>, GameObjectBoxes<LocalizedString>> {
        public LinePair3() : base(0.1f, 0.9f) {
        }
    }

    [Serializable]
    public class LinePair2 : KiiValuePair<string, LinePair3> {
        public LinePair2() : base(0.25f, 0.75f) {
        }
    }
}