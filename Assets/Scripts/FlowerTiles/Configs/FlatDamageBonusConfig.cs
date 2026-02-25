using UnityEngine;

[System.Serializable]
public class FlatDamageBonusConfig : FlowerTileEffectConfig
{
    public System.Int32 flatDamageBonus;

    public override FlowerTileEffect CreateInstance()
    {
        var rt = new FlatDamageBonus();
        rt.flatDamageBonus = flatDamageBonus;
        return rt;
    }
}

