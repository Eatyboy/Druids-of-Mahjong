using System;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public class Tile
{
    public MahjongTile baseTileData;
    public TileSuit suit = TileSuit.None;
    public int rank = 0;
    public int damageValue = 0;
    public Sprite faceSprite;

    public Tile(MahjongTile tile)
    {
        baseTileData = tile;
        suit = baseTileData.suit;
        rank = tile.rank;
        damageValue = tile.damageValue;
        faceSprite = tile.faceSprite;
    }

    public bool SameAs(object obj)
    {
        if (obj is not Tile other) return false;
        return suit == other.suit && rank == other.rank;
    }

    /// <summary>
    /// Gets the unique ID corresponding to this tile type
    /// </summary>
    public int GetTileID()
    {
        int suitValue = suit switch
        {
            TileSuit.None => 0, // default/null value
            TileSuit.Bamboo => 1, // 1-9 bamboo (9 tiles)
            TileSuit.Dot => 10, // 1-9 dot (9 tiles)
            TileSuit.Character => 19, // 1-9 character (9 tiles)
            TileSuit.Wind => 28, // NESW winds (4 tiles)
            TileSuit.Dragon => 32, // GRW dragons (3 tiles)
            _ => -1 // error
        };

        return suitValue + rank;
    }
}
