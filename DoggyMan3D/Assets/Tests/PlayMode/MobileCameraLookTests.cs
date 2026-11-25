#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class MobileCameraLookTests
{
    private GameObject root;
    private GameObject canvas;
    private GameObject surface;
    private Component input;
    private FieldInfo pauseField;
    private bool wasPaused;
    private float oldTimeScale;

    [SetUp]
    public void SetUp()
    {
        Assembly assembly = Assembly.Load("Assembly-CSharp");
        pauseField = assembly.GetType("MainGameManager", true)
            .GetField("_gamePauseMenuIsVisible", BindingFlags.Static | BindingFlags.NonPublic);
        wasPaused = (bool)pauseField.GetValue(null);
        pauseField.SetValue(null, false);
        oldTimeScale = Time.timeScale;
        Time.timeScale = 1f;
        root = new GameObject("Mobile camera test");
        root.SetActive(false);
        input = root.AddComponent(assembly.GetType("GameInputs", true));
        input.GetType().GetField("cursorLocked").SetValue(input, false);
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Mobile/Prefabs/CanvasInputs/UI_Canvas_StarterAssetsInputs_Joysticks.prefab");
        canvas = UnityEngine.Object.Instantiate(prefab, root.transform);
        Component controller = canvas.GetComponent(assembly.GetType("StarterAssets.UICanvasControllerInput", true));
        controller.GetType().GetField("InputSystem").SetValue(controller, input);
        Component mobileControls = canvas.GetComponent(assembly.GetType("MobileDisableAutoSwitchControls", true));
        FieldInfo playerInputField = mobileControls.GetType().GetField("playerInput");
        if (playerInputField != null)
            playerInputField.SetValue(mobileControls, root.AddComponent<PlayerInput>());
        surface = canvas.transform.Find("UI_Virtual_Camera_Look").gameObject;
        root.SetActive(true);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(root);
        pauseField.SetValue(null, wasPaused);
        Time.timeScale = oldTimeScale;
    }

    private Vector2 ConsumeLook()
    {
        return (Vector2)input.GetType().GetMethod("ConsumeLookDelta").Invoke(input, null);
    }

    private PointerEventData Pointer(int id, Vector2 delta)
    {
        return new PointerEventData(EventSystem.current)
        {
            pointerId = id,
            button = PointerEventData.InputButton.Left,
            delta = delta
        };
    }

    private GameObject Hit(Vector2 position)
    {
        var hits = new List<RaycastResult>();
        canvas.GetComponent<GraphicRaycaster>().Raycast(
            new PointerEventData(EventSystem.current) { position = position }, hits);
        Assert.That(hits, Is.Not.Empty);
        return hits[0].gameObject;
    }

    [UnityTest]
    public IEnumerator DragTracksOneFingerAccumulatesMotionAndStopsWithoutDrift()
    {
        yield return null;
        Assert.That(canvas.transform.Find("UI_Virtual_Joystick_Look"), Is.Null);
        var first = Pointer(10, new Vector2(30f, 15f));
        var second = Pointer(11, new Vector2(100f, 100f));
        ExecuteEvents.Execute(surface, first, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(surface, first, ExecuteEvents.initializePotentialDrag);
        Assert.That(first.useDragThreshold, Is.False);
        ExecuteEvents.Execute(surface, second, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(surface, second, ExecuteEvents.dragHandler);
        ExecuteEvents.Execute(surface, second, ExecuteEvents.pointerUpHandler);
        Assert.That(ConsumeLook(), Is.EqualTo(Vector2.zero));
        ExecuteEvents.Execute(surface, first, ExecuteEvents.dragHandler);
        ExecuteEvents.Execute(surface, first, ExecuteEvents.dragHandler);
        float scale = 180f / Mathf.Min(Screen.width, Screen.height);
        Vector2 expected = new Vector2(60f, -30f) * scale;
        Assert.That(Vector2.Distance(ConsumeLook(), expected), Is.LessThan(0.001f));
        yield return null;
        Assert.That(ConsumeLook(), Is.EqualTo(Vector2.zero));
        ExecuteEvents.Execute(surface, first, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(surface, first, ExecuteEvents.dragHandler);
        Assert.That(ConsumeLook(), Is.EqualTo(Vector2.zero));
    }

    [UnityTest]
    public IEnumerator ControlsKeepPriorityAndWorkWhileAnotherFingerLooks()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        Assert.That(Hit(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f)), Is.EqualTo(surface));
        var cameraFinger = Pointer(10, new Vector2(20f, 0f));
        ExecuteEvents.Execute(surface, cameraFinger, ExecuteEvents.pointerDownHandler);

        foreach (Transform child in canvas.transform)
        {
            if (child.gameObject == surface || !child.gameObject.activeSelf)
                continue;
            Vector2 center = RectTransformUtility.WorldToScreenPoint(null, ((RectTransform)child).position);
            GameObject target = ExecuteEvents.GetEventHandler<IPointerDownHandler>(Hit(center));
            Assert.That(target, Is.EqualTo(child.gameObject), child.name);
            var controlFinger = Pointer(11, Vector2.zero);
            controlFinger.position = center + Vector2.right * 20f;
            ExecuteEvents.Execute(target, controlFinger, ExecuteEvents.pointerDownHandler);
            if (child.name.Contains("Move"))
                Assert.That((Vector2)input.GetType().GetField("move").GetValue(input), Is.Not.EqualTo(Vector2.zero));
            Assert.That(ConsumeLook(), Is.EqualTo(Vector2.zero), child.name);
            ExecuteEvents.Execute(surface, cameraFinger, ExecuteEvents.dragHandler);
            Assert.That(ConsumeLook().x, Is.GreaterThan(0f), child.name);
            ExecuteEvents.Execute(target, controlFinger, ExecuteEvents.pointerUpHandler);
        }
    }

    [UnityTest]
    public IEnumerator PauseDisableAndFocusLossCancelTheGesture()
    {
        yield return null;
        var pointer = Pointer(10, new Vector2(20f, 0f));
        ExecuteEvents.Execute(surface, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(surface, pointer, ExecuteEvents.dragHandler);
        surface.SetActive(false);
        surface.SetActive(true);
        Assert.That(ConsumeLook(), Is.EqualTo(Vector2.zero));
        ExecuteEvents.Execute(surface, pointer, ExecuteEvents.dragHandler);
        Assert.That(ConsumeLook(), Is.EqualTo(Vector2.zero));

        ExecuteEvents.Execute(surface, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(surface, pointer, ExecuteEvents.dragHandler);
        pauseField.SetValue(null, true);
        yield return null;
        pauseField.SetValue(null, false);
        Assert.That(ConsumeLook(), Is.EqualTo(Vector2.zero));
        ExecuteEvents.Execute(surface, pointer, ExecuteEvents.dragHandler);
        Assert.That(ConsumeLook(), Is.EqualTo(Vector2.zero));

        ExecuteEvents.Execute(surface, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(surface, pointer, ExecuteEvents.dragHandler);
        surface.SendMessage("OnApplicationFocus", false);
        Assert.That(ConsumeLook(), Is.EqualTo(Vector2.zero));
        ExecuteEvents.Execute(surface, pointer, ExecuteEvents.dragHandler);
        Assert.That(ConsumeLook(), Is.EqualTo(Vector2.zero));
    }
}
#endif
