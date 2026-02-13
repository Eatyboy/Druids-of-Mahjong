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

    public override IEnumerator OnPostAttack(Player.PlayerAttackContext ctx)
    {
        qi = Player.GetQi();

        ctx.addedDamageModifier += qi * qiMult;

        yield break;
    }
}
