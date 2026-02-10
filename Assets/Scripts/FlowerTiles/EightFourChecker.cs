using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EightFourChecker : FlowerTileEffect
{
    public override IEnumerator OnInitialize(List<Tile> playerHand, List<Tile> selectedHand)
    {
        yield break;
    }

    public override IEnumerator OnPlayHand(Player.PlayerAttackContext attackContext)
    {
        float multiplier = 1.0f;
        foreach (Tile tile in attackContext.selectedHand)
        {
            if (tile.rank == 8)
            {
                multiplier *= 1.5f;
            }
            else if (tile.rank == 4)
            {
                multiplier *= 0.75f;
            }
        }

        // apply multiplier to damage
        // INSERT CODE HERE

        yield break;
    }

    public override IEnumerator OnIncomingAttack(int possibleDamage)
    {
        yield break;
    }

    public override IEnumerator OnTakeDamage(int damageTaken)
    {
        yield break;
    }

    public override IEnumerator OnDiscard()
    {
        yield break;
    }
}