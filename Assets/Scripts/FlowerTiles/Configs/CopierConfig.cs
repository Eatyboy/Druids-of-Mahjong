using UnityEngine;

[System.Serializable]
public class CopierConfig : FlowerTileEffectConfig
{
    public System.Collections.Generic.Queue<FlowerTileEffect> copiedEffectsList = new();
    public FlowerTileEffect copiedEffect;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new Copier();
        rt.copiedEffectsList = copiedEffectsList;
        rt.copiedEffect = copiedEffect;
        return rt;
    }
}

