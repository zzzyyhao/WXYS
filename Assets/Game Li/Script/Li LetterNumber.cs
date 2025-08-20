using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiLetterNumber : MonoBehaviour
{
    [Header("数字物体引用")]
    [SerializeField] private GameObject number0; // 拖拽数字0的物体到这里
    [SerializeField] private GameObject number1; // 拖拽数字1的物体到这里
    [SerializeField] private GameObject number2; // 拖拽数字2的物体到这里
    [SerializeField] private GameObject number3; // 拖拽数字3的物体到这里
    [SerializeField] private GameObject number4; // 拖拽数字4的物体到这里
    [SerializeField] private GameObject number5; // 拖拽数字5的物体到这里
    
    [Header("特殊计数管理")]
    public int thirdDestroyedCount = 0; // third预制体被PlayerController销毁的数量
    
    [Header("调试信息")]
    [SerializeField] private string debugInfo = "等待游戏开始...";
    
    // 数字物体数组
    private GameObject[] numberObjects;
    
    void Start()
    {
        // 初始化特殊计数
        InitializeSpecialCounts();
        
        // 初始化数字物体数组
        InitializeNumberObjects();
        
        // 设置初始数字为0
        UpdateLetterNumberDisplay(0);
        
        Debug.Log("LiLetterNumber: 初始化完成");
    }
    
    // 初始化特殊计数
    void InitializeSpecialCounts()
    {
        thirdDestroyedCount = 0;
        UpdateDebugInfo();
    }
    
    // 初始化数字物体数组
    void InitializeNumberObjects()
    {
        numberObjects = new GameObject[6];
        numberObjects[0] = number0;
        numberObjects[1] = number1;
        numberObjects[2] = number2;
        numberObjects[3] = number3;
        numberObjects[4] = number4;
        numberObjects[5] = number5;
        
        // 检查所有数字物体是否正确设置
        for (int i = 0; i < 6; i++)
        {
            if (numberObjects[i] == null)
            {
                Debug.LogWarning($"LiLetterNumber: 数字 {i} 的物体未设置！");
            }
            else
            {
                Debug.Log($"LiLetterNumber: 数字 {i} 的物体已设置: {numberObjects[i].name}");
            }
        }
    }
    
    // 更新数字显示（通过开关子物体）
    void UpdateLetterNumberDisplay(int number)
    {
        if (number < 0 || number >= 6)
        {
            Debug.LogWarning($"LiLetterNumber: 数字 {number} 超出范围 (0-5)");
            return;
        }
        
        Debug.Log($"LiLetterNumber: 开始更新数字显示为 {number}");
        
        // 关闭所有数字物体
        for (int i = 0; i < 6; i++)
        {
            if (numberObjects[i] != null)
            {
                numberObjects[i].SetActive(false);
                Debug.Log($"LiLetterNumber: 关闭数字 {i} 的物体");
            }
            else
            {
                Debug.LogWarning($"LiLetterNumber: 数字 {i} 的物体为空，无法关闭");
            }
        }
        
        // 只显示当前数字
        if (numberObjects[number] != null)
        {
            numberObjects[number].SetActive(true);
            Debug.Log($"LiLetterNumber: 显示数字 {number} 的物体: {numberObjects[number].name}");
        }
        else
        {
            Debug.LogError($"LiLetterNumber: 数字 {number} 的物体为空，无法显示！");
        }
    }
    
    // 公共方法：记录third被PlayerController销毁
    public void OnThirdDestroyed()
    {
        thirdDestroyedCount++;
        
        Debug.Log($"LiLetterNumber: Third销毁计数增加到 {thirdDestroyedCount}");
        
        // 更新数字显示
        UpdateLetterNumberDisplay(thirdDestroyedCount);
        
        // 更新调试信息
        UpdateDebugInfo();
        
        // 检查是否达到游戏结束条件（特殊计数达到5）
        if (thirdDestroyedCount >= 5)
        {
            Debug.Log("LiLetterNumber: 特殊计数达到5，触发游戏结束！");
            TriggerGameOver();
        }
    }

    // 触发游戏结束
    private void TriggerGameOver()
    {
        // 查找GameOverManager并触发游戏结束
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
            var triggerMethod = gameOverManager.GetType().GetMethod("TriggerGameOver");
            if (triggerMethod != null)
            {
                triggerMethod.Invoke(gameOverManager, null);
                Debug.Log("LiLetterNumber: 通过GameOverManager触发游戏结束");
            }
            else
            {
                Debug.LogError("LiLetterNumber: 未找到TriggerGameOver方法！");
            }
        }
        else
        {
            Debug.LogError("LiLetterNumber: 未找到LiGameOverManager！");
        }
    }
    
    // 更新调试信息
    void UpdateDebugInfo()
    {
        debugInfo = $"Third: {thirdDestroyedCount}/5";
    }
    
    // 手动测试方法（用于调试）
    [ContextMenu("测试Third销毁")]
    public void TestThirdDestroyed()
    {
        OnThirdDestroyed();
    }
    
    [ContextMenu("重置特殊计数")]
    public void ResetSpecialCounts()
    {
        thirdDestroyedCount = 0;
        UpdateLetterNumberDisplay(0);
        UpdateDebugInfo();
        Debug.Log("LiLetterNumber: 特殊计数已重置");
    }
    
    // 公共方法：获取特殊计数信息
    public int GetThirdDestroyedCount()
    {
        return thirdDestroyedCount;
    }
    
    // 公共方法：设置特殊计数（用于外部重置）
    public void SetThirdDestroyedCount(int count)
    {
        thirdDestroyedCount = count;
        UpdateLetterNumberDisplay(thirdDestroyedCount);
        UpdateDebugInfo();
    }
    
    // 公共方法：手动设置数字显示
    public void SetNumberDisplay(int number)
    {
        UpdateLetterNumberDisplay(number);
        Debug.Log($"LiLetterNumber: 手动设置数字显示为 {number}");
    }
    
    // 公共方法：获取当前显示的数字
    public int GetCurrentDisplayNumber()
    {
        for (int i = 0; i < 6; i++)
        {
            if (numberObjects[i] != null && numberObjects[i].activeInHierarchy)
            {
                return i;
            }
        }
        return -1; // 没有找到活动的数字
    }
    
    // 公共方法：检查数字物体是否正确设置
    public bool AreNumberObjectsSet()
    {
        for (int i = 0; i < 6; i++)
        {
            if (numberObjects[i] == null)
            {
                return false;
            }
        }
        return true;
    }
    
    // 公共方法：测试所有数字显示
    [ContextMenu("测试所有数字显示")]
    public void TestAllNumbers()
    {
        Debug.Log("=== 测试所有数字显示 ===");
        for (int i = 0; i <= 5; i++)
        {
            Debug.Log($"测试显示数字 {i}");
            UpdateLetterNumberDisplay(i);
            // 等待一小段时间再测试下一个
            StartCoroutine(TestNumberWithDelay(i));
        }
    }
    
    // 协程：延迟测试数字显示
    private IEnumerator TestNumberWithDelay(int number)
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log($"数字 {number} 显示测试完成");
    }
}
