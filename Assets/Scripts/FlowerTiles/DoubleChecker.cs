using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;

public class DoubleChecker : FlowerTileEffect
{
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

    public override IEnumerator OnPlayHand(List<Tile> playerHand, List<Tile> selectedHand)
    {
        int damageBonus;
        
        switch (MahjongHands.GetMahjongHand(selectedHand))
        {
            case MahjongHandTypes.Pair:
                damageBonus = 5;
                break;
            case MahjongHandTypes.Set:
                damageBonus = 10;
                break;
            case MahjongHandTypes.Run:
                damageBonus = 15;
                break;
            case MahjongHandTypes.Quad:
                damageBonus = 20;
                break;
            case MahjongHandTypes.ThreePairs:
                damageBonus = 25;
                break;
            case MahjongHandTypes.SetAndRun:
                damageBonus = 30;
                break;
            case MahjongHandTypes.TwoRuns:
                damageBonus = 35;
                break;
            case MahjongHandTypes.TwoSets:
                damageBonus = 40;
                break;
            case MahjongHandTypes.TwoQuads:
                damageBonus = 45;
                break;
            case MahjongHandTypes.ThreeSets:
                damageBonus = 50;
                break;
            case MahjongHandTypes.NineRun:
                damageBonus = 60;
                break;
            case MahjongHandTypes.AllPairs:
                damageBonus = 65;
                break;
            case MahjongHandTypes.FullWin:
                damageBonus = 70;
                break;
            default:
                damageBonus = 0;
                break;
        }

        //For testing purposes
        int damageBefore = HandAttackResolver.GetFinalDamageForHand(selectedHand);
        
        HandAttackResolver.UpdateModifierBonus(damageBonus);

        int damageAfter = HandAttackResolver.GetFinalDamageForHand(selectedHand);
        
        // Debug.Log("Player did: " + damageBefore + " damage without flower tile.");

        // Debug.Log("Player did: " + damageAfter + " damage without flower tile.");

        yield return null;
    }
}
