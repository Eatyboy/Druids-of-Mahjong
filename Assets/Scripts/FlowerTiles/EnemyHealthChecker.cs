using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using System;
using System.Linq;

public class EnemyHealthChecker : FlowerTileEffect
{

    public float healthMult = 1.5f; // Damage multiplier

    public override IEnumerator OnPostAttack(Player.PlayerAttackContext ctx)
    {
        if (EnemyManager.instance.currentEnemy.currentHP == EnemyManager.instance.currentEnemy.maxHP){
            ctx.damage *= healthMult;
        }

        yield break;
    }
}
