using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class VampirismConfig : FlowerTileEffectConfig
{
    public float healPercent = 0.25f;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new Vampirism();
        rt.healPercent = healPercent;
        return rt;
    }
}

