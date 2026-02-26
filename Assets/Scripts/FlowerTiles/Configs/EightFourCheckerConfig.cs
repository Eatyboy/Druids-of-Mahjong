using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EightFourCheckerConfig : FlowerTileEffectConfig
{
    public float eightMultiplier = 1.5f;
    public float fourMultiplier = 0.75f;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new EightFourChecker();
        rt.eightMultiplier = eightMultiplier;
        rt.fourMultiplier = fourMultiplier;
        return rt;
    }
}

