using System;
using System.Collections.Generic;

/// <summary>
/// Result of resolving a played hand into an attack. Extensible for modifiers, status effects, etc.
/// </summary>
public class AttackResult
{
    public int BaseDamage { get; set; }
    public int FinalDamage { get; set; }
    public MahjongHandTypes HandType { get; set; }
    public List<Tile> Tiles { get; set; }

    // Future: tile modifiers, honor buffs, status effects
    // public List<DamageModifier> ModifiersApplied { get; set; }
    // public List<StatusEffect> StatusEffectsToApply { get; set; }
}

/// <summary>
/// Evaluates a played mahjong hand, computes damage (with hooks for modifiers and status effects),
/// and raises an event so the enemy (or other listeners) can react.
/// </summary>
public static class HandAttackResolver
{
    /// <summary>
    /// Raised when a hand attack is resolved. Subscribe to deal damage, show VFX, etc.
    /// </summary>
    public static event Action<AttackResult> OnAttackResolved;

    /// <summary>
    /// Takes the played hand, determines its type, computes damage, and raises OnAttackResolved.
    /// </summary>
    /// <param name="hand">The list of tiles representing the mahjong hand.</param>
    public static void ResolveHandAttack(List<Tile> hand)
    {
        if (hand == null || hand.Count == 0)
        {
            RaiseResult(0, 0, MahjongHandTypes.None, hand);
            return;
        }

        MahjongHandTypes handType = MahjongHands.GetMahjongHand(hand);
        int baseDamage = MahjongHands.GetScoreForHand(handType);

        int honorBonus = GetHonorDamageBonus(hand);
        int modifierBonus = GetTileModifierBonus(hand);
        int finalDamage = Math.Max(0, baseDamage + honorBonus + modifierBonus);

        // Future: collect status effects from tiles and attach to result
        // var statusEffects = GetStatusEffectsFromTiles(hand);

        RaiseResult(baseDamage, finalDamage, handType, hand);
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

    /// <summary>
    /// Bonus damage from honor tiles (Winds, Dragons). Override or extend for game-specific rules.
    /// </summary>
    static int GetHonorDamageBonus(List<Tile> hand)
    {
        if (hand == null) return 0;
        int honorCount = 0;
        foreach (var t in hand)
        {
            if (t != null && (t.suit == TileSuit.Winds || t.suit == TileSuit.Dragons))
                honorCount++;
        }
        // Placeholder: +1 per honor tile. Replace with your formula.
        return honorCount;
    }

    /// <summary>
    /// Extra damage from tile modifiers (e.g. equipment, enchanted tiles). Extend as needed.
    /// </summary>
    static int GetTileModifierBonus(List<Tile> hand)
    {
        if (hand == null) return 0;
        // Future: e.g. sum of tile.ModifierDamage, or check for Modifier component
        return 0;
    }
}
