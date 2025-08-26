using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // 添加DOTween命名空间

public class JBCamera : MonoBehaviour
{
    [Header("相机抖动设置")]
    public float shakeDuration = 0.5f; // 抖动持续时间
    public float shakeStrength = 0.3f; // 抖动强度
    public int shakeVibrato = 10; // 抖动频率
    public float shakeRandomness = 90f; // 抖动随机性
    
    [Header("相机缩放设置")]
    public float punchScale = 0.1f; // 缩放强度
    public float punchDuration = 0.3f; // 缩放持续时间
    public int punchVibrato = 10; // 缩放频率
    public float punchElasticity = 1f; // 缩放弹性
    
    private Vector3 originalPosition; // 原始位置
    private Vector3 originalScale; // 原始缩放
    private bool isShaking = false; // 是否正在抖动
    
    void Start()
    {
        // 保存原始位置和缩放
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;
        
        Debug.Log("JBCamera: 相机控制器初始化完成");
    }
    
    // 相机抖动效果
    public void ShakeCamera()
    {
        if (isShaking) return; // 如果正在抖动，则忽略
        
        isShaking = true;
        Debug.Log("JBCamera: 开始相机抖动");
        
        // 使用DOTween.Shake进行位置抖动
        transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, true)
            .OnComplete(() => {
                // 抖动完成后重置位置
                transform.localPosition = originalPosition;
                isShaking = false;
                Debug.Log("JBCamera: 相机抖动完成");
            });
    }
    
    // 相机缩放效果
    public void PunchScaleCamera()
    {
        Debug.Log("JBCamera: 开始相机缩放");
        
        // 使用DOTween.Punch进行缩放效果
        transform.DOPunchScale(Vector3.one * punchScale, punchDuration, punchVibrato, punchElasticity)
            .OnComplete(() => {
                // 缩放完成后重置
                transform.localScale = originalScale;
                Debug.Log("JBCamera: 相机缩放完成");
            });
    }
    
    // 组合效果：抖动+缩放
    public void ShakeAndPunchCamera()
    {
        if (isShaking) return;
        
        isShaking = true;
        Debug.Log("JBCamera: 开始相机抖动+缩放组合效果");
        
        // 同时执行抖动和缩放
        transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, true)
            .OnComplete(() => {
                transform.localPosition = originalPosition;
                isShaking = false;
            });
            
        transform.DOPunchScale(Vector3.one * punchScale, punchDuration, punchVibrato, punchElasticity)
            .OnComplete(() => {
                transform.localScale = originalScale;
            });
    }
    
    // 重置相机到原始状态
    public void ResetCamera()
    {
        transform.localPosition = originalPosition;
        transform.localScale = originalScale;
        isShaking = false;
        Debug.Log("JBCamera: 相机已重置到原始状态");
    }
}
