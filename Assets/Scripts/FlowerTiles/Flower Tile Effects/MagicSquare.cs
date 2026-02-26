using System.Collections;
using System.Linq;
using UnityEngine;

public class MagicSquare : FlowerTileEffect
{
    private const int MagicSum = 15;
    private const int BonusDamage = 15;

    private static bool IsNumberedSuit(TileSuit s) =>
        s == TileSuit.Bamboo || s == TileSuit.Dot || s == TileSuit.Character;

    public override IEnumerator OnIntraAttack(Player.PlayerAttackContext attackContext)
    {
        if (attackContext.selectedHand == null || attackContext.selectedHand.Count == 0)
            yield break;

        int rankSum = attackContext.selectedHand
            .Where(t => IsNumberedSuit(t.suit))
            .Sum(t => t.rank);

        if (rankSum == MagicSum)
            attackContext.addedDamageModifier += BonusDamage;

        yield break;
    }
}
