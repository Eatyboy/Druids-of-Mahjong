using UnityEngine;

public enum FlowerTileType
{
    None,
    SuitChecker, //Give bonus damage for each suit corresponding to the flower tile
}

[CreateAssetMenu(fileName = "New Flower Tile", menuName = "Flower Tile")]
public class FlowerTileData : ScriptableObject
{
    public FlowerTileType flowerTile;
    public string tileName;
    public Sprite sprite;
    public string description;
}
