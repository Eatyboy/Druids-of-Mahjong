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
    /// <summary>
    /// Computes the base damage for an attack
    /// </summary>
    /// <param name="ctx">The current attack context</param>
    public static IEnumerator GetBaseAttackDamage(Player.PlayerAttackContext ctx)
    {
        if (ctx.selectedHand == null || ctx.selectedHand.Count == 0)
        {
            ctx.baseDamage = 0;
            ctx.damage = 0;
            ctx.handType = MahjongHandTypes.None;
            yield break;
        }

        var hand = ctx.selectedHand;

        MahjongHandTypes handType = MahjongHands.GetMahjongHand(hand);
        int baseDamage = MahjongHands.GetScoreForHand(handType);

        int honorBonus = GetHonorDamageBonus(hand);
        int modifierBonus = GetTileModifierBonus(hand);

        int finalDamage = Math.Max(0, baseDamage + honorBonus + modifierBonus);

        ctx.baseDamage = baseDamage;
        ctx.damage = finalDamage;
        ctx.handType = handType;

        yield break;
    }

    /// <summary>
    /// Computes the base damage for an parry
    /// </summary>
    /// <param name="ctx">The current parry context</param>
    public static int GetBaseParryDamage(ParryHandler.ParryContext ctx)
    {
        if (ctx.parryHand == null || ctx.parryHand.Count == 0) return 0;

        var hand = ctx.parryHand;
        MahjongHandTypes handType = ctx.parryHandType;

        int baseDamage = MahjongHands.GetScoreForHand(handType);

        int honorBonus = GetHonorDamageBonus(hand);
        int modifierBonus = GetTileModifierBonus(hand);
        int finalDamage = Math.Max(0, baseDamage + honorBonus + modifierBonus);

        return finalDamage;
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

        return 0;
    }
}
