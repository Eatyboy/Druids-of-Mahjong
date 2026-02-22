using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlowerTileInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI infoNameText;
    [SerializeField] private TextMeshProUGUI infoDescriptionText;

    [SerializeField] private FlowerTileInfoController controller;

    public void Open(FlowerTileData flowerTileData, Vector2 anchoredPosition)
    {
        rectTransform.anchoredPosition = anchoredPosition;

        infoNameText.text = flowerTileData.tileName;
        infoDescriptionText.text = flowerTileData.description;

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (controller == null) return; 
        controller.PointerEntered();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (controller == null) return; 
        controller.PointerExited();
    }
}
