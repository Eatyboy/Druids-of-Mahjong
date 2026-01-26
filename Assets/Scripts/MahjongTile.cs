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
}
