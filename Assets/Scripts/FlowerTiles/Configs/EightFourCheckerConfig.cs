using UnityEngine;

[System.Serializable]
public class EightFourCheckerConfig : FlowerTileEffectConfig
{

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new EightFourChecker();
        return rt;
    }
}

