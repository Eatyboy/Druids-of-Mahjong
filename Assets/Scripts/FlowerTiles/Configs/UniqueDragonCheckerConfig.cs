using UnityEngine;

[System.Serializable]
public class UniqueDragonCheckerConfig : FlowerTileEffectConfig
{

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new UniqueDragonChecker();
        return rt;
    }
}

