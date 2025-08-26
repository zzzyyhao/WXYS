using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JBAnimationManager : MonoBehaviour
{
    [Header("动画控制器")]
    public Animator animator; // 拖拽W1.controller的Animator组件到这里
    
    [Header("按钮引用")]
    public Button leftButton; // 拖拽Bt-Left.prefab到这里
    public Button rightButton; // 拖拽Bt-Right.prefab到这里
    
    [Header("动画参数名称")]
    public string leftParameterName = "L"; // 对应Animator中的L参数
    public string rightParameterName = "R"; // 对应Animator中的R参数
    
    [Header("相机效果")]
    public JBCamera cameraController; // 相机控制器引用
    
    // 动画状态跟踪
    private bool _animating = false; // 跟踪是否正在播放动画
    
    void Start()
    {
        // 自动查找Animator组件
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("JBAnimationManager: 未找到Animator组件！");
            }
        }
        
        // 自动查找相机控制器
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<JBCamera>();
            if (cameraController != null)
            {
                Debug.Log("JBAnimationManager: 已自动找到相机控制器");
            }
            else
            {
                Debug.LogWarning("JBAnimationManager: 未找到相机控制器，相机抖动效果将不可用");
            }
        }
        
        // 设置按钮监听器
        SetupButtonListeners();
        
        // 检查Animator参数
        CheckAnimatorParameters();
        
        Debug.Log("JBAnimationManager: 动画管理器初始化完成");
    }
    
    void SetupButtonListeners()
    {
        // 设置Left按钮点击事件
        if (leftButton != null)
        {
            leftButton.onClick.AddListener(OnLeftButtonClick);
            Debug.Log("JBAnimationManager: Left按钮监听器已设置");
        }
        else
        {
            Debug.LogWarning("JBAnimationManager: Left按钮未设置！");
        }
        
        // 设置Right按钮点击事件
        if (rightButton != null)
        {
            rightButton.onClick.AddListener(OnRightButtonClick);
            Debug.Log("JBAnimationManager: Right按钮监听器已设置");
        }
        else
        {
            Debug.LogWarning("JBAnimationManager: Right按钮未设置！");
        }
    }
    
    // 检查Animator参数是否存在
    void CheckAnimatorParameters()
    {
        if (animator == null) return;
        
        Debug.Log("=== Animator参数检查 ===");
        
        // 检查L参数
        bool hasLParameter = false;
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == leftParameterName)
            {
                hasLParameter = true;
                Debug.Log($"找到参数 '{leftParameterName}': 类型 = {param.type}");
                break;
            }
        }
        
        if (!hasLParameter)
        {
            Debug.LogError($"未找到参数 '{leftParameterName}'！请检查Animator Controller中的参数名称");
        }
        
        // 检查R参数
        bool hasRParameter = false;
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == rightParameterName)
            {
                hasRParameter = true;
                Debug.Log($"找到参数 '{rightParameterName}': 类型 = {param.type}");
                break;
            }
        }
        
        if (!hasRParameter)
        {
            Debug.LogError($"未找到参数 '{rightParameterName}'！请检查Animator Controller中的参数名称");
        }
        
        // 列出所有参数
        Debug.Log("所有Animator参数:");
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            Debug.Log($"- {param.name}: {param.type}");
        }
        
        Debug.Log("=== 参数检查完成 ===");
    }
    
    // Left按钮点击事件
    public void OnLeftButtonClick()
    {
        // 检查是否正在播放动画，如果是则忽略点击
        if (_animating)
        {
            Debug.Log("JBAnimationManager: 正在播放动画，忽略Left按钮点击");
            return;
        }
        
        if (animator != null)
        {
            Debug.Log($"JBAnimationManager: 尝试触发参数 '{leftParameterName}'");
            
            // 设置动画状态为true，防止重复触发
            _animating = true;
            
            // 触发Left动画
            animator.SetTrigger(leftParameterName);
            Debug.Log("JBAnimationManager: Left按钮被点击，触发L动画");
            
            // 触发相机抖动效果
            if (cameraController != null)
            {
                cameraController.ShakeCamera();
            }
        }
        else
        {
            Debug.LogError("JBAnimationManager: Animator组件未找到！");
        }
    }
    
    // Right按钮点击事件
    public void OnRightButtonClick()
    {
        // 检查是否正在播放动画，如果是则忽略点击
        if (_animating)
        {
            Debug.Log("JBAnimationManager: 正在播放动画，忽略Right按钮点击");
            return;
        }
        
        if (animator != null)
        {
            Debug.Log($"JBAnimationManager: 尝试触发参数 '{rightParameterName}'");
            
            // 设置动画状态为true，防止重复触发
            _animating = true;
            
            // 触发Right动画
            animator.SetTrigger(rightParameterName);
            Debug.Log("JBAnimationManager: Right按钮被点击，触发R动画");
            
            // 触发相机抖动效果
            if (cameraController != null)
            {
                cameraController.ShakeCamera();
            }
        }
        else
        {
            Debug.LogError("JBAnimationManager: Animator组件未找到！");
        }
    }
    
    // 动画完成事件 - 在动画最后一帧调用
    public void AnimationFinished()
    {
        _animating = false;
        Debug.Log("JBAnimationManager: 动画播放完成，重置状态");
    }
    
    // 手动触发动画的方法（可选）
    public void TriggerLeftAnimation()
    {
        if (animator != null && !_animating)
        {
            _animating = true;
            animator.SetTrigger(leftParameterName);
            Debug.Log("JBAnimationManager: 手动触发L动画");
        }
    }
    
    public void TriggerRightAnimation()
    {
        if (animator != null && !_animating)
        {
            _animating = true;
            animator.SetTrigger(rightParameterName);
            Debug.Log("JBAnimationManager: 手动触发R动画");
        }
    }
    
    // 重置所有触发器（可选）
    public void ResetAllTriggers()
    {
        if (animator != null)
        {
            animator.ResetTrigger(leftParameterName);
            animator.ResetTrigger(rightParameterName);
            _animating = false;
            Debug.Log("JBAnimationManager: 所有触发器已重置");
        }
    }
}
