using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiUIAgainButton : MonoBehaviour
{
    private Button againButton;
    private MonoBehaviour cachedGameManager;
    private MonoBehaviour cachedUIManager;

    void Start()
    {
        // 获取Button组件
        againButton = GetComponent<Button>();
        if (againButton == null)
        {
            Debug.LogError("LiUIAgainButton: 未找到Button组件！");
            return;
        }

        // 缓存管理器引用
        CacheManagerReferences();

        // 添加点击事件监听器
        againButton.onClick.AddListener(OnAgainButtonClick);
        
        Debug.Log("LiUIAgainButton: Again按钮监听器设置成功");
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

        if (cachedGameManager == null)
        {
            Debug.LogError("LiUIAgainButton: 未找到LiGameManager！");
        }
        
        if (cachedUIManager == null)
        {
            Debug.LogError("LiUIAgainButton: 未找到LiUIManager！");
        }
    }

    // Again按钮点击事件
    public void OnAgainButtonClick()
    {
        Debug.Log("LiUIAgainButton: Again按钮被点击，重新开始游戏");
        
        // 关闭当前面板
        CloseCurrentPanel();
        
        // 重新开始游戏
        RestartGame();
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
                Debug.Log("LiUIAgainButton: 关闭Li Setting面板");
                
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
                var allManagers = FindObjectsOfType<MonoBehaviour>();
                MonoBehaviour gameOverManager = null;
                
                foreach (var manager in allManagers)
                {
                    if (manager.GetType().Name == "LiGameOverManager")
                    {
                        gameOverManager = manager;
                        break;
                    }
                }
                
                if (gameOverManager != null)
                {
                    var hideMethod = gameOverManager.GetType().GetMethod("HideGameOverPanel");
                    var resumeMethod = gameOverManager.GetType().GetMethod("ResumeGame");
                    
                    if (hideMethod != null && resumeMethod != null)
                    {
                        hideMethod.Invoke(gameOverManager, null);
                        resumeMethod.Invoke(gameOverManager, null);
                        Debug.Log("LiUIAgainButton: 通过GameOverManager关闭Game Over面板并恢复游戏");
                    }
                    else
                    {
                        // 备用方案：直接销毁面板
                        Destroy(gameOverPanel);
                        Time.timeScale = 1f;
                        Debug.Log("LiUIAgainButton: 直接关闭Game Over面板");
                    }
                }
                else
                {
                    // 备用方案：直接销毁面板
                    Destroy(gameOverPanel);
                    Time.timeScale = 1f;
                    Debug.Log("LiUIAgainButton: 直接关闭Game Over面板");
                }
                return;
            }

            // 如果找不到具体的面板，尝试关闭父级面板
            CloseParentPanel();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LiUIAgainButton: 关闭面板时发生错误: {e.Message}");
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
                Debug.Log($"LiUIAgainButton: 关闭面板 {parent.name}");
                return;
            }
            parent = parent.parent;
        }

        Debug.Log("LiUIAgainButton: 未找到需要关闭的面板");
    }

    // 重新开始游戏
    private void RestartGame()
    {
        try
        {
            // 确保时间缩放正常
            Time.timeScale = 1f;
            
            // 使用缓存的GameManager引用
            if (cachedGameManager != null)
            {
                var restartMethod = cachedGameManager.GetType().GetMethod("RestartGame");
                if (restartMethod != null)
                {
                    restartMethod.Invoke(cachedGameManager, null);
                    Debug.Log("LiUIAgainButton: 成功重新开始游戏");
                    
                    // 延迟一帧后确保UI管理器状态正确
                    StartCoroutine(DelayedUIManagerReset());
                    return;
                }
                else
                {
                    Debug.LogError("LiUIAgainButton: 未找到RestartGame方法！");
                }
            }
            else
            {
                Debug.LogError("LiUIAgainButton: 缓存的GameManager为空！");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LiUIAgainButton: 重新开始游戏时发生错误: {e.Message}");
        }
    }

    // 延迟重置UI管理器状态
    private IEnumerator DelayedUIManagerReset()
    {
        // 等待一帧，确保GameManager的RestartGame方法执行完成
        yield return null;
        
        // 确保UI管理器状态正确
        EnsureUIManagerState();
    }

    // 确保UI管理器状态正确
    private void EnsureUIManagerState()
    {
        try
        {
            if (cachedUIManager != null)
            {
                // 强制重置设置面板状态（这个方法包含了所有必要的重置操作）
                var resetMethod = cachedUIManager.GetType().GetMethod("ForceResetSettingPanelState");
                if (resetMethod != null)
                {
                    resetMethod.Invoke(cachedUIManager, null);
                    Debug.Log("LiUIAgainButton: UI管理器状态已重置");
                }
                else
                {
                    Debug.LogWarning("LiUIAgainButton: 未找到ForceResetSettingPanelState方法");
                }
            }
            else
            {
                Debug.LogWarning("LiUIAgainButton: 缓存的UIManager为空，无法重置UI状态");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LiUIAgainButton: 重置UI状态时发生错误: {e.Message}");
        }
    }
} 