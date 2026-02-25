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

    [SerializeField] private bool hasPurchasedFlowerTile;
    [SerializeField] private bool hasPurchasedCharmScroll;
    public FlowerTileContainer ftContainer;
    public ShopTileInfoController shopInfoController;
    [SerializeField] private FlowerTile ftPrefab;

    [Header("Menus")]
    [SerializeField] private GameObject flowerTileUI;
    [SerializeField] private GameObject charmScrollUI;
    private bool uiSwitch;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    private void OnEnable()
    {
        // race condition
        StartCoroutine(DelayedEnable(0.1f));
        uiSwitch = false;
    }

    IEnumerator DelayedEnable(float sec)
    {
        yield return new WaitForSeconds(sec);

        buyOptions = buyOptionsObject.GetComponentsInChildren<BuyOption>();
        hasPurchasedFlowerTile = false;
        hasPurchasedCharmScroll = false;

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

    public bool TryPurchaseFlowerTile(FlowerTile tile, int index)
    {
        if (GameManager.playerData.qi < qiCost || hasPurchasedFlowerTile) return false;

        GameManager.playerData.qi -= qiCost;
        UpdateQiText(GameManager.playerData.qi);

        GameManager.playerData.flowerTiles.Add(tile.instance);
        ftContainer.AddFlowerTile(tile.instance);
        shopInfoController.ForceClose();
        hasPurchasedFlowerTile = true;

        return true;
    }

    // edit this to your liking
    public bool TryPurchaseCharmScroll(CharmScroll scroll, int index)
    {
        if (GameManager.playerData.qi < qiCost || hasPurchasedCharmScroll) return false;

        GameManager.playerData.qi -= qiCost;
        UpdateQiText(GameManager.playerData.qi);

        // charm scroll stuff probably goes here?

        return true;
    }

    public void UpdateQiText(int qi)
    {
        qiText.text = " " + qi;
        costText.text = "Cost: " + qiCost + " (Limit 1)";
    }

    private void UpdateShop()
    {
        // use this when charm scrolls are added
        int numFlowers = UnityEngine.Random.Range(0, 4);
        int numScrolls = 3 - numFlowers;
        for (int i = 0; i < 3; i++)
        {
            FlowerTileInstance fti = FlowerTileManager.instance.GetRandomFlowerTile();
            FlowerTile addedFlowerTile = Instantiate(ftPrefab);
            addedFlowerTile.Initialize(fti, shopInfoController);
            buyOptions[i].Initialize(addedFlowerTile);
        }

    }

    public void UpdateUI()
    {
        UpdateQiText(GameManager.playerData.qi);
        // UpdateFlowerTilesInventory();
    }

    public void SwitchUI()
    {
        uiSwitch = !uiSwitch;
        flowerTileUI.SetActive(!uiSwitch);
        charmScrollUI.SetActive(uiSwitch);
    }
}

