using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerTileObject : TileObject, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject selectedOverlay;
    public GameObject highlightedOverlay;

    public bool isSelected = false;

    [Header("Selection animation")]
    [SerializeField] private float selectionOffsetY = 20f;
    [SerializeField] private float selectionAnimDuration = 0.2f;

    private float initialAnchoredPositionY;
    private Coroutine selectionAnimCoroutine;

    protected override void Awake()
    {
        base.Awake();

        initialAnchoredPositionY = rt.anchoredPosition.y;
        isSelected = false;
        selectedOverlay.SetActive(false);
        highlightedOverlay.SetActive(false);
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
