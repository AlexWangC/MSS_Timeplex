using System.IO;
using System.Linq;
# if UNITY_EDITOR
using UnityEditor;
# endif
using UnityEngine.SceneManagement;

namespace Menu {
    public static class SceneScreenshotCamera {
        # if UNITY_EDITOR
        [InitializeOnLoadMethod]
        public static void checkScreenshots() {
            string originalSceneName = SceneManager.GetActiveScene().name;
            string[] scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => Path.GetFileNameWithoutExtension(s.path))
                .ToArray();
            
        }
# endif
    }
}