using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MagicSquareConfig : FlowerTileEffectConfig
{

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new MagicSquare();
        return rt;
    }
}

