using System;
using System.Collections.Generic;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    public static TilesManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public List<MajongTiles> tilesList;
    public List<Tiles> selectedTiles;

    public int numTiles = 14;

    public void Start()
    {
        tilesList = new List<MajongTiles>(numTiles);

        GenerateTiles();
    }

    public void GenerateTiles()
    {
        for (int i = 0; i < numTiles; i++)
        {
            MajongTiles.TileSuit suit = (MajongTiles.TileSuit)
                                            UnityEngine.Random.Range
                                            (0, Enum.GetNames(typeof
                                            (MajongTiles.TileSuit)).Length);

            int value = UnityEngine.Random.Range(1, 10);

            MajongTiles tiles = MajongTiles.CreateInstance(suit, value);

            tilesList.Add(tiles);
        }
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
