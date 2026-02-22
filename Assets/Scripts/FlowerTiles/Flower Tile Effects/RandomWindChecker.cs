using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomWindChecker : FlowerTileEffect
{
    public int currentWind = 1;
    private Dictionary<int, string> windDirections = new Dictionary<int, string>
    {
        {1, "East"},
        {2, "North"},
        {3, "South"},
        {4, "West"},
    };

    private void Awake()
    {
        GetRandomWind();
    }
    private void GetRandomWind()
    {
        currentWind = Random.Range(1,4);
        // update tile description
        string windName = windDirections[currentWind];
        //Debug.Log(windName);
        gameObject.GetComponent<FlowerTile>().data.description = 
            "Multiply damage by x1.5 for each wind tile that matches the current wind direction (" 
                + windName + ")";
    }
    public override IEnumerator OnTurnStart()
    {
        GetRandomWind();
        yield break;
    }
    public override IEnumerator OnIntraAttack(Player.PlayerAttackContext attackContext)
    {
        float multiplier = 1f;
        foreach (Tile tile in attackContext.selectedHand)
        {
            if (tile.suit == TileSuit.Wind && tile.rank == currentWind)
            {
                multiplier *= 1.5f;
            }
        }
        attackContext.increasedDamageModifier *= multiplier;
        yield break;
    }
}
