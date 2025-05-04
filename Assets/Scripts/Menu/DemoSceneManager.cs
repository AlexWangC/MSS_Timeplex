using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 演示场景管理器
/// </summary>
public class DemoSceneManager : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private LevelSelectorController levelSelector;
    [SerializeField] private Text selectedLevelText;
    
    private void Start()
    {
        // 确保引用有效
        if (levelSelector == null)
        {
            levelSelector = FindObjectOfType<LevelSelectorController>();
        }
        
        // 添加关卡选择事件监听
        if (levelSelector != null)
        {
            // 为每个关卡按钮添加事件
            for (int i = 0; i < levelSelector.transform.childCount; i++)
            {
                Transform child = levelSelector.transform.GetChild(i);
                LevelWidget widget = child.GetComponent<LevelWidget>();
                
                if (widget != null)
                {
                    
                }
            }
        }
    }
    
    /// <summary>
    /// 关卡选择回调
    /// </summary>
    /// <param name="levelIndex">选中的关卡索引</param>
    private void OnLevelSelected(int levelIndex)
    {
        Debug.Log($"选择了关卡 {levelIndex + 1}");
        
        // 更新UI显示
        if (selectedLevelText != null)
        {
            selectedLevelText.text = $"已选择关卡: {levelIndex + 1}";
        }
    }
}
