using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;
    [SerializeField] private GameObject settingsObj;
    public bool isSettingsOpen; 

    [Header("Volume")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider ambienceVolumeSlider;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    private void Start()
    {
        isSettingsOpen = false;
        HideSettings();
    }

    public void ShowSettings()
    {
        masterVolumeSlider.value = AudioManager.instance.masterVolume;
        musicVolumeSlider.value = AudioManager.instance.musicVolume;
        sfxVolumeSlider.value = AudioManager.instance.sfxVolume;
        ambienceVolumeSlider.value = AudioManager.instance.ambienceVolume;

        isSettingsOpen = true;
        settingsObj.SetActive(true);
    }

    public void HideSettings()
    {
        isSettingsOpen = false;
        settingsObj.SetActive(false);
    }

    public void SwitchSettingsState()
    {
        if (isSettingsOpen)
        {
            HideSettings();
        }
        else
        {
            ShowSettings();
        }
    }

    public async void UpdateVolume()
    {
        if (AudioManager.instance == null)
        {
            UnityEngine.Debug.LogWarning("AudioManager not found in SettingsManager. Cannot update volume.");
            return;
        }
        
        AudioManager.instance.masterVolume = masterVolumeSlider.value;
        AudioManager.instance.musicVolume = musicVolumeSlider.value;
        AudioManager.instance.sfxVolume = sfxVolumeSlider.value;
        AudioManager.instance.ambienceVolume = ambienceVolumeSlider.value;

        if (SaveSystem.loaded)
        {
            await SaveSystem.SaveSettings();
        }
    }

    public async void ResetToDefault()
    {
        AudioManager.instance.masterVolume = AudioManager.DEFAULT_VOLUME;
        AudioManager.instance.musicVolume = AudioManager.DEFAULT_VOLUME;
        AudioManager.instance.sfxVolume = AudioManager.DEFAULT_VOLUME;
        AudioManager.instance.ambienceVolume = AudioManager.DEFAULT_VOLUME;

        masterVolumeSlider.SetValueWithoutNotify(AudioManager.instance.masterVolume);
        musicVolumeSlider.SetValueWithoutNotify(AudioManager.instance.musicVolume);
        sfxVolumeSlider.SetValueWithoutNotify(AudioManager.instance.sfxVolume);
        ambienceVolumeSlider.SetValueWithoutNotify(AudioManager.instance.ambienceVolume);
        
        await SaveSystem.SaveSettings();
    }

    public void LoadSettings(SettingsSaveData saveData)
    {
        AudioManager.instance.masterVolume = saveData.masterVolume;
        AudioManager.instance.musicVolume = saveData.musicVolume;
        AudioManager.instance.sfxVolume = saveData.sfxVolume;
        AudioManager.instance.ambienceVolume = saveData.ambienceVolume;

        masterVolumeSlider.value = saveData.masterVolume;
        musicVolumeSlider.value = saveData.musicVolume;
        sfxVolumeSlider.value = saveData.sfxVolume;
        ambienceVolumeSlider.value = saveData.ambienceVolume;
    }

    public SettingsSaveData GetSaveData()
    {
        return new SettingsSaveData
        {
            masterVolume = AudioManager.instance.masterVolume,
            musicVolume = AudioManager.instance.musicVolume,
            sfxVolume = AudioManager.instance.sfxVolume,
            ambienceVolume = AudioManager.instance.ambienceVolume,
        };
    }
}

[Serializable]
public class SettingsSaveData
{
    public float masterVolume = AudioManager.DEFAULT_VOLUME;
    public float musicVolume = AudioManager.DEFAULT_VOLUME;
    public float sfxVolume = AudioManager.DEFAULT_VOLUME;
    public float ambienceVolume = AudioManager.DEFAULT_VOLUME;
}
