using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player instance;
    private InputSystem_Actions ctrl;

    public float health, maxHealth;
    public int qi = 0;

    [SerializeField] private HealthBarUI healthBar;
    [SerializeField] private QiCounter qiCounter;
    public ParryHandler parryHandler;

    [SerializeField] private float baseMaxHealth = 10.0f;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        ctrl = new();
    }

    // Start is called before the first frame update
    void Start()
    {
        qi = 0;
        maxHealth = baseMaxHealth;
        health = maxHealth;

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(health);
        qiCounter.SetQi(qi);
    }

    private void OnEnable()
    {
        ctrl.Enable();
        ctrl.Player.Parry.performed += Parry;
    }

    private void OnDisable()
    {
        ctrl.Player.Parry.performed -= Parry;
        ctrl.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Keyboard.current.qKey.wasPressedThisFrame)
        //{
        //    ChangeHealth(-1.0f);
        //}

        //if (Keyboard.current.eKey.wasPressedThisFrame)
        //{
        //    ChangeHealth(1.0f);
        //}
    }

    public class PlayerAttackContext
    {
        public float damage = 0.0f;
        public float baseDamage = 0.0f;
        public float addedDamageModifier = 0.0f;
        public float increasedDamageModifier = 1.0f;

        public List<Tile> playerHand = null;
        public List<Tile> selectedHand = null;
        public MahjongHandTypes handType = MahjongHandTypes.None;

        public int unresolvedFlowerTiles = 0;
    }

    public IEnumerator Attack(List<Tile> selectedHand)
    {
        PlayerAttackContext context = new()
        {
            selectedHand = selectedHand,
            playerHand = PlayerHand.instance.currentHand.Select(tObj => tObj.tileData).ToList(),
        };

        FlowerTileManager.instance.ActivateFlowerTilesOnPreAttack(context);
        CombatManager.instance.EnqueueAction(() => HandAttackResolver.GetBaseAttackDamage(context), nameof(HandAttackResolver.GetBaseAttackDamage));
        FlowerTileManager.instance.ActivateFlowerTilesOnIntraAttack(context);
        CombatManager.instance.EnqueueAction(() => GetModifiedAttackDamage(context), nameof(GetModifiedAttackDamage));
        FlowerTileManager.instance.ActivateFlowerTilesOnPostAttack(context);
        CombatManager.instance.EnqueueAction(() => EnemyManager.instance.currentEnemy.EnemyTakeDamage((int)context.damage), nameof(EnemyManager.instance.currentEnemy.EnemyTakeDamage));

        yield break;
    }

    public IEnumerator GetModifiedAttackDamage(PlayerAttackContext ctx)
    {
        ctx.damage = (ctx.baseDamage + ctx.addedDamageModifier) * ctx.increasedDamageModifier;
        yield break;
    }

    public IEnumerator PlayerTakeDamage(int damageToTake)
    {
        ChangeHealth(-damageToTake);
        yield break;
    }

    public void ChangeHealth(float healthChange) {
        health += healthChange;
        health = Mathf.Clamp(health, 0, maxHealth);

        healthBar.SetHealth(health);
    }

    public void AddQi(int qiChange)
    {
        qi += qiChange;
        qiCounter.SetQi(qi);
    }


    public void Parry(InputAction.CallbackContext ctx)
    {
        if (!parryHandler.isParryWindowOpen) return;

        StartCoroutine(parryHandler.DoParry());
    }
}
