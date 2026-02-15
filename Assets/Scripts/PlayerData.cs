using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int health = 10;
    public int maxHealth = 10;
    public int qi = 0;
    public List<Tile> deck = null;
    public List<FlowerTileType> flowerTiles = new();
}

public static class SaveSystem
{
    private static string savePath => Application.persistentDataPath + "/saveData.json";

    public static void SaveData(PlayerData data)
    {
        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, jsonData);
    }

    public static PlayerData LoadData()
    {
        if (!File.Exists(savePath))
        {
            return new PlayerData();
        }

        string jsonData = File.ReadAllText(savePath);
        return JsonUtility.FromJson<PlayerData>(jsonData);
    }
}
