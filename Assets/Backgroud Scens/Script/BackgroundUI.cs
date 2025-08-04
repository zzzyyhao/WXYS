using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundUI : MonoBehaviour
{
    public GameObject butDayPrefab;
    public GameObject butNightPrefab;
    public Transform buttonParent; // 按钮的父物体（如UI Canvas下的某个空物体）
    public float buttonPosX = -240f;
    public float buttonPosY = 640f;

    public Button settingButton;              // 拖拽 Setting 按钮
    public GameObject settingPanelPrefab;     // 拖拽 Setting Panel 预制体
    public GameObject maskButtonPrefab;       // 拖拽全屏透明遮罩Button预制体
    public Transform uiParent;                // 拖拽 UI Canvas 或其下空物体

    public BackgroundImage backgroundImage; // Inspector拖Day and Night空物体上的BackgroundImage脚本

    private GameObject currentButton;
    private GameObject currentSettingPanel;
    private GameObject currentMaskButton;

    void Start()
    {
        SpawnDayButton();
        backgroundImage.SwitchBackground(1); // 运行时自动生成白天背景
        settingButton.onClick.AddListener(OnSettingButtonClick);
    }

    void SpawnDayButton()
    {
        if (currentButton != null) Destroy(currentButton);
        currentButton = Instantiate(butDayPrefab, buttonParent);
        currentButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(buttonPosX, buttonPosY);
        currentButton.GetComponent<Button>().onClick.AddListener(OnDayButtonClick);
    }

    void SpawnNightButton()
    {
        if (currentButton != null) Destroy(currentButton);
        currentButton = Instantiate(butNightPrefab, buttonParent);
        currentButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(buttonPosX, buttonPosY);
        currentButton.GetComponent<Button>().onClick.AddListener(OnNightButtonClick);
    }

    void OnDayButtonClick()
    {
        AudioManager.Instance.PlayClickSFX();
        CloseSettingPanel();
        backgroundImage.SwitchBackground(2); // 切换到黑夜
        SpawnNightButton();
    }

    void OnNightButtonClick()
    {
        AudioManager.Instance.PlayClickSFX();
        CloseSettingPanel();
        backgroundImage.SwitchBackground(1); // 切换到白天
        SpawnDayButton();
    }

    public void OnSettingButtonClick()
    {
        AudioManager.Instance.PlayClickSFX();
        if (currentSettingPanel == null)
        {
            // 先生成遮罩
            currentMaskButton = Instantiate(maskButtonPrefab, uiParent);
            currentMaskButton.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            currentMaskButton.GetComponent<Button>().onClick.AddListener(CloseSettingPanel);

            // 再生成Panel，确保Panel在遮罩之上
            currentSettingPanel = Instantiate(settingPanelPrefab, uiParent);
            currentSettingPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        else
        {
            currentMaskButton.SetActive(true);
            currentSettingPanel.SetActive(true);
        }
    }

    public void CloseSettingPanel()
    {
        if (currentSettingPanel != null)
        {
            Destroy(currentSettingPanel);
            currentSettingPanel = null;
        }
        if (currentMaskButton != null)
        {
            Destroy(currentMaskButton);
            currentMaskButton = null;
        }
    }
}
