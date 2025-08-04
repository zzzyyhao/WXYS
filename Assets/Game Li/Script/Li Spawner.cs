using UnityEngine;

public class LiSpawner : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
           // Debug.LogError("请给Spawner添加BoxCollider2D组件！");
        }
    }

    void Update()
    {
        // 1. 保持Spawner中心与相机中心对齐
        Vector3 pos = transform.position;
        pos.x = Camera.main.transform.position.x;
        transform.position = pos;

        // 2. 适配宽度为相机宽度的90%
        if (boxCollider != null)
        {
            float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
            float camWidth = camHalfWidth * 2f * 0.9f; // 90%
            boxCollider.size = new Vector2(camWidth, boxCollider.size.y);
            boxCollider.offset = Vector2.zero;
        }
    }

    // 供GameManager调用的生成方法
    public GameObject SpawnSpecificPrefab(GameObject specificPrefab)
    {
        if (specificPrefab == null || boxCollider == null) return null;

        // 获取Spawner中心和宽度
        float centerX = transform.position.x;
        float width = boxCollider.size.x * transform.lossyScale.x;

        // 计算左右边界
        float left = centerX - width / 2f;
        float right = centerX + width / 2f;

        // 随机X
        float randomX = Random.Range(left, right);

        // 计算下边界Y
        float height = boxCollider.size.y * transform.lossyScale.y;
        float bottomY = transform.position.y - height / 2f;

        // 生成在下边界并返回生成的GameObject
        return Instantiate(specificPrefab, new Vector3(randomX, bottomY, 0), Quaternion.identity);
    }
}
