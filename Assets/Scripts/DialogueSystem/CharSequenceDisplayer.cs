using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Fries;
using Fries.Pool;
using TMPro;
using UnityEngine;

namespace DialogueSystem {
    public class CharSequenceDisplayer : MonoBehaviour {
        private static CompPool<TMP_Text> optionBoxes;
        public float fixedXPos;
        public float fixedYStart;
        public float fixedHeight;

        public GameObject optionsRoot;
        public GameObject optionPrefab;
        
        public float speed = 0.2f;
        public TMP_Text mainTextBox;

        private TweenerCore<float, float, FloatOptions> mainTextAnim;
        private List<TweenerCore<float, float, FloatOptions>> optionAnims = new();
        private List<TMP_Text> activeOptions = new(); 

        private void Start() {
            if (optionBoxes == null) optionBoxes = optionPrefab.toPool<TMP_Text>(optionsRoot.transform);
        }

        private void Update() {
            int i = 0;
            activeOptions.ForEach(option => {
                option.transform.position = transform.position + fixedXPos.f__(fixedYStart + i * fixedHeight, 0);
                i++;
            });
        }

        private string mainTextLastContent = "";
        private float playTime = 0;
        public void display(string mainText) {
            float totalTime = speed * mainText.Length;
            mainTextAnim = DOTween.To(() => playTime, x => {
                int charCount = (int)(x / speed);
                string content = mainText.Substring(0, charCount);
                string newContent = "";
                foreach (var t in content) 
                    newContent += t + "\\u200B";
                if (mainTextLastContent == newContent) return;
                mainTextLastContent = newContent;
                mainTextBox.text = newContent;
            }, totalTime, totalTime).OnComplete(() => {
                string content = mainText;
                string newContent = "";
                foreach (var t in content) 
                    newContent += t + "\\u200B";
                if (mainTextLastContent == newContent) return;
                mainTextLastContent = newContent;
                mainTextBox.text = newContent;
            }).SetEase(Ease.Linear);
        }

        private Dictionary<string, string> lastOptionContents = new();
        public void listOptions(DialogueDisplayer dd, string lineId, List<string> options) {
            activeOptions.ForEach(option => optionBoxes.deactivate(option));
            activeOptions.Clear();
            
            optionAnims.ForEach(anim => {
                if (anim != null && anim.IsActive() && anim.IsPlaying())
                    anim?.Kill();
            });
            optionAnims.Clear();
            
            int i = 0;
            foreach (var optionContent in options) {
                TMP_Text text = optionBoxes.activate();
                activeOptions.Add(text);
                Option op = text.GetComponent<Option>();
                op.dd = dd;
                op.optionContent = optionContent;
                op.optionTargetId = dd.GetOptionTarget(lineId, optionContent);

                if (!lastOptionContents.ContainsKey(optionContent))
                    lastOptionContents[optionContent] = "";
                
                text.transform.position = transform.position + fixedXPos.f__(fixedYStart + i * fixedHeight, 0);
                float totalTime = speed * optionContent.Length;
                var anim = DOTween.To(() => playTime, x => {
                    int charCount = (int)(x / speed);
                    string content = optionContent.Substring(0, charCount);
                    string newContent = "";
                    foreach (var t in content) 
                        newContent += t + "\\u200B";
                    if (lastOptionContents[optionContent] == newContent) return;
                    lastOptionContents[optionContent] = newContent;
                    text.text = newContent;
                }, totalTime, totalTime)
                    .OnComplete(() => {
                        string content = optionContent;
                        string newContent = "";
                        foreach (var t in content) 
                            newContent += t + "\\u200B";
                        if (lastOptionContents[optionContent] == newContent) return;
                        lastOptionContents[optionContent] = newContent;
                        text.text = newContent;
                    }).SetEase(Ease.Linear);
                optionAnims.Add(anim);
                i++;
            }
        }

        public void clear() {
            if (mainTextAnim != null && mainTextAnim.IsActive() && mainTextAnim.IsPlaying())
                mainTextAnim?.Kill();
            optionAnims.ForEach(anim => {
                if (anim != null && anim.IsActive() && anim.IsPlaying())
                    anim?.Kill();
            });
            optionAnims.Clear();
            
            mainTextBox.text = "";
            activeOptions.ForEach(option => optionBoxes.deactivate(option));
            activeOptions.Clear();
        }
    }
}