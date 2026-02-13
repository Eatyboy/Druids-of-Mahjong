using System.Collections;
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

    [SerializeField] private bool isPlayerTile = true;

    [Header("Selection animation")]
    [SerializeField] private float selectionOffsetY = 20f;
    [SerializeField] private float selectionAnimDuration = 0.2f;

    private float initialAnchoredPositionY;
    private Coroutine selectionAnimCoroutine;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        initialAnchoredPositionY = rt.anchoredPosition.y;
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
                    1 => "F",
                    2 => "C",
                    3 => "B",
                    _ => "X"
                },
                _ => "X"
            };
        }
    }

    public void ToggleSelected()
    {
        if (!isPlayerTile) return;

        isSelected = !isSelected;
        if (isSelected)
        {
            PlayerHand.instance.SelectTile(this);
        }
        else
        {
            PlayerHand.instance.DeselectTile(this);
        }
        AnimateToSelected(isSelected);
    }

    /// <summary>
    /// Instantly reset vertical position to initial (e.g. when hand is sorted and layout will move the tile).
    /// </summary>
    public void ResetToInitialPosition()
    {
        if (selectionAnimCoroutine != null)
        {
            StopCoroutine(selectionAnimCoroutine);
            selectionAnimCoroutine = null;
        }
        Vector2 p = rt.anchoredPosition;
        rt.anchoredPosition = new Vector2(p.x, initialAnchoredPositionY);
    }

    public void AnimateToSelected(bool toSelected)
    {
        if (selectionAnimCoroutine != null)
            StopCoroutine(selectionAnimCoroutine);
        selectionAnimCoroutine = StartCoroutine(AnimateToSelectedCoroutine(toSelected));
    }

    private IEnumerator AnimateToSelectedCoroutine(bool toSelected)
    {
        float startY = rt.anchoredPosition.y;
        float endY = toSelected ? initialAnchoredPositionY + selectionOffsetY : initialAnchoredPositionY;
        float elapsed = 0f;

        while (elapsed < selectionAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / selectionAnimDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float y = Mathf.Lerp(startY, endY, smoothT);
            Vector2 p = rt.anchoredPosition;
            rt.anchoredPosition = new Vector2(p.x, y);
            yield return null;
        }

        Vector2 final = rt.anchoredPosition;
        rt.anchoredPosition = new Vector2(final.x, endY);
        selectionAnimCoroutine = null;
    }

    public void SetHighlighted(bool isHighlighted)
    {
        if (!isPlayerTile) return;

        highlightedOverlay.SetActive(isHighlighted);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isPlayerTile) return;

        ToggleSelected();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isPlayerTile) return;

        selectedOverlay.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isPlayerTile) return;

        selectedOverlay.SetActive(false);
    }
}
