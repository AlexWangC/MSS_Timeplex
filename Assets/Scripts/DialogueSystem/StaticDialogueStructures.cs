using System;
using System.Collections.Generic;
using System.Linq;
using Fries.Inspector;
using Unity.VisualScripting;
using UnityEngine;

namespace DialogueSystem {

    [Serializable]
    public class StaticDialogueData : DialogueData {
        public List<LinePair> lines;
        private Dictionary<string, StaticLine> data;

        public override void init() {
            data = new();
            lines.ForEach(pair => {
                string lineId = pair.key;
                string lineContent = pair.value.key;
                string lineOptionsRaw = pair.value.value;
                string[] lineOptions = lineOptionsRaw.Split(" | ");
                if (lineOptionsRaw.Trim() == "") lineOptions = Array.Empty<string>();
                
                if (data.ContainsKey(lineId)) 
                    Debug.LogWarning($"{lineId} is already present in the line set! Please make sure there is no duplicate names!");
                data[lineId] = new StaticLine(lineContent, lineOptions);
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

    public class StaticLine {
        private readonly string lineContent;
        private readonly Dictionary<string, string> options = new();

        public StaticLine(string lineContent, string[] lineOptions) {
            this.lineContent = lineContent;
            foreach (var lineOption in lineOptions) {
                if (lineOption.Trim() == "") continue;
                if (!lineOption.Contains("->")) 
                    Debug.LogError($"Option missing -> sign, please check your option settings. Error Option: {lineOption}");
                string[] comps = lineOption.Split("->");
                string optionContent = comps[0].Trim();
                string optionTarget = comps[1].Trim();
                options[optionContent] = optionTarget;
            }
        }

        public string getLine() {
            return lineContent;
        }

        public List<string> getOptionContents() {
            return options.Keys.ToList();
        }

        public string getOptionTarget(string optionContent) {
            return options[optionContent];
        }
    }
    
    [Serializable]
    public class LinePair1 : KiiValuePair<string, string> {
        public LinePair1() : base(0.6f, 0.4f) {
        }
    }

    [Serializable]
    public class LinePair : KiiValuePair<string, LinePair1> {
        public LinePair() : base(0.25f, 0.75f) {
        }
    }

}