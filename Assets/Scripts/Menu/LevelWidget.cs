using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

/// <summary>
/// 单个关卡选择按钮的组件
/// </summary>
public class LevelWidget : MonoBehaviour {
    [Header("组件引用")] [Tooltip("灰色遮罩图层，用于非高亮状态")] [SerializeField]
    public Image grayMask;

    [Header("配置")] [Tooltip("点击时的缩放动画大小")] [SerializeField]
    private float clickScaleSize = 0.9f;

    [Tooltip("点击时的缩放动画时长")] [SerializeField]
    private float clickScaleDuration = 0.1f;

    // 当前是否为高亮状态
    private bool _isHighlighted = false;

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

    /// <summary>
    /// 播放点击动画
    /// </summary>
    private void PlayClickAnimation() {
        // 使用DOTween创建点击时的缩放动画
        transform.DOScale(transform.localScale * clickScaleSize, clickScaleDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                // 动画完成后恢复原始大小
                transform.DOScale(transform.localScale / clickScaleSize, clickScaleDuration)
                    .SetEase(Ease.OutQuad);
            });
    }
}
