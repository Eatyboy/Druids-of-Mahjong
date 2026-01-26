using System;
using UnityEngine;


[CreateAssetMenu(fileName = "New Tiles", menuName = "Majong Tiles")]
public class MajongTiles : ScriptableObject
{    
    public enum TileSuit {Bamboo, Dot, Character, Wind, Dragon}

    public TileSuit suit;

    public int value;
    public Boolean selected;

    internal static MajongTiles CreateInstance(TileSuit suit, int value)
    {
        MajongTiles newTiles = ScriptableObject.CreateInstance<MajongTiles>();

        newTiles.suit = suit;

        newTiles.value = value;

        newTiles.selected = false;

        return newTiles;
    }
}
