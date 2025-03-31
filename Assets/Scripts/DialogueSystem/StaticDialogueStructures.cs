using System;
using System.Collections.Generic;
using System.Linq;
using Fries;
using Fries.Inspector;
using Fries.Inspector.GameObjectBoxField;
using Newtonsoft.Json;
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

                GameObjectBox<StringSso> ssso = pair.value.value.value;
                DialogueSystem.processCmds(ssso, name, lineId);

                string lineContent = pair.value.key;
                string lineOptionsRaw = pair.value.value.key;
                string lineEndCommand = pair.value.value.value.sysObj?.get<string>();
                string[] lineOptions = lineOptionsRaw.Split(" | ");
                if (lineOptionsRaw.Trim() == "") lineOptions = Array.Empty<string>();
                
                if (data.ContainsKey(lineId)) 
                    Debug.LogWarning($"{lineId} is already present in the line set! Please make sure there is no duplicate names!");
                data[lineId] = new StaticLine(lineContent, lineEndCommand, lineOptions);
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

        public override string toString() {
            var kvpList = data.ToList();
            string json = JsonConvert.SerializeObject(kvpList, Formatting.Indented);
            return $"{name} #Dialogue Name \n\n"+json;
        }
        
        public static StaticDialogueData load(string raw) {
            string[] comps = raw.Split(" #Dialogue Name \n\n");
            var deserializedList = JsonConvert.DeserializeObject<List<KeyValuePair<string, StaticLine>>>(comps[1]);
            Dictionary<string, StaticLine> deserializedDict = deserializedList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var sdd = new StaticDialogueData {
                name = comps[0],
                data = deserializedDict
            };

            sdd.lines = new List<LinePair>();
            foreach (var keyValuePair in sdd.data) {
                string lineId = keyValuePair.Key;
                string content = keyValuePair.Value.lineContent;
                string[] optionsArr = new string[keyValuePair.Value.options.Count];
                keyValuePair.Value.options.ForEach(((i, pair) => {
                    string option = $"{pair.Key} -> {pair.Value}";
                    optionsArr[i] = option;
                }));
                string options = string.Join(" | ", optionsArr);
                string lineEndCommand = keyValuePair.Value.lineEndCommand;
                var cmdSso = new StringSso(lineEndCommand);
                cmdSso.createId();
                
                sdd.lines.Add(new LinePair() {
                    key = lineId, 
                    value = new LinePair1() {
                        key = content,
                        value = new LinePair1Inner() {
                            key = options,
                            value = new GameObjectBox<StringSso>() {
                                sysObj = cmdSso
                            }
                        }
                    }
                });
            }
            
            sdd.data.Clear();
            return sdd;
        }
    }

    public class StaticLine {
        [JsonProperty]
        public string lineContent;
        [JsonProperty] 
        public string lineEndCommand;
        [JsonProperty]
        public Dictionary<string, string> options = new();

        [JsonConstructor]
        public StaticLine(string lineContent, string lineEndCommand, string[] lineOptions) {
            this.lineContent = lineContent;
            this.lineEndCommand = lineEndCommand;
            foreach (var lineOption in lineOptions.Nullable()) {
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
    public class LinePair1Inner : KiiValuePair<string, GameObjectBox<StringSso>> {
        public LinePair1Inner() : base(0.85f, 0.15f) {
        }
    }
    
    [Serializable]
    public class LinePair1 : KiiValuePair<string, LinePair1Inner> {
        public LinePair1() : base(0.5f, 0.5f) {
        }
    }

    [Serializable]
    public class LinePair : KiiValuePair<string, LinePair1> {
        public LinePair() : base(0.25f, 0.75f) {
        }
    }

}