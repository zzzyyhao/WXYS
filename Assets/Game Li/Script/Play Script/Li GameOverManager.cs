using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LiGameOverManager : MonoBehaviour
{
    [Header("游戏结束面板预制体")]
    public GameObject gameOverPanelPrefab; // 拖拽Game Over Panel预制体到这里
    
    private GameObject currentGameOverPanel;
    private MonoBehaviour gameManager;
    private MonoBehaviour uiManager;
    
    void Start()
    {
        // 查找管理器引用
        CacheManagerReferences();
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
                gameManager = manager;
            }
        }
        
        // 如果没找到，使用FindObjectOfType
        if (gameManager == null)
        {
            var allManagers = FindObjectsOfType<MonoBehaviour>();
            foreach (var manager in allManagers)
            {
                if (manager.GetType().Name == "LiGameManager")
                {
                    gameManager = manager;
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
                uiManager = manager;
                break;
            }
        }

        if (gameManager == null)
        {
            Debug.LogError("LiGameOverManager: 未找到LiGameManager！");
        }
        
        if (uiManager == null)
        {
            Debug.LogError("LiGameOverManager: 未找到LiUIManager！");
        }
    }
    
    // 公共方法：触发游戏结束
    public void TriggerGameOver()
    {
        Debug.Log("LiGameOverManager: 触发游戏结束");
        
        // 暂停游戏
        PauseGame();
        
        // 显示游戏结束面板
        ShowGameOverPanel();
        
        // 显示光标
        ShowCursor();
    }
    
    // 暂停游戏
    private void PauseGame()
    {
        // 暂停时间
        Time.timeScale = 0f;
        
        // 通知GameManager暂停游戏
        if (gameManager != null)
        {
            var pauseMethod = gameManager.GetType().GetMethod("PauseGame");
            if (pauseMethod != null)
            {
                pauseMethod.Invoke(gameManager, null);
            }
        }
        
        Debug.Log("LiGameOverManager: 游戏已暂停");
    }
    
    // 显示游戏结束面板
    private void ShowGameOverPanel()
    {
        // 如果已经有游戏结束面板，先销毁它
        if (currentGameOverPanel != null)
        {
            Destroy(currentGameOverPanel);
        }
        
        // 查找Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("LiGameOverManager: 未找到Canvas！");
            return;
        }
        
        // 实例化游戏结束面板
        if (gameOverPanelPrefab != null)
        {
            currentGameOverPanel = Instantiate(gameOverPanelPrefab, canvas.transform);
            currentGameOverPanel.name = "Game over Panel";
            
            // 设置面板位置和大小
            RectTransform rectTransform = currentGameOverPanel.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
            }
            
            // 同步分数显示
            SyncGameOverScore();
            
            // 触发内存管理器的面板打开垃圾回收
            TriggerMemoryManagerGC("Game Over");
            
            Debug.Log("LiGameOverManager: 游戏结束面板已显示");
        }
        else
        {
            Debug.LogError("LiGameOverManager: gameOverPanelPrefab未设置！");
        }
    }

    // 同步游戏结束面板中的分数
    private void SyncGameOverScore()
    {
        if (currentGameOverPanel == null)
        {
            Debug.LogWarning("LiGameOverManager: 游戏结束面板为空，无法同步分数");
            return;
        }

        // 查找游戏结束面板中的LiGameOverScoreDisplay组件
        var scoreDisplays = currentGameOverPanel.GetComponentsInChildren<MonoBehaviour>();
        foreach (var scoreDisplay in scoreDisplays)
        {
            if (scoreDisplay.GetType().Name == "LiGameOverScoreDisplay")
            {
                var updateMethod = scoreDisplay.GetType().GetMethod("UpdateScoreDisplay");
                if (updateMethod != null)
                {
                    updateMethod.Invoke(scoreDisplay, null);
                    Debug.Log("LiGameOverManager: 通过LiGameOverScoreDisplay同步分数");
                    return;
                }
            }
        }
        
        // 如果没有找到LiGameOverScoreDisplay，使用手动同步
        ManualSyncScore();
    }

    // 手动同步分数（备用方案）
    private void ManualSyncScore()
    {
        // 查找名为"Score"的子物体
        Transform scoreTransform = currentGameOverPanel.transform.Find("Score");
        if (scoreTransform == null)
        {
            // 如果没找到，尝试递归查找
            scoreTransform = FindChildRecursively(currentGameOverPanel.transform, "Score");
        }

        if (scoreTransform != null)
        {
            // 获取TextMeshProUGUI组件
            TextMeshProUGUI scoreText = scoreTransform.GetComponent<TextMeshProUGUI>();
            if (scoreText != null)
            {
                int currentScore = GetCurrentGameScore();
                scoreText.text = currentScore.ToString("D6");
                Debug.Log($"LiGameOverManager: 手动同步分数为 {currentScore}");
            }
            else
            {
                Debug.LogWarning("LiGameOverManager: Score物体未找到TextMeshProUGUI组件");
            }
        }
        else
        {
            Debug.LogWarning("LiGameOverManager: 未找到游戏结束面板中的Score物体");
        }
    }

    // 获取当前游戏分数
    private int GetCurrentGameScore()
    {
        // 尝试从GameManager获取分数
        if (gameManager != null)
        {
            var scoreField = gameManager.GetType().GetField("currentScore");
            if (scoreField != null)
            {
                return (int)scoreField.GetValue(gameManager);
            }
        }

        // 尝试从UIManager获取分数
        if (uiManager != null)
        {
            var scoreField = uiManager.GetType().GetField("score", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (scoreField != null)
            {
                return (int)scoreField.GetValue(uiManager);
            }
        }

        Debug.LogWarning("LiGameOverManager: 无法获取当前游戏分数");
        return 0;
    }

    // 递归查找子物体
    private Transform FindChildRecursively(Transform parent, string childName)
    {
        if (parent.name == childName)
        {
            return parent;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Transform result = FindChildRecursively(child, childName);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
    
    // 显示光标
    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Debug.Log("LiGameOverManager: 光标已显示");
    }
    
    // 公共方法：隐藏游戏结束面板
    public void HideGameOverPanel()
    {
        if (currentGameOverPanel != null)
        {
            Destroy(currentGameOverPanel);
            currentGameOverPanel = null;
            Debug.Log("LiGameOverManager: 游戏结束面板已隐藏");
        }
    }
    
    // 公共方法：恢复游戏
    public void ResumeGame()
    {
        // 隐藏游戏结束面板
        HideGameOverPanel();
        
        // 恢复时间
        Time.timeScale = 1f;
        
        // 通知GameManager恢复游戏
        if (gameManager != null)
        {
            var resumeMethod = gameManager.GetType().GetMethod("ResumeGame");
            if (resumeMethod != null)
            {
                resumeMethod.Invoke(gameManager, null);
            }
        }
        
        // 隐藏光标
        if (uiManager != null)
        {
            var hideCursorMethod = uiManager.GetType().GetMethod("HideCursor");
            if (hideCursorMethod != null)
            {
                hideCursorMethod.Invoke(uiManager, null);
            }
        }
        
        Debug.Log("LiGameOverManager: 游戏已恢复");
    }

    // 公共方法：手动同步游戏结束面板分数
    public void SyncScore()
    {
        if (currentGameOverPanel != null)
        {
            SyncGameOverScore();
        }
        else
        {
            Debug.LogWarning("LiGameOverManager: 游戏结束面板未显示，无法同步分数");
        }
    }
    
    // 触发内存管理器的垃圾回收
    private void TriggerMemoryManagerGC(string panelName)
    {
        try
        {
            // 查找内存管理器
            var allManagers = FindObjectsOfType<MonoBehaviour>();
            foreach (var manager in allManagers)
            {
                if (manager.GetType().Name == "LiMemoryManager")
                {
                    var onPanelOpenedMethod = manager.GetType().GetMethod("OnPanelOpened");
                    if (onPanelOpenedMethod != null)
                    {
                        onPanelOpenedMethod.Invoke(manager, new object[] { panelName });
                        Debug.Log($"LiGameOverManager: 已触发内存管理器的面板打开垃圾回收: {panelName}");
                        return;
                    }
                }
            }
            
            Debug.LogWarning("LiGameOverManager: 未找到LiMemoryManager组件或OnPanelOpened方法");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LiGameOverManager: 触发内存管理器垃圾回收时发生错误: {e.Message}");
        }
    }
}