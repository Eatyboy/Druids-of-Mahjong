using UnityEngine;

public enum FlowerTileType
{
    None,
    BambooBonus, //Give bonus damage for each suit corresponding to the flower tile
    DotBonus, //Give bonus damage for each suit corresponding to the flower tile
    CharacterBonus, //Give bonus damage for each suit corresponding to the flower tile
    QiBonus, //Give bonus damage according to amount of qi
}

[CreateAssetMenu(fileName = "New Flower Tile", menuName = "Flower Tile")]
public class FlowerTileData : ScriptableObject
{
    public FlowerTileType flowerTile;
    public string tileName;
    public Sprite sprite;
    public string description;
}
