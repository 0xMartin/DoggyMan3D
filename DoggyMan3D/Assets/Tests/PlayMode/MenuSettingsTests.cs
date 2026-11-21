using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSettingsTests
{
    private bool hadMouseSensitivity;
    private float oldMouseSensitivity;

    [SetUp]
    public void SetUp()
    {
        hadMouseSensitivity = PlayerPrefs.HasKey("mouseSensitivity");
        oldMouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 1f);
        PlayerPrefs.DeleteKey("mouseSensitivity");
    }

    [TearDown]
    public void TearDown()
    {
        if (hadMouseSensitivity)
            PlayerPrefs.SetFloat("mouseSensitivity", oldMouseSensitivity);
        else
            PlayerPrefs.DeleteKey("mouseSensitivity");
        PlayerPrefs.Save();
    }

    [UnityTest]
    public IEnumerator SensitivityIsAppliedAndRestoredInMenu()
    {
#if !UNITY_STANDALONE && !UNITY_EDITOR
        Assert.Ignore("The desktop menu is not available on this platform.");
#endif
        yield return SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        yield return null;

        Assembly gameAssembly = Assembly.Load("Assembly-CSharp");
        Type menuType = gameAssembly.GetType("MenuScript", true);
        Component menu = UnityEngine.Object.FindObjectOfType(menuType) as Component;
        Assert.That(menu, Is.Not.Null);
        var settingsButton = (Button)menuType.GetField("MainSettings").GetValue(menu);
        settingsButton.onClick.Invoke();
        yield return null;

        Type settingsType = gameAssembly.GetType("MouseSensitivitySettings", true);
        Component settings = UnityEngine.Object.FindObjectOfType(settingsType) as Component;
        Assert.That(settings, Is.Not.Null);
        var slider = (Slider)settingsType.GetField("SensitivitySlider").GetValue(settings);
        var label = (TMP_Text)settingsType.GetField("SensitivityLabel").GetValue(settings);
        var apply = (Button)settingsType.GetField("ApplyButton").GetValue(settings);
        var back = (Button)menuType.GetField("SettingsBack").GetValue(menu);

        Canvas.ForceUpdateCanvases();
        label.ForceMeshUpdate();
        Assert.That(label.isTextOverflowing, Is.False);
        var sliderCorners = new Vector3[4];
        slider.GetComponent<RectTransform>().GetWorldCorners(sliderCorners);
        Vector2 sliderCenter = RectTransformUtility.WorldToScreenPoint(null, (sliderCorners[0] + sliderCorners[2]) / 2f);
        Assert.That(sliderCenter.x, Is.InRange(0, Screen.width));
        Assert.That(sliderCenter.y, Is.InRange(0, Screen.height));
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = sliderCenter }, hits);
        Assert.That(hits.Exists(hit => hit.gameObject.transform.IsChildOf(slider.transform)), Is.True);

        Assert.That(slider.minValue, Is.EqualTo(25));
        Assert.That(slider.maxValue, Is.EqualTo(300));
        Assert.That(slider.wholeNumbers, Is.True);
        Assert.That(slider.value, Is.EqualTo(100));
        slider.value = 200;
        Assert.That(label.text, Does.Contain("200%"));
        Assert.That(PlayerPrefs.HasKey("mouseSensitivity"), Is.False);
        back.onClick.Invoke();
        settingsButton.onClick.Invoke();
        Assert.That(slider.value, Is.EqualTo(100));

        slider.value = 200;
        apply.onClick.Invoke();
        Assert.That(PlayerPrefs.GetFloat("mouseSensitivity"), Is.EqualTo(2f));
        back.onClick.Invoke();
        settingsButton.onClick.Invoke();
        Assert.That(slider.value, Is.EqualTo(200));

        yield return SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        yield return null;
        menu = UnityEngine.Object.FindObjectOfType(menuType) as Component;
        settingsButton = (Button)menuType.GetField("MainSettings").GetValue(menu);
        settingsButton.onClick.Invoke();
        settings = UnityEngine.Object.FindObjectOfType(settingsType) as Component;
        slider = (Slider)settingsType.GetField("SensitivitySlider").GetValue(settings);
        Assert.That(slider.value, Is.EqualTo(200));
    }
}
