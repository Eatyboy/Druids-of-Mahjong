using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class DummyEffectConfig : FlowerTileEffectConfig
{

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new DummyEffect();
        return rt;
    }

    public override Type GetEffectType() => typeof(DummyEffect);

}

