using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("移动控制")]
    public bool keepYAxisFixed = true; // 勾选框：是否保持Y轴不变

    private LiGameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<LiGameManager>();
    }

    void Update()
    {
        // 获取相机边界
        float halfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float leftLimit = -halfWidth;
        float rightLimit = halfWidth;

        // 获取鼠标在世界坐标中的位置，添加边界检查
        Vector3 mousePos = Input.mousePosition;
        
        // 检查鼠标位置是否有效
        if (float.IsInfinity(mousePos.x) || float.IsInfinity(mousePos.y) || 
            float.IsNaN(mousePos.x) || float.IsNaN(mousePos.y))
        {
            return; // 如果鼠标位置无效，直接返回
        }
        
        // 确保鼠标在屏幕范围内
        mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);
        
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // 根据勾选框设置位置
        Vector3 newPos = transform.position;
        newPos.x = Mathf.Clamp(worldPos.x, leftLimit, rightLimit);
        
        if (keepYAxisFixed)
        {
            // 保持Y轴不变
            newPos.y = transform.position.y;
        }
        else
        {
            // Y轴跟随鼠标
            newPos.y = worldPos.y;
        }
        
        // Z轴始终保持不变
        newPos.z = transform.position.z;
        
        transform.position = newPos;
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
