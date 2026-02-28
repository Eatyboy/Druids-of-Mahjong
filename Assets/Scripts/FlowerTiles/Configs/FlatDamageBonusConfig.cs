using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class FlatDamageBonusConfig : FlowerTileEffectConfig
{
    public int flatDamageBonus = 1;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new FlatDamageBonus();
        rt.flatDamageBonus = flatDamageBonus;
        return rt;
    }

    public override Type GetEffectType() => typeof(FlatDamageBonus);

}

