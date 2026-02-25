using UnityEngine;
using System.Collections.Generic;
using System.Numerics;

public class BuyOption : MonoBehaviour
{
    [Header("Flower Tile")]
    public FlowerTileInstance tileInstance;
    public FlowerTile fTile;
    [SerializeField] private RectTransform fTileRT;

    [Header("Charm Scroll")]
    public CharmScroll cScroll;
    [SerializeField] private RectTransform csRT;

    [Header("Other References")]    
    [SerializeField] private RectTransform rt;
    [SerializeField] private int index;
    public bool hasPurchased;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Initialize(FlowerTile ft)
    {
        this.cScroll = null;

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

        rt = this.GetComponent<RectTransform>();
        this.cScroll = cs;

        cScroll = Instantiate(this.cScroll);
        /*
        csRT = cScroll.GetComponent<RectTransform>();
        csRT.SetParent(rt);
        // translate
        csRT.anchorMin = new UnityEngine.Vector2(0.5f, 0.5f);
        csRT.anchorMax = new UnityEngine.Vector2(0.5f, 0.5f);
        csRT.anchoredPosition = new(0.0f, 0.0f);
        */

        // tile.GetComponent<RectTransform>().SetParent(rt);
        // // translate
        // // using tileRT doesnt work? not sure why
        // tile.GetComponent<RectTransform>().anchorMin = new UnityEngine.Vector2(0.5f, 0.5f);
        // tile.GetComponent<RectTransform>().anchorMax = new UnityEngine.Vector2(0.5f, 0.5f);
        // tile.GetComponent<RectTransform>().anchoredPosition = new(0.0f, 0.0f);
        // tile.UpdateImage();

        hasPurchased = false;
    }

    public void Purchase(int index)
    {
        if (cScroll == null)
        {
            if (QiTreeManager.instance.TryPurchaseFlowerTile(fTile, index))
            {
                hasPurchased = true;
                Destroy(fTile.gameObject);
                return;
            }
            else
            {
                // make this actual text lol
                UnityEngine.Debug.Log("Didnt buy. Either you're broke or greedy");
            }   
        }
        else if (fTile == null)
        {
            if (QiTreeManager.instance.TryPurchaseCharmScroll(cScroll, index))
            {
                hasPurchased = true;
                // Destroy(cScroll.gameObject);
                return;
            }
            else
            {
                // make this actual text lol
                UnityEngine.Debug.Log("Didnt buy. Either you're broke or greedy");
            }  
        }
    }
}
