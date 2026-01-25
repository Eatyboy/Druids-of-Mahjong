using UnityEngine;

/**
    This is where enemy will be created and the player too
*/
public class Unit : MonoBehaviour
{
    public string unitName;
    
    public int damage;
    
    public int maxHP;

    public int currentHP;

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
