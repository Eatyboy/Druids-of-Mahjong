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

    public void UpdateVolume()
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
    }

}
