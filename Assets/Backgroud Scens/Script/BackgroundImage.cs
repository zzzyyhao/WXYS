using UnityEngine;

public class BackgroundImage : MonoBehaviour
{
    public GameObject backgroundDaytimePrefab;
    public GameObject backgroundDarknightPrefab;
    private GameObject currentBackground;

    // public CameraController cameraController; // 现在不再需要

    // 1=白天，2=黑夜
    public void SwitchBackground(int state)
    {
        if (currentBackground != null)
        {
            Destroy(currentBackground);
        }
        if (state == 1)
        {
            currentBackground = Instantiate(backgroundDaytimePrefab, transform);
        }
        else
        {
            currentBackground = Instantiate(backgroundDarknightPrefab, transform);
        }
        // 不再需要 cameraController 相关逻辑
    }

    public void ClearBackground()
    {
        if (currentBackground != null)
        {
            Destroy(currentBackground);
            currentBackground = null;
        }
    }
}
