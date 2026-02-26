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
    QiBonus, //Give bonus damage according to amount of qi
    EnemyFullHealthBonus, //Multiply damage positively if enemy is at full hp
    TileBonus, //Give bonus damage according to amount of tiles selected
    UniqueDragonBonus, // Multiply damage for each unique type of dragon tile in hand
    TileCountBonus, // Added damage per tile played 
    Copier, //Copy the effect of the tile to its left.
    SkipOneInRun, // Can skip 1 number in runs of tiles
    MixedSets, // Sets, pairs, and runs do not need to be of the same suit
    WindRuns, // Can make runs of 3 different wind tiles
    MagicSquare, // +15 damage if the ranks of the tiles in the hand add up to 15
    PermanentDiscard, // If exactly 1 tile is discarded, it is permanently removed from the deck
    Vampirism, // The player heals a certain percentage of the damage dealt to the enemy
}

[CreateAssetMenu(fileName = "New Flower Tile", menuName = "Flower Tile")]
public class FlowerTileData : ScriptableObject
{
    public FlowerTileType flowerTile;
    public string tileName;
    public Sprite sprite;
    public string description;

    [SerializeReference, SubclassSelector] public FlowerTileEffectConfig effectConfig;
}

[System.Serializable]
public class FlowerTileSaveData
{
    public string tileID;
    public string effectJson;
}
