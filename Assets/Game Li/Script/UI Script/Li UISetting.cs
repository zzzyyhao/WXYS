using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiUISetting : MonoBehaviour
{
    [Header("设置面板组件")]
    public Button exitButton; // 拖拽Exit按钮到这里
    public Button againButton; // 拖拽Again按钮到这里
    public Button markButton; // 拖拽Mark按钮到这里
    public Button getOnButton; // 拖拽Get On按钮到这里
    
    [Header("设置选项")]
    public Slider musicSlider; // 音乐音量滑块
    public Slider soundSlider; // 音效音量滑块
    
    // 使用MonoBehaviour类型而不是具体的类名，避免编译错误
    private MonoBehaviour uiManager;
    
    void Start()
    {
        // 查找UI管理器 - 修复查找逻辑
        FindUIManager();
        
        // 设置按钮监听器
        SetupButtonListeners();
        
        // 初始化设置值
        InitializeSettings();
    }
    
    // 查找UI管理器
    void FindUIManager()
    {
        // 查找所有MonoBehaviour，然后找到LiUIManager
        MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour mb in allMonoBehaviours)
        {
            if (mb.GetType().Name == "LiUIManager")
            {
                uiManager = mb;
                Debug.Log("LiUISetting: 找到LiUIManager");
                break;
            }
        }
        
        if (uiManager == null)
        {
            Debug.LogError("LiUISetting: 未找到LiUIManager！");
        }
    }

    void SetupButtonListeners()
    {
        // Exit按钮点击事件
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClick);
        }
        
        // Again按钮点击事件
        if (againButton != null)
        {
            againButton.onClick.AddListener(OnAgainButtonClick);
        }
        
        // Mark按钮点击事件
        if (markButton != null)
        {
            markButton.onClick.AddListener(OnMarkButtonClick);
        }
        
        // Get On按钮点击事件
        if (getOnButton != null)
        {
            getOnButton.onClick.AddListener(OnGetOnButtonClick);
        }
        
        // 设置滑块监听器
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        
        if (soundSlider != null)
        {
            soundSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
        }
    }

    // Exit按钮点击事件
    public void OnExitButtonClick()
    {
        // 这里可以添加退出功能的逻辑
        // 例如：退出游戏或返回主菜单
        Debug.Log("Exit按钮被点击");
    }

    // Again按钮点击事件
    public void OnAgainButtonClick()
    {
        // 这里可以添加重新开始游戏的逻辑
        // 例如：重新加载场景或重置游戏状态
        Debug.Log("Again按钮被点击");
        
        // 重新开始游戏，而不是关闭面板
        RestartGame();
    }

    // Mark按钮点击事件
    public void OnMarkButtonClick()
    {
        // 这里可以添加标记功能的逻辑
        // 例如：标记当前进度或保存游戏状态
        Debug.Log("Mark按钮被点击");
        
        // 关闭设置面板
        CloseSettingPanel();
    }

    // Get On按钮点击事件
    public void OnGetOnButtonClick()
    {
        // 这里可以添加继续游戏的逻辑
        // 例如：继续当前游戏或返回游戏界面
        Debug.Log("Get On按钮被点击");
        
        // 关闭设置面板
        CloseSettingPanel();
    }

    // 音乐音量改变事件
    public void OnMusicVolumeChanged(float value)
    {
        // 这里可以添加音乐音量控制逻辑
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
        
        // 微信小游戏兼容性：可以调用微信API设置音量
        #if UNITY_WEBGL && !UNITY_EDITOR
        // 微信小游戏音量设置代码可以在这里添加
        #endif
    }

    // 音效音量改变事件
    public void OnSoundVolumeChanged(float value)
    {
        // 这里可以添加音效音量控制逻辑
        PlayerPrefs.SetFloat("SoundVolume", value);
        PlayerPrefs.Save();
        
        // 微信小游戏兼容性：可以调用微信API设置音量
        #if UNITY_WEBGL && !UNITY_EDITOR
        // 微信小游戏音量设置代码可以在这里添加
        #endif
    }

    // 初始化设置值
    void InitializeSettings()
    {
        // 加载保存的设置值
        if (musicSlider != null)
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            musicSlider.value = musicVolume;
        }
        
        if (soundSlider != null)
        {
            float soundVolume = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
            soundSlider.value = soundVolume;
        }
    }

    // 重新开始游戏
    void RestartGame()
    {
        Debug.Log("LiUISetting: 开始重新开始游戏");
        
        // 关闭设置面板
        CloseSettingPanel();
        
        // 重新加载当前场景
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
    
    // 关闭设置面板
    void CloseSettingPanel()
    {
        // 使用反射调用UI管理器的方法，避免编译错误
        if (uiManager != null)
        {
            // 尝试调用CloseSettingPanel方法
            var method = uiManager.GetType().GetMethod("CloseSettingPanel");
            if (method != null)
            {
                method.Invoke(uiManager, null);
                Debug.Log("LiUISetting: 成功调用CloseSettingPanel方法");
            }
            else
            {
                // 如果找不到方法，直接隐藏面板
                gameObject.SetActive(false);
                Debug.LogWarning("LiUISetting: 未找到CloseSettingPanel方法，直接隐藏面板");
            }
        }
        else
        {
            // 如果没有找到UI管理器，直接隐藏面板
            gameObject.SetActive(false);
            Debug.LogWarning("LiUISetting: 未找到UI管理器，直接隐藏面板");
        }
    }
} 