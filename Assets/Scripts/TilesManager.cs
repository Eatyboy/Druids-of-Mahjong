using System;
using System.Collections.Generic;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    public List<MajongTiles> tilesList;

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
}
