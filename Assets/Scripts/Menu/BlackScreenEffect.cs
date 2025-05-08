using System;
using DG.Tweening;
using Fries.TaskPerformer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu {
    public class BlackScreenEffect : MonoBehaviour {
        public Image blackSprite;
        public float duration;
        public string originalSceneName;
        public bool hasStarted = false;
        private void Start() {
            originalSceneName = SceneManager.GetActiveScene().name;
            gameObject.transform.parent.gameObject.transform.parent = null;
            DontDestroyOnLoad(gameObject.transform.parent.gameObject);
        }

        public void turnBlack(string sceneName) {
            var color = blackSprite.color;
            color.a = 1;
            blackSprite.DOColor(color, duration).OnComplete(() => {
                hasStarted = true;
                sceneName = sceneName.Replace("\u00a6", "/");
                SceneManager.LoadScene(sceneName);
            });
        }

        private void turnTransparent() {
            var color = blackSprite.color;
            color.a = 0;
            blackSprite.DOColor(color, duration).OnComplete(() => {
                Destroy(gameObject);
            });
        }

        private void Update() {
            if (!hasStarted) return;
            TaskPerformer.inst().scheduleTask((Action)(() => {
                if (SceneManager.GetActiveScene().name != originalSceneName)
                    turnTransparent();
            }), 1);
        }
    }
}