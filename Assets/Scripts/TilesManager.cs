using System;
using System.Collections.Generic;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    public static TilesManager instance;

    [SerializeField] private int defaultDuplicateCount = 4;

    public List<MahjongTile> baseTileDataList;
    public List<Tile> discardPile;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    public void Start()
    {
        EnsureDeckInitialized();
    }

    /// <summary>Call before drawing tiles (e.g. when opening Scroll Hand in Upgrade Tree) so the deck exists.</summary>
    public void EnsureDeckInitialized()
    {
        if (GameManager.playerData?.deck == null || GameManager.playerData.deck.Count == 0)
            InitializeDeck();
    }

    private void InitializeDeck()
    {
        GameManager.playerData.deck = new();

        foreach (MahjongTile tile in baseTileDataList)
        {
            if (tile.suit == TileSuit.Dragon) continue;
            for (int i = 0; i < defaultDuplicateCount; i++)
            {
                // generate random tile should be removed later - Aiden
                GameManager.playerData.deck.Add(GenerateRandomTile());
            }
        }

        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        Utils.ShuffleList(GameManager.playerData.deck);
    }

    public Tile DrawFromDeck()
    {
        int topIndex = GameManager.playerData.deck.Count - 1;
        Tile drawnTile = GameManager.playerData.deck[topIndex];
        GameManager.playerData.deck.RemoveAt(topIndex);
        return drawnTile;
    }
    
    //Create tiles with random suit and value
    public Tile GenerateRandomTile()
    {
        return new(Utils.GetRandomItemInList(baseTileDataList));
    }

    /// <summary>
    /// Returns the first MahjongTile in baseTileDataList with the given suit and rank, or null.
    /// Used e.g. to resolve face sprite when a tile's suit is changed (charm scrolls).
    /// </summary>
    public MahjongTile GetBaseTileData(TileSuit suit, int rank)
    {
        if (baseTileDataList == null) return null;
        foreach (MahjongTile t in baseTileDataList)
        {
            if (t.suit == suit && t.rank == rank) return t;
        }
        return null;
    }
}
