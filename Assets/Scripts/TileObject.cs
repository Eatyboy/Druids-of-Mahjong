using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileObject : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rt;

    public Tile tileData;
    public Image tileBackImage;
    public Image tileFaceImage;
    public GameObject selectedOverlay;
    public GameObject highlightedOverlay;

    public bool isSelected = false;

    [SerializeField] private TextMeshProUGUI tmpElement;
    [SerializeField] private TextMeshProUGUI label;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        isSelected = false;
        selectedOverlay.SetActive(false);
        highlightedOverlay.SetActive(false);
    }

    public void Initialize(Tile tile)
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
                    1 => "G",
                    2 => "R",
                    3 => "W",
                    _ => "X"
                },
                _ => "X"
            };
        }
    }

    public void ToggleSelected()
    {
        isSelected = !isSelected;
        if (isSelected)
        {
            PlayerHand.instance.SelectTile(this);
        }
        else
        {
            PlayerHand.instance.DeselectTile(this);
        }
    }

    public void SetHighlighted(bool isHighlighted)
    {
        highlightedOverlay.SetActive(isHighlighted);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleSelected();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selectedOverlay.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selectedOverlay.SetActive(false);
    }
}
