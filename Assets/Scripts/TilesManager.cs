using System;
using System.Collections.Generic;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    public static TilesManager instance;

    [SerializeField] private int defaultDuplicateCount = 4;

    public List<MahjongTile> baseTileDataList;
    public List<Tile> deck;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    public void Start()
    {
        foreach (MahjongTile tile in baseTileDataList)
        {
            if (tile.suit == TileSuit.Dragon) continue;
            for (int i = 0; i < defaultDuplicateCount; i++)
            {
                // generate random tile should be removed later - Aiden
                deck.Add(GenerateRandomTile());
            }
        }
        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        Utils.ShuffleList(deck);
    }

    public Tile DrawFromDeck()
    {
        int topIndex = deck.Count - 1;
        Tile drawnTile = deck[topIndex];
        deck.RemoveAt(topIndex);
        return drawnTile;
    }
    
    //Create tiles with random suit and value
    public Tile GenerateRandomTile()
    {
        return new(Utils.GetRandomItemInList(baseTileDataList));
    }

    // O(n)
    public List<Tile> PickOptimalHand()
    {
        List<Tile> optimalHand = new();
        int optimalHandValue = 0;
        foreach (Tile st in selectedTiles)
        {
            List<Tile> testHandStraight = new();
            List<Tile> testHandTriplet = new();
            int straightValue = 0;
            int tripletValue = 0;

            // find best combination for selected tile; straight or triplet
            foreach (Tile ht in currentHand)
            {
                if (ct.Equals(ht)) continue;
                if (ht.GetSuitFromType() != st.GetSuitFromType) continue;

                // triplets
                if (!ContainsTileType(ct.tileType, testHandTriplet) &&
                    ((int)ht.tileType + 1 == (int)ct.tileType || (int)ht.tileType - 1 == (int)ct.tileType))
                {
                    testHandTriplet.Add(ht);
                    tripletValue += 1;
                }

                // straights
                if (ct.tileType == ht.TileType)
                {
                    testHandStraight.Add(ht);
                    straightValue += 1;
                }
            }

            // check with current optimal hand (for ties, always choose straight)
            if (testHandTriplet.Count > optimalHand.Count /*testHandValue > optimalHandValue*/)
            {
                optimalHand = testHandTriplet;
            }
            if (testHandStraight.Count >= testHandTriplet.Count /*testHandValue > optimalHandValue*/)
            {
                optimalHand = testHandStraight;
            }
        }

        return optimalHand;
    }
    
    private bool ContainsTileType(TileType type, List<Tile> tiles)
    {
        foreach (Tile t in tiles)
        {
            if (t.tileType == type)
            {
                return true;
            }
        }
        return false;
    }
}
