using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LiMemoryManager : MonoBehaviour
{
    [Header("内存优化设置")]
    [SerializeField] private bool enableMemoryOptimization = true;
    [SerializeField] private float gcInterval = 30f; // 垃圾回收间隔
    [SerializeField] private int maxActiveObjects = 150; // 最大活跃对象数量
    [SerializeField] private float cleanupInterval = 15f; // 清理间隔
    
    [Header("垃圾回收优化设置")]
    [SerializeField] private bool useAsyncGC = true; // 使用异步垃圾回收
    [SerializeField] private bool useIncrementalGC = true; // 使用增量垃圾回收
    [SerializeField] private int gcFrameBudget = 2; // 每帧垃圾回收预算（毫秒）
    [SerializeField] private bool showGCPerformance = true; // 显示垃圾回收性能信息
    
    [Header("对象池设置")]
    [SerializeField] private bool useObjectPooling = true;
    [SerializeField] private int poolSize = 20; // 对象池大小
    
    // 对象池 - 使用通用对象池
    private Queue<GameObject> objectPool = new Queue<GameObject>();
    
    // 缓存
    private List<GameObject> objectsToDestroy = new List<GameObject>();
    
    // 计时器
    private float lastGCTime = 0f;
    private float lastCleanupTime = 0f;
    
    // 垃圾回收性能监控
    private float lastGCDuration = 0f;
    private float averageGCDuration = 0f;
    private int gcCount = 0;
    
    // 异步垃圾回收状态
    private bool isGCRunning = false;
    private Coroutine gcCoroutine;
    
    void Start()
    {
        InitializeMemoryManager();
    }
    
    void Update()
    {
        if (!enableMemoryOptimization) return;
        
        // 定期垃圾回收
        if (Time.time - lastGCTime >= gcInterval && !isGCRunning)
        {
            if (useAsyncGC)
            {
                StartAsyncGarbageCollection();
            }
            else
            {
                PerformGarbageCollection();
            }
            lastGCTime = Time.time;
        }
        
        // 定期清理
        if (Time.time - lastCleanupTime >= cleanupInterval)
        {
            CleanupExcessiveObjects();
            lastCleanupTime = Time.time;
        }
    }
    
    private void InitializeMemoryManager()
    {
        lastGCTime = Time.time;
        lastCleanupTime = Time.time;
        
        // 设置垃圾回收模式
        if (useIncrementalGC)
        {
            System.GC.TryStartNoGCRegion(1024 * 1024); // 尝试启动无GC区域
        }
        
        Debug.Log("LiMemoryManager: 内存管理器已初始化");
    }
    
    // 从对象池获取对象
    public GameObject GetFromPool(Vector3 position)
    {
        if (!useObjectPooling || objectPool.Count == 0) return null;
        
        GameObject obj = objectPool.Dequeue();
        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }
    
    // 将对象返回池中
    public void ReturnToPool(GameObject obj)
    {
        if (!useObjectPooling || obj == null) return;
        
        if (objectPool.Count < poolSize)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            objectPool.Enqueue(obj);
        }
        else
        {
            // 池满了，直接销毁
            Destroy(obj);
        }
    }
    
    // 添加对象到池中
    public void AddToPool(GameObject obj)
    {
        if (obj != null && objectPool.Count < poolSize)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            objectPool.Enqueue(obj);
        }
    }
    
    // 优化的垃圾回收方法
    private void PerformGarbageCollection()
    {
        if (isGCRunning) return;
        
        float startTime = Time.realtimeSinceStartup;
        Debug.Log("LiMemoryManager: 开始执行垃圾回收");
        
        // 使用更温和的垃圾回收策略
        if (useIncrementalGC)
        {
            // 增量垃圾回收 - 分步执行
            StartCoroutine(IncrementalGarbageCollection());
        }
        else
        {
            // 传统垃圾回收
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            
            // 记录性能
            float duration = (Time.realtimeSinceStartup - startTime) * 1000f;
            UpdateGCPerformance(duration);
            
            Debug.Log($"LiMemoryManager: 垃圾回收完成，耗时: {duration:F2}ms");
        }
    }
    
    // 异步垃圾回收
    private void StartAsyncGarbageCollection()
    {
        if (gcCoroutine != null)
        {
            StopCoroutine(gcCoroutine);
        }
        gcCoroutine = StartCoroutine(AsyncGarbageCollection());
    }
    
    // 异步垃圾回收协程
    private IEnumerator AsyncGarbageCollection()
    {
        isGCRunning = true;
        float startTime = Time.realtimeSinceStartup;
        
        Debug.Log("LiMemoryManager: 开始异步垃圾回收");
        
        // 等待一帧，让当前帧完成
        yield return null;
        
        // 执行垃圾回收
        System.GC.Collect();
        
        // 等待一帧
        yield return null;
        
        // 等待待处理的终结器
        System.GC.WaitForPendingFinalizers();
        
        // 再次等待一帧
        yield return null;
        
        // 最终收集
        System.GC.Collect();
        
        // 记录性能
        float duration = (Time.realtimeSinceStartup - startTime) * 1000f;
        UpdateGCPerformance(duration);
        
        Debug.Log($"LiMemoryManager: 异步垃圾回收完成，耗时: {duration:F2}ms");
        isGCRunning = false;
    }
    
    // 增量垃圾回收
    private IEnumerator IncrementalGarbageCollection()
    {
        isGCRunning = true;
        float startTime = Time.realtimeSinceStartup;
        float frameStartTime;
        
        Debug.Log("LiMemoryManager: 开始增量垃圾回收");
        
        // 第一步：标记阶段
        frameStartTime = Time.realtimeSinceStartup;
        System.GC.Collect();
        
        // 如果超过帧预算，等待下一帧
        while ((Time.realtimeSinceStartup - frameStartTime) * 1000f > gcFrameBudget)
        {
            yield return null;
            frameStartTime = Time.realtimeSinceStartup;
        }
        
        // 第二步：等待终结器
        frameStartTime = Time.realtimeSinceStartup;
        System.GC.WaitForPendingFinalizers();
        
        while ((Time.realtimeSinceStartup - frameStartTime) * 1000f > gcFrameBudget)
        {
            yield return null;
            frameStartTime = Time.realtimeSinceStartup;
        }
        
        // 第三步：最终收集
        frameStartTime = Time.realtimeSinceStartup;
        System.GC.Collect();
        
        while ((Time.realtimeSinceStartup - frameStartTime) * 1000f > gcFrameBudget)
        {
            yield return null;
            frameStartTime = Time.realtimeSinceStartup;
        }
        
        // 记录性能
        float duration = (Time.realtimeSinceStartup - startTime) * 1000f;
        UpdateGCPerformance(duration);
        
        Debug.Log($"LiMemoryManager: 增量垃圾回收完成，耗时: {duration:F2}ms");
        isGCRunning = false;
    }
    
    // 更新垃圾回收性能统计
    private void UpdateGCPerformance(float duration)
    {
        lastGCDuration = duration;
        gcCount++;
        
        // 计算平均耗时
        if (gcCount == 1)
        {
            averageGCDuration = duration;
        }
        else
        {
            averageGCDuration = (averageGCDuration * (gcCount - 1) + duration) / gcCount;
        }
    }
    
    private void CleanupExcessiveObjects()
    {
        // 查找所有Falling对象
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int count = allObjects.Length;
        
        if (count > maxActiveObjects)
        {
            int toRemove = count - maxActiveObjects;
            objectsToDestroy.Clear();
            
            // 收集需要销毁的对象
            for (int i = 0; i < count; i++)
            {
                GameObject obj = allObjects[i];
                if (obj != null && obj.CompareTag("Falling"))
                {
                    // 检查是否超出屏幕
                    Vector3 screenPos = Camera.main.WorldToViewportPoint(obj.transform.position);
                    if (screenPos.y < -0.5f || screenPos.y > 1.5f)
                    {
                        objectsToDestroy.Add(obj);
                        if (objectsToDestroy.Count >= toRemove) break;
                    }
                }
            }
            
            // 销毁对象
            foreach (GameObject obj in objectsToDestroy)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            
            Debug.Log($"LiMemoryManager: 清理了 {objectsToDestroy.Count} 个超出屏幕的对象");
        }
    }
    
    // 公共方法：手动垃圾回收
    public void ManualGarbageCollection()
    {
        if (useAsyncGC)
        {
            StartAsyncGarbageCollection();
        }
        else
        {
            PerformGarbageCollection();
        }
    }
    
    // 公共方法：手动清理对象
    public void ManualObjectCleanup()
    {
        CleanupExcessiveObjects();
    }
    
    // 公共方法：获取内存使用信息
    public string GetMemoryInfo()
    {
        long totalMemory = System.GC.GetTotalMemory(false);
        long usedMemory = System.GC.GetTotalMemory(true);
        float memoryUsage = (float)usedMemory / totalMemory;
        
        string gcInfo = "";
        if (showGCPerformance && gcCount > 0)
        {
            gcInfo = $"\n垃圾回收: 平均耗时 {averageGCDuration:F1}ms (共{gcCount}次)";
        }
        
        return $"内存使用: {usedMemory / (1024 * 1024):F1}MB / {totalMemory / (1024 * 1024):F1}MB ({memoryUsage:P1}){gcInfo}";
    }
    
    // 公共方法：获取对象池信息
    public string GetPoolInfo()
    {
        return $"对象池: {objectPool.Count}/{poolSize}";
    }
    
    // 公共方法：设置优化参数
    public void SetOptimizationParameters(float gcInterval, int maxObjects, float cleanupInterval)
    {
        this.gcInterval = Mathf.Max(1f, gcInterval);
        this.maxActiveObjects = Mathf.Max(10, maxObjects);
        this.cleanupInterval = Mathf.Max(1f, cleanupInterval);
    }
    
    // 公共方法：设置垃圾回收参数
    public void SetGCParameters(bool useAsync, bool useIncremental, int frameBudget)
    {
        useAsyncGC = useAsync;
        useIncrementalGC = useIncremental;
        gcFrameBudget = Mathf.Max(1, frameBudget);
    }
    
    // 公共方法：启用/禁用内存优化
    public void SetMemoryOptimization(bool enabled)
    {
        enableMemoryOptimization = enabled;
        Debug.Log($"LiMemoryManager: 内存优化已{(enabled ? "启用" : "禁用")}");
    }
    
    // 公共方法：启用/禁用对象池
    public void SetObjectPooling(bool enabled)
    {
        useObjectPooling = enabled;
        Debug.Log($"LiMemoryManager: 对象池已{(enabled ? "启用" : "禁用")}");
    }
    
    // 公共方法：清理对象池
    public void ClearObjectPool()
    {
        while (objectPool.Count > 0)
        {
            GameObject obj = objectPool.Dequeue();
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        Debug.Log("LiMemoryManager: 对象池已清理");
    }
    
    // 公共方法：获取垃圾回收状态
    public bool IsGCRunning()
    {
        return isGCRunning;
    }
    
    // 公共方法：获取垃圾回收性能信息
    public string GetGCPerformanceInfo()
    {
        if (gcCount == 0) return "暂无垃圾回收数据";
        
        return $"垃圾回收统计:\n" +
               $"总次数: {gcCount}\n" +
               $"平均耗时: {averageGCDuration:F2}ms\n" +
               $"最近一次: {lastGCDuration:F2}ms\n" +
               $"当前状态: {(isGCRunning ? "运行中" : "空闲")}";
    }
} 