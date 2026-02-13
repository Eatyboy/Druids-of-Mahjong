using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryHandler : MonoBehaviour
{
    public enum ParryType
    {
        None,
        Peng,
        Chi,
        Gan,
        HuLe,
    }

    public class ParryContext
    {
        public bool resolved { get; private set; }
        public bool wasParried { get; private set; }
        public Enemy enemy { get; private set; }
        public MahjongHandTypes parryHandType { get; private set; }
        public List<Tile> parryHand { get; private set; }
        public List<TileObject> parryTileObjects { get; private set; }
        public TileObject enemyAttackTile { get; private set; }
        public ParryPopup popup { get; private set; }

        public ParryContext(Enemy enemy, MahjongHandTypes parryHandType, List<Tile> parryHand, 
                            List<TileObject> parryTileObjects, TileObject enemyAttackTile)
        {
            resolved = false;
            wasParried = false;
            this.parryHandType = parryHandType;
            this.parryHand = parryHand;
            this.parryTileObjects = parryTileObjects;
            this.enemy = enemy;
            this.enemyAttackTile = enemyAttackTile;
        }

        public void Resolve(bool wasParried)
        {
            resolved = true;
            this.wasParried = wasParried;
        }
    }

    [SerializeField] private ParryPopup parryPopup;
    [SerializeField] private float baseParryWindowDuration = 3.0f;
    [SerializeField] private float parryDamageMultiplier = 1.0f; 

    public bool isParryWindowOpen { get; private set; }
    private ParryContext activeContext = null;
    private Coroutine activeParryWindow;

    private void Start()
    {
        isParryWindowOpen = false;
        parryPopup.gameObject.SetActive(false);
    }

    public void OpenParryWindow(ParryContext ctx)
    {
        if (activeContext != null) return;

        activeContext = ctx;

        activeParryWindow = StartCoroutine(ParryWindow());
    }

    private IEnumerator ParryWindow()
    {
        isParryWindowOpen = true;
        parryPopup.gameObject.SetActive(true);
        parryPopup.Open(activeContext.enemyAttackTile.rt.position);
        foreach (TileObject tileObject in activeContext.parryTileObjects)
        {
            tileObject.SetHighlighted(true);
        }

        float elapsedTime = 0.0f;

        while (elapsedTime < baseParryWindowDuration)
        {
            elapsedTime += Time.deltaTime;
            parryPopup.image.fillAmount = 1.0f - elapsedTime / baseParryWindowDuration;
            yield return null;
        }

        CloseParryWindow();
        activeContext.Resolve(false);
        //activeContext = null;
    }

    private void CloseParryWindow()
    {
        if (activeContext == null || activeContext.resolved) return;

        if (activeParryWindow != null)
        {
            StopCoroutine(activeParryWindow);
            activeParryWindow = null;
        }

        parryPopup.gameObject.SetActive(false);
        foreach (TileObject tileObject in activeContext.parryTileObjects)
        {
            if (tileObject != null && tileObject.gameObject != null)
            {
                tileObject.SetHighlighted(false);
            }
        }

        isParryWindowOpen = false;
    }

    public IEnumerator DoParry()
    {
        if (activeContext == null || activeContext.resolved) yield break;

        ParryType parryType = activeContext.parryHandType switch
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
            CloseParryWindow();
            activeContext.Resolve(false);
            //activeContext = null;
            yield break;
        }

        CloseParryWindow();

        yield return new WaitForSeconds(0.5f); // Animation time

        int handBaseDamage = HandAttackResolver.GetBaseParryDamage(activeContext);
        int parryDamage = Mathf.FloorToInt((float)handBaseDamage * parryDamageMultiplier);
        CombatManager.instance.EnqueueAction(() => activeContext.enemy.EnemyTakeDamage(parryDamage), nameof(activeContext.enemy.EnemyTakeDamage));
        Debug.Log($"{parryType}!");
        //activeContext = null;
        activeContext.Resolve(true);

        yield break;
    }
}
