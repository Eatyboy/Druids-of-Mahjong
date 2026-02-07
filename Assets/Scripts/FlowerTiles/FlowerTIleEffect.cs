using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class FlowerTileEffect : MonoBehaviour
{
    public abstract IEnumerator OnInitialize(List<Tile> playerHand, List<Tile> selectedHand);
    public abstract IEnumerator OnPlayHand(List<Tile> playerHand, List<Tile> selectedHand);
    // public abstract IEnumerator OnPlayHandWhileSelected(List<Tile> playerHand, List<Tile> selectedHand);
    public abstract IEnumerator OnIncomingAttack(int possibleDamage);
    public abstract IEnumerator OnTakeDamage(int damageTaken);
    public abstract IEnumerator OnDiscard();
}
