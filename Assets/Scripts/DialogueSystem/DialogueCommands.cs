using System;
using Fries;
using UnityEngine;

namespace DialogueSystem {
    public class DialogueCommandData {
        public string[] commandArgs;
        public GameObject gameObject;
        public DialogueDisplayer dialogueDisplayer;
        public string dialogueId;
        public string oldLineId;
        public string newLineId;
    }
    
    public class DialogueCommands : MonoBehaviour {

        private static DialogueCommands dc;
        private static DialogueCommands inst => dc;

        public static void runCmd(string commandName, DialogueCommandData data) {
            inst.cmdMan.runCommand(commandName, data);
        }
        
        private CommandManager<DialogueCommandData> cmdMan;

        private void Start() {
            cmdMan = new("dialogue");
            dc = this;
        }

        [Command("dialogue", "destroy")]
        public static void destroy(DialogueCommandData data) {
            if (data.commandArgs == null || data.commandArgs.Length == 0) {
                Debug.LogWarning("Command 'destroy' must has argument indicating what you want to destroy" +
                                 "Such as: 'destroy comp'" +
                                 "'destroy gobj'" +
                                 "'destroy comp gobj'!");
                return;
            }

            foreach (var arg in data.commandArgs.Nullable()) {
                string argProcessed = arg.ToLower().Trim();
                if (argProcessed == "comp") {
                    data.dialogueDisplayer.Close();
                    DestroyImmediate(data.dialogueDisplayer);
                }
                else if (argProcessed == "gobj") {
                    data.dialogueDisplayer.Close();
                    DestroyImmediate(data.gameObject);
                }
            }
        }
    }
}