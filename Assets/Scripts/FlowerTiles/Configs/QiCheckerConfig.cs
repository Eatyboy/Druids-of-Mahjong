using UnityEngine;

[System.Serializable]
public class QiCheckerConfig : FlowerTileEffectConfig
{
    public System.Int32 qi;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new QiChecker();
        rt.qi = qi;
        return rt;
    }
}

