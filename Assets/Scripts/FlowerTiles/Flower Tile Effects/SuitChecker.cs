using System.Collections;

public class SuitChecker : FlowerTileEffect
{
    [Configurable] public TileSuit suit;

    [Configurable] public int damage;

    public override IEnumerator OnIntraAttack(Player.PlayerAttackContext ctx)
    {
        foreach (Tile tile in ctx.selectedHand)
        {
            if (tile.suit == suit)
            {
                ctx.addedDamageModifier += damage;
            }
        }

        // For testing purposes
        // int damageBefore = HandAttackResolver.GetFinalDamageForHand(selectedHand);

        // HandAttackResolver.UpdateModifierBonus(damageBonus);

        // int damageAfter = HandAttackResolver.GetFinalDamageForHand(selectedHand);

        // Debug.Log("Player did: " + damageBefore + " damage without flower tile.");

        // Debug.Log("Player did: " + damageAfter + " damage with flower tile.");

        yield break;
    }
}
