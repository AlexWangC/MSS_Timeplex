using System;
using System.IO;
using Fries;
using Fries.Inspector.EditorEvents;
using NaughtyAttributes;
using UnityEngine;

namespace DialogueSystem {
    public class DialogueImporter : MonoBehaviour {
        [Button]
        public void selectFile() {
            string savePath = AutoSavingSystem.saveFolder;
            savePath = Path.Combine(Application.dataPath, savePath.Substring(7));
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            string path = EditorAppUtils.openFilePanel("Select Dialogue Save", savePath, "txt");
            if (string.IsNullOrEmpty(path)) return;
            
            string fileContent = File.ReadAllText(path);
            (string type, string time, string hierachyPath) info = parseFileName(path);
            GameObject go = UnityExts.mkdirs(info.hierachyPath, "=SLASH=");
            if (info.type == AutoSavingSystem.STATIC) {
                StaticDialogue sd = go.AddComponent<StaticDialogue>();
                sd.load(fileContent);
            } else if (info.type == AutoSavingSystem.COMPLEX_STATIC) {
                ComplexStaticDialogue csd = go.AddComponent<ComplexStaticDialogue>();
                csd.load(fileContent);
            } else if (info.type == AutoSavingSystem.LOCALIZED) {
                LocalizedDialogue ld = go.AddComponent<LocalizedDialogue>();
                ld.load(fileContent);
            }
        }
        
        private static (string type, string time, string hierachyPath) parseFileName(string filePath) {
            // 从完整路径中提取文件名（包含扩展名）
            string fileName = Path.GetFileName(filePath);

            // 检查文件名长度是否符合要求
            if (fileName.Length < 17) 
                throw new ArgumentException("File name is incorrect, please make sure you are selecting a dialogue save txt file");

            // 提取文件名各部分：
            string type = fileName.Substring(0, 1);
            string time = fileName.Substring(1, 16).Trim();
            string hierarchyPath = fileName.Substring(17).Trim();
            hierarchyPath = Path.GetFileNameWithoutExtension(hierarchyPath);
            return (type, time, hierarchyPath);
        }
    }
}