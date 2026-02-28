using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class EnemyHealthCheckerConfig : FlowerTileEffectConfig
{
    public float healthMult = 1.5f;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new EnemyHealthChecker();
        rt.healthMult = healthMult;
        return rt;
    }

    public override Type GetEffectType() => typeof(EnemyHealthChecker);

}

