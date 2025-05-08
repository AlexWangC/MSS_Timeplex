using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Fries;
using Fries.Data;
using UnityEngine.Rendering;

/// <summary>
/// 滚动式关卡选择界面控制器
/// </summary>
public class LevelSelectorController : MonoBehaviour {
    public static StringList<string> levels;
    public static void addLevel(string levelName) {
        levels.Add(levelName);
        PlayerPrefs.SetString("LevelSelectorController.Levels", levels.export());
    }
    
    [Header("Level Configuration")] [Tooltip("Level Button Prefab")] [SerializeField]
    private GameObject levelWidgetPrefab;

    [Tooltip("Level Count")] [SerializeField] private int levelCount = 5;

    [Header("Layout Configuration")] [Tooltip("Radius")] [SerializeField]
    private float radius = 500f;

    [Tooltip("Arc Angle Range")] [SerializeField]
    private float arcAngleRange = 60f;
    
    [Tooltip("Slope")] [SerializeField] [Range(0,1)]
    private float slope = 0.5f;
    
    [Tooltip("Height Offset")] [SerializeField]
    private float heightOffset = 0f;

    [Header("Size Configuration")] 
    [Tooltip("Center Widget Scale")] [SerializeField]
    private float centerScale = 1.0f;

    [Tooltip("Edge Widget Scale")] [SerializeField]
    private float edgeScale = 0.6f;

    [Tooltip("Scale Decay Rate")] [SerializeField] private float scaleDecayRate = 0.85f;

    [Tooltip("Edge Grey Level")] [SerializeField] [Range(0, 1)]
    private float edgeGreyLevel = 0.8f;

    [Tooltip("Grey Decay Rate")] [SerializeField] [Range(0, 1)]
    private float greyDecayRate = 0.85f;
    
    [Tooltip("Edge Alpha Level")] [SerializeField] [Range(0, 1)]
    private float edgeAlphaLevel = 0.8f;

    [Tooltip("Alpha Decay Rate")] [SerializeField] [Range(0, 1)]
    private float alphaDecayRate = 0.85f;

    [Tooltip("Alpha Shift")] [SerializeField]
    private float alphaShift = 0.85f;
    
    [Header("Animation Configuration")] [Tooltip("Scroll Animation Duration")] [SerializeField]
    private float scrollAnimDuration = 0.3f;

    [Tooltip("Animation Ease")] [SerializeField] private Ease scrollEase = Ease.OutQuad;
    [Tooltip("Scroll Sensitivity")] [SerializeField] private float scrollSensitivity = 0.1f;

    [Header("Container Configuration")] [Tooltip("Parent Container")] [SerializeField]
    private RectTransform widgetsContainer;

    // 关卡按钮列表
    private List<LevelWidget> levelWidgets = new List<LevelWidget>();

    // 当前选中的关卡索引
    private int currentIndex = 0;

    // 是否正在动画中
    private bool isAnimating = false;

    private void Start() {
        // 如果没有指定容器，使用当前对象
        if (widgetsContainer == null) {
            widgetsContainer = GetComponent<RectTransform>();
        }

        // 初始化关卡按钮
        InitializeLevelWidgets();

        // 初始布局
        ArrangeLevelWidgets(0);

        levels = new StringList<string>(
            PlayerPrefs.GetString("LevelSelectorController.Levels"),
            s => s, s => s
        );
    }

    private void Update() {
        // 检测鼠标滚轮输入
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // 如果有滚动输入且不在动画中
        if (!Mathf.Approximately(scrollInput, 0) && !isAnimating) {
            // 计算滚动方向
            int direction = scrollInput > 0 ? -1 : 1;

            // 计算目标索引
            int targetIndex = Mathf.Clamp(currentIndex + direction, 0, levelCount - 1);

            // 如果目标索引与当前索引不同，执行滚动
            if (targetIndex != currentIndex) {
                ScrollToIndex(targetIndex);
            }
        }
    }

    /// <summary>
    /// 初始化关卡按钮
    /// </summary>
    private void InitializeLevelWidgets() {
        // 清空现有按钮
        foreach (var widget in levelWidgets) {
            if (widget != null) {
                Destroy(widget.gameObject);
            }
        }

        levelWidgets.Clear();

        // 创建新按钮
        for (int i = 0; i < levels.Count; i++) {
            string levelName = levels[i];
            GameObject widgetObj = Instantiate(levelWidgetPrefab, widgetsContainer);
            LevelWidget widget = widgetObj.GetComponent<LevelWidget>();
            widget.init(levelName);

            if (widget == null) {
                widget = widgetObj.AddComponent<LevelWidget>();
            }

            widgetObj.name = levelName;
            levelWidgets.Add(widget);
        }
    }
    
    /// <summary>
    /// 按圆弧排列关卡按钮
    /// </summary>
    /// <param name="offset">中心偏移量（0-1之间）</param>
    private void ArrangeLevelWidgets(float offset) {
        // 如果没有按钮，直接返回
        if (levelWidgets.Count == 0) return;

        // 计算中心索引（浮点数）
        float centerIndexFloat = Mathf.Lerp(0, levelWidgets.Count - 1, offset);
        int centerIndex = Mathf.RoundToInt(centerIndexFloat);

        // 更新当前索引
        currentIndex = centerIndex;

        // 计算角度步长
        float angleStep = arcAngleRange / (levelWidgets.Count - 1);
        if (levelWidgets.Count == 1) angleStep = 0;

        SortedList<float, LevelWidget> order = new();
        
        // 遍历所有按钮进行排列
        for (int i = 0; i < levelWidgets.Count; i++) {
            LevelWidget widget = levelWidgets[i];

            // 计算当前按钮与中心的距离
            float distanceFromCenter = Mathf.Abs(i - centerIndexFloat);
            float temp = 10000 - distanceFromCenter;
            while (order.ContainsKey(temp)) temp += 0.001f;
            order[temp] = widget;

            // 计算角度
            float angle = (i - centerIndexFloat) * angleStep;
            angle *= Mathf.Deg2Rad; // 转换为弧度

            // 计算位置
            float x = Mathf.Sin(angle) * radius;
            float y = (1 - Mathf.Cos(angle)) * radius * slope + heightOffset; // 使用半高以使圆弧更平缓

            // 设置位置
            widget.transform.localPosition = new Vector3(x, y, 0);

            // 计算缩放
            float scale;
            float greyLevel;
            float alphaLevel;
            if (distanceFromCenter <= 0.01f) // 中心按钮
            {
                scale = centerScale;
                greyLevel = 0f;
                alphaLevel = 1f;
            }
            else // 边缘按钮
            {
                // 使用指数衰减计算缩放
                scale = Mathf.Lerp(centerScale, edgeScale, 1 - Mathf.Pow(scaleDecayRate, distanceFromCenter));
                greyLevel = Mathf.Lerp(0, edgeGreyLevel, 1 - Mathf.Pow(greyDecayRate, distanceFromCenter));
                alphaLevel = Mathf.Lerp(0, edgeAlphaLevel, 1 - Mathf.Pow(alphaDecayRate, (distanceFromCenter + alphaShift)));
                alphaLevel = 1 - alphaLevel;
            }

            // 设置缩放
            widget.transform.localScale = new Vector3(scale, scale, scale);
            var color = widget.grayMask.color;
            widget.grayMask.color = new Color(color.r, color.g, color.b, greyLevel * alphaLevel);
            color = widget.mainImage.color;
            widget.mainImage.color = new Color(color.r, color.g, color.b, alphaLevel);
            color = widget.thumbnail.color;
            widget.thumbnail.color = new Color(color.r, color.g, color.b, alphaLevel);
            color = widget.text.color;
            widget.text.color = new Color(color.r, color.g, color.b, alphaLevel);

            // 设置高亮状态
            bool isHighlighted = (i == centerIndex);
            widget.SetHighlighted(isHighlighted);
        }

        int i1 = 0;
        foreach (var widget in order.Values) {
            widget.transform.SetSiblingIndex(i1);
            i1++;
        }
    }

    private float currentOffset = 0;
    /// <summary>
    /// 滚动到指定索引
    /// </summary>
    /// <param name="targetIndex">目标索引</param>
    private void ScrollToIndex(int targetIndex) {
        // 如果正在动画中，直接返回
        if (isAnimating) return;

        // 标记为动画中
        isAnimating = true;

        // 计算目标偏移量
        float targetOffset = (float)targetIndex / (levelWidgets.Count - 1);
        if (levelWidgets.Count <= 1) targetOffset = 0;

        DOTween.To(() => currentOffset, x => {
            currentOffset = x;
            ArrangeLevelWidgets(currentOffset);
        }, targetOffset, scrollAnimDuration).SetEase(scrollEase)
            .OnComplete(() => isAnimating = false);
    }

    /// <summary>
    /// 在编辑器中验证组件
    /// </summary>
    private void OnValidate() {
        // 确保关卡数量至少为1
        levelCount = Mathf.Max(1, levelCount);

        // 确保缩放值在合理范围内
        centerScale = Mathf.Max(0.1f, centerScale);
        edgeScale = Mathf.Clamp(edgeScale, 0.1f, centerScale);

        // 确保衰减率在合理范围内
        scaleDecayRate = Mathf.Clamp01(scaleDecayRate);

        ArrangeLevelWidgets(0);
    }
}