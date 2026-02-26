using UnityEngine;
using System.Collections;

public class UniqueDragonChecker : FlowerTileEffect
{
    [Configurable] public float damageMultiplier = 2.0f;

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
                    multiplier *= damageMultiplier;
                }
            }
        }
        attackContext.increasedDamageModifier *= multiplier;
        yield break;
    }
}
