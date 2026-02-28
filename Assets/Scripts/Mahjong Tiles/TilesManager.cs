using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    public static TilesManager instance;

    [SerializeField] private int defaultDuplicateCount = 4;

    public List<MahjongTile> baseTileDataList;
    public List<Tile> discardPile;
    public Dictionary<int, MahjongTile> baseTileDataIDMap;
    public Dictionary<int, Sprite> baseTileFaceSprites;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        baseTileDataIDMap = baseTileDataList.ToDictionary(t => t.GetBaseTileID(), t => t);
        baseTileFaceSprites = baseTileDataList.ToDictionary(t => t.GetBaseTileID(), t => t.faceSprite);
    }

    public void InitializeDeck()
    {
        if (GameManager.playerData == null) { UnityEngine.Debug.LogWarning("[ScrollHand] TilesManager.InitializeDeck: GameManager.playerData is null."); return; }
        GameManager.playerData.deck = new List<Tile>();

        if (baseTileDataList == null || baseTileDataList.Count == 0)
        {
            Debug.LogWarning("[ScrollHand] TilesManager.InitializeDeck: baseTileDataList is null or empty. Assign Base Tile Data List in Inspector (UpgradeTree scene TilesManager). Deck left empty.");
            return;
        }

        foreach (MahjongTile tile in baseTileDataList)
        {
            for (int i = 0; i < defaultDuplicateCount; i++)
            {
                GameManager.playerData.deck.Add(new Tile(tile));
            }
        }

        ShuffleDeck();
        Debug.Log($"[ScrollHand] TilesManager.InitializeDeck: deck filled with {GameManager.playerData.deck.Count} tiles (baseTileDataList count={baseTileDataList.Count}).");
    }

    public void ShuffleDeck()
    {
        if (GameManager.playerData?.deck == null) return;
        Utils.ShuffleList(GameManager.playerData.deck);
    }

    /// <summary>
    /// Return all tiles from the player hand and discard pile back into the deck, then shuffle. Call at end of battle.
    /// </summary>
    public void ReturnPlayerHandAndDiscardToDeck()
    {
        if (GameManager.playerData == null) return;
        if (GameManager.playerData.deck == null)
            GameManager.playerData.deck = new List<Tile>();

        if (PlayerHand.instance != null)
        {
            List<Tile> handTiles = PlayerHand.instance.GetHandTileData();
            if (handTiles != null)
                GameManager.playerData.deck.AddRange(handTiles);
            PlayerHand.instance.ClearTiles();
        }

        if (discardPile != null && discardPile.Count > 0)
        {
            GameManager.playerData.deck.AddRange(discardPile);
            discardPile.Clear();
        }

        ShuffleDeck();
    }

    /// <summary>
    /// Return all tiles from the scroll hand back into the deck, then shuffle. Call when exiting the tree (e.g. before GoToCombat).
    /// </summary>
    public void ReturnScrollHandToDeck()
    {
        if (GameManager.playerData == null) return;
        if (ScrollHand.instance == null) return;
        if (GameManager.playerData.deck == null)
            GameManager.playerData.deck = new List<Tile>();

        List<Tile> handTiles = ScrollHand.instance.GetHandTileData();
        if (handTiles != null && handTiles.Count > 0)
        {
            GameManager.playerData.deck.AddRange(handTiles);
            ScrollHand.instance.ClearTiles();
            ShuffleDeck();
        }
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
