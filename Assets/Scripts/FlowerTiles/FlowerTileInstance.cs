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

    public string GetDescription()
    {
        var dynamicDescription = effect.GetDynamicDescription();
        return string.IsNullOrEmpty(dynamicDescription) 
            ? data.description
            : dynamicDescription;
    }
}
