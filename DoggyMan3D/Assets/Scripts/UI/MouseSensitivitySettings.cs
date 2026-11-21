using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivitySettings : MonoBehaviour
{
    public const string PreferenceKey = "mouseSensitivity";
    public const float DefaultSensitivity = 1f;
    public const float MinSensitivity = 0.25f;
    public const float MaxSensitivity = 3f;

    public Slider SensitivitySlider;
    public TMP_Text SensitivityLabel;
    public Button ApplyButton;

    public static float GetSensitivity()
    {
        float sensitivity = PlayerPrefs.GetFloat(PreferenceKey, DefaultSensitivity);
        if (float.IsNaN(sensitivity) || float.IsInfinity(sensitivity))
        {
            Debug.LogWarning("Invalid mouse sensitivity in PlayerPrefs; using the default.");
            return DefaultSensitivity;
        }

        if (sensitivity < MinSensitivity || sensitivity > MaxSensitivity)
        {
            Debug.LogWarning("Mouse sensitivity in PlayerPrefs is outside the supported range; clamping it.");
        }
        return Mathf.Clamp(sensitivity, MinSensitivity, MaxSensitivity);
    }

    private void Awake()
    {
        SensitivitySlider.onValueChanged.AddListener(UpdateLabel);
        ApplyButton.onClick.AddListener(ApplySensitivity);
    }

    private void OnEnable()
    {
        float sensitivity = GetSensitivity();
        SensitivitySlider.SetValueWithoutNotify(sensitivity * 100f);
        UpdateLabel(SensitivitySlider.value);
    }

    private void OnDestroy()
    {
        SensitivitySlider.onValueChanged.RemoveListener(UpdateLabel);
        ApplyButton.onClick.RemoveListener(ApplySensitivity);
    }

    private void UpdateLabel(float percent)
    {
        SensitivityLabel.text = $"Mouse sensitivity: {percent:0}%";
    }

    private void ApplySensitivity()
    {
        PlayerPrefs.SetFloat(PreferenceKey, SensitivitySlider.value / 100f);
        PlayerPrefs.Save();
    }
}
