using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public GameState gameState = GameState.TitleScreen;
    public int health = 10;
    public int maxHealth = 10;
    public int qi = 0;
    public List<Tile> deck = null;
    public int maxFlowerTiles = 5;
    public List<FlowerTileInstance> flowerTiles = new();
}

public static class SaveSystem
{
    public static string savePath { get; private set; }

    public static void Initialize()
    {
        savePath = Application.persistentDataPath + "/saveData.json";
    }

    public static async Task Save(PlayerData data)
    {
        string jsonData = JsonUtility.ToJson(data, true);

        using (StreamWriter sw = new(savePath, false))
        {
            await sw.WriteAsync(jsonData);
        }
    }

    public static async Task DeleteSave()
    {
        if (File.Exists(savePath))
        {
            await Task.Run(() => File.Delete(savePath));
        }
    }

    public static async Task<PlayerData> Load()
    {
        if (!File.Exists(savePath))
        {
            return new PlayerData();
        }

        using (StreamReader sr = new(savePath))
        {
            string jsonData = await sr.ReadToEndAsync();
            return JsonUtility.FromJson<PlayerData>(jsonData);
        }
    }
}
