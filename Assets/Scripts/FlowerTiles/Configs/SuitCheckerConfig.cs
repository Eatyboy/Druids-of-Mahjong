using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class SuitCheckerConfig : FlowerTileEffectConfig
{
    public TileSuit suit = TileSuit.None;
    public int damage = 0;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new SuitChecker();
        rt.suit = suit;
        rt.damage = damage;
        return rt;
    }

    public override Type GetEffectType() => typeof(SuitChecker);

}

