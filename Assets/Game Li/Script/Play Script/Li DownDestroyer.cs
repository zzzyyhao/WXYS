using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiDownDestroyer : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private LiGameManager gameManager;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            // 请给DownDestroyer添加BoxCollider2D组件！
        }
        else
        {
            boxCollider.isTrigger = true; // 设为触发器
        }
    }

    void Start()
    {
        gameManager = FindObjectOfType<LiGameManager>();
    }

    void Update()
    {
        // 让DownDestroyer宽度为相机宽度的10倍，并X轴居中
        Vector3 pos = transform.position;
        pos.x = Camera.main.transform.position.x;
        transform.position = pos;

        if (boxCollider != null)
        {
            float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
            float camWidth = camHalfWidth * 2f * 10f; // 10倍
            boxCollider.size = new Vector2(camWidth, boxCollider.size.y);
            boxCollider.offset = Vector2.zero;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否为下落的预制体
        if (other.CompareTag("Falling"))
        {
            // DownDestroyer只销毁物体，不记录任何销毁数量
            // 只有PlayerController销毁的预制体才会被记录
            
            // 销毁物体
            Destroy(other.gameObject);
        }
    }
}
