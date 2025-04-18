using System;
using Fries;
using Fries.Inspector.ComponentWrapper;
using Fries.Inspector.ValueWrapper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu {
    public class PauseScreenConfig : MonoBehaviour {
        [Header("Background")]
        public ComponentWrapper backgroundTransform;
        public ComponentWrapper backgroundImage;
        
        [Header("Logo")]
        public ComponentWrapper logoTransform;
        public ComponentWrapper logoImage;
        
        [Header("Buttons")]
        public ComponentWrapper continueTransform; 
        public ComponentWrapper continueImage;
        public ComponentWrapper continueText;
        public ComponentWrapper continueButton;
        public ComponentWrapper chooseChapterTransform; 
        public ComponentWrapper chooseChapterImage;
        public ComponentWrapper chooseChapterText;
        public ComponentWrapper chooseChapterButton;
        public ComponentWrapper settingTransform;
        public ComponentWrapper settingImage;
        public ComponentWrapper settingText;
        public ComponentWrapper settingButton;
        
        [Header("Margins")]
        public IntWrapper leftMargin;
        public FloatWrapper topMargin;
        public FloatWrapper logoContinueMargin;
        public FloatWrapper continueChooseMargin;
        public FloatWrapper chooseChapterSettingMargin;
        public FloatWrapper bottomMargin;
        
        private void Reset() {
            VerticalLayoutGroup vlg = transform.findAll("Verticle Layout")[0].GetComponent<VerticalLayoutGroup>();
            leftMargin = new IntWrapper(() => vlg.padding.left) {
                label = "Left Margin",
                setter = value => {
                    vlg.padding.left = value;
                }
            };
            
            RectTransform rt = (RectTransform)transform.findAll("Spacer 0")[0];
            topMargin = new FloatWrapper(() => rt.sizeDelta.y) {
                label = "Top Margin",
                setter = value => {
                    rt.sizeDelta = rt.sizeDelta.x_(value);
                }
            };
            
            RectTransform rt1 = (RectTransform)transform.findAll("Spacer 1")[0];
            logoContinueMargin = new FloatWrapper(() => rt1.sizeDelta.y) {
                label = "Logo Continue Margin",
                setter = value => {
                    rt1.sizeDelta = rt1.sizeDelta.x_(value);
                }
            };
            
            RectTransform rt2 = (RectTransform)transform.findAll("Spacer 2")[0];
            continueChooseMargin = new FloatWrapper(() => rt2.sizeDelta.y) {
                label = "Continue Choose Margin",
                setter = value => {
                    rt2.sizeDelta = rt2.sizeDelta.x_(value);
                }
            };

            RectTransform rt3 = (RectTransform)transform.findAll("Spacer 3")[0];
            chooseChapterSettingMargin = new FloatWrapper(() => rt3.sizeDelta.y) {
                label = "Choose Chapter Setting Margin",
                setter = value => {
                    rt3.sizeDelta = rt3.sizeDelta.x_(value);
                }
            };
            
            RectTransform rt4 = (RectTransform)transform.findAll("Spacer 4")[0];
            bottomMargin = new FloatWrapper(() => rt4.sizeDelta.y) {
                label = "Bottom Margin",
                setter = value => {
                    rt4.sizeDelta = rt4.sizeDelta.x_(value);
                }
            };
        }

        public void OnValidate() {
            if (leftMargin.setter == null)
                Reset();
        }
    }
}