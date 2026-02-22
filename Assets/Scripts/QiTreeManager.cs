using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;
using System.Collections;

public class QiTreeManager : MonoBehaviour
{
    public static QiTreeManager instance;
    [SerializeField] private TextMeshProUGUI qiText;
    [SerializeField] private TextMeshProUGUI costText;

    public int qiCost = 100;
    [SerializeField] private GameObject buyOptionsObject;
    [SerializeField] private BuyOption[] buyOptions; 
    [SerializeField] private GameObject inventoryFlowerTilesObject;
    [SerializeField] private FlowerTile[] upgradeOptions;

    [SerializeField] private bool hasPurchasedTile;
    public ShopTileInfoController shopInfoController;


    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    private void OnEnable()
    {
        // race condition
        StartCoroutine(DelayedEnable(0.1f));
    }

    IEnumerator DelayedEnable(float sec)
    {
        yield return new WaitForSeconds(sec);

        upgradeOptions = new FlowerTile[3];
        buyOptions = buyOptionsObject.GetComponentsInChildren<BuyOption>();
        hasPurchasedTile = false;

        UpdateShop();
        UpdateUI();

        yield return null;
    }


    // public void OnFlowerClick()
    // {
    //     if (GameManager.playerData.qi >= qiCost)
    //     {
    //         GameManager.playerData.qi -= qiCost;
    //         UpdateQiText(GameManager.playerData.qi);

    //         List<FlowerTileType> flowerTileOptions = new(flowerTileOptionCount);
    //         for (int i = 0; i < flowerTileOptionCount; i++)
    //         {
    //             allUpgradeOptions[i] = (Utils.GetRandomEnumValue<FlowerTileType>());
    //         }
    //         GameManager.playerData.flowerTiles.Add(flowerTileOptions[0]);
    //     }
    // }

    public bool TryPurchase(FlowerTile tile, int index)
    {
        if (GameManager.playerData.qi < qiCost || hasPurchasedTile) return false;

        GameManager.playerData.qi -= qiCost;
        UpdateQiText(GameManager.playerData.qi);

        FlowerTileManager.instance.AddFlowerTile(upgradeOptions[index]);
        hasPurchasedTile = true;

        return true;
    }

    public void UpdateQiText(int qi)
    {
        qiText.text = " " + qi;
        costText.text = "Cost: " + qiCost + " (Limit 1)";
    }

    private void UpdateShop()
    {
        Array.Clear(upgradeOptions, 0, 3);
        for (int i = 0; i < 3; i++)
        {
            // check for duplicates in the future
            FlowerTile randomFlowerTile = FlowerTileManager.instance.GetRandomFlowerTile();
            randomFlowerTile.Initialize(shopInfoController);
            buyOptions[i].Initialize(randomFlowerTile);

            upgradeOptions[i] = randomFlowerTile;
        }

    }

    public void UpdateUI()
    {
        UpdateQiText(GameManager.playerData.qi);
        UpdateFlowerTilesInventory();
    }

    public void UpdateFlowerTilesInventory()
    {

    }
}

