using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSettings : MonoBehaviour
{
    public AudioClip ButtonSound;
    public float ButtonSoundVolume = 0.7f;

    public TMP_Dropdown resolutionDropdown;
    public Button ApplyButton;

    private List<Resolution> resolutions;

    void Start()
    {
        ApplyButton.onClick.AddListener(ApplySettings);

        resolutions = new List<Resolution>(Screen.resolutions);
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Count; i++)
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
        resolutionDropdown.value = PlayerPrefs.GetInt("resolutionIndex", currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();

        SetResolution(resolutionDropdown.value);
    }

    private void ApplySettings()
    {
        AudioSource.PlayClipAtPoint(ButtonSound, new Vector3(0.0f, 1.0f, -10.0f), ButtonSoundVolume);
        int index = resolutionDropdown.value;
        PlayerPrefs.SetInt("resolutionIndex", index);
        SetResolution(index);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Debug.Log("SCREEN RESOLUTION SET ON: " + resolution.width + " x " + resolution.height);
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }
}
