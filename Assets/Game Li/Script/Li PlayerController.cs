using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        gameManager = FindObjectOfType<LiGameManager>();
        
        // 缓存相机引用
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }

    void Update()
    {
        // 获取相机边界 - 使用缓存的相机引用
        float halfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float leftLimit = -halfWidth;
        float rightLimit = halfWidth;

        // 获取鼠标在世界坐标中的位置，添加边界检查 - 重用Vector3避免GC
        tempMousePos.Set(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        
        // 检查鼠标位置是否有效
        if (float.IsInfinity(tempMousePos.x) || float.IsInfinity(tempMousePos.y) || 
            float.IsNaN(tempMousePos.x) || float.IsNaN(tempMousePos.y))
        {
            return; // 如果鼠标位置无效，直接返回
        }
        
        // 确保鼠标在屏幕范围内
        tempMousePos.x = Mathf.Clamp(tempMousePos.x, 0, Screen.width);
        tempMousePos.y = Mathf.Clamp(tempMousePos.y, 0, Screen.height);
        
        tempWorldPos = mainCamera.ScreenToWorldPoint(tempMousePos);

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
            // Y轴跟随鼠标
            tempNewPos.y = tempWorldPos.y;
        }
        
        // Z轴始终保持不变
        tempNewPos.z = transform.position.z;
        
        transform.position = tempNewPos;
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
                // 通知GameManager记录third被销毁
                if (gameManager != null)
                {
                    gameManager.OnThirdDestroyed();
                }
            }
            // 检查是否为fourth预制体
            else if (objName.Contains("fourth") || objName.Contains("Fourth") ||
                     objName.Contains("purple") || objName.Contains("Purple"))
            {
                // 通知GameManager记录fourth被销毁
                if (gameManager != null)
                {
                    gameManager.OnFourthDestroyed();
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
