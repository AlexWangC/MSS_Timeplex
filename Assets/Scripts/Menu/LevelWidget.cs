using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Menu;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 单个关卡选择按钮的组件
/// </summary>
public class LevelWidget : MonoBehaviour {

    private BlackScreenEffect screenEffect;
    public Image grayMask;
    public Image mainImage;
    public Image thumbnail;
    public TMP_Text text;

    [SerializeField]
    private float clickScaleSize = 0.9f;

    [SerializeField]
    private float clickScaleDuration = 0.1f;

    // 当前是否为高亮状态
    private bool _isHighlighted = false;

    private string sceneName;

    /// <summary>
    /// 设置高亮状态
    /// </summary>
    /// <param name="highlighted">是否高亮</param>
    public void SetHighlighted(bool highlighted) {
        _isHighlighted = highlighted;

        if (grayMask != null) {
            grayMask.gameObject.SetActive(!highlighted);
        }
    }

    public void init(string levelName, BlackScreenEffect screenEffect) {
        Sprite sp = Resources.Load<Sprite>($"LevelThumbnails/{levelName}");
        this.screenEffect = screenEffect;
        thumbnail.sprite = sp;
        text.text = levelName;
        sceneName = levelName;
    }
    
    private void playClickAnimation() {
        // 使用DOTween创建点击时的缩放动画
        transform.DOScale(transform.localScale * clickScaleSize, clickScaleDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                // 动画完成后恢复原始大小
                transform.DOScale(transform.localScale / clickScaleSize, clickScaleDuration)
                    .SetEase(Ease.OutQuad);
            });
    }

    public void onClick() {
        if (!_isHighlighted) return;
        playClickAnimation();
        screenEffect.turnBlack(sceneName);
    }
}
