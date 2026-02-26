using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class TileObject : MonoBehaviour
{
    public RectTransform rt { get; private set; }
    [SerializeField] protected Image tileBackImage;
    [SerializeField] protected Image tileFaceImage;
    [SerializeField] protected TextMeshProUGUI tmpElement;
    [SerializeField] protected TextMeshProUGUI label;

    public Tile tileData;

    protected virtual void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public virtual void Initialize(Tile tile)
    {
        tileData = tile;
        ApplyDisplayFromTileData();
    }

    // Updates the visible label and face sprite from current tileData (suit, rank).
    public virtual void RefreshDisplay()
    {
        if (tileData == null) return;
        ApplyDisplayFromTileData();
    }

    private void ApplyDisplayFromTileData()
    {
        Tile t = tileData;
        Sprite sprite = t.faceSprite ?? t.baseTileData?.faceSprite;
        if (TilesManager.instance != null)
        {
            MahjongTile baseForSuit = TilesManager.instance.GetBaseTileData(t.suit, t.rank);
            if (baseForSuit != null && baseForSuit.faceSprite != null)
            {
                sprite = baseForSuit.faceSprite;
                t.faceSprite = baseForSuit.faceSprite;
            }
        }

        if (sprite == null)
        {
            if (tmpElement != null) tmpElement.text = t.rank.ToString() + " of " + t.suit.ToString();
            if (tileFaceImage != null) tileFaceImage.enabled = false;
            if (label != null) label.enabled = false;
            if (tmpElement != null) tmpElement.enabled = true;
        }
        else
        {
            if (tmpElement != null) tmpElement.enabled = false;
            if (tileFaceImage != null)
            {
                tileFaceImage.sprite = sprite;
                tileFaceImage.enabled = true;
            }
            if (label != null)
            {
                label.enabled = true;
                label.text = GetLabelTextForSuitRank(t.suit, t.rank);
            }
        }
    }

    private static string GetLabelTextForSuitRank(TileSuit suit, int rank)
    {
        return suit switch
        {
            TileSuit.None => "X",
            TileSuit.Bamboo => rank.ToString(),
            TileSuit.Dot => rank.ToString(),
            TileSuit.Character => rank.ToString(),
            TileSuit.Wind => rank switch
            {
                1 => "N",
                2 => "E",
                3 => "S",
                4 => "W",
                _ => "X"
            },
            TileSuit.Dragon => rank switch
            {
                1 => "F",
                2 => "C",
                3 => "B",
                _ => "X"
            },
            _ => "X"
        };
    }
}
