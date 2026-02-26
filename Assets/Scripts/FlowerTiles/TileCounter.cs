using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using System;
using System.Linq;

public class TileCounter : FlowerTileEffect
{
    [Configurable] public float addedDamagePerTile = 1.0f;

    public override IEnumerator OnIntraAttack(Player.PlayerAttackContext ctx)
    {
        int count = ctx.selectedHand.Count;

        ctx.addedDamageModifier += count * addedDamagePerTile;
        yield break;
    }
}
