using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

public class SaveSystemTests
{
    private string directory;
    private Type saveSystem;
    private Type playerSave;

    [SetUp]
    public void SetUp()
    {
        directory = Path.Combine(Path.GetTempPath(), "DoggyMan3D-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);

        // Test assemblies cannot directly reference Unity's predefined Assembly-CSharp.
        Assembly gameAssembly = Assembly.Load("Assembly-CSharp");
        saveSystem = gameAssembly.GetType("SaveSystem", true);
        playerSave = gameAssembly.GetType("PlayerSave", true);
    }

    [TearDown]
    public void TearDown()
    {
        foreach (string file in Directory.GetFiles(directory))
        {
            File.Delete(file);
        }
        Directory.Delete(directory);
    }

    [Test]
    public void SameNameCreatesIndependentSavesAndUpdatesOnlySelectedSave()
    {
        object first = NewSave("Doggy / Man");
        object second = NewSave("Doggy / Man");
        string firstPath = (string)Call("CreatePlayerSave", first, directory);
        string secondPath = (string)Call("CreatePlayerSave", second, directory);

        Assert.That(firstPath, Is.Not.EqualTo(secondPath));
        playerSave.GetField("Level").SetValue(first, 2);
        Call("SavePlayer", first, firstPath);

        Assert.That(playerSave.GetField("Level").GetValue(Call("LoadPlayer", firstPath)), Is.EqualTo(2));
        object loadedSecond = Call("LoadPlayer", secondPath);
        Assert.That(playerSave.GetField("Level").GetValue(loadedSecond), Is.EqualTo(0));
        Assert.That(playerSave.GetField("Name").GetValue(loadedSecond), Is.EqualTo("Doggy / Man"));
    }

    [Test]
    public void LegacySaveIsUpdatedInItsOriginalFile()
    {
        string path = Path.Combine(directory, "Old_Player.json");
        File.WriteAllText(path, "{\"Name\":\"Old Player\",\"Level\":1}");
        object loaded = Call("LoadPlayer", path);
        playerSave.GetField("Level").SetValue(loaded, 3);

        Call("SavePlayer", loaded, path);

        object reloaded = Call("LoadPlayer", path);
        Assert.That(playerSave.GetField("Name").GetValue(reloaded), Is.EqualTo("Old Player"));
        Assert.That(playerSave.GetField("Level").GetValue(reloaded), Is.EqualTo(3));
        Assert.That(Directory.GetFiles(directory), Has.Length.EqualTo(1));
    }

    [Test]
    public void FailedReplacementLeavesOtherSavesAndNoTemporaryFile()
    {
        string existingPath = Path.Combine(directory, "existing.json");
        File.WriteAllText(existingPath, "{\"Name\":\"Existing\",\"Level\":2}");
        string missingPath = Path.Combine(directory, "missing.json");

        Assert.Throws<TargetInvocationException>(() => Call("SavePlayer", NewSave("New"), missingPath));

        Assert.That(File.ReadAllText(existingPath), Is.EqualTo("{\"Name\":\"Existing\",\"Level\":2}"));
        Assert.That(File.Exists(missingPath), Is.False);
        Assert.That(Directory.GetFiles(directory), Has.Length.EqualTo(1));
    }

    [Test]
    public void SelectingSaveClearsNameFromPreviousNewGame()
    {
        Type manager = saveSystem.Assembly.GetType("MainGameManager", true);
        FieldInfo name = manager.GetField("_playerNameToSet", BindingFlags.Static | BindingFlags.NonPublic);
        FieldInfo path = manager.GetField("_playerSaveToLoadPath", BindingFlags.Static | BindingFlags.NonPublic);
        object originalName = name.GetValue(null);
        object originalPath = path.GetValue(null);

        try
        {
            manager.GetMethod("SetPlayerNameOnGameManagerStart").Invoke(null, new object[] { "Previous player" });
            manager.GetMethod("SetPlayerSaveToLoadOnGameManagerStart").Invoke(null, new object[] { "old-save.json" });

            Assert.That(name.GetValue(null), Is.Null);
        }
        finally
        {
            name.SetValue(null, originalName);
            path.SetValue(null, originalPath);
        }
    }

    private object NewSave(string name)
    {
        object save = Activator.CreateInstance(playerSave);
        playerSave.GetField("Name").SetValue(save, name);
        return save;
    }

    private object Call(string method, params object[] arguments)
    {
        return saveSystem.GetMethod(method).Invoke(null, arguments);
    }
}
