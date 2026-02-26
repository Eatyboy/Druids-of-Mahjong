using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class QiCheckerConfig : FlowerTileEffectConfig
{
    public float qiMult = 1.0f;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new QiChecker();
        rt.qiMult = qiMult;
        return rt;
    }
}

