using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LiUIExitButton : MonoBehaviour
{
    [Header("场景设置")]
    [Tooltip("拖拽目标场景文件到这里（推荐方式）")]
    [SerializeField] private UnityEngine.Object targetSceneAsset; // 场景资源文件
    
    [Tooltip("目标场景名称（自动从场景资源获取，或手动输入）")]
    [SerializeField] private string targetSceneName = "Backgroud Scens"; // 目标场景名称
    
    [Tooltip("目标场景在Build Settings中的索引（备用方案）")]
    [SerializeField] private int targetSceneIndex = 0; // 目标场景索引（备用方案）
    
    [Header("调试选项")]
    [SerializeField] private bool enableDebugLogs = true; // 是否启用调试日志
    
    [Header("组件引用")]
    private Button exitButton;
    private MonoBehaviour cachedGameManager;
    private MonoBehaviour cachedUIManager;
    private MonoBehaviour gameOverManager;

    void Start()
    {
        // 获取Button组件
        exitButton = GetComponent<Button>();
        if (exitButton == null)
        {
            Debug.LogError("LiUIExitButton: 未找到Button组件！");
            return;
        }

        // 处理场景资源设置
        ProcessSceneAsset();

        // 缓存管理器引用
        CacheManagerReferences();

        // 添加点击事件监听器
        exitButton.onClick.AddListener(OnExitButtonClick);
        
        Debug.Log("LiUIExitButton: Exit按钮监听器设置成功");
    }

    // 处理场景资源设置
    private void ProcessSceneAsset()
    {
        if (targetSceneAsset != null)
        {
            // 从场景资源中获取场景名称
            #if UNITY_EDITOR
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(targetSceneAsset);
            if (!string.IsNullOrEmpty(assetPath))
            {
                // 提取场景名称（去掉路径和扩展名）
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                targetSceneName = sceneName;
                
                if (enableDebugLogs)
                {
                    Debug.Log($"LiUIExitButton: 从场景资源自动设置场景名称为: {targetSceneName}");
                }
            }
            #else
            // 在运行时，使用资源名称作为场景名称
            targetSceneName = targetSceneAsset.name;
            if (enableDebugLogs)
            {
                Debug.Log($"LiUIExitButton: 运行时设置场景名称为: {targetSceneName}");
            }
            #endif
        }
        else if (enableDebugLogs)
        {
            Debug.Log($"LiUIExitButton: 未设置场景资源，使用默认场景名称: {targetSceneName}");
        }
    }

    // 缓存管理器引用
    private void CacheManagerReferences()
    {
        // 查找GameManager
        GameObject gameManagerObj = GameObject.Find("game manager");
        if (gameManagerObj != null)
        {
            var manager = gameManagerObj.GetComponent<MonoBehaviour>();
            if (manager != null && manager.GetType().Name == "LiGameManager")
            {
                cachedGameManager = manager;
            }
        }
        
        // 如果没找到，使用FindObjectOfType
        if (cachedGameManager == null)
        {
            var allManagers = FindObjectsOfType<MonoBehaviour>();
            foreach (var manager in allManagers)
            {
                if (manager.GetType().Name == "LiGameManager")
                {
                    cachedGameManager = manager;
                    break;
                }
            }
        }

        // 查找UIManager
        var allUIManagers = FindObjectsOfType<MonoBehaviour>();
        foreach (var manager in allUIManagers)
        {
            if (manager.GetType().Name == "LiUIManager")
            {
                cachedUIManager = manager;
                break;
            }
        }

        // 查找GameOverManager
        var allGameOverManagers = FindObjectsOfType<MonoBehaviour>();
        foreach (var manager in allGameOverManagers)
        {
            if (manager.GetType().Name == "LiGameOverManager")
            {
                gameOverManager = manager;
                break;
            }
        }

        if (cachedGameManager == null)
        {
            Debug.LogError("LiUIExitButton: 未找到LiGameManager！");
        }
        
        if (cachedUIManager == null)
        {
            Debug.LogError("LiUIExitButton: 未找到LiUIManager！");
        }
    }

    // Exit按钮点击事件
    public void OnExitButtonClick()
    {
        Debug.Log("LiUIExitButton: Exit按钮被点击，准备退出游戏");
        
        // 关闭当前面板
        CloseCurrentPanel();
        
        // 重置游戏状态
        ResetGameState();
        
        // 触发内存管理器的场景跳转垃圾回收
        TriggerMemoryManagerGC();
        
        // 跳转场景
        LoadBackgroundScene();
    }



    // 关闭当前面板
    private void CloseCurrentPanel()
    {
        try
        {
            // 查找并关闭Li Setting面板
            GameObject settingPanel = GameObject.Find("Li Setting Instance");
            if (settingPanel != null && settingPanel.activeInHierarchy)
            {
                settingPanel.SetActive(false);
                Debug.Log("LiUIExitButton: 关闭Li Setting面板");
                
                // 使用缓存的UIManager引用
                if (cachedUIManager != null)
                {
                    var closeMethod = cachedUIManager.GetType().GetMethod("CloseSettingPanel");
                    if (closeMethod != null)
                    {
                        closeMethod.Invoke(cachedUIManager, null);
                    }
                }
                return;
            }

            // 查找并关闭Game Over面板
            GameObject gameOverPanel = GameObject.Find("Game over Panel");
            if (gameOverPanel != null && gameOverPanel.activeInHierarchy)
            {
                // 使用GameOverManager来正确处理游戏结束面板
                if (gameOverManager != null)
                {
                    var hideMethod = gameOverManager.GetType().GetMethod("HideGameOverPanel");
                    var resumeMethod = gameOverManager.GetType().GetMethod("ResumeGame");
                    
                    if (hideMethod != null && resumeMethod != null)
                    {
                        hideMethod.Invoke(gameOverManager, null);
                        resumeMethod.Invoke(gameOverManager, null);
                        Debug.Log("LiUIExitButton: 通过GameOverManager关闭Game Over面板");
                    }
                    else
                    {
                        // 备用方案：直接销毁面板
                        Destroy(gameOverPanel);
                        Time.timeScale = 1f;
                        Debug.Log("LiUIExitButton: 直接关闭Game Over面板");
                    }
                }
                else
                {
                    // 备用方案：直接销毁面板
                    Destroy(gameOverPanel);
                    Time.timeScale = 1f;
                    Debug.Log("LiUIExitButton: 直接关闭Game Over面板");
                }
                return;
            }

            // 如果找不到具体的面板，尝试关闭父级面板
            CloseParentPanel();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LiUIExitButton: 关闭面板时发生错误: {e.Message}");
        }
    }

    // 关闭父级面板
    private void CloseParentPanel()
    {
        Transform parent = transform.parent;
        while (parent != null)
        {
            // 检查是否是设置面板或游戏结束面板
            if (parent.name.Contains("Setting") || parent.name.Contains("Game over") || 
                parent.name.Contains("Li Setting") || parent.name.Contains("Game over Panel"))
            {
                parent.gameObject.SetActive(false);
                Debug.Log($"LiUIExitButton: 关闭面板 {parent.name}");
                return;
            }
            parent = parent.parent;
        }

        Debug.Log("LiUIExitButton: 未找到需要关闭的面板");
    }

    // 恢复游戏初始状态
    private void ResetGameState()
    {
        try
        {
            // 确保时间缩放正常
            Time.timeScale = 1f;
            
            // 使用缓存的GameManager引用重置游戏
            if (cachedGameManager != null)
            {
                var restartMethod = cachedGameManager.GetType().GetMethod("RestartGame");
                if (restartMethod != null)
                {
                    restartMethod.Invoke(cachedGameManager, null);
                    Debug.Log("LiUIExitButton: 游戏状态已重置");
                }
                else
                {
                    Debug.LogWarning("LiUIExitButton: 未找到RestartGame方法");
                }
            }
            else
            {
                Debug.LogWarning("LiUIExitButton: 缓存的GameManager为空");
            }

            // 重置UI管理器状态
            if (cachedUIManager != null)
            {
                var resetMethod = cachedUIManager.GetType().GetMethod("ForceResetSettingPanelState");
                if (resetMethod != null)
                {
                    resetMethod.Invoke(cachedUIManager, null);
                    Debug.Log("LiUIExitButton: UI管理器状态已重置");
                }
                else
                {
                    Debug.LogWarning("LiUIExitButton: 未找到ForceResetSettingPanelState方法");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LiUIExitButton: 重置游戏状态时发生错误: {e.Message}");
        }
    }

    // 跳转到背景场景
    private void LoadBackgroundScene()
    {
        Debug.Log($"LiUIExitButton: 准备跳转到场景: {targetSceneName}");
        
        // 检查当前场景
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"LiUIExitButton: 当前场景: {currentSceneName}");
        
        // 如果当前场景就是目标场景，不需要跳转
        if (currentSceneName == targetSceneName)
        {
            Debug.Log("LiUIExitButton: 当前已在目标场景中，无需跳转");
            return;
        }
        
        // 直接尝试场景跳转，不检查Build Settings（因为手动测试可以跳转）
        Debug.Log($"LiUIExitButton: 开始加载场景: {targetSceneName}");
        
        try
        {
            // 方法1：使用场景名称
            SceneManager.LoadScene(targetSceneName);
            Debug.Log("LiUIExitButton: 场景加载命令已执行（使用场景名称）");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LiUIExitButton: 场景名称跳转失败: {e.Message}");
            
            try
            {
                // 方法2：使用场景索引
                SceneManager.LoadScene(targetSceneIndex);
                Debug.Log($"LiUIExitButton: 场景加载命令已执行（使用索引: {targetSceneIndex}）");
            }
            catch (System.Exception e2)
            {
                Debug.LogError($"LiUIExitButton: 场景索引跳转也失败: {e2.Message}");
                
                try
                {
                    // 方法3：使用索引0
                    SceneManager.LoadScene(0);
                    Debug.Log("LiUIExitButton: 场景加载命令已执行（使用索引0）");
                }
                catch (System.Exception e3)
                {
                    Debug.LogError($"LiUIExitButton: 所有跳转方法都失败: {e3.Message}");
                }
            }
        }
    }

    // 公共方法：手动跳转到背景场景
    [ContextMenu("手动跳转到背景场景")]
    public void ManualLoadBackgroundScene()
    {
        Debug.Log("LiUIExitButton: 手动触发场景跳转");
        LoadBackgroundScene();
    }

    // 触发内存管理器的垃圾回收
    private void TriggerMemoryManagerGC()
    {
        try
        {
            // 查找内存管理器
            var allManagers = FindObjectsOfType<MonoBehaviour>();
            foreach (var manager in allManagers)
            {
                if (manager.GetType().Name == "LiMemoryManager")
                {
                    var onSceneTransitionMethod = manager.GetType().GetMethod("OnSceneTransition");
                    if (onSceneTransitionMethod != null)
                    {
                        onSceneTransitionMethod.Invoke(manager, null);
                        Debug.Log("LiUIExitButton: 已触发内存管理器的场景跳转垃圾回收");
                        return;
                    }
                }
            }
            
            Debug.LogWarning("LiUIExitButton: 未找到LiMemoryManager组件或OnSceneTransition方法");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LiUIExitButton: 触发内存管理器垃圾回收时发生错误: {e.Message}");
        }
    }
    
    // 强制跳转到Backgroud Scens场景
    [ContextMenu("强制跳转到Backgroud Scens")]
    public void ForceLoadBackgroudScens()
    {
        Debug.Log("LiUIExitButton: 强制跳转到Backgroud Scens场景");
        SceneManager.LoadScene("Backgroud Scens");
    }

    // 公共方法：设置目标场景
    public void SetTargetScene(string sceneName, int sceneIndex = -1)
    {
        targetSceneName = sceneName;
        if (sceneIndex >= 0)
        {
            targetSceneIndex = sceneIndex;
        }
        Debug.Log($"LiUIExitButton: 目标场景已设置为 - 名称: {targetSceneName}, 索引: {targetSceneIndex}");
    }

    // 公共方法：设置场景资源
    public void SetTargetSceneAsset(UnityEngine.Object sceneAsset)
    {
        targetSceneAsset = sceneAsset;
        ProcessSceneAsset();
        Debug.Log($"LiUIExitButton: 场景资源已设置，场景名称: {targetSceneName}");
    }

    // 公共方法：获取当前场景信息
    public void GetCurrentSceneInfo()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log($"LiUIExitButton: 当前场景信息 - 名称: {currentSceneName}, 索引: {currentSceneIndex}");
        
        Debug.Log($"LiUIExitButton: 目标场景设置 - 名称: {targetSceneName}, 索引: {targetSceneIndex}");
        
        // 检查目标场景是否在Build Settings中
        #if UNITY_EDITOR
        int buildIndex = SceneUtility.GetBuildIndexByScenePath($"Assets/Scenes/{targetSceneName}.scene");
        Debug.Log($"LiUIExitButton: 目标场景在Build Settings中的索引: {buildIndex}");
        #else
        Debug.Log($"LiUIExitButton: 运行时无法检查Build Settings，使用场景索引: {targetSceneIndex}");
        #endif
    }


}