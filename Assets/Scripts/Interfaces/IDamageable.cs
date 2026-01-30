using UnityEngine;

[Tooltip("Interface Class that allows Any Object/Unit to take Damage")]
public interface IDamageable
{
    // Can be expandable to include other methods
    bool TakeDamage(int dmg) { return true; }
}
