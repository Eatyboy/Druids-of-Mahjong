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
}
