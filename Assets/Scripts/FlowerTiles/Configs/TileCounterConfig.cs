using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileCounterConfig : FlowerTileEffectConfig
{
    public float addedDamagePerTile = 1.0f;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new TileCounter();
        rt.addedDamagePerTile = addedDamagePerTile;
        return rt;
    }
}

