using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using System;
using System.Linq;

public class SuitChecker : FlowerTileEffect
{
    public TileSuit suit;

    public int damage;

    public override IEnumerator OnIntraAttack(Player.PlayerAttackContext ctx)
    {
        foreach (Tile tile in ctx.selectedHand)
        {
            if (tile.suit == suit)
            {
                ctx.addedDamageModifier += damage;
            }
        }

        // For testing purposes
        // int damageBefore = HandAttackResolver.GetFinalDamageForHand(selectedHand);

        // HandAttackResolver.UpdateModifierBonus(damageBonus);

        // int damageAfter = HandAttackResolver.GetFinalDamageForHand(selectedHand);

        // Debug.Log("Player did: " + damageBefore + " damage without flower tile.");

        // Debug.Log("Player did: " + damageAfter + " damage with flower tile.");

        yield break;
    }
}
