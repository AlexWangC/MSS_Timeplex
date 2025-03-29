using Fries;
using Fries.Inspector.EditorEvents;
using UnityEngine;

namespace DialogueSystem {
    public class AutoSavingSystem {
        public static string STATIC = "0";
        public static string COMPLEX_STATIC = "1";
        public static string LOCALIZED = "2";
        
        public static string saveFolder = "Assets/Editor/Dialogue Saves/";
        
        private double lastSaveTime = 0;
        private double savePeriod = 180;
        
        [EditorUpdate]
        public void EditorUpdate() {
            if (!EditorAppUtils.isEditor()) return;
            if (EditorAppUtils.timeSinceStartUp() > lastSaveTime + savePeriod) {
                lastSaveTime = EditorAppUtils.timeSinceStartUp();

                StaticDialogue[] staticDialogues = GameObject.FindObjectsByType<StaticDialogue>(FindObjectsSortMode.None);
                ComplexStaticDialogue[] complexStaticDialogues = GameObject.FindObjectsByType<ComplexStaticDialogue>(FindObjectsSortMode.None);
                LocalizedDialogue[] localizedDialogue = GameObject.FindObjectsByType<LocalizedDialogue>(FindObjectsSortMode.None);
                
                staticDialogues.ForEach(dialogue => {
                    dialogue.save();
                });
                complexStaticDialogues.ForEach(dialogue => {
                    dialogue.save();
                });
                localizedDialogue.ForEach(dialogue => {
                    dialogue.save();
                });
            }
        }
    }
}