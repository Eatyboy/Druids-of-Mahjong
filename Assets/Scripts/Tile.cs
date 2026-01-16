using System;
using UnityEngine;

public enum TileType
{
    None,

    OneSticks,
    TwoSticks,
    ThreeSticks,
    FourSticks,
    FiveSticks,
    SixSticks,
    SevenSticks,
    EightSticks,
    NineSticks,

    OneCircles,
    TwoCircles,
    ThreeCircles,
    FourCircles,
    FiveCircles,
    SixCircles,
    SevenCircles,
    EightCircles,
    NineCircles,

    OneNumbers,
    TwoNumbers,
    ThreeNumbers,
    FourNumbers,
    FiveNumbers,
    SixNumbers,
    SevenNumbers,
    EightNumbers,
    NineNumbers,

    EastWind,
    WestWind,
    NorthWind,
    SouthWind,

    RedDragon,
    GreenDragon,
    WhiteDragon,
}

public enum TileSuit
{
    None,
    Sticks,
    Circles,
    Numbers,
    Winds,
    Dragons,
}

public class Tile : MonoBehaviour
{
    public TileType type;
    public TileSuit suit;

    private void Awake()
    {
        suit = GetSuitFromType(type);
    }

    public TileSuit GetSuitFromType(TileType tileType) =>
        tileType switch
        {
            TileType.None => TileSuit.None,
            TileType.OneSticks => TileSuit.Sticks,
            TileType.TwoSticks => TileSuit.Sticks,
            TileType.ThreeSticks => TileSuit.Sticks,
            TileType.FourSticks => TileSuit.Sticks,
            TileType.FiveSticks => TileSuit.Sticks,
            TileType.SixSticks => TileSuit.Sticks,
            TileType.SevenSticks => TileSuit.Sticks,
            TileType.EightSticks => TileSuit.Sticks,
            TileType.NineSticks => TileSuit.Sticks,
            TileType.OneCircles => TileSuit.Circles,
            TileType.TwoCircles => TileSuit.Circles,
            TileType.ThreeCircles => TileSuit.Circles,
            TileType.FourCircles => TileSuit.Circles,
            TileType.FiveCircles => TileSuit.Circles,
            TileType.SixCircles => TileSuit.Circles,
            TileType.SevenCircles => TileSuit.Circles,
            TileType.EightCircles => TileSuit.Circles,
            TileType.NineCircles => TileSuit.Circles,
            TileType.OneNumbers => TileSuit.Numbers,
            TileType.TwoNumbers => TileSuit.Numbers,
            TileType.ThreeNumbers => TileSuit.Numbers,
            TileType.FourNumbers => TileSuit.Numbers,
            TileType.FiveNumbers => TileSuit.Numbers,
            TileType.SixNumbers => TileSuit.Numbers,
            TileType.SevenNumbers => TileSuit.Numbers,
            TileType.EightNumbers => TileSuit.Numbers,
            TileType.NineNumbers => TileSuit.Numbers,
            TileType.EastWind => TileSuit.Winds,
            TileType.WestWind => TileSuit.Winds,
            TileType.NorthWind => TileSuit.Winds,
            TileType.SouthWind => TileSuit.Winds,
            TileType.RedDragon => TileSuit.Dragons,
            TileType.GreenDragon => TileSuit.Dragons,
            TileType.WhiteDragon => TileSuit.Dragons,
            _ => throw new ArgumentException("Invalid TileType enum ", nameof(tileType))
        };
}
