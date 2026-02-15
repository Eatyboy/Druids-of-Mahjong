using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using System;
using System.Linq;

public class QiChecker : FlowerTileEffect
{
    public int qi;

    private readonly float qiMult = 1.0f; // Qi multiplier

    public override IEnumerator OnIntraAttack(Player.PlayerAttackContext ctx)
    {
        qi = GameManager.playerData.qi;

        ctx.addedDamageModifier += qi * qiMult;

        yield break;
    }
}
