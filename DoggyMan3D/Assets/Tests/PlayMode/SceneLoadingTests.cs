using System;
using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SceneLoadingTests
{
    private string savePath;
    private Type managerType;

    [SetUp]
    public void SetUp()
    {
        savePath = Path.Combine(Path.GetTempPath(), "DoggyMan3D-scene-test-" + Guid.NewGuid().ToString("N") + ".json");
        File.WriteAllText(savePath, "{\"Name\":\"Scene test\",\"Level\":0,\"Inventory\":[]}");
        managerType = Assembly.Load("Assembly-CSharp").GetType("MainGameManager", true);
        managerType.GetMethod("SetPlayerSaveToLoadOnGameManagerStart").Invoke(null, new object[] { savePath });
    }

    [TearDown]
    public void TearDown()
    {
        Time.timeScale = 1f;
        managerType.GetMethod("SetPlayerSaveToLoadOnGameManagerStart").Invoke(null, new object[] { null });
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }

    [UnityTest]
    public IEnumerator LoadsAdvancesAndRestartsLevelsAndRecoversFromMissingDescriptor()
    {
        yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        yield return WaitForLevel(5, null);

        Component manager = UnityEngine.Object.FindObjectOfType(managerType) as Component;
        Assert.That(manager, Is.Not.Null);
        Assert.That(SceneManager.GetActiveScene().buildIndex, Is.EqualTo(1));

        managerType.GetMethod("OnLevelFinished").Invoke(manager, null);
        yield return WaitForLevel(6, null);
        Assert.That(SceneManager.GetSceneByBuildIndex(5).isLoaded, Is.False);
        Assert.That(File.ReadAllText(savePath), Does.Contain("\"Level\":1"));

        object oldLevel = managerType.GetMethod("GetCurrentLevel").Invoke(null, null);
        managerType.GetMethod("OnLevelReset").Invoke(manager, null);
        yield return WaitForLevel(6, oldLevel);

        managerType.GetField("LevelInfoObjName").SetValue(manager, "Missing level descriptor");
        LogAssert.Expect(LogType.Error, "LevelInfo not found in scene 6. Returning to the main menu.");
        managerType.GetMethod("OnLevelReset").Invoke(manager, null);
        float deadline = Time.realtimeSinceStartup + 60f;
        while (SceneManager.GetActiveScene().buildIndex != 0 && Time.realtimeSinceStartup < deadline)
        {
            yield return null;
        }
        Assert.That(SceneManager.GetActiveScene().buildIndex, Is.EqualTo(0));
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
