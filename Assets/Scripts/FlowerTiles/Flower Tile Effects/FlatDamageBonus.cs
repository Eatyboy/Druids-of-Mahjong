using System.Collections;
using UnityEngine;

public class FlatDamageBonus : FlowerTileEffect 
{
    public int flatDamageBonus = 1;

    public override IEnumerator OnIntraAttack(Player.PlayerAttackContext attackContext)
    {
        attackContext.addedDamageModifier += flatDamageBonus;
        yield break;
    }
}
