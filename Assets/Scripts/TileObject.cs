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
        tmpElement.text = tile.rank.ToString() + " of " + tile.suit.ToString();
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
