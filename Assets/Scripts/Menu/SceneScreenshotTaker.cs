using System.Collections;
using System.IO;
using Fries.Inspector.SceneBehaviours;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu {
    public class SceneScreenshotTaker : MonoBehaviour {
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            prevSceneName = SceneManager.GetActiveScene().path;
        }

        private string prevSceneName;
        private void Update() {
            string sceneName = SceneManager.GetActiveScene().path;
            if (sceneName == prevSceneName) return;
            
            prevSceneName = sceneName;
            NotLevelMarker nlm = SceneManager.GetActiveScene().getBehaviour<NotLevelMarker>();
            if (nlm != null) return;

            sceneName = sceneName.Replace('\\', '\u00a6').Replace('/', '\u00a6');
            Sprite screenshot = Resources.Load<Sprite>($"LevelThumbnails/{sceneName}");
            if (!screenshot) StartCoroutine(captureAndSave(sceneName));
        }

        private IEnumerator captureAndSave(string sceneName) {
            yield return new WaitForEndOfFrame();

            int w = Screen.width;
            int h = Screen.height;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            tex.Apply();

            byte[] png = tex.EncodeToPNG();
            Destroy(tex);

            string relPath = "Resources/LevelThumbnails";
            string absDir = Path.Combine(Application.dataPath, relPath);
            if (!Directory.Exists(absDir)) Directory.CreateDirectory(absDir);

            string fileName = sceneName + ".png";
            string absPath = Path.Combine(absDir, fileName);
            File.WriteAllBytes(absPath, png);
            
            LevelSelectorController.addLevel(sceneName);
        }
    }
}