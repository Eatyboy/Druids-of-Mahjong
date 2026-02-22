using UnityEngine;
using System.Collections.Generic;
using System.Numerics;

public class BuyOption : MonoBehaviour
{
    [SerializeField] private FlowerTile tile;
    [SerializeField] private RectTransform tileRT;
    [SerializeField] private RectTransform rt;
    [SerializeField] private int index;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Initialize(FlowerTile flowerTile)
    {
        rt = this.GetComponent<RectTransform>();
        this.tile = flowerTile;
        tileRT = this.tile.GetComponent<RectTransform>();

        tile = Instantiate(flowerTile);
        tile.GetComponent<RectTransform>().SetParent(rt);
        // translate
        // using tileRT doesnt work? not sure why
        tile.GetComponent<RectTransform>().anchorMin = new UnityEngine.Vector2(0.5f, 0.5f);
        tile.GetComponent<RectTransform>().anchorMax = new UnityEngine.Vector2(0.5f, 0.5f);
        tile.GetComponent<RectTransform>().anchoredPosition = new(0.0f, 0.0f);
        tile.UpdateImage();

        // initialize without CombatManager (might have unintended consequences)
        tile.infoController = FlowerTileManager.instance.infoController;
    }

    public void Purchase(int index)
    {
        if (QiTreeManager.instance.TryPurchase(tile, index))
        {
            // Destroy(this);
            return;
        }
        else
        {
            UnityEngine.Debug.Log("Didnt buy. Either you're broke or greedy");
        }
    }
}
