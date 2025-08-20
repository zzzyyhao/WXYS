using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiGameManager : MonoBehaviour
{
    [Header("预制体引用")]
    public GameObject firstPrefab; // 拖拽first预制体到这里
    public GameObject secondPrefab; // 拖拽second预制体到这里
    public GameObject thirdPrefab; // 拖拽third预制体到这里

    [Header("---------------第一阶段参数---------------")]
    [Header("First预制体 - 1")]
    public float stage1FirstSpawnDelay = 1f;
    public float stage1FirstSpawnInterval = 2f;
    public float stage1FirstFallSpeed = 2f;
    public int stage1FirstScore = 1;

    [Header("Third预制体 - 1")]
    public bool useThirdInStage1 = false; // 是否在第一阶段使用third预制体
    public float stage1ThirdSpawnDelay = 0.5f;
    public float stage1ThirdSpawnInterval = 4f;
    public float stage1ThirdFallSpeed = 3.5f;

    [Header("第一阶段游戏任务")]
    public int stage1FirstDestroyCount = 5; // 第一阶段需要销毁多少个first预制体才能进入第二阶段


    [Header("---------------第二阶段参数---------------")]
    [Header("First预制体 - 2")]
    public float stage2FirstSpawnDelay = 1f;
    public float stage2FirstSpawnInterval = 1.5f;
    public float stage2FirstFallSpeed = 2.5f;
    public int stage2FirstScore = 1;

    [Header("Second预制体 - 2")]
    public float stage2SecondSpawnDelay = 0.5f;
    public float stage2SecondSpawnInterval = 3f;
    public float stage2SecondFallSpeed = 2.5f;
    public int stage2SecondScore = 2;

    [Header("Third预制体 - 2")]
    public bool useThirdInStage2 = false; // 是否在第二阶段使用third预制体
    public float stage2ThirdSpawnDelay = 0.5f;
    public float stage2ThirdSpawnInterval = 3.5f;
    public float stage2ThirdFallSpeed = 4f;

    [Header("第二阶段游戏任务")]
    public int stage2FirstDestroyCount = 5; // 第二阶段需要销毁多少个first预制体
    public int stage2SecondDestroyCount = 5; // 第二阶段需要销毁多少个second预制体


    [Header("---------------第三阶段参数---------------")]
    [Header("First预制体 - 3")]
    public float stage3FirstSpawnDelay = 1f;
    public float stage3FirstSpawnInterval = 1f;
    public float stage3FirstFallSpeed = 3f;
    public int stage3FirstScore = 1;

    [Header("Second预制体 - 3")]
    public float stage3SecondSpawnDelay = 0.5f;
    public float stage3SecondSpawnInterval = 2.5f;
    public float stage3SecondFallSpeed = 3f;
    public int stage3SecondScore = 2;

    [Header("Third预制体 - 3")]
    public bool useThirdInStage3 = false; // 是否在第三阶段使用third预制体
    public float stage3ThirdSpawnDelay = 0.5f;
    public float stage3ThirdSpawnInterval = 4f;
    public float stage3ThirdFallSpeed = 3.5f;

    [Header("第三阶段游戏任务")]
    public int stage3FirstDestroyCount = 5; // 第三阶段需要销毁多少个first预制体
    public int stage3SecondDestroyCount = 5; // 第三阶段需要销毁多少个second预制体


    [Header("---------------第四阶段参数（无尽模式）---------------")]
    [Header("First预制体 - 4")]
    public float stage4FirstSpawnDelay = 1f;
    public float stage4FirstSpawnInterval = 0.8f;
    public float stage4FirstFallSpeed = 3.5f;
    public int stage4FirstScore = 1;

    [Header("First预制体递增配置")]
    public float stage4FirstSpawnDelayIncrement = 0.1f; // first预制体生成延迟递增
    public float stage4FirstSpawnIntervalIncrement = 0.1f; // first预制体生成间隔递增
    public float stage4FirstFallSpeedIncrement = 0.2f; // first预制体下落速度递增

    [Header("Second预制体 - 4")]
    public float stage4SecondSpawnDelay = 0.5f;
    public float stage4SecondSpawnInterval = 2f;
    public float stage4SecondFallSpeed = 3.5f;
    public int stage4SecondScore = 2;

    [Header("Second预制体递增配置")]
    public float stage4SecondSpawnDelayIncrement = 0.1f; // second预制体生成延迟递增
    public float stage4SecondSpawnIntervalIncrement = 0.1f; // second预制体生成间隔递增
    public float stage4SecondFallSpeedIncrement = 0.2f; // second预制体下落速度递增

    [Header("Third预制体 - 4")]
    public bool useThirdInStage4 = true; // 是否在第四阶段使用third预制体
    public float stage4ThirdSpawnDelay = 0.5f;
    public float stage4ThirdSpawnInterval = 3f;
    public float stage4ThirdFallSpeed = 4f;

    [Header("Third预制体递增配置")]
    public float stage4ThirdSpawnDelayIncrement = 0.1f; // third预制体生成延迟递增
    public float stage4ThirdSpawnIntervalIncrement = 0.1f; // third预制体生成间隔递增
    public float stage4ThirdFallSpeedIncrement = 0.2f; // third预制体下落速度递增

    [Header("第四阶段任务递增配置")]
    public int stage4FirstBaseCount = 8; // 第四阶段first预制体基础目标数量
    public int stage4SecondBaseCount = 8; // 第四阶段second预制体基础目标数量
    public int stage4FirstIncrement = 3; // 每次目标完成后first预制体目标增加数量
    public int stage4SecondIncrement = 3; // 每次目标完成后second预制体目标增加数量
    public int stage4CurrentTarget = 1; // 当前目标编号（从1开始）

    [Header("第四阶段速度恢复配置")]
    public float stage4SpeedRecoveryTime = 10f; // 速度恢复到目标值的时间（秒）
    public float stage4SpeedReductionPercent = 0.3f; // 目标切换时速度减少的百分比（0.3 = 减少30%）

    [Header("------")]
    [Header("---------------游戏状态---------------")]
    public bool isGameActive = true; // 游戏是否激活
    public int currentScore = 0; // 当前分数
    public int firstDestroyedCount = 0; // 已销毁的first数量（只记录PlayerController销毁的）
    public int secondDestroyedCount = 0; // 已销毁的second数量（只记录PlayerController销毁的）
    public bool secondSpawnEnabled = false; // 是否启用second生成
    public bool thirdSpawnEnabled = false; // 是否启用third生成
    public int currentGameStage = 1; // 当前游戏阶段：1=第一阶段，2=第二阶段，3=第三阶段，4=第四阶段（无尽模式）

    // 第四阶段动态目标
    public int stage4CurrentFirstTarget = 8; // 当前first预制体目标数量
    public int stage4CurrentSecondTarget = 8; // 当前second预制体目标数量

    // 第四阶段动态参数
    [Header("预制体1参数")]
    public float stage4CurrentFirstSpawnDelay = 1f;
    public float stage4CurrentFirstSpawnInterval = 0.8f;
    public float stage4CurrentFirstFallSpeed = 3.5f;

    [Header("预制体2参数")]
    public float stage4CurrentSecondSpawnDelay = 0.5f;
    public float stage4CurrentSecondSpawnInterval = 2f;
    public float stage4CurrentSecondFallSpeed = 3.5f;

    [Header("预制体3参数")]
    public float stage4CurrentThirdSpawnDelay = 0.5f;
    public float stage4CurrentThirdSpawnInterval = 3f;
    public float stage4CurrentThirdFallSpeed = 4f;

    // 第四阶段速度恢复相关
    [Header("第四阶段速度恢复相关")]
    public bool stage4SpeedRecoveryActive = false;
    public float stage4SpeedRecoveryTimer = 0f;
    public float stage4TargetFirstFallSpeed = 3.5f;
    public float stage4TargetSecondFallSpeed = 3.5f;
    public float stage4TargetThirdFallSpeed = 4f;

    [Header("UI引用")]
    public LiUIManager uiManager; // UI管理器引用
    
    [Header("特殊计数管理器引用")]
    public LiLetterNumber letterNumberManager; // 特殊计数管理器引用

    private float firstSpawnTimer = 0f;
    private float secondSpawnTimer = 0f;
    private float thirdSpawnTimer = 0f;

    void Start()
    {
        // 初始化游戏
        InitializeGame();
    }

    void Update()
    {
        if (isGameActive)
        {
            // 管理first预制体生成（所有阶段都生成）
            ManageFirstSpawn();
            
            // 管理second预制体生成（第二阶段和第三阶段）
            if (secondSpawnEnabled)
            {
                ManageSecondSpawn();
            }
            
            // 管理third预制体生成（第三阶段）
            if (thirdSpawnEnabled)
            {
                ManageThirdSpawn();
            }
            
            // 第四阶段速度恢复逻辑
            if (currentGameStage == 4 && stage4SpeedRecoveryActive)
            {
                ManageStage4SpeedRecovery();
            }
        }
    }

    void InitializeGame()
    {
        // 重置分数和计数
        currentScore = 0;
        firstDestroyedCount = 0;
        secondDestroyedCount = 0;
        currentGameStage = 1;
        
        // 禁用second和third预制体生成（第一阶段只生成first）
        secondSpawnEnabled = false;
        thirdSpawnEnabled = false;
        
        // 初始化第四阶段目标
        stage4CurrentTarget = 1;
        stage4CurrentFirstTarget = stage4FirstBaseCount;
        stage4CurrentSecondTarget = stage4SecondBaseCount;
        
        // 初始化第四阶段动态参数
        stage4CurrentFirstSpawnDelay = stage4FirstSpawnDelay;
        stage4CurrentFirstSpawnInterval = stage4FirstSpawnInterval;
        stage4CurrentFirstFallSpeed = stage4FirstFallSpeed;
        stage4CurrentSecondSpawnDelay = stage4SecondSpawnDelay;
        stage4CurrentSecondSpawnInterval = stage4SecondSpawnInterval;
        stage4CurrentSecondFallSpeed = stage4SecondFallSpeed;
        stage4CurrentThirdSpawnDelay = stage4ThirdSpawnDelay;
        stage4CurrentThirdSpawnInterval = stage4ThirdSpawnInterval;
        stage4CurrentThirdFallSpeed = stage4ThirdFallSpeed;
        
        // 初始化速度恢复相关
        stage4SpeedRecoveryActive = false;
        stage4SpeedRecoveryTimer = 0f;
        stage4TargetFirstFallSpeed = stage4FirstFallSpeed;
        stage4TargetSecondFallSpeed = stage4SecondFallSpeed;
        stage4TargetThirdFallSpeed = stage4ThirdFallSpeed;
        
        // 重置生成计时器
        firstSpawnTimer = stage1FirstSpawnDelay;
        secondSpawnTimer = stage2SecondSpawnDelay;
        thirdSpawnTimer = GetThirdSpawnDelay();
        
        // 检查是否启用third预制体
        CheckThirdSpawnEnabled();
        
        // 查找UI管理器
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<LiUIManager>();
        }
        
        // 查找特殊计数管理器
        if (letterNumberManager == null)
        {
            letterNumberManager = FindObjectOfType<LiLetterNumber>();
        }
        
        // 重置特殊计数
        if (letterNumberManager != null)
        {
            letterNumberManager.ResetSpecialCounts();
        }
        
        Debug.Log("LiGameManager: 游戏初始化完成，当前阶段: " + currentGameStage + 
                  ", secondSpawnEnabled: " + secondSpawnEnabled + 
                  ", thirdSpawnEnabled: " + thirdSpawnEnabled);
    }

    void ManageFirstSpawn()
    {
        firstSpawnTimer -= Time.deltaTime;
        
        if (firstSpawnTimer <= 0f)
        {
            SpawnFirstPrefab();
            // 根据当前阶段设置不同的间隔
            switch (currentGameStage)
            {
                case 1:
                    firstSpawnTimer = stage1FirstSpawnInterval;
                    break;
                case 2:
                    firstSpawnTimer = stage2FirstSpawnInterval;
                    break;
                case 3:
                    firstSpawnTimer = stage3FirstSpawnInterval;
                    break;
                case 4:
                    firstSpawnTimer = stage4CurrentFirstSpawnInterval;
                    break;
            }
        }
    }

    void ManageSecondSpawn()
    {
        secondSpawnTimer -= Time.deltaTime;
        
        if (secondSpawnTimer <= 0f)
        {
            SpawnSecondPrefab();
            // 根据当前阶段设置不同的间隔
            switch (currentGameStage)
            {
                case 2:
                    secondSpawnTimer = stage2SecondSpawnInterval;
                    break;
                case 3:
                    secondSpawnTimer = stage3SecondSpawnInterval;
                    break;
                case 4:
                    secondSpawnTimer = stage4CurrentSecondSpawnInterval;
                    break;
            }
        }
    }

    void ManageThirdSpawn()
    {
        // third预制体根据勾选框决定是否生成
        if (!thirdSpawnEnabled)
        {
            return;
        }
        
        thirdSpawnTimer -= Time.deltaTime;
        
        if (thirdSpawnTimer <= 0f)
        {
            SpawnThirdPrefab();
            // 根据当前阶段设置不同的间隔
            switch (currentGameStage)
            {
                case 1:
                    thirdSpawnTimer = stage1ThirdSpawnInterval;
                    break;
                case 2:
                    thirdSpawnTimer = stage2ThirdSpawnInterval;
                    break;
                case 3:
                    thirdSpawnTimer = stage3ThirdSpawnInterval;
                    break;
                case 4:
                    thirdSpawnTimer = stage4CurrentThirdSpawnInterval;
                    break;
            }
        }
    }

    // 缓存Spawner组件，避免重复查找
    private LiSpawner cachedSpawner;
    private Vector3 tempSpawnPosition = Vector3.zero; // 重用Vector3，避免GC
    
    void SpawnFirstPrefab()
    {
        if (firstPrefab == null) return;

        // 缓存Spawner组件，避免重复查找
        if (cachedSpawner == null)
        {
            cachedSpawner = FindObjectOfType<LiSpawner>();
        }
        
        if (cachedSpawner != null)
        {
            // 使用Spawner的生成逻辑
            GameObject first = cachedSpawner.SpawnSpecificPrefab(firstPrefab);
            if (first != null)
            {
                SetupFirstMovement(first);
            }
        }
        else
        {
            // 备用生成逻辑 - 重用Vector3避免GC
            float randomX = Random.Range(-8f, 8f);
            tempSpawnPosition.Set(randomX, 10f, 0f);
            GameObject first = Instantiate(firstPrefab, tempSpawnPosition, Quaternion.identity);
            
            SetupFirstMovement(first);
        }
    }

    void SpawnSecondPrefab()
    {
        if (secondPrefab == null) 
        {
            // Second预制体未赋值！请在Inspector中拖拽Second预制体到secondPrefab字段
            return;
        }

        // 获取Spawner的位置和宽度
        LiSpawner spawner = FindObjectOfType<LiSpawner>();
        if (spawner != null)
        {
            // 使用Spawner的生成逻辑
            GameObject second = spawner.SpawnSpecificPrefab(secondPrefab);
            if (second != null)
            {
                SetupSecondMovement(second);
            }
        }
        else
        {
            // 备用生成逻辑
            float randomX = Random.Range(-8f, 8f);
            Vector3 spawnPosition = new Vector3(randomX, 10f, 0f);
            GameObject second = Instantiate(secondPrefab, spawnPosition, Quaternion.identity);
            
            SetupSecondMovement(second);
        }
    }

    void SpawnThirdPrefab()
    {
        if (thirdPrefab == null) 
        {
            // Third预制体未赋值！请在Inspector中拖拽Third预制体到thirdPrefab字段
            return;
        }

        // 获取Spawner的位置和宽度
        LiSpawner spawner = FindObjectOfType<LiSpawner>();
        if (spawner != null)
        {
            // 使用Spawner的生成逻辑
            GameObject third = spawner.SpawnSpecificPrefab(thirdPrefab);
            if (third != null)
            {
                SetupThirdMovement(third);
            }
        }
        else
        {
            // 备用生成逻辑
            float randomX = Random.Range(-8f, 8f);
            Vector3 spawnPosition = new Vector3(randomX, 10f, 0f);
            GameObject third = Instantiate(thirdPrefab, spawnPosition, Quaternion.identity);
            
            SetupThirdMovement(third);
        }
    }

    // 缓存Layer索引，避免重复查找
    private int fallingLayerIndex = -1;
    private Vector2 tempVelocity = Vector2.zero; // 重用Vector2，避免GC
    
    void SetupFirstMovement(GameObject first)
    {
        // 缓存Layer索引，避免重复查找
        if (fallingLayerIndex == -1)
        {
            fallingLayerIndex = LayerMask.NameToLayer("Falling");
        }
        first.layer = fallingLayerIndex;
        
        // 获取或添加Rigidbody2D
        Rigidbody2D rb = first.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = first.AddComponent<Rigidbody2D>();
        }

        // 设置Rigidbody2D属性
        rb.gravityScale = 0f; // 关闭重力，实现匀速下落
        rb.drag = 0f; // 关闭阻力
        rb.angularDrag = 0f; // 关闭角阻力
        
        // 根据当前阶段设置不同的下落速度
        float fallSpeed = GetFirstFallSpeed();
        
        // 重用Vector2避免GC
        tempVelocity.Set(0f, -fallSpeed);
        rb.velocity = tempVelocity;
        
        // 设置自动销毁（恒定值500秒）
        Destroy(first, 500f);
    }
    
    // 获取First预制体的下落速度
    private float GetFirstFallSpeed()
    {
        switch (currentGameStage)
        {
            case 1: return stage1FirstFallSpeed;
            case 2: return stage2FirstFallSpeed;
            case 3: return stage3FirstFallSpeed;
            case 4: return stage4CurrentFirstFallSpeed;
            default: return 2f;
        }
    }

    void SetupSecondMovement(GameObject second)
    {
        // 设置Layer为"Falling"
        second.layer = LayerMask.NameToLayer("Falling");
        
        // 获取或添加Rigidbody2D
        Rigidbody2D rb = second.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = second.AddComponent<Rigidbody2D>();
        }

        // 设置Rigidbody2D属性
        rb.gravityScale = 0f; // 关闭重力，实现匀速下落
        rb.drag = 0f; // 关闭阻力
        rb.angularDrag = 0f; // 关闭角阻力
        
        // 根据当前阶段设置不同的下落速度
        float fallSpeed = 2.5f;
        switch (currentGameStage)
        {
            case 2:
                fallSpeed = stage2SecondFallSpeed;
                break;
            case 3:
                fallSpeed = stage3SecondFallSpeed;
                break;
            case 4:
                fallSpeed = stage4CurrentSecondFallSpeed;
                break;
        }
        
        // 设置恒定下落速度
        rb.velocity = Vector2.down * fallSpeed;
        
        // 设置自动销毁（恒定值500秒）
        Destroy(second, 500f);
    }

    void SetupThirdMovement(GameObject third)
    {
        // 设置Layer为"Falling"
        third.layer = LayerMask.NameToLayer("Falling");
        
        // 获取或添加Rigidbody2D
        Rigidbody2D rb = third.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = third.AddComponent<Rigidbody2D>();
        }

        // 设置Rigidbody2D属性
        rb.gravityScale = 0f; // 关闭重力，实现匀速下落
        rb.drag = 0f; // 关闭阻力
        rb.angularDrag = 0f; // 关闭角阻力
        
        // 根据当前阶段设置不同的下落速度
        float fallSpeed = 3.5f; // 使用stage1ThirdFallSpeed
        switch (currentGameStage)
        {
            case 1:
                fallSpeed = stage1ThirdFallSpeed;
                break;
            case 2:
                fallSpeed = stage2ThirdFallSpeed;
                break;
            case 3:
                fallSpeed = stage3ThirdFallSpeed;
                break;
            case 4:
                fallSpeed = stage4CurrentThirdFallSpeed;
                break;
        }
        
        // 设置恒定下落速度
        rb.velocity = Vector2.down * fallSpeed;
        
        // 设置自动销毁（恒定值500秒）
        Destroy(third, 500f);
    }

    // 获取third预制体的生成延迟
    float GetThirdSpawnDelay()
    {
        switch (currentGameStage)
        {
            case 1:
                return stage1ThirdSpawnDelay;
            case 2:
                return stage2ThirdSpawnDelay;
            case 3:
                return stage3ThirdSpawnDelay;
            case 4:
                return stage4CurrentThirdSpawnDelay;
            default:
                return 0.5f;
        }
    }

    // 获取third预制体的生成间隔
    float GetThirdSpawnInterval()
    {
        switch (currentGameStage)
        {
            case 1:
                return stage1ThirdSpawnInterval;
            case 2:
                return stage2ThirdSpawnInterval;
            case 3:
                return stage3ThirdSpawnInterval;
            case 4:
                return stage4CurrentThirdSpawnInterval;
            default:
                return 4f;
        }
    }

    // 检查是否启用third预制体生成
    void CheckThirdSpawnEnabled()
    {
        switch (currentGameStage)
        {
            case 1:
                thirdSpawnEnabled = useThirdInStage1;
                break;
            case 2:
                thirdSpawnEnabled = useThirdInStage2;
                break;
            case 3:
                thirdSpawnEnabled = useThirdInStage3;
                break;
            case 4:
                thirdSpawnEnabled = useThirdInStage4;
                break;
        }
    }

    // 检查阶段切换条件
    void CheckStageTransition()
    {
        switch (currentGameStage)
        {
            case 1:
                // 第一阶段：只需要销毁first预制体
                if (firstDestroyedCount >= stage1FirstDestroyCount)
                {
                    EnterStage2();
                }
                break;
            case 2:
                // 第二阶段：需要同时满足first和second的销毁条件
                if (firstDestroyedCount >= stage2FirstDestroyCount && secondDestroyedCount >= stage2SecondDestroyCount)
                {
                    EnterStage3();
                }
                break;
            case 3:
                // 第三阶段：需要同时满足first和second的销毁条件（third不作为切换条件）
                if (firstDestroyedCount >= stage3FirstDestroyCount && secondDestroyedCount >= stage3SecondDestroyCount)
                {
                    EnterStage4();
                }
                break;
            case 4:
                // 第四阶段：无尽模式，完成当前目标后进入下一个目标
                if (firstDestroyedCount >= stage4CurrentFirstTarget && secondDestroyedCount >= stage4CurrentSecondTarget)
                {
                    CompleteStage4Target();
                }
                break;
        }
    }

    void EnterStage2()
    {
        currentGameStage = 2;
        secondSpawnEnabled = true;
        secondSpawnTimer = stage2SecondSpawnDelay;
        
        // 重置特殊计数
        if (letterNumberManager != null)
        {
            letterNumberManager.ResetSpecialCounts();
        }
        
        // 检查是否启用third预制体
        CheckThirdSpawnEnabled();
        
        // 如果启用third预制体，更新计时器
        if (thirdSpawnEnabled)
        {
            thirdSpawnTimer = GetThirdSpawnDelay();
        }
    }

    void EnterStage3()
    {
        currentGameStage = 3;
        
        // 重置特殊计数
        if (letterNumberManager != null)
        {
            letterNumberManager.ResetSpecialCounts();
        }
        
        // 检查是否启用third预制体
        CheckThirdSpawnEnabled();
        
        // 如果启用third预制体，更新计时器
        if (thirdSpawnEnabled)
        {
            thirdSpawnTimer = GetThirdSpawnDelay();
        }
    }

    void EnterStage4()
    {
        currentGameStage = 4;
        
        // 启用所有预制体生成
        secondSpawnEnabled = true;
        thirdSpawnEnabled = true;
        
        // 重置计数
        firstDestroyedCount = 0;
        secondDestroyedCount = 0;
        
        // 重置特殊计数
        if (letterNumberManager != null)
        {
            letterNumberManager.ResetSpecialCounts();
        }
        
        // 设置初始目标
        stage4CurrentTarget = 1;
        stage4CurrentFirstTarget = stage4FirstBaseCount;
        stage4CurrentSecondTarget = stage4SecondBaseCount;
        
        // 初始化第四阶段动态参数
        stage4CurrentFirstSpawnDelay = stage4FirstSpawnDelay;
        stage4CurrentFirstSpawnInterval = stage4FirstSpawnInterval;
        stage4CurrentFirstFallSpeed = stage4FirstFallSpeed;
        stage4CurrentSecondSpawnDelay = stage4SecondSpawnDelay;
        stage4CurrentSecondSpawnInterval = stage4SecondSpawnInterval;
        stage4CurrentSecondFallSpeed = stage4SecondFallSpeed;
        stage4CurrentThirdSpawnDelay = stage4ThirdSpawnDelay;
        stage4CurrentThirdSpawnInterval = stage4ThirdSpawnInterval;
        stage4CurrentThirdFallSpeed = stage4ThirdFallSpeed;
        
        // 初始化目标速度
        stage4TargetFirstFallSpeed = stage4FirstFallSpeed;
        stage4TargetSecondFallSpeed = stage4SecondFallSpeed;
        stage4TargetThirdFallSpeed = stage4ThirdFallSpeed;
        
        // 重置生成计时器
        firstSpawnTimer = stage4CurrentFirstSpawnDelay;
        secondSpawnTimer = stage4CurrentSecondSpawnDelay;
        thirdSpawnTimer = GetThirdSpawnDelay();
        
        // 检查是否启用third预制体
        CheckThirdSpawnEnabled();
    }

    void CompleteStage4Target()
    {
        // 增加目标编号
        stage4CurrentTarget++;
        
        // 增加下一个目标的数量
        stage4CurrentFirstTarget += stage4FirstIncrement;
        stage4CurrentSecondTarget += stage4SecondIncrement;
        
        // 递增第四阶段参数
        stage4CurrentFirstSpawnDelay += stage4FirstSpawnDelayIncrement;
        stage4CurrentFirstSpawnInterval += stage4FirstSpawnIntervalIncrement;
        stage4CurrentFirstFallSpeed += stage4FirstFallSpeedIncrement;
        stage4CurrentSecondSpawnDelay += stage4SecondSpawnDelayIncrement;
        stage4CurrentSecondSpawnInterval += stage4SecondSpawnIntervalIncrement;
        stage4CurrentSecondFallSpeed += stage4SecondFallSpeedIncrement;
        stage4CurrentThirdSpawnDelay += stage4ThirdSpawnDelayIncrement;
        stage4CurrentThirdSpawnInterval += stage4ThirdSpawnIntervalIncrement;
        stage4CurrentThirdFallSpeed += stage4ThirdFallSpeedIncrement;
        
        // 更新目标速度
        stage4TargetFirstFallSpeed = stage4CurrentFirstFallSpeed;
        stage4TargetSecondFallSpeed = stage4CurrentSecondFallSpeed;
        stage4TargetThirdFallSpeed = stage4CurrentThirdFallSpeed;
        
        // 启动速度恢复机制
        StartStage4SpeedRecovery();
        
        // 重置计数
        firstDestroyedCount = 0;
        secondDestroyedCount = 0;
        
        // 重置特殊计数
        if (letterNumberManager != null)
        {
            letterNumberManager.ResetSpecialCounts();
        }
    }

    // 公共方法：记录first被销毁（只由PlayerController调用）
    public void OnFirstDestroyed()
    {
        firstDestroyedCount++;
        
        // 检查阶段切换条件
        CheckStageTransition();
    }

    // 公共方法：记录second被销毁（只由PlayerController调用）
    public void OnSecondDestroyed()
    {
        secondDestroyedCount++;
        
        // 检查阶段切换条件
        CheckStageTransition();
    }

    // 公共方法：记录third被PlayerController销毁（委托给LetterNumberManager）
    public void OnThirdDestroyed()
    {
        if (letterNumberManager != null)
        {
            letterNumberManager.OnThirdDestroyed();
        }
    }



    // 公共方法：根据预制体类型添加分数
    public void AddScoreByPrefabType(string prefabName)
    {
        int scoreToAdd = 0; // 默认分数为0
        
        if (prefabName.Contains("first") || prefabName.Contains("First") ||
            prefabName.Contains("green") || prefabName.Contains("Green"))
        {
            // 根据当前阶段设置不同的分数
            switch (currentGameStage)
            {
                case 1:
                    scoreToAdd = stage1FirstScore;
                    break;
                case 2:
                    scoreToAdd = stage2FirstScore;
                    break;
                case 3:
                    scoreToAdd = stage3FirstScore;
                    break;
                case 4:
                    scoreToAdd = stage4FirstScore;
                    break;
            }
        }
        else if (prefabName.Contains("second") || prefabName.Contains("Second") ||
                 prefabName.Contains("red") || prefabName.Contains("Red"))
        {
            // 根据当前阶段设置不同的分数
            switch (currentGameStage)
            {
                case 2:
                    scoreToAdd = stage2SecondScore;
                    break;
                case 3:
                    scoreToAdd = stage3SecondScore;
                    break;
                case 4:
                    scoreToAdd = stage4SecondScore;
                    break;
            }
        }
        // third预制体不计算分数，scoreToAdd保持为0
        // 注意：blue是third预制体，它不应该加分
        
        // 只有当分数大于0时才添加分数
        if (scoreToAdd > 0)
        {
            AddScore(scoreToAdd);
        }
    }

    // 游戏控制方法
    public void StartGame()
    {
        isGameActive = true;
        InitializeGame();
    }

    public void PauseGame()
    {
        isGameActive = false;
    }

    public void ResumeGame()
    {
        isGameActive = true;
    }

    public void RestartGame()
    {
        // 确保游戏处于激活状态
        isGameActive = true;
        
        // 恢复时间缩放
        Time.timeScale = 1f;
        
        // 清理现有物体
        GameObject[] fallingObjects = GameObject.FindGameObjectsWithTag("Falling");
        foreach (GameObject obj in fallingObjects)
        {
            Destroy(obj);
        }
        
        // 重新初始化
        InitializeGame();
        
        // 重置UI分数显示
        if (uiManager != null)
        {
            uiManager.ResetScore();
        }
        
        Debug.Log("LiGameManager: 游戏重新开始完成，游戏状态已激活");
    }

    // 分数管理
    public void AddScore(int points)
    {
        currentScore += points;
        if (uiManager != null)
        {
            uiManager.AddScore(points);
        }
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
        if (letterNumberManager != null)
        {
            letterNumberManager.ResetSpecialCounts();
        }
    }

    [ContextMenu("检查预制体状态")]
    public void CheckPrefabStatus()
    {
        // 预制体状态检查
    }

    void StartStage4SpeedRecovery()
    {
        // 减少当前速度（减少指定百分比）
        stage4CurrentFirstFallSpeed *= (1f - stage4SpeedReductionPercent);
        stage4CurrentSecondFallSpeed *= (1f - stage4SpeedReductionPercent);
        stage4CurrentThirdFallSpeed *= (1f - stage4SpeedReductionPercent);
        
        // 启动速度恢复
        stage4SpeedRecoveryActive = true;
        stage4SpeedRecoveryTimer = 0f;
    }

    void ManageStage4SpeedRecovery()
    {
        stage4SpeedRecoveryTimer += Time.deltaTime;
        
        // 计算恢复进度（0到1之间）
        float recoveryProgress = stage4SpeedRecoveryTimer / stage4SpeedRecoveryTime;
        
        if (recoveryProgress >= 1f)
        {
            // 恢复完成
            stage4CurrentFirstFallSpeed = stage4TargetFirstFallSpeed;
            stage4CurrentSecondFallSpeed = stage4TargetSecondFallSpeed;
            stage4CurrentThirdFallSpeed = stage4TargetThirdFallSpeed;
            
            stage4SpeedRecoveryActive = false;
        }
        else
        {
            // 线性插值恢复速度
            stage4CurrentFirstFallSpeed = Mathf.Lerp(stage4CurrentFirstFallSpeed, stage4TargetFirstFallSpeed, Time.deltaTime / stage4SpeedRecoveryTime);
            stage4CurrentSecondFallSpeed = Mathf.Lerp(stage4CurrentSecondFallSpeed, stage4TargetSecondFallSpeed, Time.deltaTime / stage4SpeedRecoveryTime);
            stage4CurrentThirdFallSpeed = Mathf.Lerp(stage4CurrentThirdFallSpeed, stage4TargetThirdFallSpeed, Time.deltaTime / stage4SpeedRecoveryTime);
        }
    }
}
