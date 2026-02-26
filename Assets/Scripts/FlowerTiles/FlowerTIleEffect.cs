using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class FlowerTileEffect
{
    public virtual string GetDynamicDescription() { return string.Empty; }

    public virtual IEnumerator OnInitialize(List<Tile> playerHand, List<Tile> selectedHand) { yield break; }

    // These three correspond to the three phases of attack damage calculation:
    // Before the base damage is computed, before the intermediate (base + added) * increased
    // is computed, and after the intermediate damage is computed
    public virtual IEnumerator OnPreAttack(Player.PlayerAttackContext attackContext) { yield break; }
    public virtual IEnumerator OnIntraAttack(Player.PlayerAttackContext attackContext) { yield break; }
    public virtual IEnumerator OnPostAttack(Player.PlayerAttackContext attackContext) { yield break; }

    // public abstract IEnumerator OnPlayHandWhileSelected(List<Tile> playerHand, List<Tile> selectedHand);
    public virtual IEnumerator OnIncomingAttack(int possibleDamage) { yield break; }
    public virtual IEnumerator OnTakeDamage(int damageTaken) { yield break; }
    public virtual IEnumerator OnDiscard() { yield break; }
    public virtual IEnumerator OnTurnStart() { yield break; }

    public virtual string Serialize() 
    { 
        return JsonUtility.ToJson(this);
    }

    public virtual void Deserialize(string jsonData)
    {
        JsonUtility.FromJsonOverwrite(jsonData, this);
    }
}
