using System;
using Fries;
using Fries.Inspector.ComponentWrapper;
using Fries.Inspector.ValueWrapper;
using Fries.TaskPerformer;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Menu {
    public class StartScreenConfig : MonoBehaviour {
        [Header("Background")]
        public ComponentWrapper backgroundTransform;
        public ComponentWrapper backgroundImage;
        
        [Header("Logo")]
        public ComponentWrapper logoTransform;
        public ComponentWrapper logoImage;
        
        [Header("Buttons")]
        public ComponentWrapper newGameTransform; 
        public ComponentWrapper newGameImage;
        public ComponentWrapper newGameText;
        public ComponentWrapper newGameButton;
        public ComponentWrapper settingTransform;
        public ComponentWrapper settingImage;
        public ComponentWrapper settingText;
        public ComponentWrapper settingButton;
        
        [Header("Margins")]
        public FloatWrapper topMargin;
        public FloatWrapper logoNewGameSpacer;
        public FloatWrapper newGameSettingSpacer;
        public FloatWrapper bottomMargin;
        public UndoPropertyModification[] marginUndoRedo(UndoPropertyModification[] mods) {
            Debug.Log(1);
            return mods;
        }
        
        private void Reset() {
            RectTransform rt = (RectTransform)transform.findAll("Spacer 0")[0];
            topMargin = new FloatWrapper(() => rt.sizeDelta.y) {
                label = "Top Margin",
                setter = value => {
                    rt.sizeDelta = rt.sizeDelta.x_(value);
                }
            };
            
            RectTransform rt1 = (RectTransform)transform.findAll("Spacer 1")[0];
            logoNewGameSpacer = new FloatWrapper(() => rt1.sizeDelta.y) {
                label = "Logo New Game Margin",
                setter = value => {
                    rt1.sizeDelta = rt1.sizeDelta.x_(value);
                }
            };
            
            RectTransform rt2 = (RectTransform)transform.findAll("Spacer 2")[0];
            newGameSettingSpacer = new FloatWrapper(() => rt2.sizeDelta.y) {
                label = "New Game Setting Margin",
                setter = value => {
                    rt2.sizeDelta = rt2.sizeDelta.x_(value);
                }
            };
            
            RectTransform rt3 = (RectTransform)transform.findAll("Spacer 3")[0];
            bottomMargin = new FloatWrapper(() => rt3.sizeDelta.y) {
                label = "Bottom Margin",
                setter = value => {
                    rt3.sizeDelta = rt3.sizeDelta.x_(value);
                }
            };
        }

        public void OnValidate() {
            if (topMargin.setter == null)
                Reset();
        }
    }
}