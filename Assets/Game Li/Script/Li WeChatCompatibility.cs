using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiWeChatCompatibility : MonoBehaviour
{
    [Header("微信小游戏设置")]
    public bool enableWeChatFeatures = true; // 是否启用微信小游戏功能
    public bool hideCursorOnMobile = true; // 在移动设备上隐藏光标
    
    [Header("移动设备设置")]
    public bool enableTouchInput = true; // 启用触摸输入
    public float touchSensitivity = 1.0f; // 触摸灵敏度
    
    [Header("调试信息")]
    public bool showDebugInfo = true; // 是否显示调试信息
    
    private bool isWeChatPlatform = false;
    private bool isMobilePlatform = false;
    
    void Start()
    {
        // 检测平台
        DetectPlatform();
        
        // 初始化平台特定设置
        InitializePlatformSettings();
        
        // 显示设备信息
        if (showDebugInfo)
        {
            Debug.Log($"设备信息: {GetDeviceInfo()}");
        }
    }
    
    // 检测当前平台
    void DetectPlatform()
    {
        // 检测微信小游戏平台
        #if UNITY_WEBGL && !UNITY_EDITOR
        isWeChatPlatform = true;
        if (showDebugInfo)
        {
            Debug.Log("检测到微信小游戏平台");
        }
        #endif
        
        // 检测移动设备平台
        #if UNITY_ANDROID || UNITY_IOS
        isMobilePlatform = true;
        if (showDebugInfo)
        {
            Debug.Log("检测到移动设备平台");
        }
        #endif
        
        // 检测是否为移动设备
        if (Application.isMobilePlatform)
        {
            isMobilePlatform = true;
            if (showDebugInfo)
            {
                Debug.Log("检测到移动设备");
            }
        }
        
        // 编辑器中的平台检测
        #if UNITY_EDITOR
        if (showDebugInfo)
        {
            Debug.Log("当前在Unity编辑器中运行");
        }
        #endif
    }
    
    // 初始化平台特定设置
    void InitializePlatformSettings()
    {
        // 移动设备光标设置
        if (isMobilePlatform && hideCursorOnMobile)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (showDebugInfo)
            {
                Debug.Log("移动设备：隐藏并锁定光标");
            }
        }
        
        // 微信小游戏特定设置
        if (isWeChatPlatform && enableWeChatFeatures)
        {
            InitializeWeChatFeatures();
        }
        
        // 屏幕适配
        AdaptToScreen();
    }
    
    // 初始化微信小游戏功能
    void InitializeWeChatFeatures()
    {
        if (showDebugInfo)
        {
            Debug.Log("初始化微信小游戏功能");
        }
        
        // 这里可以添加微信小游戏特定的初始化代码
        // 例如：设置分享、广告、支付等功能
        
        // 设置微信小游戏的基本配置
        SetupWeChatConfig();
    }
    
    // 设置微信小游戏配置
    void SetupWeChatConfig()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        // 微信小游戏配置代码
        if (showDebugInfo)
        {
            Debug.Log("设置微信小游戏配置");
        }
        #endif
    }
    
    // 公共方法：获取当前平台信息
    public bool IsWeChatPlatform()
    {
        return isWeChatPlatform;
    }
    
    public bool IsMobilePlatform()
    {
        return isMobilePlatform;
    }
    
    // 公共方法：设置音量（微信小游戏兼容）
    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        // 微信小游戏音量设置
        SetWeChatMusicVolume(volume);
        #endif
        
        if (showDebugInfo)
        {
            Debug.Log($"设置音乐音量: {volume}");
        }
    }
    
    public void SetSoundVolume(float volume)
    {
        PlayerPrefs.SetFloat("SoundVolume", volume);
        PlayerPrefs.Save();
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        // 微信小游戏音量设置
        SetWeChatSoundVolume(volume);
        #endif
        
        if (showDebugInfo)
        {
            Debug.Log($"设置音效音量: {volume}");
        }
    }
    
    // 微信小游戏音量设置（需要根据实际微信API调整）
    void SetWeChatMusicVolume(float volume)
    {
        // 这里需要根据微信小游戏的API来实现
        // 例如：调用微信的背景音乐API
        if (showDebugInfo)
        {
            Debug.Log($"微信小游戏：设置音乐音量 {volume}");
        }
    }
    
    void SetWeChatSoundVolume(float volume)
    {
        // 这里需要根据微信小游戏的API来实现
        // 例如：调用微信的音效API
        if (showDebugInfo)
        {
            Debug.Log($"微信小游戏：设置音效音量 {volume}");
        }
    }
    
    // 公共方法：分享功能（微信小游戏）
    public void ShareGame()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        // 微信小游戏分享功能
        ShareToWeChat();
        #else
        if (showDebugInfo)
        {
            Debug.Log("分享功能（仅微信小游戏支持）");
        }
        #endif
    }
    
    void ShareToWeChat()
    {
        // 微信小游戏分享API调用
        if (showDebugInfo)
        {
            Debug.Log("调用微信小游戏分享功能");
        }
    }
    
    // 公共方法：显示广告（微信小游戏）
    public void ShowAd()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        // 微信小游戏广告功能
        ShowWeChatAd();
        #else
        if (showDebugInfo)
        {
            Debug.Log("广告功能（仅微信小游戏支持）");
        }
        #endif
    }
    
    void ShowWeChatAd()
    {
        // 微信小游戏广告API调用
        if (showDebugInfo)
        {
            Debug.Log("调用微信小游戏广告功能");
        }
    }
    
    // 公共方法：获取设备信息
    public string GetDeviceInfo()
    {
        string info = $"平台: {Application.platform}, ";
        info += $"设备型号: {SystemInfo.deviceModel}, ";
        info += $"操作系统: {SystemInfo.operatingSystem}, ";
        info += $"屏幕分辨率: {Screen.width}x{Screen.height}, ";
        info += $"微信小游戏: {isWeChatPlatform}, ";
        info += $"移动设备: {isMobilePlatform}";
        
        return info;
    }
    
    // 公共方法：适配屏幕
    public void AdaptToScreen()
    {
        if (isMobilePlatform)
        {
            // 移动设备屏幕适配
            Screen.orientation = ScreenOrientation.Portrait; // 竖屏模式
            
            // 设置安全区域（适配刘海屏等）
            #if UNITY_IOS
            // iOS安全区域适配
            if (showDebugInfo)
            {
                Debug.Log("应用iOS安全区域适配");
            }
            #endif
            
            #if UNITY_ANDROID
            // Android安全区域适配
            if (showDebugInfo)
            {
                Debug.Log("应用Android安全区域适配");
            }
            #endif
            
            if (showDebugInfo)
            {
                Debug.Log("移动设备屏幕适配完成");
            }
        }
    }
    
    // 公共方法：获取音量设置
    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", 1.0f);
    }
    
    public float GetSoundVolume()
    {
        return PlayerPrefs.GetFloat("SoundVolume", 1.0f);
    }
    
    // 公共方法：重置设置
    public void ResetSettings()
    {
        PlayerPrefs.DeleteKey("MusicVolume");
        PlayerPrefs.DeleteKey("SoundVolume");
        PlayerPrefs.Save();
        
        if (showDebugInfo)
        {
            Debug.Log("设置已重置");
        }
    }
} 