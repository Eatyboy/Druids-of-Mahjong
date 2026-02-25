using UnityEngine;

[System.Serializable]
public class DummyEffectConfig : FlowerTileEffectConfig
{

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new DummyEffect();
        return rt;
    }
}

