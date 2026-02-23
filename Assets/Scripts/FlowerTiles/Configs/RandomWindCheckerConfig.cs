using UnityEngine;

[System.Serializable]
public class RandomWindCheckerConfig : FlowerTileEffectConfig
{
    public System.Int32 currentWind;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new RandomWindChecker();
        rt.currentWind = currentWind;
        return rt;
    }
}

