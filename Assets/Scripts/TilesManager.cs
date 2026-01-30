using System;
using System.Collections.Generic;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    public static TilesManager instance;

    [SerializeField] private int defaultDuplicateCount = 4;

    public List<MahjongTile> baseTileDataList;
    public List<Tile> deck;
    public List<Tile> discardPile;

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
}
