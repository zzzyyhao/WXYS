using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LiUIManager : MonoBehaviour
{
    [Header("UI组件")]
    public TMPro.TextMeshProUGUI scoreText; // 拖拽UI上的Text组件到这里
    public Button settingButton; // 拖拽Setting-Bt按钮到这里
    public GameObject liSettingPanel; // 拖拽场景中已存在的Li Setting面板到这里
    
    private int score = 0;
    private bool isSettingPanelOpen = false;
    private LiGameManager gameManager; // 游戏管理器引用

    void Start()
    {
        // 查找游戏管理器
        gameManager = FindObjectOfType<LiGameManager>();
        
        // 设置初始光标状态
        InitializeCursor();
        
        UpdateScoreText();
        SetupButtonListeners();
        
        // 确保设置面板初始状态为关闭
        EnsureSettingPanelClosed();
    }



    void EnsureSettingPanelClosed()
    {
        if (liSettingPanel != null)
        {
            liSettingPanel.SetActive(false);
            isSettingPanelOpen = false;
        }
    }

    void SetupButtonListeners()
    {
        // 设置按钮点击事件
        if (settingButton != null)
        {
            // 先清除可能存在的监听器
            settingButton.onClick.RemoveAllListeners();
            
            // 添加新的监听器
            settingButton.onClick.AddListener(OnSettingButtonClick);
            
            Debug.Log("LiUIManager: Setting按钮监听器设置成功");
        }
        else
        {
            Debug.LogError("LiUIManager: settingButton 未设置！请在Inspector中拖拽Setting-Bt按钮");
        }
    }

    // 设置按钮点击事件
    public void OnSettingButtonClick()
    {
        Debug.Log("LiUIManager: Setting按钮被点击");
        
        if (!isSettingPanelOpen)
        {
            OpenSettingPanel();
        }
        else
        {
            Debug.Log("LiUIManager: 设置面板已经打开");
        }
    }

    // 打开设置面板
    public void OpenSettingPanel()
    {
        Debug.Log("LiUIManager: 尝试打开设置面板");
        
        if (liSettingPanel != null)
        {
            liSettingPanel.SetActive(true);
            isSettingPanelOpen = true;
            
            Debug.Log("LiUIManager: 设置面板已打开");
            
            // 确保面板在最前面
            Canvas canvas = liSettingPanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 999; // 设置最高层级
            }
            
            // 暂停游戏
            PauseGame();
            
            // 显示光标
            ShowCursor();
            
            // 触发内存管理器的面板打开垃圾回收
            TriggerMemoryManagerGC("Li Setting");
        }
        else
        {
            Debug.LogError("LiUIManager: liSettingPanel 为空！无法打开设置面板");
        }
    }

    // 关闭设置面板
    public void CloseSettingPanel()
    {
        if (liSettingPanel != null)
        {
            liSettingPanel.SetActive(false);
            isSettingPanelOpen = false;
            
            // 恢复游戏
            ResumeGame();
            
            // 隐藏光标
            HideCursor();
        }
    }

    // 暂停游戏
    void PauseGame()
    {
        if (gameManager != null)
        {
            gameManager.PauseGame();
        }
        
        // 暂停时间
        Time.timeScale = 0f;
    }

    // 恢复游戏
    void ResumeGame()
    {
        if (gameManager != null)
        {
            gameManager.ResumeGame();
        }
        
        // 恢复时间
        Time.timeScale = 1f;
    }
    
    // 显示光标
    void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    // 隐藏光标
    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 初始化光标设置
    void InitializeCursor()
    {
        // 游戏开始时隐藏光标
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreText();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            // 将分数格式化为6位数字，从右边开始显示
            // 例如：0 -> 000000, 123 -> 000123, 12345 -> 012345
            scoreText.text = score.ToString("D6");
        }
    }

    // 公共方法：检查设置面板是否打开
    public bool IsSettingPanelOpen()
    {
        return isSettingPanelOpen;
    }

    // 公共方法：切换设置面板状态
    public void ToggleSettingPanel()
    {
        if (isSettingPanelOpen)
        {
            CloseSettingPanel();
        }
        else
        {
            OpenSettingPanel();
        }
    }

    // 强制重置设置面板状态（用于游戏重新开始时）
    public void ForceResetSettingPanelState()
    {
        isSettingPanelOpen = false;
        
        // 确保设置面板被关闭
        if (liSettingPanel != null)
        {
            liSettingPanel.SetActive(false);
        }
        
        // 确保游戏处于激活状态
        if (gameManager != null)
        {
            gameManager.ResumeGame();
        }
        
        // 确保时间缩放正常
        Time.timeScale = 1f;
        
        // 隐藏光标
        HideCursor();
        
        Debug.Log("LiUIManager: 设置面板状态已强制重置");
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
                        Debug.Log($"LiUIManager: 已触发内存管理器的面板打开垃圾回收: {panelName}");
                        return;
                    }
                }
            }
            
            Debug.LogWarning("LiUIManager: 未找到LiMemoryManager组件或OnPanelOpened方法");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LiUIManager: 触发内存管理器垃圾回收时发生错误: {e.Message}");
        }
    }
}
