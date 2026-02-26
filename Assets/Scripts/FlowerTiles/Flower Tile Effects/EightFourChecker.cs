using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EightFourChecker : FlowerTileEffect
{
    [Configurable] public float eightMultiplier = 1.5f;
    [Configurable] public float fourMultiplier = 0.75f;

    static bool IsNumberedSuit(TileSuit suit) =>
        suit == TileSuit.Bamboo || suit == TileSuit.Dot || suit == TileSuit.Character;

    public override IEnumerator OnPostAttack(Player.PlayerAttackContext attackContext)
    {
        float multiplier = 1.0f;
        foreach (Tile tile in attackContext.selectedHand)
        {
            if (!IsNumberedSuit(tile.suit)) continue;

            if (tile.rank == 8)
                multiplier *= eightMultiplier;
            else if (tile.rank == 4)
                multiplier *= fourMultiplier;
        }

        attackContext.damage *= multiplier;
        yield break;
    }
}