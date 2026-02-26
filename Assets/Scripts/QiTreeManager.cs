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

    [Header("Flower Tiles")]
    [SerializeField] private GameObject ftBuyOptionsObject;
    [SerializeField] private BuyOption[] ftBuyOptions; 
    [SerializeField] private GameObject inventoryFlowerTilesObject;
    [SerializeField] private GameObject unsuccessfulBuyPromptObject;

    [SerializeField] private bool hasPurchasedFlowerTile;
    [SerializeField] private bool hasPurchasedCharmScroll;
    public FlowerTileContainer ftContainer;
    public ShopTileInfoController shopInfoController;
    [SerializeField] private FlowerTile ftPrefab;

    [Header("Charm Scrolls (Scrolls under CharmScrollsUI)")]
    [SerializeField] private GameObject scrollBuyOptionsContainer;
    [SerializeField] private BuyOption[] csBuyOptions; 
    [SerializeField] private List<CharmScrollDefinition> scrollDefinitions = new List<CharmScrollDefinition>();

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
        HideUnsuccessfulBuyPrompt();
        // race condition
        StartCoroutine(DelayedEnable(0.1f));
        uiSwitch = false;
        SwitchToFlowerTileShop();
    }

    IEnumerator DelayedEnable(float sec)
    {
        yield return new WaitForSeconds(sec);

        hasPurchasedFlowerTile = false;
        hasPurchasedCharmScroll = false;

        UpdateShop();
        UpdateScrollShop();
        UpdateUI();

        yield return StartCoroutine(PopulateScrollHandOnSceneEnter());
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
        if (GameManager.playerData.qi < qiCost || hasPurchasedFlowerTile) 
        {
            ShowUnsuccessfulBuyPrompt();
            return false;
        }

        GameManager.playerData.qi -= qiCost;
        UpdateQiText(GameManager.playerData.qi);

        GameManager.playerData.flowerTiles.Add(tile.instance);
        ftContainer.AddFlowerTile(tile.instance);
        shopInfoController.ForceClose();
        hasPurchasedFlowerTile = true;

        HideUnsuccessfulBuyPrompt();
        return true;
    }

    public bool TryPurchaseCharmScroll(CharmScrollDefinition definition, int index)
    {
        if (GameManager.playerData.qi < qiCost || hasPurchasedCharmScroll) 
        {
            ShowUnsuccessfulBuyPrompt();
            return false;
        }

        GameManager.playerData.qi -= qiCost;
        UpdateQiText(GameManager.playerData.qi);
        hasPurchasedCharmScroll = true;

        HideUnsuccessfulBuyPrompt();
        return true;
    }

    public bool TryPurchaseCharmScroll(CharmScroll scroll, int index)
    {
        if (GameManager.playerData.qi < qiCost || hasPurchasedCharmScroll) return false;

        GameManager.playerData.qi -= qiCost;
        UpdateQiText(GameManager.playerData.qi);
        hasPurchasedCharmScroll = true;
        return true;
    }

    /// <summary>Returns a random charm scroll definition from the list. Reuses same pattern as flower tiles.</summary>
    public CharmScrollDefinition GetRandomCharmScrollDefinition()
    {
        if (scrollDefinitions == null || scrollDefinitions.Count == 0) return null;
        return Utils.GetRandomItemInList(scrollDefinitions);
    }

    private void UpdateScrollShop()
    {
        if (scrollBuyOptionsContainer == null || scrollDefinitions == null || scrollDefinitions.Count == 0) return;

        for (int i = 0; i < 3; i++)
        {
            CharmScrollDefinition def = GetRandomCharmScrollDefinition();
            if (def != null) csBuyOptions[i].InitializeCS(def);
        }
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
            ftBuyOptions[i].InitializeFT(addedFlowerTile);
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
        if (flowerTileUI != null) flowerTileUI.SetActive(!uiSwitch);
        if (charmScrollUI != null) charmScrollUI.SetActive(uiSwitch);
        if (uiSwitch)
            StartCoroutine(ShowScrollHandAndPopulateTiles());
        else if (ScrollHand.instance != null)
            ScrollHand.instance.gameObject.SetActive(false);
    }

    private void SwitchToFlowerTileShop()
    {
        flowerTileUI.SetActive(true);
        charmScrollUI.SetActive(false);
    }

    private void SwitchToCharmScrollShop()
    {
        flowerTileUI.SetActive(false);
        charmScrollUI.SetActive(true);
    }

    /// <summary>
    /// When tree scene is entered: ensure deck exists and draw 14 tiles into Scroll Hand (hand stays hidden until tab is opened).
    /// </summary>
    private IEnumerator PopulateScrollHandOnSceneEnter()
    {
        UnityEngine.Debug.Log("[ScrollHand] PopulateScrollHandOnSceneEnter: started.");
        if (ScrollHand.instance == null) { UnityEngine.Debug.LogWarning("[ScrollHand] PopulateScrollHandOnSceneEnter: ScrollHand.instance is null. Bailing."); yield break; }
        if (TilesManager.instance == null) { UnityEngine.Debug.LogWarning("[ScrollHand] PopulateScrollHandOnSceneEnter: TilesManager.instance is null. Bailing."); yield break; }
        TilesManager.instance.EnsureDeckInitialized();
        ScrollHand.instance.gameObject.SetActive(false);
        ScrollHand.instance.ClearTiles();
        yield return ScrollHand.instance.DrawUntilFullHand();
        UnityEngine.Debug.Log($"[ScrollHand] PopulateScrollHandOnSceneEnter: done. Hand count = {ScrollHand.instance.currentHand?.Count ?? 0}");
    }

    /// <summary>
    /// When Charm Scroll tab is opened: show Scroll Hand (uses tiles already drawn on scene enter). Only draws if hand is empty.
    /// </summary>
    private IEnumerator ShowScrollHandAndPopulateTiles()
    {
        yield return null;
        if (ScrollHand.instance == null) yield break;
        ScrollHand.instance.gameObject.SetActive(true);
        bool handEmpty = ScrollHand.instance.currentHand == null || ScrollHand.instance.currentHand.Count == 0;
        if (handEmpty && TilesManager.instance != null)
        {
            TilesManager.instance.EnsureDeckInitialized();
            ScrollHand.instance.ClearTiles();
            yield return ScrollHand.instance.DrawUntilFullHand();
        }
    }

    public void ShowUnsuccessfulBuyPrompt()
    {
        unsuccessfulBuyPromptObject.SetActive(true);
    }

    public void HideUnsuccessfulBuyPrompt()
    {
        unsuccessfulBuyPromptObject.SetActive(false);
    }
}

