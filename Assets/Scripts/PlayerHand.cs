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
        if (discardsText != null) discardsText.text = $"{currentDiscards}/{maxDiscards}";
    }

    public void DiscardButton()
    {
        if (currentDiscards <= 0 || selectedTiles.Count == 0) return;

        currentDiscards--;
        if (discardsText != null) discardsText.text = $"{currentDiscards}/{maxDiscards}";

        StartCoroutine(DiscardTiles(drawWhenDone: true));
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

    public IEnumerator DiscardTiles(bool drawWhenDone = false)
    {
        float duration = discardDuration;
        foreach (PlayerTileObject tileObj in selectedTiles.ToList())
        {
            yield return new WaitForSeconds(drawDuration);
            yield return DiscardAnim(tileObj.transform);

            if (TilesManager.instance != null) TilesManager.instance.discardPile.Add(tileObj.tileData);
            currentHand.Remove(tileObj);
            Destroy(tileObj.gameObject);
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
        if (selectedTiles.Count == 0 && castSpellButton != null) castSpellButton.SetActive(false);
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
        if (castSpellButton != null) castSpellButton.SetActive(false);
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
