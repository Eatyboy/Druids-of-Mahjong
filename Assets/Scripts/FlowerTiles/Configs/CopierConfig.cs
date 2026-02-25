using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CopierConfig : FlowerTileEffectConfig
{
    public Queue<FlowerTileEffect> copiedEffectsList = new Queue<FlowerTileEffect>();

    public FlowerTileEffect copiedEffect;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new Copier();
        
        rt.copiedEffectsList = copiedEffectsList;

        rt.copiedEffect = copiedEffect;
        
        return rt;
    }
}

