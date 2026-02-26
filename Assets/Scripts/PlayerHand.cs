using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHand : HandBase
{
    public static PlayerHand instance { get; private set; }

    [Header("Battle UI")]
    [SerializeField] private GameObject castSpellButton;
    [SerializeField] private TextMeshProUGUI discardsText;
    [SerializeField] private TextMeshProUGUI castSpellText;

    [Header("Battle / Discard")]
    [SerializeField] private int defaultMaxDiscards = 3;
    [SerializeField] private float discardDuration = 0.25f;
    [SerializeField] private float playDuration = 0.25f;

    public List<FlowerTile> flowerTiles;
    public int maxDiscards;
    public int currentDiscards;
    public bool isTurnActive = false;
    public MahjongHandTypes currentHandType = MahjongHandTypes.None;

    protected override void Awake()
    {
        base.Awake();

        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        maxDiscards = defaultMaxDiscards;
        currentDiscards = maxDiscards;
        discardsText.text = $"{currentDiscards}/{maxDiscards}";

        foreach (Transform tileObj in tileContainer) Destroy(tileObj.gameObject);
    }

    private void Update()
    {
        if (selectedTiles.Count == 0)
        {
            castSpellButton.SetActive(false);
        }
        else
        {
            castSpellButton.SetActive(true);
        }
    }

    public IEnumerator DrawTile()
    {
        yield return new WaitForSeconds(drawDuration);

        PlayerTileObject newTileObj = Instantiate(tileObjectPrefab, tileContainer);
        newTileObj.rt.position = tileDrawOrigin.position;
        newTileObj.Initialize(TilesManager.instance.DrawFromDeck());
        newTileObj.name = $"{newTileObj.tileData.rank} of {newTileObj.tileData.suit}";
        newTileObj.transform.SetAsFirstSibling();
        currentHand.Add(newTileObj);
    }

    public IEnumerator DrawUntilFullHand()
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.tileShuffle);
        while (currentHand.Count < currentHandSize)
        {
            if (GameManager.playerData.deck.Count == 0) yield break;
            if (currentHand.Count >= currentHandSize) yield break;

            yield return DrawTile();
        }

        yield return SortTilesInHand();
    }

    public void DiscardButton()
    {
        if (currentDiscards <= 0 || selectedTiles.Count == 0) return;

        currentDiscards--;
        if (discardsText != null) discardsText.text = $"{currentDiscards}/{maxDiscards}";

        StartCoroutine(DiscardTiles(drawWhenDone: true, fromDiscardButton: true));
    }

    public IEnumerator DiscardAnim(Transform target, float punchAngle = -45f)
    {
        float startRotation = target.eulerAngles.z;
        float targetRotation = startRotation + punchAngle;

        Vector3 startPos = target.position;
        Vector3 discardPos = discardsText != null ? discardsText.gameObject.transform.position : target.position;

        float elapsedTime = 0f;
        float durDecrement = 0.05f;
        float minDuration = 0.1f;

        while (elapsedTime < discardDuration)
        {
            float t = elapsedTime / discardDuration;
            float punchStrength = Mathf.Sin(t * Mathf.PI);
            float currentAngle = Mathf.Lerp(startRotation, targetRotation, punchStrength);
            target.eulerAngles = new Vector3(0, 0, currentAngle);
            target.position = Vector3.Lerp(startPos, discardPos, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.eulerAngles = new Vector3(0, 0, startRotation);
        discardDuration = Mathf.Max(minDuration, discardDuration - durDecrement);
    }

    public IEnumerator DiscardTile(PlayerTileObject tileObj, bool addToDiscardPile = true)
    {
        yield return DiscardAnim(tileObj.transform);
        if (addToDiscardPile)
            TilesManager.instance.discardPile.Add(tileObj.tileData);
        currentHand.Remove(tileObj);
        Destroy(tileObj.gameObject);
    }

    public IEnumerator DiscardTiles(bool drawWhenDone = false, bool fromDiscardButton = false)
    {
        float duration = discardDuration;
        bool permanentDiscardSingle = fromDiscardButton
            && FlowerTileManager.instance != null
            && FlowerTileManager.instance.IsFlowerTileActive(FlowerTileType.PermanentDiscard)
            && selectedTiles.Count == 1;

        foreach (PlayerTileObject tileObj in selectedTiles.ToList())
        {
            yield return new WaitForSeconds(drawDuration);
            yield return DiscardTile(tileObj, addToDiscardPile: !permanentDiscardSingle);
        }
        selectedTiles.Clear();
        discardDuration = duration;

        if (!drawWhenDone) yield break;

        yield return new WaitForSeconds(0.7f);
        yield return DrawUntilFullHand();
    }

    public override void SelectTile(PlayerTileObject tile)
    {
        if (CombatManager.instance != null && CombatManager.instance.combatState != CombatState.PlayerTurn) return;

        base.SelectTile(tile);
        UpdateCurrentHandType();
        if (castSpellButton != null) castSpellButton.SetActive(true);
    }

    public override void DeselectTile(PlayerTileObject tile)
    {
        if (CombatManager.instance != null && CombatManager.instance.combatState != CombatState.PlayerTurn) return;

        base.DeselectTile(tile);
        UpdateCurrentHandType();
    }

    // made these two functions public. if its an issue, let me know and ill revert it -aiden
    public List<Tile> GetSelectedTileData()
    {
        List<Tile> list = new List<Tile>();
        foreach (PlayerTileObject t in selectedTiles)
            list.Add(t.tileData);
        return list;
    }

    public List<Tile> GetPlayerHandTileData() => GetHandTileData();

    void UpdateCurrentHandType()
    {
        currentHandType = MahjongHands.GetMahjongHand(GetSelectedTileData());
        if (castSpellText != null) castSpellText.text = $"Cast {(currentHandType == MahjongHandTypes.None ? "Nothing" : currentHandType)}";
    }

    public IEnumerator PlayHandAnim()
    {
        foreach (PlayerTileObject tileObj in selectedTiles.ToList())
        {
            float elapsedTime = 0f;
            Vector3 startPos = tileObj.transform.localPosition;
            Vector3 endPos = new Vector3(startPos.x, startPos.y + 30f, startPos.z);

            while (elapsedTime < playDuration)
            {
                tileObj.transform.localPosition = Vector3.Lerp(startPos, endPos, elapsedTime / playDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        yield return DiscardTiles(drawWhenDone: false);
    }

    public void PlaySelectedHand()
    {
        if (CombatManager.instance == null || CombatManager.instance.combatState != CombatState.PlayerTurn) return;

        CombatManager.instance.EnqueueAction(() => Player.instance.Attack(GetSelectedTileData()), nameof(Player.instance.Attack));
        
        isTurnActive = false;
    }

    public override IEnumerator SortTilesInHand()
    {
        yield return base.SortTilesInHand();
        currentHandType = MahjongHandTypes.None;
        if (castSpellText != null) castSpellText.text = "Cast Nothing";
        if (castSpellButton != null) castSpellButton.SetActive(false);
    }
}
