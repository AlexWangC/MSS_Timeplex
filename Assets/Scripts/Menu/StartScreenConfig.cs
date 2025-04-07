using System;
using Fries.Inspector.ComponentWrapper;
using TMPro;
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
        public ComponentWrapper topMarginTransform;
        public ComponentWrapper logoNewGameDivider;
        public ComponentWrapper newGameSettingDivider;
        public ComponentWrapper bottomDivider;
    }
}