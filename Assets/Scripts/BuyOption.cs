using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Numerics;
using TMPro;

public class BuyOption : MonoBehaviour
{
    [Header("Flower Tile")]
    public FlowerTileInstance tileInstance;
    public FlowerTile fTile;
    [SerializeField] private RectTransform fTileRT;

    [Header("Charm Scroll")]
    public CharmScroll cScroll;
    public CharmScrollDefinition scrollDefinition;
    [SerializeField] private RectTransform csRT;
    [Tooltip("Optional: show scroll name/description when initialized with a definition.")]
    [SerializeField] private TextMeshProUGUI scrollNameText;
    [SerializeField] private TextMeshProUGUI scrollDescriptionText;

    [Header("Other References")]    
    [SerializeField] private RectTransform rt;
    [SerializeField] private int index;
    public bool hasPurchased;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Initialize(FlowerTile ft)
    {
        this.cScroll = null;
        this.scrollDefinition = null;

        rt = this.GetComponent<RectTransform>();
        this.fTile = ft;

        fTile = Instantiate(this.fTile);
        fTileRT = fTile.GetComponent<RectTransform>();
        fTileRT.SetParent(rt);
        // translate
        fTileRT.anchorMin = new UnityEngine.Vector2(0.5f, 0.5f);
        fTileRT.anchorMax = new UnityEngine.Vector2(0.5f, 0.5f);
        fTileRT.anchoredPosition = new(0.0f, 0.0f);

        // tile.GetComponent<RectTransform>().SetParent(rt);
        // // translate
        // // using tileRT doesnt work? not sure why
        // tile.GetComponent<RectTransform>().anchorMin = new UnityEngine.Vector2(0.5f, 0.5f);
        // tile.GetComponent<RectTransform>().anchorMax = new UnityEngine.Vector2(0.5f, 0.5f);
        // tile.GetComponent<RectTransform>().anchoredPosition = new(0.0f, 0.0f);
        fTile.UpdateImage();

        hasPurchased = false;
    }

    // can edit this to your liking
    public void Initialize(CharmScroll cs)
    {
        this.fTile = null;
        this.scrollDefinition = null;

        rt = this.GetComponent<RectTransform>();
        this.cScroll = cs;

        cScroll = Instantiate(this.cScroll);
        hasPurchased = false;
    }

    /// <summary>Initialize this option with a random scroll definition (used by Scrolls under CharmScrollsUI).</summary>
    public void Initialize(CharmScrollDefinition definition)
    {
        this.fTile = null;
        this.cScroll = null;
        this.scrollDefinition = definition;

        rt = this.GetComponent<RectTransform>();
        hasPurchased = false;

        if (scrollNameText != null && definition != null) scrollNameText.text = definition.scrollName;
        if (scrollDescriptionText != null && definition != null) scrollDescriptionText.text = definition.description;

        var button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(SelectScrollForHand);
        }
    }

    /// <summary>Call when this option is a scroll: sets the current scroll on ScrollHand so the player can select tiles and apply.</summary>
    public void SelectScrollForHand()
    {
        if (scrollDefinition == null) return;
        if (ScrollHand.instance != null)
            ScrollHand.instance.SetCurrentCharmScroll(scrollDefinition);
    }

    public void Purchase(int index)
    {
        if (scrollDefinition != null)
        {
            if (QiTreeManager.instance.TryPurchaseCharmScroll(scrollDefinition, index))
            {
                hasPurchased = true;
                return;
            }
            return;
        }
        if (cScroll != null && fTile == null)
        {
            if (QiTreeManager.instance.TryPurchaseCharmScroll(cScroll, index))
            {
                hasPurchased = true;
                return;
            }
            return;
        }
        if (cScroll == null && fTile != null)
        {
            if (QiTreeManager.instance.TryPurchaseFlowerTile(fTile, index))
            {
                hasPurchased = true;
                Destroy(fTile.gameObject);
                return;
            }
        }
    }
}
