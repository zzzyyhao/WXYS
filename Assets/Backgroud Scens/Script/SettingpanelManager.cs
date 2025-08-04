using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingpanelManager : MonoBehaviour
{
    public Slider bgmSlider; // 拖拽Setting Panel里的Slider
    public Slider sfxSlider; // 拖拽SFX滑块

    void Start()
    {
        // 初始化滑块
        if (AudioManager.Instance != null && AudioManager.Instance.bgmSource != null)
            bgmSlider.value = AudioManager.Instance.bgmSource.volume;

        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxSlider.value = AudioManager.Instance.sfxSource.volume;
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    void OnBGMVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetBGMVolume(value);
    }

    void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }
}
