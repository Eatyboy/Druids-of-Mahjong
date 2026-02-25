using UnityEngine;

[System.Serializable]
public class SuitCheckerConfig : FlowerTileEffectConfig
{
    public TileSuit suit;
    public System.Int32 damage;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new SuitChecker();
        rt.suit = suit;
        rt.damage = damage;
        return rt;
    }
}

