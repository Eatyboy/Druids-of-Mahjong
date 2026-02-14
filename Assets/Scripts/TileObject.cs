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
        if (tileData.baseTileData.faceSprite == null )
        {
            tmpElement.text = tile.rank.ToString() + " of " + tile.suit.ToString();
            tileFaceImage.enabled = false;
            label.enabled = false;
        }
        else
        {
            tmpElement.enabled = false;
            tileFaceImage.sprite = tileData.baseTileData.faceSprite;
            label.text = tile.baseTileData.suit switch
            {
                TileSuit.None => "X",
                TileSuit.Bamboo => tile.rank.ToString(),
                TileSuit.Dot => tile.rank.ToString(),
                TileSuit.Character => tile.rank.ToString(),
                TileSuit.Wind => tile.rank switch
                {
                    1 => "N",
                    2 => "E",
                    3 => "S",
                    4 => "W",
                    _ => "X"
                },
                TileSuit.Dragon => tile.rank switch
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
}
