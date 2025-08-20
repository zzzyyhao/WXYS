using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("移动控制")]
    public bool keepYAxisFixed = true; // 勾选框：是否保持Y轴不变

    private LiGameManager gameManager;
    
    // 缓存组件引用，避免重复查找
    private Camera mainCamera;
    private Vector3 tempMousePos = Vector3.zero; // 重用Vector3，避免GC
    private Vector3 tempWorldPos = Vector3.zero; // 重用Vector3，避免GC
    private Vector3 tempNewPos = Vector3.zero; // 重用Vector3，避免GC
    
    // 添加状态变量来管理UI交互后的移动延迟
    private bool wasSettingPanelOpen = false; // 上一帧设置面板是否打开
    private bool isWaitingForMouseLeave = false; // 是否正在等待鼠标离开UI区域
    
    // Add force delay mechanism
    private bool isInForceDelay = false; // Whether in force delay period
    private float forceDelayTimer = 0f; // Force delay timer
    private const float FORCE_DELAY_TIME = 0.3f; // Force delay 0.3 seconds
    
    // 添加鼠标输入忽略机制
    private bool shouldIgnoreMouseInput = false; // 是否应该忽略鼠标输入
    private float mouseIgnoreTimer = 0f; // 鼠标忽略计时器
    private const float MOUSE_IGNORE_TIME = 0.5f; // 鼠标忽略时间0.5秒
    
    // 添加UI点击坐标忽略机制
    private Vector3 lastValidInputPosition; // 最后一个有效的输入位置
    
    // 特殊计数管理器引用
    private LiLetterNumber letterNumberManager;

    void Start()
    {
        gameManager = FindObjectOfType<LiGameManager>();
        
        // 查找特殊计数管理器
        letterNumberManager = FindObjectOfType<LiLetterNumber>();
        
        // 缓存相机引用
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }

    void Update()
    {
        bool isSettingPanelOpen = IsSettingPanelOpen();
        
        // 检查设置面板是否打开，如果打开则不移动平台
        if (isSettingPanelOpen)
        {
            // 重置所有状态
            isWaitingForMouseLeave = false;
            isInForceDelay = false;
            forceDelayTimer = 0f;
            shouldIgnoreMouseInput = false;
            mouseIgnoreTimer = 0f;
            lastValidInputPosition = Vector3.zero; // 重置有效输入位置
            wasSettingPanelOpen = true;
            return; // 如果设置面板打开，直接返回，不执行任何移动逻辑
        }
        
        // 检查设置面板是否刚刚关闭
        if (wasSettingPanelOpen && !isSettingPanelOpen)
        {
            // 设置面板刚刚关闭，启动强制延迟和鼠标忽略
            isWaitingForMouseLeave = true;
            isInForceDelay = true; // 启动强制延迟
            forceDelayTimer = 0f; // 重置计时器
            shouldIgnoreMouseInput = true; // 启动鼠标忽略
            mouseIgnoreTimer = 0f; // 重置鼠标忽略计时器
            lastValidInputPosition = Vector3.zero; // 重置有效输入位置，防止使用UI点击坐标
            wasSettingPanelOpen = false;
            Debug.Log("PlayerController: 设置面板关闭，启动强制延迟和鼠标忽略，重置输入位置");
            return; // 立即返回，不执行任何移动逻辑
        }
        
        // 处理强制延迟
        if (isInForceDelay)
        {
            forceDelayTimer += Time.deltaTime;
            if (forceDelayTimer >= FORCE_DELAY_TIME)
            {
                isInForceDelay = false; // 强制延迟结束
                Debug.Log("PlayerController: 强制延迟结束");
            }
            else
            {
                // 在强制延迟期间，不执行任何移动逻辑
                return;
            }
        }
        
        // 处理鼠标输入忽略
        if (shouldIgnoreMouseInput)
        {
            mouseIgnoreTimer += Time.deltaTime;
            if (mouseIgnoreTimer >= MOUSE_IGNORE_TIME)
            {
                shouldIgnoreMouseInput = false; // 鼠标忽略结束
                Debug.Log("PlayerController: 鼠标忽略结束");
            }
            else
            {
                // 在鼠标忽略期间，不执行任何移动逻辑
                return;
            }
        }
        
        // 如果正在等待鼠标离开UI区域
        if (isWaitingForMouseLeave)
        {
            // 检查鼠标是否还在UI上
            if (IsPointerOverUI())
            {
                return; // 鼠标还在UI上，继续等待，不执行任何移动逻辑
            }
            else
            {
                // 鼠标已离开UI区域，恢复移动
                isWaitingForMouseLeave = false;
                Debug.Log("PlayerController: 鼠标离开UI区域，恢复移动");
            }
        }
        
        // 检查是否点击在UI上，如果是则不移动平台
        if (IsPointerOverUI())
        {
            return; // 如果点击在UI上，直接返回，不执行任何移动逻辑
        }
        
        // 执行移动逻辑
        HandlePlatformMovement();
    }

    // 处理平台移动逻辑
    void HandlePlatformMovement()
    {
        // 如果应该忽略鼠标输入，直接返回
        if (shouldIgnoreMouseInput)
        {
            return;
        }
        
        // 获取相机边界 - 使用缓存的相机引用
        float halfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float leftLimit = -halfWidth;
        float rightLimit = halfWidth;

        // 获取输入位置（支持鼠标和触摸）
        Vector3 inputPosition = GetInputPosition();
        
        // 检查输入位置是否有效
        if (float.IsInfinity(inputPosition.x) || float.IsInfinity(inputPosition.y) || 
            float.IsNaN(inputPosition.x) || float.IsNaN(inputPosition.y))
        {
            return; // 如果输入位置无效，直接返回
        }
        
        // 检查是否点击在UI上，如果是则忽略这次输入
        if (IsPointerOverUI())
        {
            return; // 如果点击在UI上，忽略这次输入
        }
        
        // 确保输入位置在屏幕范围内
        inputPosition.x = Mathf.Clamp(inputPosition.x, 0, Screen.width);
        inputPosition.y = Mathf.Clamp(inputPosition.y, 0, Screen.height);
        
        // 保存有效的输入位置
        lastValidInputPosition = inputPosition;
        
        tempWorldPos = mainCamera.ScreenToWorldPoint(inputPosition);

        // 根据勾选框设置位置 - 重用Vector3避免GC
        tempNewPos.Set(transform.position.x, transform.position.y, transform.position.z);
        tempNewPos.x = Mathf.Clamp(tempWorldPos.x, leftLimit, rightLimit);
        
        if (keepYAxisFixed)
        {
            // 保持Y轴不变
            tempNewPos.y = transform.position.y;
        }
        else
        {
            // Y轴跟随输入
            tempNewPos.y = tempWorldPos.y;
        }
        
        // Z轴始终保持不变
        tempNewPos.z = transform.position.z;
        
        transform.position = tempNewPos;
    }
    
    // 获取输入位置（支持鼠标和触摸）
    Vector3 GetInputPosition()
    {
        // 如果应该忽略鼠标输入，返回无效位置
        if (shouldIgnoreMouseInput)
        {
            return Vector3.negativeInfinity;
        }
        
        // 优先使用触摸输入（移动设备）
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            // 检查触摸是否在UI上
            if (IsPointerOverUI())
            {
                return Vector3.negativeInfinity; // 返回无效位置
            }
            return new Vector3(touch.position.x, touch.position.y, 0);
        }
        
        // 使用鼠标输入（PC设备）
        Vector3 mousePos = Input.mousePosition;
        
        // 检查鼠标是否在UI上
        if (IsPointerOverUI())
        {
            return Vector3.negativeInfinity; // 返回无效位置
        }
        
        return mousePos;
    }

    // 检查鼠标是否在UI元素上
    bool IsPointerOverUI()
    {
        // 检查是否有EventSystem
        if (EventSystem.current == null)
        {
            Debug.LogWarning("PlayerController: EventSystem.current 为空！");
            return false;
        }
        
        // 方法1：使用EventSystem的IsPointerOverGameObject
        bool isOverUI1 = EventSystem.current.IsPointerOverGameObject();
        
        // 方法2：使用射线检测UI元素
        bool isOverUI2 = IsPointerOverUIWithRaycast();
        
        // 方法3：检查是否点击了按钮
        bool isOverUI3 = IsClickingButton();
        
        // 使用最严格的检测：任一方法检测到UI就返回true
        bool isOverUI = isOverUI1 || isOverUI2 || isOverUI3;
        
        // 只在调试模式下输出调试信息
        if (Debug.isDebugBuild && (Input.GetMouseButton(0) || Input.touchCount > 0))
        {
            Debug.Log($"PlayerController: UI检测 - EventSystem: {isOverUI1}, Raycast: {isOverUI2}, Button: {isOverUI3}, 最终结果: {isOverUI}");
        }
        
        return isOverUI;
    }
    
    // 检查是否点击了按钮
    bool IsClickingButton()
    {
        // 检查鼠标是否按下
        if (!Input.GetMouseButton(0) && Input.touchCount == 0)
        {
            return false;
        }
        
        // 获取鼠标位置
        Vector2 mousePos = Input.mousePosition;
        
        // 检查是否在屏幕上半部分（UI通常在上半部分）
        bool isInUpperHalf = mousePos.y > Screen.height * 0.5f;
        
        // 检查是否在屏幕顶部区域（按钮通常在这里）
        bool isInTopArea = mousePos.y > Screen.height * 0.7f;
        
        // 如果鼠标在上半部分且正在点击，认为可能是在点击UI
        return (isInUpperHalf || isInTopArea) && (Input.GetMouseButton(0) || Input.touchCount > 0);
    }
    
    // 使用射线检测UI元素
    bool IsPointerOverUIWithRaycast()
    {
        // 创建射线数据
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        
        // 射线检测结果列表
        List<RaycastResult> results = new List<RaycastResult>();
        
        // 获取所有GraphicRaycaster组件
        GraphicRaycaster[] raycasters = FindObjectsOfType<GraphicRaycaster>();
        
        if (raycasters.Length == 0)
        {
            Debug.LogWarning("PlayerController: 未找到GraphicRaycaster组件！");
            return false;
        }
        
        foreach (GraphicRaycaster raycaster in raycasters)
        {
            raycaster.Raycast(eventData, results);
        }
        
        // 如果射线击中了UI元素，返回true
        bool hasHit = results.Count > 0;
        
        // 只在调试模式下输出详细信息
        if (hasHit && Debug.isDebugBuild)
        {
            Debug.Log($"PlayerController: 射线检测到 {results.Count} 个UI元素");
            foreach (var result in results)
            {
                Debug.Log($"PlayerController: 击中UI元素: {result.gameObject.name}");
            }
        }
        
        return hasHit;
    }

    // 检查设置面板是否打开
    bool IsSettingPanelOpen()
    {
        // 查找UI管理器
        LiUIManager uiManager = FindObjectOfType<LiUIManager>();
        if (uiManager != null)
        {
            return uiManager.IsSettingPanelOpen();
        }
        return false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Falling"))
        {
            // 添加详细的名称检查调试信息
            string objName = collision.gameObject.name;
            
            // 检查是否为first预制体（兼容旧名称）
            if (objName.Contains("first") || objName.Contains("First") ||
                objName.Contains("green") || objName.Contains("Green"))
            {
                // 通知GameManager记录first被销毁
                if (gameManager != null)
                {
                    gameManager.OnFirstDestroyed();
                }
            }
            // 检查是否为second预制体（兼容旧名称）
            else if (objName.Contains("second") || objName.Contains("Second") ||
                     objName.Contains("red") || objName.Contains("Red"))
            {
                // 通知GameManager记录second被销毁
                if (gameManager != null)
                {
                    gameManager.OnSecondDestroyed();
                }
            }
            // 检查是否为third预制体
            else if (objName.Contains("third") || objName.Contains("Third") ||
                     objName.Contains("blue") || objName.Contains("Blue"))
            {
                // 只通知GameManager，避免重复调用
                if (gameManager != null)
                {
                    gameManager.OnThirdDestroyed();
                }
            }
            
            // 销毁物体并根据预制体类型加分
            if (gameManager != null)
            {
                gameManager.AddScoreByPrefabType(collision.gameObject.name);
            }
            
            Destroy(collision.gameObject);
        }
    }
}
