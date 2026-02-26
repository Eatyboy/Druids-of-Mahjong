using System.Collections;
using UnityEngine;

public class Vampirism : FlowerTileEffect
{
    [Configurable] public float healPercent = 0.25f;

    public override IEnumerator OnPostAttack(Player.PlayerAttackContext attackContext)
    {
        float damageDealt = attackContext.damage;
        int healAmount = Mathf.CeilToInt(damageDealt * healPercent);
        if (healAmount > 0 && Player.instance != null)
        {
            Player.instance.ChangeHealth(healAmount);
        }
        yield break;
    }
}
