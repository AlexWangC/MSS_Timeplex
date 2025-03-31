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
    public class ComplexStaticDialogueData : DialogueData {
        [FieldAnchor]
        public List<LinePairOut> lines;
        private Dictionary<string, ComplexStaticLine> data;

        public override void init() {
            data = new();
            lines.ForEach(pair => {
                string lineId = pair.key;
                GameObjectBoxes<StringSso> lineContentRaw = pair.value.key;
                GameObjectBoxes<StringSso> lineOptionsRaw = pair.value.value.key;
                
                GameObjectBox<StringSso> ssso = pair.value.value.value;
                DialogueSystem.processCmds(ssso, name, lineId);

                List<string> lineContents = new();
                    foreach (var gob in lineContentRaw.list) {
                        StringSso str = gob.sysObj as StringSso;
                        if (str == null) continue;
                        lineContents.Add(str.get<string>());
                    }
                    
                List<string> lineOptionsProcessedRaw = new();
                    foreach (var gob in lineOptionsRaw.list) {
                        StringSso str = gob.sysObj as StringSso;
                        if (str == null) continue;
                        lineOptionsProcessedRaw.Add(str.get<string>());
                    }

                List<List<string>> lineOptions = new();
                foreach (var lineOptionsRawSingle in lineOptionsProcessedRaw) {
                    string[] lineOptionsArr = lineOptionsRawSingle.Split(" | ");
                    if (lineOptionsRawSingle.Trim() == "") lineOptionsArr = Array.Empty<string>();
                    lineOptions.Add(lineOptionsArr.ToList());
                }
                
                if (data.ContainsKey(lineId)) 
                    Debug.LogWarning($"{lineId} is already present in the line set! Please make sure there is no duplicate names!");
                data[lineId] = new ComplexStaticLine(lineContents, ssso.sysObj?.get<string>(), lineOptions);
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
        
        public static ComplexStaticDialogueData load(string raw) {
            string[] comps = raw.Split(" #Dialogue Name \n\n");
            var deserializedList = JsonConvert.DeserializeObject<List<KeyValuePair<string, ComplexStaticLine>>>(comps[1]);
            Dictionary<string, ComplexStaticLine> deserializedDict = deserializedList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var csdd = new ComplexStaticDialogueData() {
                name = comps[0],
                data = deserializedDict
            };

            csdd.lines = new List<LinePairOut>();
            foreach (var keyValuePair in csdd.data) {
                string lineId = keyValuePair.Key;

                List<string> contents = keyValuePair.Value.lineContent;
                List<GameObjectBox<StringSso>> contentSso = contents.Select(str => {
                    var sso = new StringSso(str);
                    sso.createId();
                    var box = new GameObjectBox<StringSso>();
                    box.sysObj = sso;
                    box.createId();
                    return box;
                }).ToList();
                GameObjectBoxes<StringSso> contentBoxes = new GameObjectBoxes<StringSso>() {
                    list = contentSso
                };
                contentBoxes.createId();

                List<Dictionary<string, string>> options = keyValuePair.Value.options;
                List<string> processedOptions = options.Select(dict => {
                    string[] optionsArr = new string[dict.Count];
                    dict.ForEach(((i, pair) => {
                        string option = $"{pair.Key} -> {pair.Value}";
                        optionsArr[i] = option;
                    }));
                    string optionsRaw = string.Join(" | ", optionsArr);
                    return optionsRaw;
                }).ToList();
                List<GameObjectBox<StringSso>> optionSso = processedOptions.Select(raw1 => {
                    var sso = new StringSso(raw1);
                    sso.createId();
                    var box = new GameObjectBox<StringSso>();
                    box.sysObj = sso;
                    box.createId();
                    return box;
                }).ToList();
                GameObjectBoxes<StringSso> optionBoxes = new GameObjectBoxes<StringSso>() {
                    list = optionSso
                };
                optionBoxes.createId();

                string lineEndCommand = keyValuePair.Value.lineEndCommand;
                var cmdSso = new StringSso(lineEndCommand);
                cmdSso.createId();

                csdd.lines.Add(new LinePairOut() {
                    key = lineId,
                    value = new LinePairUnionContentNOption() {
                        key = contentBoxes,
                        value = new LinePairUnionContentNOptionInner() {
                            key = optionBoxes,
                            value = new GameObjectBox<StringSso>() {
                                sysObj = cmdSso
                            }
                        }
                    }
                });
            }

            csdd.data.Clear();
            return csdd;
        }
    }

    public class ComplexStaticLine {
        [JsonProperty]
        public List<string> lineContent;
        [JsonProperty]
        public List<Dictionary<string, string>> options = new();
        [JsonProperty] 
        public string lineEndCommand;

        [JsonConstructor]
        public ComplexStaticLine(List<string> possibleContents, string lineEndCommand, List<List<string>> possibleOptions) {
            this.lineContent = possibleContents;
            this.lineEndCommand = lineEndCommand;
            foreach (var singleOptions in possibleOptions.Nullable()) {
                Dictionary<string, string> singleOptionsProcessed = new();
                foreach (var lineOption in singleOptions.Nullable()) {
                    if (lineOption.Trim() == "") continue;
                    if (!lineOption.Contains("->")) 
                        Debug.LogError($"Option missing -> sign, please check your option settings. Error Option: {lineOption}");
                    string[] comps = lineOption.Split("->");
                    
                    string optionContent = comps[0].Trim();
                    string optionTarget = comps[1].Trim();
                    singleOptionsProcessed[optionContent] = optionTarget;
                }
                options.Add(singleOptionsProcessed);
            }
        }

        private string randomContent;
        private Dictionary<string, string> randomOptions;
        public string getLine() {
            randomContent = lineContent.RandomElement();
            return randomContent;
        }

        public List<string> getOptionContents() {
            randomOptions = options.RandomElement();
            return randomOptions.Keys.ToList();
        }

        public string getOptionTarget(string optionContent) {
            return randomOptions[optionContent];
        }
    }
    
    [Serializable]
    public class LinePairUnionContentNOptionInner : KiiValuePair<GameObjectBoxes<StringSso>, GameObjectBox<StringSso>> {
        public LinePairUnionContentNOptionInner() : base(0.9f, 0.1f) {
        }
    }
    
    [Serializable]
    public class LinePairUnionContentNOption : KiiValuePair<GameObjectBoxes<StringSso>, LinePairUnionContentNOptionInner> {
        public LinePairUnionContentNOption() : base(0.5f, 0.5f) {
        }
    }

    [Serializable]
    public class LinePairOut : KiiValuePair<string, LinePairUnionContentNOption> {
        public LinePairOut() : base(0.25f, 0.75f) {
        }
    }

}