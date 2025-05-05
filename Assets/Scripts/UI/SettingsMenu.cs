using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Slider masterVolumeSlider, musicVolumeSlider, sfxVolumeSlider;
    [SerializeField] private Toggle fullScreenToggle;

    Resolution[] resolutions;

    private void Awake()
    {
        // Initialize fullscreen toggle
        fullScreenToggle.isOn = Screen.fullScreen;

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;

            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }

        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

    }

    public void Start()
    {
        resetSounds();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

    }

    private void resetSounds()
    {
        musicVolumeSlider.value = 1;
        masterVolumeSlider.value = 1;
        sfxVolumeSlider.value = 1;
        AudioManager.Instance.ChangeMasterVolume(0);
        AudioManager.Instance.ChangeMusicVolume(0);
        AudioManager.Instance.ChangeSFXVolume(0);
    }

    public void SetMusicVolume()
    {
        float dB = musicVolumeSlider.value > 0 ? 20 * Mathf.Log10(musicVolumeSlider.value) : -80;
        AudioManager.Instance.ChangeMusicVolume(dB);
    }
    public void SetMasterVolume()
    {
        float dB = masterVolumeSlider.value > 0 ? 20 * Mathf.Log10(masterVolumeSlider.value) : -80;
        AudioManager.Instance.ChangeMasterVolume(dB);
    }
    public void SetSFXVolume()
    {
        float dB = sfxVolumeSlider.value > 0 ? 20 * Mathf.Log10(sfxVolumeSlider.value) : -80;
        AudioManager.Instance.ChangeSFXVolume(dB);
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void ToggleFullScreen(bool isFullscreen)
    {
        SetFullscreen(isFullscreen);
    }

}