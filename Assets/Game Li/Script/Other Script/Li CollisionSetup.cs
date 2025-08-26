using UnityEngine;

public class LiCollisionSetup : MonoBehaviour
{
    void Awake()
    {
        // 禁用Falling层与自身的碰撞
        int fallingLayer = LayerMask.NameToLayer("Falling");
        if (fallingLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(fallingLayer, fallingLayer, true);
        }
    }
} 