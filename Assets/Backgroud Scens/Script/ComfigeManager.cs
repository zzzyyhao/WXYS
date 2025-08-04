using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ComfigeManager : MonoBehaviour
{
    public static ComfigeManager Instance;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float scroll = 0f;
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
            scroll = Mouse.current.scroll.ReadValue().y / 120f; // 120是标准一格
#else
        scroll = Input.GetAxis("Mouse ScrollWheel");
#endif

        if (Mathf.Abs(scroll) > 0.01f)
        {
            // Assuming initialOrthoSize is defined elsewhere or needs to be added
            // ZoomCamera(-scroll * initialOrthoSize * 0.2f);
        }
    }

    public void SetDayNightState(bool isDay)
    {
        PlayerPrefs.SetInt("DayNight", isDay ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetDayNightButtonState(int value)
    {
        PlayerPrefs.SetInt("DayNightButtonState", value);
        PlayerPrefs.Save();
    }

    public int GetDayNightButtonState()
    {
        return PlayerPrefs.GetInt("DayNightButtonState", 1); // 默认1
    }
}
