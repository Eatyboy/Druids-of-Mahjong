using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    public List<TileObject> currentHand;
    public List<TileObject> selectedTiles;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        StartCoroutine(DrawInitialHand());
    }

    public IEnumerator DrawInitialHand()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < defaultHandSize; i++)
        {
            DrawTile();
        }
    }

    public void DrawTile()
    {
        TileObject newTileObj = Instantiate(tileObjectPrefab, tileContainer);
        newTileObj.Initialize(TilesManager.instance.DrawFromDeck());
        currentHand.Add(newTileObj);
        RepositionTiles(newTileObj);
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
        (MahjongHandTypes type, List<TileObject> optimalTiles) = MahjongHands.GetOptimalHand(currentHand, selectedTiles);
        UnityEngine.Debug.Log("Optimal hand type: " + type.ToString());
        MahjongHands.PrintTilesList(optimalTiles);
        //List<Tile> optimalHand = PickOptimalHand();
        //foreach(Tile t in optimalHand)
        //{
        //    t.gameObject.GetComponent<Image>().color = new Color(0.8f, 1f, 1f, 1f);
        //}
    }

    public void DeselectTile(TileObject tile)
    {
        selectedTiles.Remove(tile);
        tile.rt.anchoredPosition = tile.rt.anchoredPosition - tileSelectedOffset;
        tile.selectedOverlay.SetActive(false);
        (MahjongHandTypes type, List<TileObject> optimalTiles) = MahjongHands.GetOptimalHand(currentHand, selectedTiles);
        UnityEngine.Debug.Log("Optimal hand type: " + type.ToString());
        MahjongHands.PrintTilesList(optimalTiles);
        //List<Tile> optimalHand = PickOptimalHand();
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
