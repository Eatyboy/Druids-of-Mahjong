using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHand : MonoBehaviour
{
    public static PlayerHand instance {get; private set;}

    [Header("References and Such")]
    [SerializeField] private Transform tileContainer;
    [SerializeField] private TileObject tileObjectPrefab;
    [SerializeField] private float maxHorizontalTileOffset;

    [Header("Hand/Tiles")]
    [SerializeField] private int defaultHandSize = 14;
    [SerializeField] private float tileOffsetX;
    [SerializeField] private Vector2 tileSelectedOffset;
    [SerializeField] private float drawDuration = 0.03f;

    public List<TileObject> currentHand;
    public List<TileObject> selectedTiles;
    public int currentHandSize = 14;

    [Header("Current hand type (for display)")]
    public MahjongHandTypes currentHandType = MahjongHandTypes.None;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        currentHandSize = defaultHandSize;

        StartCoroutine(DrawInitialHand());
    }

    public IEnumerator DrawInitialHand()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < defaultHandSize; i++)
        {
            yield return new WaitForSeconds(0.5f / (float)defaultHandSize);
            StartCoroutine(DrawTile());
        }
    }

    public IEnumerator DrawTile()
    {
        yield return new WaitForSeconds(drawDuration);

        TileObject newTileObj = Instantiate(tileObjectPrefab, tileContainer);
        newTileObj.Initialize(TilesManager.instance.DrawFromDeck());
        currentHand.Add(newTileObj);
        RepositionTiles(newTileObj);
    }

    public void DiscardButton()
    {
        StartCoroutine(DiscardTiles());
    }

    public IEnumerator DiscardTiles()
    {
        foreach (TileObject tileObj in selectedTiles)
        {
            yield return new WaitForSeconds(drawDuration);

            TilesManager.instance.discardPile.Add(tileObj.tileData);
            currentHand.Remove(tileObj);
            Destroy(tileObj.gameObject);
        }
        selectedTiles.Clear();

        yield return new WaitForSeconds(0.7f);

        while (currentHand.Count < currentHandSize)
        {
            if (TilesManager.instance.deck.Count == 0) break;

            yield return DrawTile();
        }
    }

    public void CastSpell()
    {
        HandAttackResolver.ResolveHandAttack(selectedTiles.Select((o) => o.tileData).ToList());
        DiscardTiles();
    }

    public void AddTile(Tile tile)
    {

    }

    public void RemoveTile(Tile tile)
    {

    }

    public void SelectTile(TileObject tile)
    {
        selectedTiles.Add(tile);
        tile.rt.anchoredPosition = tile.rt.anchoredPosition + tileSelectedOffset;
        tile.selectedOverlay.SetActive(true);
        UpdateCurrentHandType();
    }

    public void DeselectTile(TileObject tile)
    {
        selectedTiles.Remove(tile);
        tile.rt.anchoredPosition = tile.rt.anchoredPosition - tileSelectedOffset;
        tile.selectedOverlay.SetActive(false);
        UpdateCurrentHandType();
    }

    List<Tile> GetSelectedTileData()
    {
        List<Tile> list = new List<Tile>();
        foreach (TileObject t in selectedTiles)
            list.Add(t.tileData);
        return list;
    }

    void UpdateCurrentHandType()
    {
        currentHandType = MahjongHands.GetMahjongHand(GetSelectedTileData());
    }

    public void PlaySelectedHand()
    {
        HandAttackResolver.ResolveHandAttack(GetSelectedTileData());
    }

    // clear
    public void ClearTiles()
    {
        foreach (TileObject t in currentHand)
        {
            Destroy(t.gameObject);
        }
    }

    public void RepositionTiles(TileObject tileObj)
    {
        int numTiles = currentHand.Count;
        float offsetPerTile = tileObj.rt.rect.width * 1.1f;
        
        // if too many tiles, they should overlap
        // 200 bc of ui space vs world space shenanigans
        if (offsetPerTile * numTiles > maxHorizontalTileOffset * 2.0f * 100.0f)
        {
            offsetPerTile = 100.0f * maxHorizontalTileOffset / (0.5f * (float)numTiles);
        }

        float initOffsetX = (0.5f * offsetPerTile) * (1.0f - (float)numTiles); 

        for (int i = 0; i < numTiles; i++)
        {
            currentHand[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new(tileOffsetX * 100.0f + initOffsetX + offsetPerTile * i, 0.0f);
        }
    }

    public void SortTiles()
    {
        
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
