using UnityEngine;

public enum FlowerTileType
{
    None,
    FlatDamageBuff,
    BambooBonus, //Give bonus damage for each suit corresponding to the flower tile
    DotBonus, //Give bonus damage for each suit corresponding to the flower tile
    CharacterBonus, //Give bonus damage for each suit corresponding to the flower tile
    EightFourBonus, //Multiply damgage positively for each 8 and negatively for each 4
    RandomWindBonus, //Multiply damage positively for each wind tile that matches the current round's wind
}

[CreateAssetMenu(fileName = "New Flower Tile", menuName = "Flower Tile")]
public class FlowerTileData : ScriptableObject
{
    public FlowerTileType flowerTile;
    public string tileName;
    public Sprite sprite;
    public string description;
}
