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
        healthBar.SetMaxHealth(GameManager.playerData.maxHealth);
        healthBar.SetHealth(GameManager.playerData.health);
        qiCounter.SetQi(GameManager.playerData.qi);
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
        CombatManager.instance.EnqueueAction(() => PlayerHand.instance.PlayHandAnim(), nameof(PlayerHand.instance.PlayHandAnim)); // Resolve Player Combat Animations Here
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
        AudioManager.instance.PlayOneShot(AudioManager.instance.playerHurt);
        yield break;
    }

    public void ChangeHealth(int healthChange) {
        GameManager.playerData.health += healthChange;
        GameManager.playerData.health = Mathf.Clamp(GameManager.playerData.health, 0, GameManager.playerData.maxHealth);

        healthBar.SetHealth(GameManager.playerData.health);
    }

    // Refreshes the health bar from current playerData
    public void RefreshHealthBar()
    {
        if (healthBar != null && GameManager.playerData != null)
        {
            healthBar.SetMaxHealth(GameManager.playerData.maxHealth);
            healthBar.SetHealth(GameManager.playerData.health);
        }
    }

    public void AddQi(int qiChange)
    {
        GameManager.playerData.qi += qiChange;
        qiCounter.SetQi(GameManager.playerData.qi);
    }

    public void Parry(InputAction.CallbackContext ctx)
    {
        if (!parryHandler.isParryWindowOpen) return;

        StartCoroutine(parryHandler.DoParry());
    }
}
