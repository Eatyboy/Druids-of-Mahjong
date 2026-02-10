using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using System;
using System.Linq;

public class SuitChecker : FlowerTileEffect
{
    [Serializable]
    class SuitBonusDamage
    {
        public TileSuit suit;

        public int damage;
    }

    [SerializeField] private List<SuitBonusDamage> suitToCheck;

    private Dictionary<TileSuit, int> bonusDamage;

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
    public override IEnumerator OnInitialize(List<Tile> playerHand, List<Tile> selectedHand)
    {
        yield break;
    }

    public void Awake()
    {
        bonusDamage = new Dictionary<TileSuit, int>();

        foreach (var tile in suitToCheck)
        {
            bonusDamage[tile.suit] = tile.damage;
        }
    }

    public override IEnumerator OnPlayHand(List<Tile> playerHand, List<Tile> selectedHand)
    {
        int damageBonus = 0;

        foreach (Tile tile in selectedHand)
        {
            if (bonusDamage.TryGetValue(tile.suit, out int damage))
            {
                damageBonus += damage;
            }
        }
        
        // For testing purposes
        // int damageBefore = HandAttackResolver.GetFinalDamageForHand(selectedHand);
        
        // HandAttackResolver.UpdateModifierBonus(damageBonus);

        // int damageAfter = HandAttackResolver.GetFinalDamageForHand(selectedHand);
        
        // Debug.Log("Player did: " + damageBefore + " damage without flower tile.");

        // Debug.Log("Player did: " + damageAfter + " damage with flower tile.");

        yield return null;
    }
}
