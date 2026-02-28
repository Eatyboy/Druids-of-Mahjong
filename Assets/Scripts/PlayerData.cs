using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public PlayerData() { }

    public PlayerData(PlayerSaveData saveData)
    {
        gameState = saveData.gameState;
        health = saveData.health;
        maxHealth = saveData.maxHealth;
        qi = saveData.qi;
        deck = saveData.deck.Select(t => new Tile(t)).ToList();
        maxFlowerTiles = saveData.maxFlowerTiles;
        flowerTiles = saveData.flowerTiles.Select(ft => new FlowerTileInstance(ft)).ToList();
    }

    public PlayerSaveData GetSaveData()
    {
        return new PlayerSaveData()
        {
            gameState = gameState, 
            health = health, 
            maxHealth = maxHealth, 
            qi = qi,
            deck = deck.Select(t => t.GetSaveData()).ToList(),
            maxFlowerTiles = maxFlowerTiles,
            flowerTiles = flowerTiles.Select(ft => ft.GetSaveData()).ToList(), 
        };
    }
}

[Serializable]
public class PlayerSaveData
{
    public GameState gameState = GameState.TitleScreen;
    public int health = 10;
    public int maxHealth = 10;
    public int qi = 0;
    public List<TileSaveData> deck = null;
    public int maxFlowerTiles = 5;
    public List<FlowerTileSaveData> flowerTiles = new();
}

public static class SaveSystem
{
    public static string savePath { get; private set; }
    public static string settingsPath { get; private set; }

    public static bool loaded { get; private set; }

    public static void Initialize()
    {
        savePath = Application.persistentDataPath + "/saveData.json";
        settingsPath = Application.persistentDataPath + "/settings.json";
    }

    public static async Task Save(PlayerData data)
    {
        string jsonData = JsonUtility.ToJson(data.GetSaveData(), true);

        using (StreamWriter sw = new(savePath, false))
        {
            await sw.WriteAsync(jsonData);
        }
    }

    public static async Task SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(SettingsManager.instance.GetSaveData());

        using (StreamWriter sw = new(settingsPath, false))
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

    public static async Task DeleteSettings()
    {
        if (File.Exists(settingsPath))
        {
            await Task.Run(() => File.Delete(settingsPath));
        }
    }

    public static async Task<PlayerData> LoadPlayerSave()
    {
        if (!File.Exists(savePath))
        {
            return new PlayerData();
        }

        using (StreamReader sr = new(savePath))
        {
            string jsonData = await sr.ReadToEndAsync();
            var playerSaveData = JsonUtility.FromJson<PlayerSaveData>(jsonData);
            return new PlayerData(playerSaveData);
        }
    }

    public static async Task LoadSettings()
    {
        loaded = false;

        if (!File.Exists(settingsPath))
        {
            await SaveSettings();
            loaded = true;
            return;
        }

        using (StreamReader sr = new(settingsPath))
        {
            string jsonData = await sr.ReadToEndAsync();
            var settingsSaveData = JsonUtility.FromJson<SettingsSaveData>(jsonData);
            SettingsManager.instance.LoadSettings(settingsSaveData);
            loaded = true;
        }
    }
}
