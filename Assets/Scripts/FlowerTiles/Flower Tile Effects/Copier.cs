using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Copier : FlowerTileEffect
{
    public Queue<FlowerTileEffect> copiedEffectsList = new Queue<FlowerTileEffect>();

    public FlowerTileEffect copiedEffect;

    public void AddCopiedEffect(FlowerTileEffect effectToCopy)
    {
        copiedEffectsList.Enqueue(effectToCopy);
    }

    public void ClearCopiedEffect()
    {
        copiedEffectsList.Clear();
        copiedEffect = null;
    }

    public override IEnumerator OnPreAttack(Player.PlayerAttackContext attackContext)
    {
        if (copiedEffectsList != null && copiedEffectsList.Count > 0)
        {
            copiedEffect = copiedEffectsList.Dequeue();
        }

        if (copiedEffect != null) yield return copiedEffect.OnPreAttack(attackContext);
        yield break;
    }

    public override IEnumerator OnIntraAttack(Player.PlayerAttackContext attackContext)
    {        
        Debug.Log(copiedEffectsList.Count);

        if (copiedEffectsList != null && copiedEffectsList.Count > 0)
        {
            copiedEffect = copiedEffectsList.Dequeue();
        }

        if (copiedEffect != null) yield return copiedEffect.OnIntraAttack(attackContext);
        yield break;
    }

    public override IEnumerator OnPostAttack(Player.PlayerAttackContext attackContext)
    {
        if (copiedEffectsList != null && copiedEffectsList.Count > 0)
        {
            copiedEffect = copiedEffectsList.Dequeue();
        }

        if (copiedEffect != null) yield return copiedEffect.OnPostAttack(attackContext);
        yield break;
    }

    public override IEnumerator OnIncomingAttack(int possibleDamage)
    {
        if (copiedEffectsList != null && copiedEffectsList.Count > 0)
        {
            copiedEffect = copiedEffectsList.Dequeue();
        }

        if (copiedEffect != null) yield return copiedEffect.OnIncomingAttack(possibleDamage);
        yield break;
    }

    public override IEnumerator OnTakeDamage(int damageTaken)
    {   
        if (copiedEffectsList != null && copiedEffectsList.Count > 0)
        {
            copiedEffect = copiedEffectsList.Dequeue();
        }

        if (copiedEffect != null) yield return copiedEffect.OnTakeDamage(damageTaken);
        yield break;
    }

    public override IEnumerator OnTurnStart()
    {
        Debug.Log(copiedEffectsList.Count);

        if (copiedEffectsList != null && copiedEffectsList.Count > 0)
        {
            copiedEffect = copiedEffectsList.Dequeue();
        }

        if (copiedEffect != null) yield return copiedEffect.OnTurnStart();
        yield break;
    }
}