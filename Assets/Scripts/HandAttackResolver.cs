using System;
using System.Collections;
using System.Collections.Generic;

// Result of resolving a played hand into an attack
public class AttackResult
{
    public int BaseDamage { get; set; }
    public int FinalDamage { get; set; }
    public MahjongHandTypes HandType { get; set; }
    public List<Tile> Tiles { get; set; }
}

// Evaluates a played mahjong hand, computes damage,
// and raises an event so the enemy (or other listeners) can react
public static class HandAttackResolver
{
    // Raised when a hand attack is resolved. Subscribe to deal damage, show VFX, etc
    public static event Action<AttackResult> OnAttackResolved;

    public static IEnumerator HandAttack(List<Tile> hand)
    {
        if (hand == null || hand.Count == 0)
        {
            RaiseResult(0, 0, MahjongHandTypes.None, hand);
            yield break;
        }

        MahjongHandTypes handType = MahjongHands.GetMahjongHand(hand);
        int baseDamage = MahjongHands.GetScoreForHand(handType);

        int honorBonus = GetHonorDamageBonus(hand);
        int modifierBonus = GetTileModifierBonus(hand);
        int finalDamage = Math.Max(0, baseDamage + honorBonus + modifierBonus);

        RaiseResult(baseDamage, finalDamage, handType, hand);

        yield break;
    }

    static void RaiseResult(int baseDamage, int finalDamage, MahjongHandTypes handType, List<Tile> tiles)
    {
        var result = new AttackResult
        {
            BaseDamage = baseDamage,
            FinalDamage = finalDamage,
            HandType = handType,
            Tiles = tiles ?? new List<Tile>()
        };
        OnAttackResolved?.Invoke(result);
    }

    // Bonus damage from honor tiles (Wind, Dragon)
    // Simply +1 damage per honor tile for now
    // Can be changed later
    static int GetHonorDamageBonus(List<Tile> hand)
    {
        if (hand == null) return 0;
        int honorCount = 0;
        foreach (var t in hand)
        {
            if (t != null && (t.suit == TileSuit.Wind || t.suit == TileSuit.Dragon))
                honorCount++;
        }
        return honorCount;
    }

    // Extra damage from tile modifiers
    // Nothing implemented yet, but can be later
    static int GetTileModifierBonus(List<Tile> hand)
    {
        if (hand == null) return 0;
        // Implement later
        return 0;
    }
}
