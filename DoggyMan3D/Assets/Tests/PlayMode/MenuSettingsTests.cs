using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class MenuSettingsTests
{
    private bool hadMouseSensitivity;
    private float oldMouseSensitivity;
    private bool hadResolutionIndex;
    private int oldResolutionIndex;

    [SetUp]
    public void SetUp()
    {
        hadMouseSensitivity = PlayerPrefs.HasKey("mouseSensitivity");
        oldMouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 1f);
        hadResolutionIndex = PlayerPrefs.HasKey("resolutionIndex");
        oldResolutionIndex = PlayerPrefs.GetInt("resolutionIndex");
        PlayerPrefs.DeleteKey("mouseSensitivity");
        PlayerPrefs.DeleteKey("resolutionIndex");
    }

    [TearDown]
    public void TearDown()
    {
        if (hadMouseSensitivity)
            PlayerPrefs.SetFloat("mouseSensitivity", oldMouseSensitivity);
        else
            PlayerPrefs.DeleteKey("mouseSensitivity");
        if (hadResolutionIndex)
            PlayerPrefs.SetInt("resolutionIndex", oldResolutionIndex);
        else
            PlayerPrefs.DeleteKey("resolutionIndex");
        PlayerPrefs.Save();
    }

    [UnityTest]
    public IEnumerator MenuButtonFeedbackRespondsToPointerKeyboardAndDisabledStates()
    {
#if !UNITY_STANDALONE && !UNITY_EDITOR
        Assert.Ignore("The desktop menu is not available on this platform.");
#endif
        yield return SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        yield return null;

        Type menuType = Assembly.Load("Assembly-CSharp").GetType("MenuScript", true);
        Component menu = UnityEngine.Object.FindFirstObjectByType(menuType) as Component;
        var button = (Button)menuType.GetField("MainNewGame").GetValue(menu);
        Type feedbackType = Assembly.Load("Assembly-CSharp").GetType("MenuButtonFeedback", true);
        Assert.That(button.GetComponent(feedbackType), Is.Not.Null);
        var wash = button.transform.Find("Menu Button Wash").GetComponent<Image>();
        var accent = button.transform.Find("Menu Button Accent").GetComponent<Image>();
        Assert.That(wash.raycastTarget, Is.False);
        Assert.That(accent.raycastTarget, Is.False);
        float normalAlpha = wash.color.a;
        Assert.That(normalAlpha, Is.Zero);
        Assert.That(accent.color.a, Is.Zero);

        Canvas.ForceUpdateCanvases();
        var corners = new Vector3[4];
        button.GetComponent<RectTransform>().GetWorldCorners(corners);
        Vector2 center = RectTransformUtility.WorldToScreenPoint(null, (corners[0] + corners[2]) / 2f);
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = center }, hits);
        Assert.That(hits.Count, Is.GreaterThan(0));
        Assert.That(ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject), Is.EqualTo(button.gameObject));

        var pointer = new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left };
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
        yield return new WaitForSecondsRealtime(0.25f);
        Assert.That(wash.color.a, Is.GreaterThan(normalAlpha + 0.1f));
        Assert.That(wash.color.r, Is.EqualTo(wash.color.g));
        Assert.That(wash.color.g, Is.EqualTo(wash.color.b));
        Assert.That(accent.color.r, Is.EqualTo(accent.color.g));
        Assert.That(accent.color.g, Is.EqualTo(accent.color.b));

        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerExitHandler);
        yield return new WaitForSecondsRealtime(0.25f);
        Assert.That(wash.color.a, Is.EqualTo(normalAlpha).Within(0.001f));

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button.gameObject);
        yield return new WaitForSecondsRealtime(0.25f);
        Assert.That(wash.color.a, Is.GreaterThan(normalAlpha + 0.1f));
        button.interactable = false;
        yield return new WaitForSecondsRealtime(0.25f);
        Assert.That(wash.color.a, Is.EqualTo(normalAlpha).Within(0.001f));
        button.interactable = true;
        EventSystem.current.SetSelectedGameObject(null);

        var touch = new ExtendedPointerEventData(EventSystem.current)
        {
            button = PointerEventData.InputButton.Left,
            pointerType = UIPointerType.Touch
        };
        ExecuteEvents.Execute(button.gameObject, touch, ExecuteEvents.pointerEnterHandler);
        yield return new WaitForSecondsRealtime(0.25f);
        Assert.That(wash.color.a, Is.EqualTo(normalAlpha).Within(0.001f));
        ExecuteEvents.Execute(button.gameObject, touch, ExecuteEvents.pointerDownHandler);
        yield return new WaitForSecondsRealtime(0.25f);
        Assert.That(wash.color.a, Is.GreaterThan(normalAlpha + 0.05f));
        ExecuteEvents.Execute(button.gameObject, touch, ExecuteEvents.pointerUpHandler);
        yield return new WaitForSecondsRealtime(0.25f);
        Assert.That(wash.color.a, Is.EqualTo(normalAlpha).Within(0.001f));

        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
        yield return new WaitForSecondsRealtime(0.25f);
        button.gameObject.SetActive(false);
        button.gameObject.SetActive(true);
        Assert.That(wash.color.a, Is.EqualTo(normalAlpha).Within(0.001f));
    }

    [UnityTest]
    public IEnumerator SaveButtonsAlsoReceiveFeedback()
    {
#if !UNITY_STANDALONE && !UNITY_EDITOR
        Assert.Ignore("The desktop menu is not available on this platform.");
#endif
        Directory.CreateDirectory(Application.persistentDataPath);
        string path = Path.Combine(Application.persistentDataPath, "menu-feedback-test-" + Guid.NewGuid().ToString("N") + ".json");
        File.WriteAllText(path, "{\"Name\":\"Menu feedback test\",\"Level\":0,\"Inventory\":[]}");
        try
        {
            yield return SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            yield return null;
            Type menuType = Assembly.Load("Assembly-CSharp").GetType("MenuScript", true);
            Component menu = UnityEngine.Object.FindFirstObjectByType(menuType) as Component;
            ((Button)menuType.GetField("MainLoadGame").GetValue(menu)).onClick.Invoke();
            yield return null;

            var content = (Transform)menuType.GetField("scrollViewContent").GetValue(menu);
            Type feedbackType = Assembly.Load("Assembly-CSharp").GetType("MenuButtonFeedback", true);
            Button[] saveButtons = content.GetComponentsInChildren<Button>();
            Assert.That(saveButtons.Length, Is.GreaterThan(0));
            foreach (Button saveButton in saveButtons)
                Assert.That(saveButton.GetComponent(feedbackType), Is.Not.Null);
        }
        finally
        {
            File.Delete(path);
        }
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
        Assert.That(((GameObject)menuType.GetField("MainSection").GetValue(menu)).activeSelf, Is.True);
        Assert.That(((GameObject)menuType.GetField("SettigsSection").GetValue(menu)).activeSelf, Is.False);
        settingsButton.onClick.Invoke();
        Assert.That(slider.value, Is.EqualTo(200));
        slider.value = 150;
        apply.onClick.Invoke();
        Assert.That(PlayerPrefs.GetFloat("mouseSensitivity"), Is.EqualTo(1.5f));
        Assert.That(((GameObject)menuType.GetField("MainSection").GetValue(menu)).activeSelf, Is.True);
        Assert.That(((GameObject)menuType.GetField("SettigsSection").GetValue(menu)).activeSelf, Is.False);

        yield return SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        yield return null;
        menu = UnityEngine.Object.FindObjectOfType(menuType) as Component;
        settingsButton = (Button)menuType.GetField("MainSettings").GetValue(menu);
        settingsButton.onClick.Invoke();
        settings = UnityEngine.Object.FindObjectOfType(settingsType) as Component;
        slider = (Slider)settingsType.GetField("SensitivitySlider").GetValue(settings);
        Assert.That(slider.value, Is.EqualTo(150));
    }
}
