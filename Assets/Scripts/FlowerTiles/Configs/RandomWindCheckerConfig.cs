using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RandomWindCheckerConfig : FlowerTileEffectConfig
{
    public float damageMultiplier = 1.5f;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new RandomWindChecker();
        rt.damageMultiplier = damageMultiplier;
        return rt;
    }
}

