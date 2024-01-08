using System.IO;
using System.Linq;
using UnityEngine;

public class SaveSystem
{
    private static string SavePath => Application.persistentDataPath;

    public static void SavePlayer(PlayerSave playerSave)
    {
        string json = JsonUtility.ToJson(playerSave);
        string path = Path.Combine(SavePath, playerSave.Name.Replace(' ', '_') + ".json");
        File.WriteAllText(path, json);
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
            return null;
        }

        return Directory.GetFiles(directoryPath);
    }
}