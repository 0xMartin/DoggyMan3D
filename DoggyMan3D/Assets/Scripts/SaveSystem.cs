using System;
using System.IO;
using UnityEngine;

public class SaveSystem
{
    public static string SavePath => Application.persistentDataPath;

    public static string CreatePlayerSave(PlayerSave playerSave, string directoryPath)
    {
        string json = JsonUtility.ToJson(playerSave);
        string path = Path.Combine(directoryPath, "save-" + Guid.NewGuid().ToString("N") + ".json");
        using (StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.CreateNew, FileAccess.Write)))
        {
            writer.Write(json);
        }
        Debug.Log("Game saved to file: " + path);
        return path;
    }

    public static void SavePlayer(PlayerSave playerSave, string path)
    {
        string json = JsonUtility.ToJson(playerSave);
        string temporaryPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (FileStream stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(json);
                writer.Flush();
                stream.Flush(true);
            }
            File.Replace(temporaryPath, path, null);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
        Debug.Log("Game saved to file: " + path);
    }

    public static PlayerSave LoadPlayer(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("Game loaded from file: " + path);
            return JsonUtility.FromJson<PlayerSave>(json);
        }

        return null;
    }

    public static string[] ListFilesInDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            return Array.Empty<string>();
        }

        return Directory.GetFiles(directoryPath);
    }
}