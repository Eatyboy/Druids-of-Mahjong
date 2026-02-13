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
    [SerializeField] private TileObject tileObjectPrefab;
    [SerializeField] private float maxHorizontalTileOffset;

    [Header("Hand/Tiles")]
    [SerializeField] private int defaultHandSize = 14;
    [SerializeField] private int defaultMaxDiscards = 3;
    [SerializeField] private float tileOffsetX;
    [SerializeField] private Vector2 tileSelectedOffset;
    [SerializeField] private RectTransform tileDrawOrigin;
    [SerializeField] private float drawDuration = 0.03f;
    [SerializeField] private float sortDelay = 0.4f;

    public List<TileObject> currentHand;
    public List<TileObject> selectedTiles;
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

        TileObject newTileObj = Instantiate(tileObjectPrefab, tileContainer);
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
            if (TilesManager.instance.deck.Count == 0) yield break;
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

    public IEnumerator DiscardTiles(bool drawWhenDone = false)
    {
        foreach (TileObject tileObj in selectedTiles)
        {
            yield return new WaitForSeconds(drawDuration);

            TilesManager.instance.discardPile.Add(tileObj.tileData);
            currentHand.Remove(tileObj);
            Destroy(tileObj.gameObject);
        }
        selectedTiles.Clear();

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

    public void SelectTile(TileObject tile)
    {
        if (CombatManager.instance.combatState != CombatState.PlayerTurn) return;

        selectedTiles.Add(tile);
        UpdateCurrentHandType();

        castSpellButton.SetActive(true);
    }

    public void DeselectTile(TileObject tile)
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
        foreach (TileObject t in selectedTiles)
            list.Add(t.tileData);
        return list;
    }

    public List<Tile> GetPlayerHandTileData()
    {
        List<Tile> list = new List<Tile>();
        foreach (TileObject t in currentHand)
            list.Add(t.tileData);
        return list;
    }

    void UpdateCurrentHandType()
    {
        currentHandType = MahjongHands.GetMahjongHand(GetSelectedTileData());
        castSpellText.text = $"Cast {(currentHandType == MahjongHandTypes.None ? "Nothing" : currentHandType)}";
    }

    public IEnumerator PlayHandAnim()
    {

        foreach (TileObject tileObj in selectedTiles)
        {
            Vector3 tileScale = tileObj.transform.localScale;
            float elapsedTime = 0f;
            float duration = 1f;
            while (elapsedTime < duration)
            {
                tileObj.transform.localScale = Vector3.Lerp(tileScale, tileScale * 0.2f, elapsedTime / duration);
                yield return null;
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.5f);
    }

    public void PlaySelectedHand()
    {
        if (CombatManager.instance.combatState != CombatState.PlayerTurn) return;

        CombatManager.instance.EnqueueAction(() => Player.instance.Attack(GetSelectedTileData()), nameof(Player.instance.Attack));
        castSpellButton.SetActive(false);

        /*foreach (TileObject tileObj in selectedTiles)
        {
            Vector3 tileScale = tileObj.transform.localScale;
            float elapsedTime = 0f;
            float duration = 1f;
            while (elapsedTime < duration)
            {
                tileObj.transform.localScale = Vector3.Lerp(tileScale, tileScale * 0.2f, elapsedTime / duration);
            }
        }*/

        StartCoroutine(DiscardTiles(drawWhenDone: false));
        
        isTurnActive = false;
    }

    // clear
    public void ClearTiles()
    {
        foreach (TileObject t in currentHand)
        {
            Destroy(t.gameObject);
        }
    }

    public IEnumerator SortTilesInHand()
    {
        foreach (TileObject tile in selectedTiles)
        {
            tile.isSelected = false;
            tile.ResetToInitialPosition();
        }
        currentHandType = MahjongHandTypes.None;
        castSpellText.text = $"Cast Nothing";
        selectedTiles.Clear();
        castSpellButton.SetActive(false);

        TileObject[] sortedTiles = currentHand
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
    //     foreach (TileObject stObj in selectedTiles)
    //     {
    //         Tile st = stObj.tileData;
    //         List<Tile> testHandStraight = new();
    //         List<Tile> testHandTriplet = new();
    //         testHandTriplet.Add(st);
    //         testHandStraight.Add(st);
    //         int straightValue = 1;
    //         int tripletValue = 1;

    //         // find best combination for selected tile; straight or triplet
    //         foreach (TileObject htObj in currentHand)
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
