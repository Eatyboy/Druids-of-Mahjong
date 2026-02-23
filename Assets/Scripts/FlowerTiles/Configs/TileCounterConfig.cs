using UnityEngine;

[System.Serializable]
public class TileCounterConfig : FlowerTileEffectConfig
{

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new TileCounter();
        return rt;
    }
}

