using UnityEngine;

[System.Serializable]
public class EnemyHealthCheckerConfig : FlowerTileEffectConfig
{
    public System.Single healthMult;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new EnemyHealthChecker();
        rt.healthMult = healthMult;
        return rt;
    }
}

