using System;
using UnityEngine;

public enum TileSuit 
{ 
    None, 
    Bamboo, 
    Dot, 
    Character, 
    Wind, 
    Dragon 
}

public enum WindRank
{
    None,
    North,
    East,
    South,
    West,
}

public enum DragonTileType
{
    None,
    Green,
    Red,
    White,
}

/// <summary>
/// The base tile data for a Mahjong tile
/// </summary>
[CreateAssetMenu(fileName = "New Tile", menuName = "Mahjong Tile")]
public class MahjongTile : ScriptableObject
{    
    public TileSuit suit = TileSuit.None;
    public int rank = 0;
    public int damageValue = 0;
    public Sprite faceSprite;

    /// <summary>
    /// Gets the unique ID corresponding to this tile type
    /// </summary>
    public int GetBaseTileID()
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
