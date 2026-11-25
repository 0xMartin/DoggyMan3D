using System;
using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SceneLoadingTests
{
    private string savePath;
    private Type managerType;
    private bool hadMouseSensitivity;
    private float oldMouseSensitivity;

    [SetUp]
    public void SetUp()
    {
        savePath = Path.Combine(Path.GetTempPath(), "DoggyMan3D-scene-test-" + Guid.NewGuid().ToString("N") + ".json");
        File.WriteAllText(savePath, "{\"Name\":\"Scene test\",\"Level\":0,\"Inventory\":[]}");
        managerType = Assembly.Load("Assembly-CSharp").GetType("MainGameManager", true);
        managerType.GetMethod("SetPlayerSaveToLoadOnGameManagerStart").Invoke(null, new object[] { savePath });
        hadMouseSensitivity = PlayerPrefs.HasKey("mouseSensitivity");
        oldMouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 1f);
        PlayerPrefs.SetFloat("mouseSensitivity", 2f);
    }

    [TearDown]
    public void TearDown()
    {
        Time.timeScale = 1f;
        managerType.GetMethod("SetPlayerSaveToLoadOnGameManagerStart").Invoke(null, new object[] { null });
        if (hadMouseSensitivity)
            PlayerPrefs.SetFloat("mouseSensitivity", oldMouseSensitivity);
        else
            PlayerPrefs.DeleteKey("mouseSensitivity");
        PlayerPrefs.Save();
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }

    [UnityTest]
    public IEnumerator GameplayPostProcessingIsIsolatedAndMobileSettingsPreserveSharedProfile()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Assert.Ignore("This test simulates Android settings from a desktop player or editor.");
#endif
        yield return SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        yield return null;
        var menuVolume = Camera.main.GetComponent<PostProcessVolume>();
        Assert.That(menuVolume, Is.Not.Null);
        var menuProfile = menuVolume.sharedProfile;

        yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        yield return WaitForLevel(5, null);
        var camera = Camera.main;
        var volume = camera.GetComponent<PostProcessVolume>();
        var layer = camera.GetComponent<PostProcessLayer>();
        Assert.That(volume.sharedProfile, Is.Not.Null);
        Assert.That(volume.sharedProfile, Is.Not.SameAs(menuProfile));
        Assert.That(layer.antialiasingMode, Is.EqualTo(PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing));
        Assert.That(volume.sharedProfile.TryGetSettings(out AmbientOcclusion sharedOcclusion), Is.True);
        Assert.That(volume.sharedProfile.TryGetSettings(out Bloom sharedBloom), Is.True);
        Assert.That(sharedOcclusion.enabled.value, Is.True);
        Assert.That(sharedBloom.fastMode.value, Is.False);

        Type settingsType = Assembly.Load("Assembly-CSharp").GetType("GameplayPostProcessing", true);
        Component settings = camera.GetComponent(settingsType);
        Assert.That(settings, Is.Not.Null);
        settingsType.GetMethod("ApplyMobileSettings", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(settings, null);
        Assert.That(layer.antialiasingMode, Is.EqualTo(PostProcessLayer.Antialiasing.FastApproximateAntialiasing));
        Assert.That(volume.profile.TryGetSettings(out AmbientOcclusion mobileOcclusion), Is.True);
        Assert.That(volume.profile.TryGetSettings(out Bloom mobileBloom), Is.True);
        Assert.That(mobileOcclusion.enabled.overrideState, Is.True);
        Assert.That(mobileOcclusion.enabled.value, Is.False);
        Assert.That(mobileBloom.fastMode.overrideState, Is.True);
        Assert.That(mobileBloom.fastMode.value, Is.True);
        Assert.That(sharedOcclusion.enabled.value, Is.True);
        Assert.That(sharedBloom.fastMode.value, Is.False);

        Component manager = UnityEngine.Object.FindObjectOfType(managerType) as Component;
        managerType.GetMethod("OnLevelFinished").Invoke(manager, null);
        yield return WaitForLevel(6, null);
        Assert.That(Camera.main, Is.SameAs(camera));
        Assert.That(layer.antialiasingMode, Is.EqualTo(PostProcessLayer.Antialiasing.FastApproximateAntialiasing));
        Assert.That(volume.profile, Is.Not.SameAs(volume.sharedProfile));
        Assert.That(mobileOcclusion.enabled.value, Is.False);

        yield return SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        yield return null;
        Assert.That(Camera.main.GetComponent<PostProcessVolume>().sharedProfile, Is.SameAs(menuProfile));
    }

    [UnityTest]
    public IEnumerator LoadsAdvancesAndRestartsLevelsAndRecoversFromMissingDescriptor()
    {
        yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        yield return WaitForLevel(5, null);

        Component manager = UnityEngine.Object.FindObjectOfType(managerType) as Component;
        Assert.That(manager, Is.Not.Null);
        Assert.That(SceneManager.GetActiveScene().buildIndex, Is.EqualTo(1));
        Assert.That(RenderSettings.fog, Is.True);
        Assert.That(RenderSettings.fogMode, Is.EqualTo(FogMode.Linear));
        Assert.That(RenderSettings.fogColor, Is.EqualTo(new Color(0.12f, 0.16f, 0.2f, 1f)));
        Assert.That(RenderSettings.fogStartDistance, Is.EqualTo(70f));
        Assert.That(RenderSettings.fogEndDistance, Is.EqualTo(350f));

        object firstLevel = managerType.GetMethod("GetCurrentLevel").Invoke(null, null);
        managerType.GetMethod("OnLevelReset").Invoke(manager, null);
        yield return WaitForLevel(5, firstLevel);
        Assert.That(RenderSettings.fog, Is.True);
        Assert.That(RenderSettings.fogStartDistance, Is.EqualTo(70f));

        Type controllerType = Assembly.Load("Assembly-CSharp").GetType("PlayerController", true);
        Component controller = UnityEngine.Object.FindObjectOfType(controllerType) as Component;
        Assert.That(controller, Is.Not.Null);
        PlayerInput playerInput = controller.GetComponent<PlayerInput>();
        Mouse mouse = InputSystem.AddDevice<Mouse>();
        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        Gamepad gamepad = InputSystem.AddDevice<Gamepad>();
        try
        {
            playerInput.SwitchCurrentControlScheme("KeyboardMouse", keyboard, mouse);
            yield return null;
            FieldInfo yaw = controllerType.GetField("_cinemachineTargetYaw", BindingFlags.Instance | BindingFlags.NonPublic);
            float previousYaw = (float)yaw.GetValue(controller);
            Assert.That(playerInput.currentControlScheme, Is.EqualTo("KeyboardMouse"));
            Component input = controller.GetComponent(Assembly.Load("Assembly-CSharp").GetType("GameInputs", true));
            input.GetType().GetMethod("LookInput").Invoke(input, new object[] { new Vector2(0.075f, 0f) });
            controllerType.GetMethod("CameraRotation", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(controller, null);
            Assert.That((float)yaw.GetValue(controller) - previousYaw, Is.EqualTo(0.15f).Within(0.001f));
            controllerType.GetField("LockCameraPosition").SetValue(controller, true);
            controllerType.GetMethod("CameraRotation", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(controller, null);
            Assert.That((float)yaw.GetValue(controller) - previousYaw, Is.EqualTo(0.15f).Within(0.001f));
            controllerType.GetField("LockCameraPosition").SetValue(controller, false);

            playerInput.SwitchCurrentControlScheme("Gamepad", gamepad);
            Assert.That(playerInput.currentControlScheme, Is.EqualTo("Gamepad"));
            controllerType.GetMethod("CameraRotation", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(controller, null);
            Assert.That((float)yaw.GetValue(controller) - previousYaw, Is.EqualTo(0.15f).Within(0.001f));
            input.GetType().GetMethod("LookInput").Invoke(input, new object[] { Vector2.zero });
        }
        finally
        {
            InputSystem.RemoveDevice(mouse);
            InputSystem.RemoveDevice(keyboard);
            InputSystem.RemoveDevice(gamepad);
        }

        managerType.GetMethod("OnLevelFinished").Invoke(manager, null);
        yield return WaitForLevel(6, null);
        Assert.That(SceneManager.GetSceneByBuildIndex(5).isLoaded, Is.False);
        Assert.That(RenderSettings.fog, Is.False);
        Assert.That(RenderSettings.fogMode, Is.EqualTo(FogMode.ExponentialSquared));
        Assert.That(RenderSettings.fogColor, Is.EqualTo(new Color(0.5f, 0.5f, 0.5f, 1f)));
        Assert.That(RenderSettings.fogStartDistance, Is.EqualTo(0f));
        Assert.That(RenderSettings.fogEndDistance, Is.EqualTo(300f));
        Assert.That(File.ReadAllText(savePath), Does.Contain("\"Level\":1"));

        object oldLevel = managerType.GetMethod("GetCurrentLevel").Invoke(null, null);
        managerType.GetMethod("OnLevelReset").Invoke(manager, null);
        yield return WaitForLevel(6, oldLevel);

        managerType.GetMethod("OnLevelFinished").Invoke(manager, null);
        yield return WaitForLevel(7, null);
        Terrain terrain = Array.Find(Terrain.activeTerrains, t => t.gameObject.scene.buildIndex == 7);
        Assert.That(terrain, Is.Not.Null);
        Assert.That(terrain.detailObjectDistance, Is.EqualTo(80f));
        Assert.That(terrain.treeMaximumFullLODCount, Is.EqualTo(50));
        Assert.That(terrain.heightmapPixelError, Is.EqualTo(5f));
        Assert.That(RenderSettings.fog, Is.False);

        managerType.GetField("LevelInfoObjName").SetValue(manager, "Missing level descriptor");
        LogAssert.Expect(LogType.Error, "LevelInfo not found in scene 7. Returning to the main menu.");
        managerType.GetMethod("OnLevelReset").Invoke(manager, null);
        float deadline = Time.realtimeSinceStartup + 60f;
        while (SceneManager.GetActiveScene().buildIndex != 0 && Time.realtimeSinceStartup < deadline)
        {
            yield return null;
        }
        Assert.That(SceneManager.GetActiveScene().buildIndex, Is.EqualTo(0));
        Assert.That(RenderSettings.fog, Is.False);

        File.WriteAllText(savePath, "{\"Name\":\"Scene test\",\"Level\":0,\"Inventory\":[]}");
        managerType.GetMethod("SetPlayerSaveToLoadOnGameManagerStart").Invoke(null, new object[] { savePath });
        yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        yield return WaitForLevel(5, null);
        Assert.That(RenderSettings.fog, Is.True);
        yield return SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        Assert.That(RenderSettings.fog, Is.False);
    }

    [UnityTest]
    public IEnumerator TouchCameraDeltaIsConsumedOnceAndDesktopLookIsPreserved()
    {
        yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        yield return WaitForLevel(5, null);
        Type controllerType = Assembly.Load("Assembly-CSharp").GetType("PlayerController", true);
        Component controller = UnityEngine.Object.FindFirstObjectByType(controllerType) as Component;
        Component input = controller.GetComponent(Assembly.Load("Assembly-CSharp").GetType("GameInputs", true));
        MethodInfo rotate = controllerType.GetMethod("CameraRotation", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo addDelta = input.GetType().GetMethod("AddLookDelta");
        MethodInfo lookInput = input.GetType().GetMethod("LookInput");
        FieldInfo yaw = controllerType.GetField("_cinemachineTargetYaw", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo paused = managerType.GetField("_gamePauseMenuIsVisible", BindingFlags.Static | BindingFlags.NonPublic);
        PlayerInput playerInput = controller.GetComponent<PlayerInput>();
        Mouse mouse = InputSystem.AddDevice<Mouse>();
        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        Gamepad gamepad = InputSystem.AddDevice<Gamepad>();
        try
        {
            playerInput.SwitchCurrentControlScheme("Gamepad", gamepad);
            lookInput.Invoke(input, new object[] { Vector2.zero });
            float previousYaw = (float)yaw.GetValue(controller);
            addDelta.Invoke(input, new object[] { new Vector2(12f, 0f) });
            rotate.Invoke(controller, null);
            Assert.That((float)yaw.GetValue(controller) - previousYaw, Is.EqualTo(12f).Within(0.001f));
            rotate.Invoke(controller, null);
            Assert.That((float)yaw.GetValue(controller) - previousYaw, Is.EqualTo(12f).Within(0.001f));

            controllerType.GetField("LockCameraPosition").SetValue(controller, true);
            addDelta.Invoke(input, new object[] { new Vector2(12f, 0f) });
            rotate.Invoke(controller, null);
            controllerType.GetField("LockCameraPosition").SetValue(controller, false);
            rotate.Invoke(controller, null);
            Assert.That((float)yaw.GetValue(controller) - previousYaw, Is.EqualTo(12f).Within(0.001f));

            paused.SetValue(null, true);
            addDelta.Invoke(input, new object[] { new Vector2(12f, 0f) });
            rotate.Invoke(controller, null);
            paused.SetValue(null, false);
            rotate.Invoke(controller, null);
            Assert.That((float)yaw.GetValue(controller) - previousYaw, Is.EqualTo(12f).Within(0.001f));

            playerInput.SwitchCurrentControlScheme("KeyboardMouse", keyboard, mouse);
            previousYaw = (float)yaw.GetValue(controller);
            lookInput.Invoke(input, new object[] { new Vector2(0.075f, 0f) });
            rotate.Invoke(controller, null);
            Assert.That((float)yaw.GetValue(controller) - previousYaw, Is.EqualTo(0.15f).Within(0.001f));

            playerInput.SwitchCurrentControlScheme("Gamepad", gamepad);
            lookInput.Invoke(input, new object[] { new Vector2(20f, 0f) });
            previousYaw = (float)yaw.GetValue(controller);
            rotate.Invoke(controller, null);
            Assert.That((float)yaw.GetValue(controller) - previousYaw, Is.EqualTo(20f * Time.deltaTime).Within(0.001f));
        }
        finally
        {
            paused.SetValue(null, false);
            lookInput.Invoke(input, new object[] { Vector2.zero });
            InputSystem.RemoveDevice(mouse);
            InputSystem.RemoveDevice(keyboard);
            InputSystem.RemoveDevice(gamepad);
        }
    }

    private IEnumerator WaitForLevel(int buildIndex, object previousLevel)
    {
        FieldInfo loadingDone = managerType.GetField("_levelLoadingDone", BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo currentLevel = managerType.GetMethod("GetCurrentLevel");
        MethodInfo currentScene = managerType.GetMethod("GetCurrentLevelSceneId");
        float deadline = Time.realtimeSinceStartup + 60f;
        while (Time.realtimeSinceStartup < deadline)
        {
            object level = currentLevel.Invoke(null, null);
            if ((bool)loadingDone.GetValue(null) && (int)currentScene.Invoke(null, null) == buildIndex &&
                level != null && !ReferenceEquals(level, previousLevel))
            {
                yield break;
            }
            yield return null;
        }
        Assert.Fail("Timed out waiting for level " + buildIndex);
    }
}
