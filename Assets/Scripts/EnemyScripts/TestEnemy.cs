using UnityEngine;

public class TestEnemy : Enemy
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Attack();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            OnDeath();
        }
    }
}
