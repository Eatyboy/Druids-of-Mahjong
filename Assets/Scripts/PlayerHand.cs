using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHand : MonoBehaviour
{
    public static PlayerHand instance {get; private set;}

    [Header("References and Such")]
    [SerializeField] private Transform tileContainer;
    [SerializeField] private GameObject castSpellButton;
    [SerializeField] private TextMeshProUGUI discardsText;
    [SerializeField] private TextMeshProUGUI castSpellText;
    [SerializeField] private PlayerTileObject tileObjectPrefab;
    [SerializeField] private float maxHorizontalTileOffset;

    [Header("Hand/Tiles")]
    [SerializeField] private int defaultHandSize = 14;
    [SerializeField] private int defaultMaxDiscards = 3;
    [SerializeField] private float tileOffsetX;
    [SerializeField] private Vector2 tileSelectedOffset;
    [SerializeField] private RectTransform tileDrawOrigin;
    [SerializeField] private float drawDuration = 0.03f;
    [SerializeField] private float sortDelay = 0.4f;

    [Header("Animation Parameters")]
    [SerializeField] private float discardDuration = 0.25f;
    [SerializeField] private float playDuration = 0.25f;

    public List<PlayerTileObject> currentHand;
    public List<PlayerTileObject> selectedTiles;
    public List<FlowerTile> flowerTiles;
    public int currentHandSize = 14;
    public int maxDiscards;
    public int currentDiscards;

    public bool isTurnActive = false;

    [Header("Current hand type (for display)")]
    public MahjongHandTypes currentHandType = MahjongHandTypes.None;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        currentHandSize = defaultHandSize;
        maxDiscards = defaultMaxDiscards;
        currentDiscards = maxDiscards;
        discardsText.text = $"{currentDiscards}/{maxDiscards}";

        foreach (Transform tileObj in tileContainer) Destroy(tileObj.gameObject);
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
        discardsText.text = $"{currentDiscards}/{maxDiscards}";

        StartCoroutine(DiscardTiles(drawWhenDone: true));
    }

    public IEnumerator DiscardAnim(Transform target, float punchAngle = -45f)
    {
        float startRotation = target.eulerAngles.z;
        float targetRotation = startRotation + punchAngle;

        Vector3 startPos = target.position;
        Vector3 discardPos = discardsText.gameObject.transform.position;

        float elapsedTime = 0f;
        float durDecrement = 0.05f;
        float minDuration = 0.1f;

        while (elapsedTime < discardDuration)
        {
            float t = elapsedTime / discardDuration;

            // Punch curve: goes up then back down
            float punchStrength = Mathf.Sin(t * Mathf.PI); // 0 -> 1 -> 0

            float currentAngle = Mathf.Lerp(startRotation, targetRotation, punchStrength);
            target.eulerAngles = new Vector3(0, 0, currentAngle);   // Punch Rotation Effect
            target.position = Vector3.Lerp(startPos, discardPos, t);// Sends Tile to Discard Pile

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.eulerAngles = new Vector3(0, 0, startRotation);
        discardDuration = Mathf.Max(minDuration, discardDuration - durDecrement);
    }

    public IEnumerator DiscardTile(PlayerTileObject tileObj)
    {
        yield return DiscardAnim(tileObj.transform);
        TilesManager.instance.discardPile.Add(tileObj.tileData);
        currentHand.Remove(tileObj);
        Destroy(tileObj.gameObject);
    }

    public IEnumerator DiscardTiles(bool drawWhenDone = false)
    {
        float duration = discardDuration;
        foreach (PlayerTileObject tileObj in selectedTiles)
        {
            yield return new WaitForSeconds(drawDuration);
            yield return DiscardTile(tileObj);
        }
        selectedTiles.Clear();
        discardDuration = duration;

        if (!drawWhenDone) yield break;

        yield return new WaitForSeconds(0.7f);

        yield return DrawUntilFullHand();
    }

    public void AddTile(Tile tile)
    {

    }

    public void RemoveTile(Tile tile)
    {

    }

    public void SelectTile(PlayerTileObject tile)
    {
        if (CombatManager.instance.combatState != CombatState.PlayerTurn) return;

        selectedTiles.Add(tile);
        UpdateCurrentHandType();

        castSpellButton.SetActive(true);
    }

    public void DeselectTile(PlayerTileObject tile)
    {
        if (CombatManager.instance.combatState != CombatState.PlayerTurn) return;

        selectedTiles.Remove(tile);
        UpdateCurrentHandType();

        if (selectedTiles.Count == 0)
        {
            castSpellButton.SetActive(false);
        }
    }

    // made these two functions public. if its an issue, let me know and ill revert it -aiden
    public List<Tile> GetSelectedTileData()
    {
        List<Tile> list = new List<Tile>();
        foreach (PlayerTileObject t in selectedTiles)
            list.Add(t.tileData);
        return list;
    }

    public List<Tile> GetPlayerHandTileData()
    {
        List<Tile> list = new List<Tile>();
        foreach (PlayerTileObject t in currentHand)
            list.Add(t.tileData);
        return list;
    }

    void UpdateCurrentHandType()
    {
        currentHandType = MahjongHands.GetMahjongHand(GetSelectedTileData());
        castSpellText.text = $"Cast {(currentHandType == MahjongHandTypes.None ? "Nothing" : currentHandType)}";
    }

    // Called by Player.Attack()
    public IEnumerator PlayHandAnim()
    {
        foreach (PlayerTileObject tileObj in selectedTiles)
        {
            float elapsedTime = 0f;

            Vector3 startPos = tileObj.transform.localPosition;
            Vector3 endPos = new Vector3(startPos.x, startPos.y + 30f, startPos.z); // Should be enemy pos or center screen
            //UnityEngine.Debug.Log(endPos);

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
        if (CombatManager.instance.combatState != CombatState.PlayerTurn) return;

        CombatManager.instance.EnqueueAction(() => Player.instance.Attack(GetSelectedTileData()), nameof(Player.instance.Attack));
        castSpellButton.SetActive(false);
        
        isTurnActive = false;
    }

    // clear
    public void ClearTiles()
    {
        foreach (PlayerTileObject t in currentHand)
        {
            Destroy(t.gameObject);
        }
    }

    public IEnumerator SortTilesInHand()
    {
        foreach (PlayerTileObject tile in selectedTiles)
        {
            tile.isSelected = false;
            tile.ResetToInitialPosition();
        }
        currentHandType = MahjongHandTypes.None;
        castSpellText.text = $"Cast Nothing";
        selectedTiles.Clear();
        castSpellButton.SetActive(false);

        PlayerTileObject[] sortedTiles = currentHand
            .OrderBy(t => t.tileData.suit)
            .ThenBy(t =>  t.tileData.rank)
            .ToArray();

        yield return new WaitForSeconds(sortDelay);

        for (int i = 0; i < sortedTiles.Length; ++i)
        {
            sortedTiles[i].transform.SetSiblingIndex(i);
        }
    }

    // O(n)
    // deprecated - Aiden
    // public List<Tile> PickOptimalHand()
    // {
    //     List<Tile> optimalHand = new();
    //     int optimalHandValue = 0;
    //     foreach (PlayerTileObject stObj in selectedTiles)
    //     {
    //         Tile st = stObj.tileData;
    //         List<Tile> testHandStraight = new();
    //         List<Tile> testHandTriplet = new();
    //         testHandTriplet.Add(st);
    //         testHandStraight.Add(st);
    //         int straightValue = 1;
    //         int tripletValue = 1;

    //         // find best combination for selected tile; straight or triplet
    //         foreach (PlayerTileObject htObj in currentHand)
    //         {
    //             Tile ht = htObj.tileData;
    //             if (st.Equals(ht)) continue;
    //             if (ht.suit != st.suit) continue;

    //             // straights
    //             if (ht.rank + 1 == st.rank || ht.rank - 1 == st.rank)
    //             {
    //                 testHandStraight.Add(ht);
    //                 straightValue += 1;
    //             }

    //             // triplets
    //             if (st.rank == ht.rank)
    //             {
    //                 testHandTriplet.Add(ht);
    //                 tripletValue += 1;
    //             }
    //         }

    //         // check with current optimal hand (for ties, always choose straight)
    //         if (testHandTriplet.Count > optimalHand.Count /*testHandValue > optimalHandValue*/)
    //         {
    //             optimalHand = testHandTriplet;
    //         }
    //         if (testHandStraight.Count >= testHandTriplet.Count /*testHandValue > optimalHandValue*/)
    //         {
    //             optimalHand = testHandStraight;
    //         }
    //     }

    //     return optimalHand;
    // }
    
    // Ethan: I removed TileType, so this is deprecated
    //private bool ContainsTileType(TileType typeToCheck, List<Tile> tiles)
    //{
    //    foreach (Tile t in tiles)
    //    {
    //        if (t.type == typeToCheck)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
}
