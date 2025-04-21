using UnityEngine;
using UnityEditor;

/// <summary>
/// 编辑器工具，用于测试LevelSelectorController功能
/// </summary>
[CustomEditor(typeof(LevelSelectorController))]
public class LevelSelectorControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 绘制默认Inspector
        DrawDefaultInspector();
        
        // 获取目标组件
        LevelSelectorController selector = (LevelSelectorController)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("测试工具", EditorStyles.boldLabel);
        
        // 添加测试按钮
        if (GUILayout.Button("重新初始化关卡按钮"))
        {
            // 调用私有方法需要通过反射
            System.Reflection.MethodInfo method = typeof(LevelSelectorController).GetMethod(
                "InitializeLevelWidgets", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (method != null)
            {
                method.Invoke(selector, null);
                
                // 重新排列
                method = typeof(LevelSelectorController).GetMethod(
                    "ArrangeLevelWidgets", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (method != null)
                {
                    method.Invoke(selector, new object[] { 0f });
                }
                
                EditorUtility.SetDirty(selector);
            }
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "提示：\n" +
            "1. 确保已安装DOTween插件\n" +
            "2. 确保LevelWidgetPrefab已设置并包含LevelWidget组件\n" +
            "3. 运行时使用鼠标滚轮滚动切换关卡\n" +
            "4. 点击中间高亮的关卡触发选择事件", 
            MessageType.Info);
    }
}
