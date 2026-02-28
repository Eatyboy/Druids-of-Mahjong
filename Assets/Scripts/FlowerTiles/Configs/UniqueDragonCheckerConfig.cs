using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class UniqueDragonCheckerConfig : FlowerTileEffectConfig
{
    public float damageMultiplier = 2.0f;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new UniqueDragonChecker();
        rt.damageMultiplier = damageMultiplier;
        return rt;
    }

    public override Type GetEffectType() => typeof(UniqueDragonChecker);

}

