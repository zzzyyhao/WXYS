using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LiGameOverScoreDisplay : MonoBehaviour
{
    [Header("分数显示组件")]
    public TMPro.TextMeshProUGUI scoreText; // 分数文本组件（拖拽或自动获取）
    
    private MonoBehaviour gameManager;
    private MonoBehaviour uiManager;
    
    void Start()
    {
        // 查找管理器引用
        CacheManagerReferences();
        
        // 如果没有手动设置scoreText，尝试自动获取
        if (scoreText == null)
        {
            scoreText = GetComponent<TMPro.TextMeshProUGUI>();
            if (scoreText == null)
            {
                Debug.LogError("LiGameOverScoreDisplay: 未找到TextMeshProUGUI组件！");
            }
        }
        
        // 初始化时更新分数
        UpdateScoreDisplay();
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
    }
    
    // 公共方法：更新分数显示
    public void UpdateScoreDisplay()
    {
        if (scoreText == null)
        {
            Debug.LogError("LiGameOverScoreDisplay: scoreText为空，无法更新分数显示");
            return;
        }
        
        int currentScore = GetCurrentGameScore();
        scoreText.text = currentScore.ToString("D6");
        
        Debug.Log($"LiGameOverScoreDisplay: 分数显示已更新为 {currentScore}");
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

        Debug.LogWarning("LiGameOverScoreDisplay: 无法获取当前游戏分数");
        return 0;
    }
    
    // 当物体被激活时自动更新分数
    void OnEnable()
    {
        // 延迟一帧更新，确保所有组件都已初始化
        StartCoroutine(DelayedUpdateScore());
    }
    
    // 延迟更新分数
    private IEnumerator DelayedUpdateScore()
    {
        yield return null; // 等待一帧
        UpdateScoreDisplay();
    }
    
    // 公共方法：手动刷新分数
    [ContextMenu("手动刷新分数")]
    public void RefreshScore()
    {
        UpdateScoreDisplay();
    }
}