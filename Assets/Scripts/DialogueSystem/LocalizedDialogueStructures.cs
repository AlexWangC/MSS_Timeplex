using System;
using System.Collections.Generic;
using System.Linq;
using Fries.Inspector;
using Fries.Inspector.GameObjectBoxField;
using Newtonsoft.Json;
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
                    Debug.LogWarning($"Unassigned localized string presents in line content of {lineId}!");
                    return;
                }
                
                GameObjectBox<StringSso> ssso = pair.value.value.value;
                DialogueSystem.processCmds(ssso, name, lineId);

                GameObjectBoxes<LocalizedString> lineOptionsRaw = pair.value.value.key;
                List<LocalizedString> lineOptions = 
                    lineOptionsRaw.list.Select(optionRaw => {
                        if (optionRaw.sysObj == null || optionRaw.sysObj.get<LocalizedString>() == null) 
                            Debug.LogError($"Unassigned localized string presents in line options of {lineId}!");
                        return optionRaw.sysObj.get<LocalizedString>();
                    }).ToList();

                if (data.ContainsKey(lineId)) 
                    Debug.LogWarning($"{lineId} is already present in the line set! Please make sure there is no duplicate names!");
                data[lineId] = new LocalizedLine(lineContent.sysObj.get<LocalizedString>(), lineOptions, ssso.sysObj?.get<string>());
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
        
        public static LocalizedDialogueData load(string raw) {
            string[] comps = raw.Split(" #Dialogue Name \n\n");
            var deserializedList = JsonConvert.DeserializeObject<List<KeyValuePair<string, LocalizedLine>>>(comps[1]);
            Dictionary<string, LocalizedLine> deserializedDict = deserializedList.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var ldd = new LocalizedDialogueData {
                name = comps[0],
                data = deserializedDict
            };

            ldd.lines = new List<LinePair2>();
            foreach (var keyValuePair in ldd.data) {
                string lineId = keyValuePair.Key;
                LocalizedString content = keyValuePair.Value.lineContent.localizedString();
                List<LocalizedString> options = keyValuePair.Value.options.Select(sls => sls.localizedString()).ToList();
                List<GameObjectBox<LocalizedString>> optionBoxes = options.Select(ls => {
                    GameObjectBox<LocalizedString> bls = new GameObjectBox<LocalizedString> {
                        sysObj = new LocalizedStringKey(ls.TableReference.TableCollectionName,
                            ls.TableEntryReference.Key)
                    };
                    bls.sysObj.createId();
                    bls.createId();
                    return bls;
                }).ToList();
                string cmd = keyValuePair.Value.lineEndCommand;
                var cmdSso = new StringSso(cmd);
                cmdSso.createId();
                var contentSso = new LocalizedStringKey(content.TableReference.TableCollectionName,
                    content.TableEntryReference.Key);
                contentSso.createId();
                
                ldd.lines.Add(new LinePair2 {
                    key = lineId,
                    value = new LinePair3() {
                        key = new GameObjectBox<LocalizedString>() {
                            sysObj = contentSso
                        },
                        value = new LinePair3Inner() {
                            key = new GameObjectBoxes<LocalizedString>() {
                                list = optionBoxes
                            },
                            value = new GameObjectBox<StringSso>() {
                                sysObj = cmdSso
                            }
                        }
                    }
                });
            }
            
            ldd.data.Clear();
            return ldd;
        }
    }

    public class LocalizedLine {
        [JsonProperty]
        public LocalizedStringId lineContent;
        [JsonProperty]
        public List<LocalizedStringId> options;
        [JsonProperty] 
        public string lineEndCommand;

        [JsonConstructor]
        public LocalizedLine(LocalizedStringId lsi, List<LocalizedStringId> options, string lineEndCommand) {
            this.lineContent = lsi;
            this.options = options;
            this.lineEndCommand = lineEndCommand;
        }

        public LocalizedLine(LocalizedString lineContent, List<LocalizedString> lineOptions, string lineEndCommand) {
            this.lineContent = LocalizedStringId.construct(lineContent);
            this.lineEndCommand = lineEndCommand;
            List<LocalizedStringId> slss = new List<LocalizedStringId>();
            lineOptions.ForEach(item => { slss.Add( LocalizedStringId.construct(item)); });
            this.options = slss;
        }

        public string getLine() {
            return lineContent.localizedString().GetLocalizedString();
        }

        public List<string> getOptionContents() {
            if (options.Count == 0) return new List<string>();
            return options.Select(option => option.localizedString().GetLocalizedString().Split("->")[0].Trim()).ToList();
        }

        public string getOptionTarget(string optionContent) {
            if (options.Count == 0) return null;
            foreach (var option in options.Where(option => !option.localizedString().GetLocalizedString().Contains("->"))) 
                Debug.LogError($"Option missing -> sign, please check your option settings. Error Option: {option.localizedString().GetLocalizedString()}");
            return (from option in options 
                select option.localizedString().GetLocalizedString().Split("->") 
                into comps 
                where comps[0].Trim() == optionContent 
                select comps[1]
                ).FirstOrDefault();
        }
    }
    
    [Serializable]
    public class LinePair3Inner : KiiValuePair<GameObjectBoxes<LocalizedString>, GameObjectBox<StringSso>> {
        public LinePair3Inner() : base(0.9f, 0.1f) {
        }
    }
    
    [Serializable]
    public class LinePair3 : KiiValuePair<GameObjectBox<LocalizedString>, LinePair3Inner> {
        public LinePair3() : base(0.1f, 0.9f) {
        }
    }
    

    [Serializable]
    public class LinePair2 : KiiValuePair<string, LinePair3> {
        public LinePair2() : base(0.25f, 0.75f) {
        }
    }
}