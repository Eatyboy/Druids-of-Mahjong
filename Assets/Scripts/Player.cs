using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ParryType
{
    None,
    Peng,
    Chi,
    Gan,
    HuLe,
}

public class Player : MonoBehaviour
{
    public static Player instance;
    private InputSystem_Actions ctrl;

    public float health, maxHealth;
    public int qi = 0;

    [SerializeField] private HealthBarUI healthBar;
    [SerializeField] private QiCounter qiCounter;
    [SerializeField] private ParryPopup parryPopup;

    [SerializeField] private float baseMaxHealth = 10.0f;
    public float baseParryWindow = 3.0f;
    [SerializeField] private float parryDamageMultiplier = 1.0f; 

    private bool isParryWindowOpen = false;
    private ParryContext parryContext;

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
        ctrl.Enable();
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

    public class ParryContext
    {
        public bool resolved { get; private set; }
        public bool wasParried { get; private set; }
        public Enemy enemy { get; private set; }
        public MahjongHandTypes parryHandType { get; private set; }
        public List<Tile> parryHand { get; private set; }

        public ParryContext(Enemy enemy, MahjongHandTypes parryHandType, List<Tile> parryHand)
        {
            resolved = false;
            wasParried = false;
            this.parryHandType = parryHandType;
            this.parryHand = parryHand;
        }

        public void Resolve(bool wasParried)
        {
            resolved = true;
            this.wasParried = wasParried;
        }
    }

    public void OpenParryWindow(ParryContext ctx)
    {
        isParryWindowOpen = true;
        parryContext = ctx;
        parryPopup.Open(baseParryWindow, ctx.enemy.transform.position);
    }

    public void Parry(InputAction.CallbackContext ctx)
    {
        if (!isParryWindowOpen) return;

        CombatManager.instance.actionQueue.Enqueue(() => DoParry(parryContext));
    }

    private IEnumerator DoParry(ParryContext ctx)
    {
        if (ctx.resolved) yield break;

        ParryType parryType = ctx.parryHandType switch
        {
            MahjongHandTypes.None => ParryType.None,
            MahjongHandTypes.Pair => ParryType.None,
            MahjongHandTypes.Set => ParryType.Peng,
            MahjongHandTypes.Run => ParryType.Chi,
            MahjongHandTypes.Quad => ParryType.Gan,
            MahjongHandTypes.ThreePairs => ParryType.None,
            MahjongHandTypes.SetAndRun => ParryType.Peng,
            MahjongHandTypes.TwoRuns => ParryType.Chi,
            MahjongHandTypes.TwoSets => ParryType.Peng,
            MahjongHandTypes.TwoQuads => ParryType.Gan,
            MahjongHandTypes.ThreeSets => ParryType.Peng,
            MahjongHandTypes.NineRun => ParryType.Chi,
            MahjongHandTypes.AllPairs => ParryType.None,
            MahjongHandTypes.FullWin => ParryType.HuLe,
            _ => ParryType.None
        };

        if (parryType == ParryType.None)
        {
            ctx.Resolve(false);
            yield break;
        }

        int handBaseDamage = HandAttackResolver.GetHandBaseDamage(ctx.parryHandType, ctx.parryHand);
        int parryDamage = Mathf.FloorToInt((float)handBaseDamage * parryDamageMultiplier);
        CombatManager.instance.actionQueue.Enqueue(() => ctx.enemy.EnemyTakeDamage(parryDamage));
        Debug.Log($"{parryType}!");
        ctx.Resolve(true);

        yield break;
    }
}
