using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Sprite soundMute;
    public Sprite soundPlay;
    public Sprite musicMute;
    public Sprite musicPlay;

    public GameObject musicButton;
    public GameObject sfxButton;

    public Slider musicSlider;
    public Slider sfxSlider;
    public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        if(resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
        }

        int currentResolutionIndex = 0;

        // Dictionary to store the highest refresh rate for each resolution
        Dictionary<string, Resolution> highestResolutions = new Dictionary<string, Resolution>();

        foreach (var res in resolutions)
        {
            if ((res.width == 1920 && res.height == 1080) || (res.width == 1280 && res.height == 720))
            {
                string resolutionKey = res.width + "x" + res.height;
                if (!highestResolutions.ContainsKey(resolutionKey) ||
                    res.refreshRateRatio.numerator / res.refreshRateRatio.denominator >
                    highestResolutions[resolutionKey].refreshRateRatio.numerator / highestResolutions[resolutionKey].refreshRateRatio.denominator)
                {
                    highestResolutions[resolutionKey] = res;
                }
            }
        }

        List<string> resolutionOptions = new List<string>();
        foreach (var res in highestResolutions.Values)
        {
            string option = res.width + "x" + res.height + " " + (res.refreshRateRatio.numerator / res.refreshRateRatio.denominator) + "Hz";
            resolutionOptions.Add(option);

            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height &&
                res.refreshRateRatio.numerator / res.refreshRateRatio.denominator == Screen.currentResolution.refreshRateRatio.numerator / Screen.currentResolution.refreshRateRatio.denominator)
            {
                currentResolutionIndex = resolutionOptions.Count - 1;
            }
        }

        if(resolutionDropdown != null)
        {
            resolutionDropdown.AddOptions(resolutionOptions);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        // Set the initial fullscreen mode
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    public void SetResolution(int resolutionIndex)
    {
        string selectedOption = resolutionDropdown.options[resolutionIndex].text;
        bool fullscreen = Screen.fullScreen;

        // Extract the resolution and refresh rate from the selected option
        string[] parts = selectedOption.Split(' ');
        string[] resolutionParts = parts[0].Split('x');
        int width = int.Parse(resolutionParts[0]);
        int height = int.Parse(resolutionParts[1]);
        int refreshRateValue = int.Parse(parts[1].Replace("Hz", "").Trim());
        RefreshRate refreshRate = new RefreshRate { numerator = (uint)refreshRateValue, denominator = 1 };

        // Determine the fullscreen mode
        FullScreenMode fullscreenMode = fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        Screen.SetResolution(width, height, fullscreenMode, refreshRate);
    }

    void AdjustDropdownWidth()
    {
        RectTransform rt = resolutionDropdown.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, rt.sizeDelta.y); // Adjust the width as needed
    }

    public void ToggleMusic()
    {
        if (AudioManager.instance.musicSource.mute)
        {
            musicButton.GetComponent<Image>().sprite = musicPlay;
        }
        else
        {
            musicButton.GetComponent<Image>().sprite = musicMute;
        }
        AudioManager.instance.ToggleMusic();
    }

    public void ToggleSfx()
    {
        if (AudioManager.instance.sfxSource.mute)
        {
            sfxButton.GetComponent<Image>().sprite = soundPlay;
        }
        else
        {
            sfxButton.GetComponent<Image>().sprite = soundMute;
        }
        AudioManager.instance.ToggleSfx();
    }

    public void MusicVolume()
    {
        AudioManager.instance.MusicVolume(musicSlider.value);
    }

    public void SfxVolume()
    {
        AudioManager.instance.SfxVolume(sfxSlider.value);
    }

    public void OnSfxSliderRelease()
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.purchaseCard);
    }

    public void SetFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
