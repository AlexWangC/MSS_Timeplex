using System;
using System.Collections.Generic;
using System.IO;
using Fries;
# if UNITY_EDITOR
using UnityEditor;
# endif
using UnityEngine;
using UnityEngine.Localization;

namespace DialogueSystem {
    public class LocalizedDialogue : MonoBehaviour {
        public List<LocalizedDialogueData> dialogues;

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
        
        public void save() {
            try {
                string monoBehaviourPath = gameObject.getPath();
                monoBehaviourPath = monoBehaviourPath.Replace("/", "=SLASH=");
                string[] dataRows = new string[dialogues.Count];

                dialogues.ForEach((i, data) => {
                    if (DialogueSystem.dialogueData.TryGetValue(data.name, out var value))
                        Debug.LogWarning(
                            $"Global Dialogue Data already has dialogue with name {data.name}. The old one will be discarded.");

                    data.init();
                    string dataStr = data.toString();
                    dataRows[i] = dataStr;
                });

                string dataString = string.Join("\n=============== New Item ===============\n", dataRows);
                string savePath = AutoSavingSystem.saveFolder;
                savePath = Path.Combine(Application.dataPath, savePath.Substring(7));
                if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

                string fileName = $"{AutoSavingSystem.LOCALIZED} {DateTime.Now:yyyyMMddHHmmss} " + monoBehaviourPath + ".txt";
                string filePath = Path.Combine(savePath, fileName);
                File.WriteAllText(filePath, dataString);
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }
        
        public void load(string raw) {
            try {
                dialogues = new List<LocalizedDialogueData>();
                string[] items = raw.Split("\n=============== New Item ===============\n");
                foreach (var item in items) {
                    LocalizedDialogueData ldd = LocalizedDialogueData.load(item);
                    dialogues.Add(ldd);
                }
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }
    }
}