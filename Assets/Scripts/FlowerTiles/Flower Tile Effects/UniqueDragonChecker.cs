using UnityEngine;
using System.Collections;

public class UniqueDragonChecker : FlowerTileEffect
{
    public override IEnumerator OnIntraAttack(Player.PlayerAttackContext attackContext)
    {
        float multiplier = 1.0f;
        bool[] seenDragons = new bool[4];
        foreach (Tile tile in attackContext.selectedHand)
        {
            if (tile.suit == TileSuit.Dragon)
            { 
                int dragonType = tile.rank;
                if (!seenDragons[dragonType]) {
                    seenDragons[dragonType] = true;
                    multiplier *= 2.0f;
                }
            }
        }
        attackContext.increasedDamageModifier *= multiplier;
        yield break;
    }
}
