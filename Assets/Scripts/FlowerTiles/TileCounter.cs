using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using System;
using System.Linq;

public class TileCounter : FlowerTileEffect
{

    private readonly float tileMult = 1.0f; // Damage multiplier

    public override IEnumerator OnIntraAttack(Player.PlayerAttackContext ctx)
    {
        int count = ctx.selectedHand.Count;

        ctx.addedDamageModifier += count * tileMult;
        yield break;
    }
}
