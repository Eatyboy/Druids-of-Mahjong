using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using System;
using System.Linq;

public class QiChecker : FlowerTileEffect
{
    [Configurable] public float qiMult = 1.0f; // Qi multiplier

    public override IEnumerator OnIntraAttack(Player.PlayerAttackContext ctx)
    {
        float qi = GameManager.playerData.qi;

        ctx.addedDamageModifier += qi * qiMult;

        yield break;
    }
}
