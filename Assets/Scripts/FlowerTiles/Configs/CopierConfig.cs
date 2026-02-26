using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CopierConfig : FlowerTileEffectConfig
{

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new Copier();
        return rt;
    }
}

