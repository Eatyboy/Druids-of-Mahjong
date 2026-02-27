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

    [Tooltip("Qi cost to open a tab (flower or scroll).")]
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
    [SerializeField] private GameObject neutralHubObject;
    [Tooltip("If hub is not set, these three buttons are shown/hidden for neutral mode.")]
    [SerializeField] private GameObject openFlowerTabButton;
    [SerializeField] private GameObject openScrollTabButton;
    [SerializeField] private GameObject leaveButton;
    [SerializeField] private GameObject flowerTileUI;
    [SerializeField] private GameObject charmScrollUI;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        // Hide tab menus and scroll hand immediately so they never flash on scene load (DelayedEnable will set neutral state later)
        if (flowerTileUI != null) flowerTileUI.SetActive(false);
        if (charmScrollUI != null) charmScrollUI.SetActive(false);
        if (ScrollHand.instance != null) ScrollHand.instance.gameObject.SetActive(false);
    }

    private void Start()
    {
        AudioManager.instance.PlayMusic(AudioManager.instance.treeMusic);
    }

    private void OnEnable()
    {
        HideUnsuccessfulBuyPrompt();
        StartCoroutine(DelayedEnable(0.1f));
    }

    IEnumerator DelayedEnable(float sec)
    {
        yield return new WaitForSeconds(sec);

        hasPurchasedFlowerTile = false;
        hasPurchasedCharmScroll = false;
        UpdateUI();

        // Neutral mode: show hub or 3 buttons, hide both tabs, hide ScrollHand (no pre-draw)
        SetNeutralButtonsVisible(true);
        if (flowerTileUI != null) flowerTileUI.SetActive(false);
        if (charmScrollUI != null) charmScrollUI.SetActive(false);
        if (ScrollHand.instance != null) ScrollHand.instance.gameObject.SetActive(false);
    }

    private void SetNeutralButtonsVisible(bool visible)
    {
        if (neutralHubObject != null)
            neutralHubObject.SetActive(visible);
        else
        {
            if (openFlowerTabButton != null) openFlowerTabButton.SetActive(visible);
            if (openScrollTabButton != null) openScrollTabButton.SetActive(visible);
            if (leaveButton != null) leaveButton.SetActive(visible);
        }
        // Cost text is part of neutral UI: hide when a tab is open
        if (costText != null) costText.gameObject.SetActive(visible);
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
        if (hasPurchasedFlowerTile) 
        {
            ShowUnsuccessfulBuyPrompt();
            return false;
        }

        // No Qi cost for selecting a flower; opening the tab already cost 100 Qi.
        GameManager.playerData.flowerTiles.Add(tile.instance);
        ftContainer.AddFlowerTile(tile.instance);
        shopInfoController.ForceClose();
        hasPurchasedFlowerTile = true;

        HideUnsuccessfulBuyPrompt();
        CloseFlowerTab();
        return true;
    }

    public bool TryPurchaseCharmScroll(CharmScrollDefinition definition, int index)
    {
        if (hasPurchasedCharmScroll) 
        {
            ShowUnsuccessfulBuyPrompt();
            return false;
        }

        // No Qi cost for using a scroll; opening the tab already cost 100 Qi.
        hasPurchasedCharmScroll = true;

        HideUnsuccessfulBuyPrompt();
        return true;
    }

    public bool TryPurchaseCharmScroll(CharmScroll scroll, int index)
    {
        if (hasPurchasedCharmScroll) return false;

        // No Qi cost for using a scroll; opening the tab already cost 100 Qi.
        hasPurchasedCharmScroll = true;
        return true;
    }

    /// <summary>Returns a random charm scroll definition from the list, excluding DoubleQi (disabled in tree).</summary>
    public CharmScrollDefinition GetRandomCharmScrollDefinition()
    {
        if (scrollDefinitions == null || scrollDefinitions.Count == 0) return null;
        // Double Qi scroll is disabled in the tree and never offered
        var allowed = new List<CharmScrollDefinition>();
        foreach (var def in scrollDefinitions)
            if (def != null && def.type != CharmScrollType.DoubleQi)
                allowed.Add(def);
        if (allowed.Count == 0) return null;
        return Utils.GetRandomItemInList(allowed);
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
        if (costText != null) costText.text = qiCost + " Qi to channel";
    }

    private void UpdateShop()
    {
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

    /// <summary>Open flower tab: costs 100 Qi, initializes 3 BuyOptions, shows flower UI, hides hub.</summary>
    public void OpenFlowerTab()
    {
        if (GameManager.playerData == null || GameManager.playerData.qi < qiCost)
        {
            ShowUnsuccessfulBuyPrompt();
            return;
        }
        GameManager.playerData.qi -= qiCost;
        UpdateQiText(GameManager.playerData.qi);
        UpdateShop();
        hasPurchasedFlowerTile = false;
        SetNeutralButtonsVisible(false);
        if (flowerTileUI != null) flowerTileUI.SetActive(true);
        if (charmScrollUI != null) charmScrollUI.SetActive(false);
        if (ScrollHand.instance != null) ScrollHand.instance.gameObject.SetActive(false);
    }

    /// <summary>Open scroll tab: costs 100 Qi, initializes 3 BuyOptions, draws new hand, shows scroll UI, hides hub.</summary>
    public void OpenScrollTab()
    {
        if (GameManager.playerData == null || GameManager.playerData.qi < qiCost)
        {
            ShowUnsuccessfulBuyPrompt();
            return;
        }
        GameManager.playerData.qi -= qiCost;
        UpdateQiText(GameManager.playerData.qi);
        UpdateScrollShop();
        hasPurchasedCharmScroll = false;
        SetNeutralButtonsVisible(false);
        if (flowerTileUI != null) flowerTileUI.SetActive(false);
        if (charmScrollUI != null) charmScrollUI.SetActive(true);
        if (ScrollHand.instance != null)
        {
            ScrollHand.instance.SetCurrentCharmScroll(null);
            ScrollHand.instance.AddCharmScrollFinishedListener(CloseScrollTabAndReturnToNeutral);
        }
        StartCoroutine(OpenScrollTabPopulateHand());
    }

    private IEnumerator OpenScrollTabPopulateHand()
    {
        if (ScrollHand.instance == null || TilesManager.instance == null) yield break;
        TilesManager.instance.EnsureDeckInitialized();
        ScrollHand.instance.gameObject.SetActive(true);
        ScrollHand.instance.ClearTiles();
        yield return ScrollHand.instance.DrawUntilFullHand();
    }

    /// <summary>Return to neutral: hide flower UI, show hub or 3 buttons.</summary>
    public void CloseFlowerTab()
    {
        if (flowerTileUI != null) flowerTileUI.SetActive(false);
        SetNeutralButtonsVisible(true);
    }

    /// <summary>Return scroll hand to deck, hide scroll UI, return to neutral. Call from onCharmScrollFinished.</summary>
    public void CloseScrollTabAndReturnToNeutral()
    {
        if (TilesManager.instance != null)
            TilesManager.instance.ReturnScrollHandToDeck();
        if (ScrollHand.instance != null)
            ScrollHand.instance.gameObject.SetActive(false);
        if (charmScrollUI != null) charmScrollUI.SetActive(false);
        SetNeutralButtonsVisible(true);
    }

    /// <summary>Leave UpgradeTree scene (e.g. go to combat).</summary>
    public void LeaveUpgradeTree()
    {
        if (GameManager.instance != null)
            GameManager.instance.GoToCombat();
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

