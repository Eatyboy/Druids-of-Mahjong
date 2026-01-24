using UnityEngine;

// Interface Class that allows Players and Different Enemies to take Damage
public interface IDamageable
{
    public int Health { get; set; }
    void TakeDamage(int dmg) { }
}
