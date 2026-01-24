using UnityEngine;

// Abstract Class that Enemies should inherit from
// Only Common Functionality and can be Overrided
public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected int health = 10; // Default: 10

    [SerializeField]
    protected Transform player;

    protected void Start()
    {
        Init();
        Debug.Log("Initializing Enemy");
    }
    protected virtual void Update()
    {

    }

    protected void Init()
    {
        // Setup any variables and object assignments here
    }

    protected virtual void Attack()
    {
        Debug.Log("Enemy Attacks!");
    }

    protected virtual void OnDeath()
    {
        Debug.Log("Enemy Died!");
    }

    // IDamageable
    public virtual void TakeDamage(int dmg) 
    {
        Health -= dmg;
        Debug.Log("Took Damage= " + dmg);
        if (Health < 0) { OnDeath(); }
    }

    public int Health { get{ return health; } set{ health = value; } }
}
