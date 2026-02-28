using System;
using UnityEngine;

[System.Serializable]
public class FlowerTileInstance
{
    public FlowerTileData data;
    public FlowerTileEffect effect;

    public FlowerTileInstance(FlowerTileData tileData)
    {
        data = tileData;
        effect = data.effectConfig.CreateInstance();
    }

    public FlowerTileInstance(FlowerTileSaveData saveData)
    {
        data = FlowerTileManager.instance.flowerTileMap[saveData.tileType];
        Type effectType = data.effectConfig.GetEffectType();
        effect = FlowerTileEffect.Deserialize(saveData.effectDataJson, effectType);
    }

    public FlowerTileSaveData GetSaveData()
    {
        return new FlowerTileSaveData()
        {
            tileType = data.id,
            effectDataJson = effect.Serialize(),
        };
    }

    public string GetDescription()
    {
        var dynamicDescription = effect.GetDynamicDescription();
        return string.IsNullOrEmpty(dynamicDescription) 
            ? data.description
            : dynamicDescription;
    }
}

[System.Serializable]
public class FlowerTileSaveData
{
    public FlowerTileType tileType;
    public string effectDataJson;
}
